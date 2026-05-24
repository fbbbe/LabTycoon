using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설치된 장비들의 효과를 관리하는 시스템.
/// 
/// 중요한 원칙:
/// - 구매 버튼 클릭 시 효과 적용하지 않음
/// - 배치 완료 시 효과 등록
/// - 장비 제거 시 효과 제거
/// </summary>
public class EquipmentEffectManager : MonoBehaviour
{
    public static EquipmentEffectManager Instance;

    private List<EquipmentData> globalEquipments = new List<EquipmentData>();

    private Dictionary<WorkstationObject, EquipmentData> workstationComputerEquipment =
        new Dictionary<WorkstationObject, EquipmentData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 연구 장비, 환경 장비, 커피 장비, 청소 장비처럼 전역 효과를 주는 장비 등록.
    /// </summary>
    public void RegisterGlobalEquipment(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        if (globalEquipments.Contains(equipmentData))
        {
            return;
        }

        globalEquipments.Add(equipmentData);
    }

    /// <summary>
    /// 전역 장비 효과 제거.
    /// 장비 철거 기능에서 사용 예정.
    /// </summary>
    public void UnregisterGlobalEquipment(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        globalEquipments.Remove(equipmentData);
    }

    /// <summary>
    /// Workstation에 설치된 컴퓨터 장비 등록.
    /// 이 효과는 해당 Workstation을 사용하는 인력에게만 적용된다.
    /// </summary>
    public void RegisterWorkstationComputer(WorkstationObject workstation, EquipmentData equipmentData)
    {
        if (workstation == null || equipmentData == null)
        {
            return;
        }

        if (workstationComputerEquipment.ContainsKey(workstation))
        {
            workstationComputerEquipment[workstation] = equipmentData;
        }
        else
        {
            workstationComputerEquipment.Add(workstation, equipmentData);
        }
    }

    /// <summary>
    /// 특정 Workstation의 컴퓨터 장비 효과 제거.
    /// </summary>
    public void UnregisterWorkstationComputer(WorkstationObject workstation)
    {
        if (workstation == null)
        {
            return;
        }

        if (workstationComputerEquipment.ContainsKey(workstation))
        {
            workstationComputerEquipment.Remove(workstation);
        }
    }

    /// <summary>
    /// 특정 Workstation을 쓰는 인력에게만 적용되는 돈 보상 증가율.
    /// 컴퓨터 장비 효과.
    /// </summary>
    public float GetPersonalMoneyRewardBonus(WorkstationObject workstation)
    {
        if (workstation == null)
        {
            return 0f;
        }

        if (workstationComputerEquipment.TryGetValue(workstation, out EquipmentData equipmentData) == false)
        {
            return 0f;
        }

        return GetEffectValueFromEquipment(
            equipmentData,
            EquipmentEffectScope.PersonalStaff,
            EquipmentEffectType.MoneyRewardBonus
        );
    }

    /// <summary>
    /// 전체 돈 보상 증가율.
    /// 커피 장비 효과 등이 여기에 해당한다.
    /// </summary>
    public float GetGlobalMoneyRewardBonus()
    {
        return GetGlobalEffectValue(
            EquipmentEffectScope.GlobalMoneyReward,
            EquipmentEffectType.MoneyRewardBonus
        );
    }

    /// <summary>
    /// 전체 연구성과 증가율.
    /// 연구 장비 효과.
    /// </summary>
    public float GetGlobalResearchResultBonus()
    {
        return GetGlobalEffectValue(
            EquipmentEffectScope.GlobalResearchResult,
            EquipmentEffectType.ResearchResultBonus
        );
    }

    /// <summary>
    /// 전체 스트레스 증가량 감소율.
    /// 환경 장비 효과.
    /// </summary>
    public float GetGlobalStressReduction()
    {
        return GetGlobalEffectValue(
            EquipmentEffectScope.GlobalStress,
            EquipmentEffectType.StressIncreaseReduction
        );
    }

    /// <summary>
    /// 전체 인력에게 적용되는 추가 스트레스.
    /// 커피 장비 패널티.
    /// </summary>
    public float GetAllStaffExtraStress()
    {
        return GetGlobalEffectValue(
            EquipmentEffectScope.AllStaffPenalty,
            EquipmentEffectType.ExtraStressIncrease
        );
    }

    /// <summary>
    /// 청소 시간 감소량.
    /// 청소 장비 효과.
    /// </summary>
    public float GetCleaningTimeReduction()
    {
        return GetGlobalEffectValue(
            EquipmentEffectScope.GlobalCleaning,
            EquipmentEffectType.CleaningTimeReduction
        );
    }

    private float GetGlobalEffectValue(EquipmentEffectScope scope, EquipmentEffectType type)
    {
        float total = 0f;

        for (int i = 0; i < globalEquipments.Count; i++)
        {
            total += GetEffectValueFromEquipment(globalEquipments[i], scope, type);
        }

        return total;
    }

    private float GetEffectValueFromEquipment(
        EquipmentData equipmentData,
        EquipmentEffectScope scope,
        EquipmentEffectType type
    )
    {
        if (equipmentData == null || equipmentData.effects == null)
        {
            return 0f;
        }

        float total = 0f;

        for (int i = 0; i < equipmentData.effects.Count; i++)
        {
            EquipmentEffectData effect = equipmentData.effects[i];

            if (effect == null)
            {
                continue;
            }

            if (effect.scope == scope && effect.effectType == type)
            {
                total += effect.value;
            }
        }

        return total;
    }
}