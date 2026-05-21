using UnityEngine;

/// <summary>
/// 장비 효과 하나를 나타내는 데이터.
/// 
/// 예:
/// - MoneyRewardBonus, 0.02
/// - ResearchResultBonus, 0.03
/// - CleaningTimeReduction, 1
/// - ExtraStressIncrease, 1
/// </summary>
[System.Serializable]
public class EquipmentEffectData
{
    [Tooltip("효과 종류입니다.")]
    public EquipmentEffectType effectType;

    [Tooltip("효과 값입니다. 비율 효과는 0.02 = 2%, 시간 효과는 1 = 1초처럼 사용합니다.")]
    public float value;

    public EquipmentEffectData(EquipmentEffectType effectType, float value)
    {
        this.effectType = effectType;
        this.value = value;
    }
}