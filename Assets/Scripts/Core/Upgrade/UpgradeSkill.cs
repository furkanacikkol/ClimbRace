using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeType",menuName = "Upgrade Skill")]
public class UpgradeSkill : ScriptableObject
{
    public float firstValue = 1;
    public float increaseValue = 0.1f;
    public float firstMoneyValue = 100;
    public float increaseMoneyValue = 1.5f;
    public float lerp;
}
