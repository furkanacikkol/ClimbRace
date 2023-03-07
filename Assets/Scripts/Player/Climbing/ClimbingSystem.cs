using System.Collections;
using DG.Tweening;
using Packages.HifiveInputManager.Scripts.Runtime;
using RootMotion.FinalIK;
using UnityEngine;

public class ClimbingSystem : MonoBehaviour
{
    public bool isEnemy;
    public FullBodyBipedIK ik;
    public GameObject stairs, flag;
    public float dist, lerpTime;
    public HandPoser rightHandPose, leftHandPose;
    [HideInInspector] public bool isDead;
    [SerializeField] private RadialProgessBar _radialProgessBar;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private FinishController finishController;
    [SerializeField] private TapHoldControl tapHoldControl;
    [SerializeField] private StaminaController staminaController;
    private int _targetIndex = 1;
    private bool _left = true, _isClimb;
    
    private SkillManager _skillManager;
    private Animator _animator;
    private Vector3 _rhStartPos, _lhStartPos, _rlStartPos, _llStartPos, _startPos;
    private Rigidbody[] _rigidbodies;

    private void Start()
    {
        _startPos = transform.position;
        _animator = GetComponent<Animator>();
        _skillManager = SkillManager.Instance;
        ik.solver.rightHandEffector.positionWeight = .75f;
        ik.solver.leftHandEffector.positionWeight = 0;
        ik.solver.rightFootEffector.positionWeight = 0;
        ik.solver.leftFootEffector.positionWeight = 0;

        ik.solver.rightHandEffector.rotationWeight = 1;
        ik.solver.leftHandEffector.rotationWeight = 0;

        _rhStartPos = ik.solver.rightHandEffector.position;
        _rlStartPos = ik.solver.rightFootEffector.position;
        _lhStartPos = ik.solver.leftHandEffector.position;
        _llStartPos = ik.solver.leftFootEffector.position;
    }

    private void Update()
    {
        if (isDead) return;
        if (_skillManager.stamina / _skillManager.tempStamina < .05f)
        {
            HapticManager.HeavyVibrate();
            RagdollActive();
            _skillManager.stamina = _skillManager.tempStamina;
            if (GameManager.instance.gameState == Consts.GameState.idle)
                StartCoroutine(RefreshGame());
            else
                GameManager.instance.gameState = Consts.GameState.fail;
            _radialProgessBar.PlayerReset();
        }

        if (GameManager.instance.gameState == Consts.GameState.fail && !isEnemy)
        {
            RagdollActive();
            enabled = false;
        }
    }

    private void RagdollActive()
    {
        if (!isEnemy)
        {
            tapHoldControl.dead = true;
            tapHoldControl.enabled = false;
            staminaController.enabled = false;
        }
       
        progressBar.gameObject.SetActive(false);
        isDead = true;
        _animator.enabled = false;
        ik.solver.IKPositionWeight = 0;
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in _rigidbodies)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        _radialProgessBar.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator RefreshGame()
    {
        yield return new WaitForSeconds(1.5f);
        foreach (var rb in _rigidbodies)
        {
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        isDead = false;
        _animator.enabled = true;
        ik.solver.IKPositionWeight = 1;
        _radialProgessBar.transform.parent.gameObject.SetActive(true);
        ResetPlayer();
    }

    private void LateUpdate()
    {
        if (_isClimb) return;

        rightHandPose.poseRoot = stairs.transform.GetChild(_targetIndex + 1).GetChild(4);
        ik.solver.rightHandEffector.rotation = rightHandPose.poseRoot.rotation;
        ik.solver.rightHandEffector.position = rightHandPose.poseRoot.position;
    }

    public void Climb()
    {
        if (GameManager.instance.gameState == Consts.GameState.win ||
            GameManager.instance.gameState == Consts.GameState.fail) return;
        
        _isClimb = true;
        ik.solver.leftHandEffector.positionWeight = 1;
        ik.solver.rightFootEffector.positionWeight = 1;
        ik.solver.leftFootEffector.positionWeight = 1;
        ik.solver.leftHandEffector.rotationWeight = 1;

        _animator.SetBool("Climb", true);


        var pos = (ik.solver.leftHandEffector.position + ik.solver.rightFootEffector.position) / 2f;
        pos.y += .75f;
        pos.z -= .3f;
        transform.DOMove(pos, .25f);

        if (stairs.transform.childCount < _targetIndex+2)
            return;
        if (!_left)
        {
            ik.solver.rightHandEffector.rotationWeight = 1;
            ik.solver.rightHandEffector.positionWeight = 1;
            rightHandPose.poseRoot = stairs.transform.GetChild(_targetIndex + 1).GetChild(2);
            ik.solver.rightHandEffector.rotation = rightHandPose.poseRoot.rotation;
            DOTween.To(() => ik.solver.rightHandEffector.position, x => ik.solver.rightHandEffector.position = x,
                    stairs.transform.GetChild(_targetIndex + 1).GetChild(2).position, .2f * lerpTime)
                .SetUpdate(UpdateType.Late);
            DOTween.To(() => ik.solver.rightFootEffector.position, x => ik.solver.rightFootEffector.position = x,
                    stairs.transform.GetChild(_targetIndex).GetChild(0).position + Vector3.up * dist -
                    Vector3.forward * .15f, .2f * lerpTime)
                .SetUpdate(UpdateType.Late)
                .OnComplete(() => _animator.SetBool("Climb", false));
            _left = true;

            _targetIndex++;
        }
        else
        {
            leftHandPose.poseRoot = stairs.transform.GetChild(_targetIndex + 1).GetChild(3);
            ik.solver.leftHandEffector.rotation = leftHandPose.poseRoot.rotation;

            DOTween.To(() => ik.solver.leftHandEffector.position, x => ik.solver.leftHandEffector.position = x,
                    stairs.transform.GetChild(_targetIndex + 1).GetChild(3).position, .2f * lerpTime)
                .SetUpdate(UpdateType.Late);

            DOTween.To(() => ik.solver.leftFootEffector.position, x => ik.solver.leftFootEffector.position = x,
                    stairs.transform.GetChild(_targetIndex).GetChild(1).position + Vector3.up * dist , .2f * lerpTime)
                .SetUpdate(UpdateType.Late)
                .OnComplete(() => _animator.SetBool("Climb", false)
                );

            _left = false;
        }
    }

    public void ResetPlayer()
    {
        _left = true;
        if (!isEnemy)
        {
            tapHoldControl.enabled = true;
            staminaController.enabled = true;
        }
   
        ik.solver.rightHandEffector.positionWeight = .75f;
        ik.solver.leftHandEffector.positionWeight = 0;
        ik.solver.rightFootEffector.positionWeight = 0;
        ik.solver.leftFootEffector.positionWeight = 0;

        ik.solver.rightHandEffector.rotationWeight = 1;
        ik.solver.leftHandEffector.rotationWeight = 0;

        _isClimb = false;

        _animator.SetBool("Climb", false);
        _targetIndex = 1;

        ik.solver.rightHandEffector.position = _rhStartPos;
        ik.solver.rightFootEffector.position = _rlStartPos;
        ik.solver.leftHandEffector.position = _lhStartPos;
        ik.solver.leftFootEffector.position = _llStartPos;

        transform.position = _startPos;
        if (finishController.gameObject.activeInHierarchy) finishController.ResetMeter();
    }

    public bool Dead()
    {
        return isDead;
    }
}