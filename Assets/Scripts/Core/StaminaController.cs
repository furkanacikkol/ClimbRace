using Framework;
using UnityEngine;
using Packages.HifiveInputManager.Scripts.Runtime;
public class StaminaController : MonoBehaviour
{
    [SerializeField] private UpgradeSkill staminaScriptableObject;
    private TapHoldControl _tapHoldControl;
    private bool _isTap;
    private SkillManager _skillManager;
    
    private void Start()
    {
        _skillManager = SkillManager.Instance;
        _tapHoldControl = GetComponent<TapHoldControl>();

    }

    private void Update()
    {
        if (_tapHoldControl.dead) return;
        _skillManager.stamina =
            _isTap
                ? Mathf.MoveTowards(_skillManager.stamina, 0, staminaScriptableObject.lerp)
                : Mathf.MoveTowards(_skillManager.stamina, _skillManager.tempStamina,
                    staminaScriptableObject.lerp * 1.5f);
    }

    public void StaminaDown()
    {
        _isTap = true;
        UIManager.Instance.GameIsStart();
    }

    public void StaminaUp()
    {
     if(_tapHoldControl.isButton)   _skillManager.stamina -= _skillManager.tempStamina / 10;
        _isTap = false;
    }
}