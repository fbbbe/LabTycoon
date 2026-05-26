using UnityEngine;

/// <summary>
/// 장비 효과 하나를 나타내는 데이터.
/// 
/// 예:
/// 컴퓨터 장비:
/// PersonalStaff + MoneyRewardBonus + 0.02
/// 
/// 연구 장비:
/// GlobalResearchResult + ResearchResultBonus + 0.03
/// 
/// 커피 장비:
/// GlobalMoneyReward + MoneyRewardBonus + 0.03
/// AllStaffPenalty + ExtraStressIncrease + 2
/// </summary>
[System.Serializable]
public class EquipmentEffectData
{
    public EquipmentEffectScope scope;
    public EquipmentEffectType effectType;
    public float value;

    public EquipmentEffectData(EquipmentEffectScope scope, EquipmentEffectType effectType, float value)
    {
        this.scope = scope;
        this.effectType = effectType;
        this.value = value;
    }
}