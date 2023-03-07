using System.Collections;
using Framework;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private ClimbingSystem climbingSystem;
    
    [SerializeField,Range(0,100)] private int aiChange;
    [SerializeField] private Image flagImg;
    [SerializeField] private Sprite[] flags;

    private void Start()
    {
        StartCoroutine(ClimbChange());
        var flag = flags[Random.Range(0, flags.Length)];
        flagImg.sprite = flag;
        UIManager.Instance.enemyTracker.sprite = flag;
    }

    private IEnumerator ClimbChange()
    {
        yield return new WaitForSeconds(Random.Range(0.25f, .75f));
        var change = Random.Range(0, 100);
        if(change > 100-aiChange)
            climbingSystem.Climb();

        StartCoroutine(ClimbChange());
    }
}
