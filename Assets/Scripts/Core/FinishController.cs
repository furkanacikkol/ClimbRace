using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework;
using TMPro;
using UnityEngine;

public class FinishController : MonoBehaviour
{
    private float _meter;
    private bool _isFirst = true;
    private int _goalIndex;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject climbWall;
    [SerializeField] private List<GameObject> stairs = new List<GameObject>();
    [SerializeField] private float[] goals;

    [SerializeField] private ParticleSystem greenConfetti;
    private UIManager _uiManager;
    private TextMeshProUGUI _meterText;
    private Vector3 _goalPosition;
    private GameObject _lastWall;

    private float _targetMeter, _currentMeter;

    public void ResetMeter()
    {
       // _goalIndex = 0;
        _currentMeter = 0;
        _targetMeter = 0;
        GoalSelector();
        FinishUp();
    }

 
    private void Start()
    {
        if (PlayerPrefs.HasKey("GoalIndex"))
            _goalIndex = PlayerPrefs.GetInt("GoalIndex");
        else
            PlayerPrefs.SetInt("GoalIndex",0);
        
        //Settings final trigger position
        var climbWallPos = climbWall.transform.transform.position;
        climbWallPos.y = climbWall.transform.GetChild(0).localScale.y * 5;
        transform.position = climbWallPos;

        _uiManager = UIManager.Instance;
        _meterText = _uiManager.sliderBar.GetComponentInChildren<TextMeshProUGUI>();

        GoalSelector();
        FinishUp();
    }

    public void FinishUp()
    {
        var currentMeter =
            _goalPosition.y - player.transform.position.y;
        currentMeter = _meter - currentMeter;
        currentMeter = Mathf.Clamp(currentMeter, 0, _meter);
        _uiManager.sliderBar.DOValue(currentMeter / _meter, .5f);
       // _meterText.text = (currentMeter + _currentMeter).ToString("0") + "/" + _targetMeter.ToString("0");
        _meterText.text = currentMeter.ToString("0") + "/" + _meter.ToString("0");
        if (currentMeter > _meter - .5f)
        {
            var pos = player.transform.position;
            pos.x = 0;
            Instantiate(greenConfetti, pos, Quaternion.identity);
            _goalIndex++;
            PlayerPrefs.SetInt("GoalIndex",_goalIndex);
            GoalSelector();
            FinishUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StairsCreator();
        }
    }


    private void GoalSelector()
    {
        _meter = goals[_goalIndex % (goals.Length - 1) + 1];
        _currentMeter = _targetMeter;
        _targetMeter = _meter + _currentMeter;
        _goalPosition = player.transform.position + Vector3.up * _meter;
    }

    private void StairsCreator()
    {
        Vector3 wallPos;
        if (_isFirst)
        {
            wallPos = climbWall.transform.position;
            wallPos.y = climbWall.transform.GetChild(0).localScale.y * 10 + wallPos.y;
            _isFirst = false;
        }
        else
        {
            wallPos = _lastWall.transform.position;
            wallPos.y = _lastWall.transform.localScale.y * 10 + wallPos.y;
        }


        var stair = Instantiate(stairs[0], wallPos, Quaternion.identity);
        stair.transform.parent = climbWall.transform;

        var wall = stair.transform.GetChild(0);
        wall.parent = stair.transform.GetChild(1);
        _lastWall = wall.gameObject;

        stair.transform.parent = climbWall.transform;
        Unparent(stair.gameObject, climbWall.gameObject);

        var finishPos = transform.position;
        finishPos.y += 50;
        transform.position = finishPos;
    }

    private static void Unparent(GameObject parentObject, GameObject targetObject)
    {
        var count = parentObject.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            parentObject.transform.GetChild(0).parent = targetObject.transform;
        }

        Destroy(parentObject);
    }
}