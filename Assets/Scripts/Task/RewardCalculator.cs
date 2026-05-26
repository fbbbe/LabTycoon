using UnityEngine;

/// <summary>
/// 과제 완료 후 돈과 연구성과 보상을 계산하는 클래스.
/// 
/// 이 클래스는 MonoBehaviour가 아니다.
/// 씬 오브젝트에 붙이지 않고, 계산용 함수만 제공한다.
/// 
/// 보상 계산 기준:
/// 1. 기본 돈 보상
/// 2. 해당 인력이 사용하는 컴퓨터 장비 효과
/// 3. 전체 돈 보상 효과
/// 4. 기본 연구성과
/// 5. 전체 연구성과 증가 효과
/// </summary>
public static class RewardCalculator
{
    /// <summary>
    /// 최종 돈 보상을 계산한다.
    /// 
    /// 컴퓨터 장비 효과:
    /// - 해당 Workstation을 쓰는 인력에게만 적용
    /// 
    /// 커피 장비 등 전역 돈 보상 효과:
    /// - 전체 인력에게 적용
    /// </summary>
    public static int CalculateMoneyReward(int baseMoneyReward, StaffWorker staff)
    {
        if (staff == null)
        {
            return baseMoneyReward;
        }

        float personalBonus = 0f;
        float globalBonus = 0f;

        if (EquipmentEffectManager.Instance != null)
        {
            // 해당 인력이 앉아 있는 Workstation의 컴퓨터 장비 효과
            personalBonus = EquipmentEffectManager.Instance.GetPersonalMoneyRewardBonus(staff.currentWorkstation);

            // 커피 장비 등 전체 돈 보상 증가 효과
            globalBonus = EquipmentEffectManager.Instance.GetGlobalMoneyRewardBonus();
        }

        float finalReward = baseMoneyReward;

        finalReward *= 1f + personalBonus;
        finalReward *= 1f + globalBonus;

        return Mathf.RoundToInt(finalReward);
    }

    /// <summary>
    /// 최종 연구성과 보상을 계산한다.
    /// 
    /// 연구 장비 효과:
    /// - 게임 시스템 전체에 적용
    /// </summary>
    public static int CalculateResearchResult(int baseResearchResult)
    {
        float globalResearchBonus = 0f;

        if (EquipmentEffectManager.Instance != null)
        {
            globalResearchBonus = EquipmentEffectManager.Instance.GetGlobalResearchResultBonus();
        }

        float finalReward = baseResearchResult;
        finalReward *= 1f + globalResearchBonus;

        return Mathf.RoundToInt(finalReward);
    }
}