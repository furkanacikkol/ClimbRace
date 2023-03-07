using System.Collections;
using DG.Tweening;
using Framework;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.UI;

public class MatchFinalController : MonoBehaviour
{
    [SerializeField] private GameObject player,enemy;
    [SerializeField] private Transform flagPoint,playerPoint;

    private Slider _slider;
    private RectTransform _playerTracker, _enemyTracker;
    
    private float _firstDist;
    private bool _isFinish;
    private void Start() 
    {
        _slider = UIManager.Instance.matchSliderBar;
        _enemyTracker = _slider.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
        _playerTracker = _slider.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();
        _firstDist = Vector3.Distance(player.transform.position, transform.position);

    }

    private void Update()
    {
        var position = transform.position;
        position.x = player.transform.position.x;
        var dist = Vector3.Distance(player.transform.position, position);
        position.x = enemy.transform.position.x;
        var enemyDist = Vector3.Distance(enemy.transform.position, position);
        var x = Calculate(dist);
        var enemyX = Calculate(enemyDist);
        if(!_isFinish)_slider.DOValue(1-x, .25f);

        _enemyTracker.anchoredPosition = new Vector2(CalculateTracker(1-enemyX),_enemyTracker.anchoredPosition.y);
        _playerTracker.anchoredPosition = new Vector2(CalculateTracker(1-x),_playerTracker.anchoredPosition.y);
        if (GameManager.instance.gameState != Consts.GameState.play) return;
        if (1 - x > .95f)
        {
            _slider.DOValue(1, .25f);
            _isFinish = true;
            GameManager.instance.gameState = Consts.GameState.win;
            player.GetComponent<Animator>().SetTrigger("Flag");
            player.GetComponent<FullBodyBipedIK>().enabled = false;
            var flag = player.GetComponent<ClimbingSystem>().flag;
            flag.SetActive(true);
            StartCoroutine(Timer(flag));

        }
        else if(1 - enemyX >.95f)
            GameManager.instance.gameState = Consts.GameState.fail;

    }

    private float Calculate(float dist)
    {
        var x = dist * 100 / _firstDist;
        x *= 0.01f;
        return x;
    }
    
    
    private  float CalculateTracker(float slideValue)
    {
        var a = 330 * slideValue;
        return a;
    }

    private IEnumerator Timer(GameObject flag)
    {
        var playerPos = playerPoint == null ? player.transform.position + Vector3.up * 1.5f : playerPoint.position;
        player.transform.DOMove(playerPos, 1f);
        yield return new WaitForSeconds(1.1f);
        flag.transform.parent = null;
        flag.tag = "trash";
        var pos = flagPoint == null ? player.transform.position + new Vector3(-1f, .75f, .75f) : flagPoint.position;
        flag.transform.DOMove(pos, .25f);
        flag.transform.DORotateQuaternion(new Quaternion(0.22150217f, -0.275845975f, 0.175646529f, -0.918691576f),
            .25f);
        player.transform.GetChild(3).gameObject.SetActive(false);
    }
}
