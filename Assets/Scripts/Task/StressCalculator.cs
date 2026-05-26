using UnityEngine;

/// <summary>
/// 인력의 스트레스 증가량을 계산하는 클래스.
/// 
/// 이 클래스는 MonoBehaviour가 아니다.
/// 과제 수행, 청소, 커피 패널티 같은 스트레스 계산에 사용한다.
/// 
/// 스트레스 계산 기준:
/// 1. 기본 스트레스 증가량
/// 2. 환경 장비의 전체 스트레스 감소 효과
/// 3. 커피 장비의 전체 인력 스트레스 패널티
/// </summary>
public static class StressCalculator
{
    /// <summary>
    /// 과제 수행으로 인한 최종 스트레스 증가량을 계산한다.
    /// </summary>
    public static int CalculateTaskStress(int baseTaskStress)
    {
        float stressReductionRate = 0f;
        float extraStress = 0f;

        if (EquipmentEffectManager.Instance != null)
        {
            // 환경 장비 효과
            stressReductionRate = EquipmentEffectManager.Instance.GetGlobalStressReduction();

            // 커피 장비 패널티
            extraStress = EquipmentEffectManager.Instance.GetAllStaffExtraStress();
        }

        float finalStress = baseTaskStress;

        finalStress *= 1f - stressReductionRate;
        finalStress += extraStress;

        if (finalStress < 0)
        {
            finalStress = 0;
        }

        return Mathf.RoundToInt(finalStress);
    }

    /// <summary>
    /// 청소로 인한 최종 스트레스 증가량을 계산한다.
    /// 
    /// 청소 스트레스도 환경 장비의 스트레스 감소 효과를 받을 수 있다.
    /// </summary>
    public static int CalculateCleaningStress(int baseCleaningStress)
    {
        float stressReductionRate = 0f;

        if (EquipmentEffectManager.Instance != null)
        {
            stressReductionRate = EquipmentEffectManager.Instance.GetGlobalStressReduction();
        }

        float finalStress = baseCleaningStress;
        finalStress *= 1f - stressReductionRate;

        if (finalStress < 0)
        {
            finalStress = 0;
        }

        return Mathf.RoundToInt(finalStress);
    }

    /// <summary>
    /// 청소 시간을 계산한다.
    /// 
    /// 청소 장비가 있으면 청소 시간이 감소한다.
    /// </summary>
    public static float CalculateCleaningTime(float baseCleaningTime)
    {
        float reduction = 0f;

        if (EquipmentEffectManager.Instance != null)
        {
            reduction = EquipmentEffectManager.Instance.GetCleaningTimeReduction();
        }

        float finalTime = baseCleaningTime - reduction;

        if (finalTime < 1f)
        {
            finalTime = 1f;
        }

        return finalTime;
    }
}