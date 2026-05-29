using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 인력 휴식 상태와 스트레스 감소 처리를 담당합니다.
///
/// 돈 차감과 스트레스 값 접근은 프로젝트마다 변수명이 조금씩 달라질 수 있어서
/// Reflection으로 최대한 안전하게 처리합니다.
/// </summary>
public class StaffRestController : MonoBehaviour
{
    public static StaffRestController Instance;

    [Header("휴식 행동 데이터 8개")]
    public RestActionData[] restActions = new RestActionData[8];

    [Header("스트레스 초과 행동 불가 설정")]
    [Tooltip("스트레스 100 이상 도달 시 행동 불가가 유지되는 시간입니다.")]
    public float stressOverloadDuration = 60f;

    [Tooltip("행동 불가 시간이 끝난 뒤 초기화할 스트레스 값입니다.")]
    public int stressResetValueAfterOverload = 50;

    private readonly HashSet<StaffWorker> restingStaffSet = new HashSet<StaffWorker>();
    private readonly HashSet<StaffWorker> stressOverloadStaffSet = new HashSet<StaffWorker>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetupDefaultRestActionsIfNeeded();
    }

    private void SetupDefaultRestActionsIfNeeded()
    {
        if (restActions == null || restActions.Length != 8)
        {
            restActions = new RestActionData[8];
        }

        SetDefaultAction(0, "물 마시기", 10f, 20, 0, false, false);
        SetDefaultAction(1, "짧은 휴식", 20f, 40, 0, false, false);
        SetDefaultAction(2, "고양이랑 놀기", 30f, 60, 0, false, false);
        SetDefaultAction(3, "낮잠", 60f, 100, 0, false, false);
        SetDefaultAction(4, "간식 먹기", 0f, 20, 3000, true, false);
        SetDefaultAction(5, "식사하기", 5f, 50, 10000, false, false);
        SetDefaultAction(6, "너튜브 보기", 10f, 100, 30000, false, false);
        SetDefaultAction(7, "회식하기", 60f, 100, 150000, false, true);
    }

    private void SetDefaultAction(int index, string name, float duration, int stressDecrease, long cost, bool isInstant, bool applyToAllStaff)
    {
        if (restActions[index] == null)
        {
            restActions[index] = new RestActionData();
        }

        restActions[index].actionName = name;
        restActions[index].duration = duration;
        restActions[index].stressDecrease = stressDecrease;
        restActions[index].cost = cost;
        restActions[index].isInstant = isInstant;
        restActions[index].applyToAllStaff = applyToAllStaff;
    }

    public bool IsStaffResting(StaffWorker staff)
    {
        return staff != null && restingStaffSet.Contains(staff);
    }

    /// <summary>
    /// 스트레스 100 이상으로 자동 행동 불가 상태인지 확인한다.
    /// </summary>
    public bool IsStaffStressOverloaded(StaffWorker staff)
    {
        return staff != null && stressOverloadStaffSet.Contains(staff);
    }

    /// <summary>
    /// 휴식 중이거나 스트레스 초과 상태인지 확인한다.
    /// 버튼 클릭 차단 자체는 RestIconImage가 위를 덮어서 Unity UI에서 처리하지만,
    /// 다른 시스템에서 상태 확인이 필요할 때 사용할 수 있게 public으로 둔다.
    /// </summary>
    public bool IsStaffActionBlocked(StaffWorker staff)
    {
        return IsStaffResting(staff) || IsStaffStressOverloaded(staff);
    }

    /// <summary>
    /// 스트레스가 변경된 직후 호출한다.
    /// 현재 스트레스가 100 이상이면 자동으로 60초 행동 불가 상태에 들어간다.
    /// </summary>
    public void CheckStressOverload(StaffWorker staff)
    {
        if (staff == null)
        {
            return;
        }

        if (IsStaffStressOverloaded(staff))
        {
            return;
        }

        int currentStress = GetCurrentStress(staff);

        if (currentStress >= 100)
        {
            StartCoroutine(StressOverloadRoutine(staff));
        }
    }

    public void StartRestAction(StaffWorker selectedStaff, int actionIndex, Sprite restIcon)
    {
        if (actionIndex < 0 || actionIndex >= restActions.Length)
        {
            Debug.LogWarning("휴식 행동 인덱스가 잘못되었습니다: " + actionIndex);
            return;
        }

        RestActionData actionData = restActions[actionIndex];

        if (actionData == null)
        {
            Debug.LogWarning("휴식 행동 데이터가 없습니다: " + actionIndex);
            return;
        }

        List<StaffWorker> targetStaffList = actionData.applyToAllStaff ? GetAllSeatedStaff() : new List<StaffWorker> { selectedStaff };

        if (targetStaffList.Count == 0 || targetStaffList[0] == null)
        {
            Debug.LogWarning("휴식할 인력이 없습니다.");
            return;
        }

        for (int i = 0; i < targetStaffList.Count; i++)
        {
            if (IsStaffResting(targetStaffList[i]))
            {
                Debug.Log("이미 휴식 중인 인력이 포함되어 있습니다: " + targetStaffList[i].name);
                return;
            }

            if (IsStaffStressOverloaded(targetStaffList[i]))
            {
                Debug.Log("스트레스 초과로 행동 불가 상태인 인력이 포함되어 있습니다: " + targetStaffList[i].name);
                return;
            }
        }

        if (TrySpendMoney(actionData.cost) == false)
        {
            Debug.Log("휴식 비용이 부족합니다. 필요 금액: " + actionData.cost);
            return;
        }

        ClosePanels();

        if (actionData.isInstant)
        {
            for (int i = 0; i < targetStaffList.Count; i++)
            {
                ApplyStressDecrease(targetStaffList[i], actionData.stressDecrease);
                RefreshStaffStatus(targetStaffList[i]);
            }

            Debug.Log(actionData.actionName + " 즉시 적용 완료");
            return;
        }

        for (int i = 0; i < targetStaffList.Count; i++)
        {
            StartCoroutine(RestRoutine(targetStaffList[i], actionData, restIcon));
        }
    }

    private IEnumerator RestRoutine(StaffWorker staff, RestActionData actionData, Sprite restIcon)
    {
        if (staff == null)
        {
            yield break;
        }

        restingStaffSet.Add(staff);
        ShowRestIcon(staff, restIcon);

        yield return new WaitForSeconds(actionData.duration);

        ApplyStressDecrease(staff, actionData.stressDecrease);
        HideRestIcon(staff);
        restingStaffSet.Remove(staff);
        RefreshStaffStatus(staff);

        Debug.Log(actionData.actionName + " 완료: " + staff.name + " / 스트레스 -" + actionData.stressDecrease);
    }

    /// <summary>
    /// 스트레스가 100 이상이 되었을 때 자동으로 들어가는 행동 불가 상태입니다.
    /// 휴식 아이콘 표시 시스템을 그대로 재사용해서 기존 행동 버튼 위를 스트레스 초과 아이콘으로 덮습니다.
    /// </summary>
    private IEnumerator StressOverloadRoutine(StaffWorker staff)
    {
        if (staff == null)
        {
            yield break;
        }

        stressOverloadStaffSet.Add(staff);
        ShowStressOverloadIcon(staff);

        Debug.Log("스트레스 100 이상 도달: 행동 불가 상태 진입 - " + staff.name);

        yield return new WaitForSeconds(stressOverloadDuration);

        SetCurrentStress(staff, stressResetValueAfterOverload);
        HideStressOverloadIcon(staff);
        stressOverloadStaffSet.Remove(staff);
        RefreshStaffStatus(staff);

        Debug.Log("스트레스 초과 행동 불가 해제: " + staff.name + " / 스트레스 " + stressResetValueAfterOverload + "으로 초기화");
    }

    private void ClosePanels()
    {
        if (RestActionPanelUI.Instance != null)
        {
            RestActionPanelUI.Instance.Close();
        }

        if (StaffStatusPanelUI.Instance != null)
        {
            StaffStatusPanelUI.Instance.Close();
        }
    }

    private List<StaffWorker> GetAllSeatedStaff()
    {
        List<StaffWorker> result = new List<StaffWorker>();
        WorkstationObject[] workstations = FindObjectsOfType<WorkstationObject>();

        for (int i = 0; i < workstations.Length; i++)
        {
            if (workstations[i] != null && workstations[i].seatedStaff != null)
            {
                result.Add(workstations[i].seatedStaff);
            }
        }

        return result;
    }

    private WorkstationObject FindWorkstationByStaff(StaffWorker staff)
    {
        if (staff == null)
        {
            return null;
        }

        WorkstationObject[] workstations = FindObjectsOfType<WorkstationObject>();

        for (int i = 0; i < workstations.Length; i++)
        {
            if (workstations[i] != null && workstations[i].seatedStaff == staff)
            {
                return workstations[i];
            }
        }

        return null;
    }

    private void ShowRestIcon(StaffWorker staff, Sprite restIcon)
    {
        WorkstationObject workstation = FindWorkstationByStaff(staff);

        if (workstation == null)
        {
            return;
        }

        WorkstationWorldUI worldUI = workstation.GetComponentInChildren<WorkstationWorldUI>(true);

        if (worldUI != null)
        {
            worldUI.ShowRestIcon(restIcon);
        }
    }

    private void HideRestIcon(StaffWorker staff)
    {
        WorkstationObject workstation = FindWorkstationByStaff(staff);

        if (workstation == null)
        {
            return;
        }

        WorkstationWorldUI worldUI = workstation.GetComponentInChildren<WorkstationWorldUI>(true);

        if (worldUI != null)
        {
            worldUI.HideRestIcon();
        }
    }

    /// <summary>
    /// Workstation 프리팹의 WorldUI 안에 이미 연결되어 있는 스트레스 초과 이미지를 표시한다.
    /// 이 컨트롤러에서는 Sprite를 따로 들고 있지 않는다.
    /// </summary>
    private void ShowStressOverloadIcon(StaffWorker staff)
    {
        WorkstationObject workstation = FindWorkstationByStaff(staff);

        if (workstation == null)
        {
            return;
        }

        WorkstationWorldUI worldUI = workstation.GetComponentInChildren<WorkstationWorldUI>(true);

        if (worldUI != null)
        {
            worldUI.ShowStressOverloadIcon(null);
        }
    }

    /// <summary>
    /// Workstation 프리팹의 WorldUI 안에 있는 스트레스 초과 이미지를 숨긴다.
    /// </summary>
    private void HideStressOverloadIcon(StaffWorker staff)
    {
        WorkstationObject workstation = FindWorkstationByStaff(staff);

        if (workstation == null)
        {
            return;
        }

        WorkstationWorldUI worldUI = workstation.GetComponentInChildren<WorkstationWorldUI>(true);

        if (worldUI != null)
        {
            worldUI.HideStressOverloadIcon();
        }
    }

    private void RefreshStaffStatus(StaffWorker staff)
    {
        WorkstationObject workstation = FindWorkstationByStaff(staff);

        if (workstation != null)
        {
            WorkstationWorldUI worldUI = workstation.GetComponentInChildren<WorkstationWorldUI>(true);

            if (worldUI != null)
            {
                worldUI.RefreshStressText();
            }
        }

        if (StaffStatusPanelUI.Instance != null)
        {
            StaffStatusPanelUI.Instance.Refresh();
        }
    }

    private int GetCurrentStress(StaffWorker staff)
    {
        if (staff == null)
        {
            return 0;
        }

        object runtimeData = GetFieldOrPropertyValue(staff, "runtimeData");
        object target = runtimeData != null ? runtimeData : staff;

        return GetIntValue(target, "currentStress", GetIntValue(staff, "currentStress", 0));
    }

    private void SetCurrentStress(StaffWorker staff, int value)
    {
        if (staff == null)
        {
            return;
        }

        object runtimeData = GetFieldOrPropertyValue(staff, "runtimeData");
        object target = runtimeData != null ? runtimeData : staff;

        int maxStress = GetIntValue(target, "maxStress", 100);
        int newStress = Mathf.Clamp(value, 0, maxStress);

        SetIntValue(target, "currentStress", newStress);

        if (runtimeData != null)
        {
            SetIntValue(staff, "currentStress", newStress);
        }
    }

    private void ApplyStressDecrease(StaffWorker staff, int amount)
    {
        if (staff == null)
        {
            return;
        }
        int currentStress = GetCurrentStress(staff);
        SetCurrentStress(staff, currentStress - amount);
    }

    private bool TrySpendMoney(long cost)
    {
        if (cost <= 0)
        {
            return true;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager.Instance가 없어서 비용 차감을 건너뜁니다.");
            return true;
        }

        object manager = ResourceManager.Instance;
        Type type = manager.GetType();

        string[] methodNames = { "TrySpendMoney", "SpendMoney", "UseMoney", "PayMoney" };

        for (int i = 0; i < methodNames.Length; i++)
        {
            MethodInfo method = type.GetMethod(methodNames[i], BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
            {
                continue;
            }

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != 1)
            {
                continue;
            }

            object argument = Convert.ChangeType(cost, parameters[0].ParameterType);
            object result = method.Invoke(manager, new object[] { argument });

            if (method.ReturnType == typeof(bool))
            {
                return (bool)result;
            }

            return true;
        }

        string[] moneyFieldNames = { "money", "currentMoney", "Money", "CurrentMoney" };

        for (int i = 0; i < moneyFieldNames.Length; i++)
        {
            FieldInfo field = type.GetField(moneyFieldNames[i], BindingFlags.Public | BindingFlags.Instance);

            if (field != null)
            {
                long currentMoney = Convert.ToInt64(field.GetValue(manager));

                if (currentMoney < cost)
                {
                    return false;
                }

                field.SetValue(manager, Convert.ChangeType(currentMoney - cost, field.FieldType));
                return true;
            }

            PropertyInfo property = type.GetProperty(moneyFieldNames[i], BindingFlags.Public | BindingFlags.Instance);

            if (property != null && property.CanRead && property.CanWrite)
            {
                long currentMoney = Convert.ToInt64(property.GetValue(manager));

                if (currentMoney < cost)
                {
                    return false;
                }

                property.SetValue(manager, Convert.ChangeType(currentMoney - cost, property.PropertyType));
                return true;
            }
        }

        Debug.LogWarning("ResourceManager에서 돈 차감 함수/변수를 찾지 못했습니다. 비용 차감을 건너뜁니다.");
        return true;
    }

    private object GetFieldOrPropertyValue(object target, string name)
    {
        if (target == null)
        {
            return null;
        }

        Type type = target.GetType();
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanRead)
        {
            return property.GetValue(target);
        }

        return null;
    }

    private int GetIntValue(object target, string name, int defaultValue)
    {
        object value = GetFieldOrPropertyValue(target, name);

        if (value == null)
        {
            return defaultValue;
        }

        return Convert.ToInt32(value);
    }

    private void SetIntValue(object target, string name, int value)
    {
        if (target == null)
        {
            return;
        }

        Type type = target.GetType();
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
        }
    }
}