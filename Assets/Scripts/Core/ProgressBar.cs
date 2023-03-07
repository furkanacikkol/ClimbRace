using DG.Tweening;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private float slideSpeed;
    [SerializeField] private Transform player,moneyParticleEffect;
    [SerializeField] private UnityEvent successEvent;

    public RectTransform greenArea, yellowArea;
    public Image succesImage;

    [Range(0.5f, 0.8f)] public float value;
    [Range(0.1f, 0.3f)] public float size;

    private float _positiveGreen, _positiveYellow;
    private float _negativeGreen, _negativeYellow;
    private SkillManager _skillManager;
    private float _tempMin, _tempMax, _timer;

    private void Start()
    {
        _skillManager = SkillManager.Instance;
        _tempMax = slideSpeed;
        _tempMin = slideSpeed / 2;
        progressBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!progressBar.gameObject.activeInHierarchy) return;
        float sizeDeltaY = 100;
        greenArea.anchoredPosition = new Vector2(0, value * 160);
        yellowArea.anchoredPosition = new Vector2(0, value * 160);

        Vector2 sizeDelta = greenArea.sizeDelta;
        float sizeY = size * sizeDeltaY;
        sizeDelta.y = sizeY;
        sizeDelta.y *= _skillManager.accuracy;
        sizeDelta.y = Mathf.Clamp(sizeDelta.y, 0, 10);
        greenArea.sizeDelta = sizeDelta;


        var yellowSizeDelta = sizeDelta;
        yellowSizeDelta.y *= 5; //10-value*5;
        yellowArea.sizeDelta = yellowSizeDelta;


        value = 1 - _skillManager.stamina / _skillManager.tempStamina;
        value = Mathf.Clamp(value, 0.5f, .8f);
        size = .8f - value;
        size = Mathf.Clamp(size, 0.01f, .3f);
    }

    public void ProgressBarDown()
    {
        if (GameManager.instance.gameState == Consts.GameState.fail ||
            GameManager.instance.gameState == Consts.GameState.win) return;
        progressBar.gameObject.SetActive(true);
    }

    public void ProgressBarStay()
    {
        if (!progressBar.gameObject.activeInHierarchy) return;

        progressBar.value = Mathf.MoveTowards(progressBar.value, 1, Time.deltaTime * slideSpeed);
        if (progressBar.value > .95f)
            progressBar.value = 0;

        slideSpeed = _skillManager.stamina < _skillManager.tempStamina / 2 ? _tempMin : _tempMax;
    }

    public void ProgressBarUp()
    {
        Calculate(out _positiveGreen, out _negativeGreen, greenArea.anchoredPosition.y, greenArea.sizeDelta.y);
        Calculate(out _positiveYellow, out _negativeYellow, yellowArea.anchoredPosition.y, yellowArea.sizeDelta.y);
        if (progressBar.value < _positiveGreen && progressBar.value > _negativeGreen)
        {
            succesImage.gameObject.SetActive(true);
            succesImage.DOColor(new Color32(0, 255, 0, 0), 1f).OnComplete(() =>
            {
                succesImage.DOColor(new Color32(0, 255, 0, 255), .01f);
                succesImage.gameObject.SetActive(false);
            });
            HapticManager.SuccesVibrate();
            MoneyParticle(true);
            successEvent.Invoke();
            DOTween.To(() => _timer, x => _timer = x, 1, .5f)
                .OnComplete(() => successEvent.Invoke());
        }
        else if (progressBar.value < _positiveYellow && progressBar.value > _negativeYellow)
        {
            HapticManager.SoftVibrate();
            successEvent.Invoke();
            MoneyParticle(false);
        }

        progressBar.value = 0;
        progressBar.gameObject.SetActive(false);
    }

    private void Calculate(out float positive, out float negative, float posY, float sizeDeltaY)
    {
        var x = 0.625f * posY;
        x *= .01f;

        var y = sizeDeltaY / 300;
        negative = x - y;
        positive = x + y;
    }

    private void MoneyParticle(bool isDouble)
    {
        var getCoin = isDouble ? SkillManager.Instance.income * 2 : SkillManager.Instance.income;
        var coin = PlayerPrefs.GetFloat("Coin");
        var moneyParticle =Instantiate(moneyParticleEffect,player.transform.position+Vector3.back*.5f+Vector3.up, Quaternion.identity);
        moneyParticle.GetComponentInChildren<TextMeshPro>().text = "+" + getCoin.ToString("F1");
        Destroy(moneyParticle.gameObject,.35f);
        coin += SkillManager.Instance.income;
        UIManager.Instance.coin.text = "" + coin.ToString("F1");
        PlayerPrefs.SetFloat("Coin",coin);
    }

}