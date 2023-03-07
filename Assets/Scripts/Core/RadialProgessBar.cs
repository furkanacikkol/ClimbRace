using DG.Tweening;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgessBar : MonoBehaviour
{
    [SerializeField] private ParticleSystem tearParticle;
    [SerializeField] private SkinnedMeshRenderer player;
    [SerializeField] private float lerpTime;

    private SkillManager _skillManager;
    private Material _playerMaterial;
    private Image _image;
    private Animator _vignetteAnimator;
    private bool _isTap;

    private void Start()
    {
        _image = GetComponent<Image>();
        _skillManager = SkillManager.Instance;
        _vignetteAnimator = UIManager.Instance.vignette.GetComponent<Animator>();
        _playerMaterial = player.material;
    }

    private void Update()
    {
        if (GameManager.instance.gameState == Consts.GameState.fail ||
            GameManager.instance.gameState == Consts.GameState.win)
        {
            PlayerReset();
            return;
        }

        _image.fillAmount = Mathf.MoveTowards(_image.fillAmount, _skillManager.stamina / _skillManager.tempStamina,
            lerpTime * Time.deltaTime);
        var fillAmount = _image.fillAmount;
        if (fillAmount > .75f)
        {
            _image.DOColor(new Color32(0, 255, 0, 255), .25f);
            _playerMaterial.SetFloat("_Value", Mathf.Lerp(_playerMaterial.GetFloat("_Value"), 1.5f, Time.deltaTime));
        }
        else if (fillAmount < .75f && fillAmount > .5f)
        {
            _image.DOColor(new Color32(150, 255, 0, 255), .25f);
            _playerMaterial.SetFloat("_Value", Mathf.Lerp(_playerMaterial.GetFloat("_Value"), 1f, Time.deltaTime));
            tearParticle.Stop();
            _vignetteAnimator.SetFloat("Tired", 0f);
        }
        else if (fillAmount < .5f && fillAmount > .25f) 
        {
            _image.DOColor(new Color32(255, 155, 0, 255), .25f);
            _playerMaterial.SetFloat("_Value", Mathf.Lerp(_playerMaterial.GetFloat("_Value"), .25f, Time.deltaTime));
            tearParticle.Play();
            _vignetteAnimator.SetFloat("Tired", 1.1f);
        }
        else if (fillAmount < .25f)
        {
            _image.DOColor(new Color32(255, 0, 0, 255), .25f);
            _playerMaterial.SetFloat("_Value", Mathf.Lerp(_playerMaterial.GetFloat("_Value"), -.2f, Time.deltaTime));
            _vignetteAnimator.SetFloat("Tired", 2.1f);
        }
    }

    public void RadialProgressDown()
    {
        _isTap = true;
    }

    public void RadialProgressUp()
    {
        if (!_isTap) return;
        _skillManager.stamina -= _skillManager.tempStamina * .2f;
        _isTap = false;
    }

    public void PlayerReset()
    {
        _image.DOColor(new Color32(0, 255, 0, 255), .01f);
        _playerMaterial.SetFloat("_Value", Mathf.Lerp(_playerMaterial.GetFloat("_Value"), 1.5f, 1f));
        _vignetteAnimator.SetFloat("Tired", 0f);
        tearParticle.Stop();
        _image.fillAmount = 1;
    }
}