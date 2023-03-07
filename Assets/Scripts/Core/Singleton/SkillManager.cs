using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoSingleton<SkillManager>
{
    #region Public UI Object

    [Header("UI Objects")] public TextMeshProUGUI accuracyLvlText;
    public TextMeshProUGUI accuracyMoneyText;
    public TextMeshProUGUI staminaLvlText;
    public TextMeshProUGUI staminaMoneyText;
    public TextMeshProUGUI incomeLvlText;
    public TextMeshProUGUI incomeMoneyText;
    public TextMeshProUGUI coinText;
    #endregion

    #region Scriptable Object

    [Header("Scriptable Object")] [SerializeField]
    private UpgradeSkill accuracyScriptableObject;

    [SerializeField] private UpgradeSkill staminaScriptableObject;
    [SerializeField] private UpgradeSkill incomeScriptableObject;

    #endregion

    public float accuracy, stamina, income , tempStamina;
    private int _accuracyLevel, _staminaLevel, _incomeLevel;

    [SerializeField] private GameObject upgradeButtons;
    private readonly string _accuracyText = "Accuracy";
    private readonly string _staminaText = "Stamina";
    private readonly string _incomeText = "Income";

    private void Awake() => GetData();

    private void OnEnable() => GetData();

    private void GetData()
    {

        //Test Coin
       // PlayerPrefs.SetFloat("Coin", 0);
        _accuracyLevel = GetUpgradeLevel(_accuracyText, accuracyLvlText, accuracyMoneyText, accuracyScriptableObject);
        accuracy = GetUpgradeValue(_accuracyText,accuracyScriptableObject);
        
        _staminaLevel = GetUpgradeLevel(_staminaText, staminaLvlText, staminaMoneyText, staminaScriptableObject);
        stamina = GetUpgradeValue(_staminaText,staminaScriptableObject);
        tempStamina = stamina;
        
        _incomeLevel = GetUpgradeLevel(_incomeText, incomeLvlText, incomeMoneyText, incomeScriptableObject);
        income = GetUpgradeValue(_incomeText,incomeScriptableObject);
    }

    private int GetUpgradeLevel(string playerPrefName, TextMeshProUGUI levelText,TextMeshProUGUI moneyText, UpgradeSkill scriptableObject )
    {
        int level;
        if (PlayerPrefs.HasKey(playerPrefName))
        {
            level = PlayerPrefs.GetInt(playerPrefName+"Level");
            levelText.text = "" + level;
            moneyText.text = "" + scriptableObject.firstMoneyValue *
                scriptableObject.increaseMoneyValue * level;
        }
        else
        {
            level = 1;
            PlayerPrefs.SetInt(playerPrefName+"Level",1);
            moneyText.text = "" + scriptableObject.firstMoneyValue;
            levelText.text = "" + 1;
        }

        return level;
    }

    private float GetUpgradeValue(string playerPrefName, UpgradeSkill scriptableObject )
    {
        float value;
        if (PlayerPrefs.HasKey(playerPrefName)) value = PlayerPrefs.GetFloat(playerPrefName);
        else
        {
            PlayerPrefs.SetFloat(playerPrefName, scriptableObject.firstValue);
            value = scriptableObject.firstValue;
        }

        return value;
    }

    private void UpgradeSkill(out int returnLevel, out float returnSkillValue, TextMeshProUGUI level,TextMeshProUGUI moneyTxt, UpgradeSkill scriptableObjct, int skillLevel, float skill, string playerPrefName)
    {
        var price = scriptableObjct.firstMoneyValue * scriptableObjct.increaseMoneyValue * skillLevel;
        if (CoinCheck(price, false))
        {
            HapticManager.SoftVibrate();
            moneyTxt.transform.parent.GetChild(3).GetComponent<Animator>().SetTrigger("Effect");
            skill += scriptableObjct.increaseValue;
            if (PlayerPrefs.HasKey(playerPrefName+"Level"))
                skillLevel = PlayerPrefs.GetInt(playerPrefName+"Level");
            else
                PlayerPrefs.SetInt(playerPrefName+"Level", 1);
            skillLevel++;
            level.text = "" + skillLevel;
            moneyTxt.text = "" + scriptableObjct.firstMoneyValue * scriptableObjct.increaseMoneyValue * skillLevel;
            PlayerPrefs.SetFloat(playerPrefName, skill);
            PlayerPrefs.SetInt(playerPrefName+"Level", skillLevel);
        }
        
        returnLevel = skillLevel;
        returnSkillValue = skill;

    }

    public void UpgradeAccuracy(TextMeshProUGUI level)
    {
        UpgradeSkill(out _accuracyLevel, out accuracy, level, accuracyMoneyText, accuracyScriptableObject,
            _accuracyLevel, accuracy, _accuracyText);
        
    }

    public void UpgradeStamina(TextMeshProUGUI level)
    {
        UpgradeSkill(out _staminaLevel, out stamina, level, staminaMoneyText, staminaScriptableObject,
            _staminaLevel, stamina, _staminaText);
        tempStamina = stamina;
    }

    public void UpgradeIncome(TextMeshProUGUI level)
    {
        UpgradeSkill(out _incomeLevel, out income, level, incomeMoneyText, incomeScriptableObject, _incomeLevel,
            income, _incomeText);
    }

    private bool CoinCheck(float price,bool onlyCheck)
    {
        var money = PlayerPrefs.GetFloat("Coin");
        if (money > price)
        {
            if(!onlyCheck)money -= price;
            PlayerPrefs.SetFloat("Coin", money);
            coinText.text = "" + money.ToString("F1");
            return true;
        }

        return false;
    }


    private void Update()
    {
        if (GameManager.instance.gameState != Consts.GameState.idle) return;
        if (!upgradeButtons.activeInHierarchy) return;
        CheckMoneyStatus(accuracyScriptableObject,_accuracyLevel,accuracyLvlText);
        CheckMoneyStatus(staminaScriptableObject,_staminaLevel,staminaLvlText);
        CheckMoneyStatus(incomeScriptableObject,_incomeLevel,incomeLvlText);
    }

    private void CheckMoneyStatus(UpgradeSkill scriptableObject,int level, TextMeshProUGUI txt)
    {
        var price = scriptableObject.firstMoneyValue * scriptableObject.increaseMoneyValue * level;
        if (!CoinCheck(price,true))
        {
          //  txt.transform.GetComponentInParent<Image>().color = Color.red;
            txt.transform.GetComponentInParent<Button>().interactable = false;
        }
        else
        {
           // txt.transform.GetComponentInParent<Image>().color = Color.white;
            txt.transform.GetComponentInParent<Button>().interactable =true;
        }
    }
}