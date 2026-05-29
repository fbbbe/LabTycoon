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

    private readonly HashSet<StaffWorker> restingStaffSet = new HashSet<StaffWorker>();

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

    private void ApplyStressDecrease(StaffWorker staff, int amount)
    {
        if (staff == null)
        {
            return;
        }

        object runtimeData = GetFieldOrPropertyValue(staff, "runtimeData");
        object target = runtimeData != null ? runtimeData : staff;

        int currentStress = GetIntValue(target, "currentStress", GetIntValue(staff, "currentStress", 0));
        int maxStress = GetIntValue(target, "maxStress", 100);
        int newStress = Mathf.Clamp(currentStress - amount, 0, maxStress);

        SetIntValue(target, "currentStress", newStress);

        if (runtimeData != null)
        {
            SetIntValue(staff, "currentStress", newStress);
        }
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