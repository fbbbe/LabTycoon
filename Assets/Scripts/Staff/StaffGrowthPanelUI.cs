using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StaffLevelUpPanelSpriteData
{
    public StaffType staffType;
    public int currentLevel;
    public Sprite panelSprite;
}

public class StaffGrowthPanelUI : MonoBehaviour
{
    public static StaffGrowthPanelUI Instance;

    [Header("레벨업 패널")]
    public GameObject levelUpPanelRoot;
    public Image levelUpBackgroundImage;
    public Button levelUpConfirmButton;
    public Button levelUpCloseButton;

    [Header("레벨업 PNG 매핑")]
    public StaffLevelUpPanelSpriteData[] levelUpPanelSprites;

    private StaffWorker selectedStaff;
    private StaffLevelUpData currentLevelUpData;
    private StaffHireData currentNextLevelStaffData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (levelUpConfirmButton != null)
        {
            levelUpConfirmButton.onClick.RemoveAllListeners();
            levelUpConfirmButton.onClick.AddListener(OnClickConfirmLevelUp);
        }

        if (levelUpCloseButton != null)
        {
            levelUpCloseButton.onClick.RemoveAllListeners();
            levelUpCloseButton.onClick.AddListener(CloseLevelUpPanel);
        }

        CloseLevelUpPanel();
    }

    public void OpenLevelUpPanel(StaffWorker staff)
    {
        if (staff == null)
        {
            Debug.LogWarning("StaffGrowthPanelUI: 선택된 인력이 없습니다.");
            return;
        }

        if (StaffDatabase.Instance == null)
        {
            Debug.LogWarning("StaffGrowthPanelUI: StaffDatabase.Instance가 없습니다.");
            return;
        }

        StaffType staffType = GetStaffType(staff);
        int currentLevel = GetStaffLevel(staff);

        currentLevelUpData = StaffDatabase.Instance.GetLevelUpData(staffType, currentLevel);

        if (currentLevelUpData == null)
        {
            Debug.Log("레벨업 정보가 없습니다. 현재 인력: " + staffType + " Lv." + currentLevel);
            return;
        }

        currentNextLevelStaffData = StaffDatabase.Instance.GetStaffData(staffType, currentLevelUpData.nextLevel);

        if (currentNextLevelStaffData == null)
        {
            Debug.LogWarning("다음 레벨 인력 데이터가 없습니다. 현재 인력: " + staffType + " Lv." + currentLevel);
            return;
        }

        selectedStaff = staff;

        Sprite panelSprite = GetLevelUpPanelSprite(staffType, currentLevel);

        if (levelUpBackgroundImage != null)
        {
            levelUpBackgroundImage.sprite = panelSprite;
        }

        if (levelUpPanelRoot != null)
        {
            levelUpPanelRoot.SetActive(true);
        }
    }

    public void CloseLevelUpPanel()
    {
        selectedStaff = null;
        currentLevelUpData = null;
        currentNextLevelStaffData = null;

        if (levelUpPanelRoot != null)
        {
            levelUpPanelRoot.SetActive(false);
        }
    }

    private void OnClickConfirmLevelUp()
    {
        if (selectedStaff == null || currentLevelUpData == null || currentNextLevelStaffData == null)
        {
            Debug.LogWarning("레벨업할 인력 또는 레벨업 데이터가 없습니다.");
            return;
        }

        if (CanLevelUp(selectedStaff, currentLevelUpData) == false)
        {
            return;
        }

        if (TrySpendMoney(currentLevelUpData.cost) == false)
        {
            Debug.Log("레벨업 비용이 부족합니다. 필요 금액: " + currentLevelUpData.cost);
            return;
        }

        ApplyLevelUp(selectedStaff, currentNextLevelStaffData);
        ResetCompletedTaskCount(selectedStaff);

        if (StaffStatusPanelUI.Instance != null)
        {
            StaffStatusPanelUI.Instance.Refresh();
        }

        Debug.Log("레벨업 완료: " + selectedStaff.staffName + " Lv." + currentNextLevelStaffData.level);

        CloseLevelUpPanel();
    }

    private bool CanLevelUp(StaffWorker staff, StaffLevelUpData levelUpData)
    {
        int completedTaskCount = GetCompletedTaskCount(staff);

        if (completedTaskCount < levelUpData.requiredCompletedTaskCount)
        {
            Debug.Log("레벨업 불가: 과제 수행 횟수 부족 " + completedTaskCount + " / " + levelUpData.requiredCompletedTaskCount);
            return false;
        }

        int currentLabLevel = GetCurrentLabLevel();

        if (currentLabLevel < levelUpData.requiredLabLevel)
        {
            Debug.Log("레벨업 불가: 연구실 레벨 부족 Lv." + currentLabLevel + " / 필요 Lv." + levelUpData.requiredLabLevel);
            return false;
        }

        return true;
    }

    private void ApplyLevelUp(StaffWorker staff, StaffHireData nextData)
    {
        if (staff == null || nextData == null)
        {
            return;
        }

        staff.level = nextData.level;
        staff.researchPower = nextData.researchPower;

        if (staff.runtimeData != null)
        {
            staff.runtimeData.level = nextData.level;
            staff.runtimeData.researchPower = nextData.researchPower;
        }
    }

    private Sprite GetLevelUpPanelSprite(StaffType staffType, int currentLevel)
    {
        if (levelUpPanelSprites == null)
        {
            return null;
        }

        for (int i = 0; i < levelUpPanelSprites.Length; i++)
        {
            StaffLevelUpPanelSpriteData data = levelUpPanelSprites[i];

            if (data == null)
            {
                continue;
            }

            if (data.staffType == staffType && data.currentLevel == currentLevel)
            {
                return data.panelSprite;
            }
        }

        Debug.LogWarning("레벨업 PNG 매핑이 없습니다: " + staffType + " Lv." + currentLevel);
        return null;
    }

    private StaffType GetStaffType(StaffWorker staff)
    {
        if (staff.runtimeData != null)
        {
            return staff.runtimeData.staffType;
        }

        return staff.staffType;
    }

    private int GetStaffLevel(StaffWorker staff)
    {
        if (staff.runtimeData != null)
        {
            return staff.runtimeData.level;
        }

        return staff.level;
    }

    private int GetCompletedTaskCount(StaffWorker staff)
    {
        if (staff == null)
        {
            return 0;
        }

        object runtimeData = staff.runtimeData;
        object target = runtimeData != null ? runtimeData : staff;

        string[] names =
        {
            "completedTaskCount",
            "completedTasksCount",
            "taskCompletedCount",
            "taskCount",
            "completedAssignmentCount"
        };

        for (int i = 0; i < names.Length; i++)
        {
            if (TryGetIntValue(target, names[i], out int value))
            {
                return value;
            }
        }

        Debug.LogWarning("과제 수행 횟수 변수를 찾지 못했습니다. StaffRuntimeData의 카운트 변수명을 확인하세요.");
        return 0;
    }

    private void ResetCompletedTaskCount(StaffWorker staff)
    {
        if (staff == null)
        {
            return;
        }

        object runtimeData = staff.runtimeData;
        object target = runtimeData != null ? runtimeData : staff;

        string[] names =
        {
            "completedTaskCount",
            "completedTasksCount",
            "taskCompletedCount",
            "taskCount",
            "completedAssignmentCount"
        };

        bool resetDone = false;

        for (int i = 0; i < names.Length; i++)
        {
            if (TrySetIntValue(target, names[i], 0))
            {
                resetDone = true;
            }
        }

        if (runtimeData != null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                TrySetIntValue(staff, names[i], 0);
            }
        }

        if (resetDone == false)
        {
            Debug.LogWarning("과제 수행 횟수 초기화 실패: 카운트 변수명을 찾지 못했습니다.");
        }
    }

    private int GetCurrentLabLevel()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager.Instance가 없어서 연구실 레벨을 1로 처리합니다.");
            return 1;
        }

        object manager = ResourceManager.Instance;

        string[] names =
        {
            "labLevel",
            "currentLabLevel",
            "level",
            "researchLabLevel"
        };

        for (int i = 0; i < names.Length; i++)
        {
            if (TryGetIntValue(manager, names[i], out int value))
            {
                return value;
            }
        }

        Debug.LogWarning("ResourceManager에서 연구실 레벨 변수를 찾지 못했습니다. 연구실 레벨을 1로 처리합니다.");
        return 1;
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

        string[] moneyNames = { "money", "currentMoney", "Money", "CurrentMoney" };

        for (int i = 0; i < moneyNames.Length; i++)
        {
            if (TryGetLongValue(manager, moneyNames[i], out long currentMoney))
            {
                if (currentMoney < cost)
                {
                    return false;
                }

                TrySetLongValue(manager, moneyNames[i], currentMoney - cost);
                return true;
            }
        }

        Debug.LogWarning("ResourceManager에서 돈 차감 함수/변수를 찾지 못했습니다. 비용 차감을 건너뜁니다.");
        return true;
    }

    private bool TryGetIntValue(object target, string name, out int value)
    {
        value = 0;

        if (target == null)
        {
            return false;
        }

        Type type = target.GetType();
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            value = Convert.ToInt32(field.GetValue(target));
            return true;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanRead)
        {
            value = Convert.ToInt32(property.GetValue(target));
            return true;
        }

        return false;
    }

    private bool TrySetIntValue(object target, string name, int value)
    {
        if (target == null)
        {
            return false;
        }

        Type type = target.GetType();
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(target, Convert.ChangeType(value, field.FieldType));
            return true;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(target, Convert.ChangeType(value, property.PropertyType));
            return true;
        }

        return false;
    }

    private bool TryGetLongValue(object target, string name, out long value)
    {
        value = 0;

        if (target == null)
        {
            return false;
        }

        Type type = target.GetType();
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            value = Convert.ToInt64(field.GetValue(target));
            return true;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanRead)
        {
            value = Convert.ToInt64(property.GetValue(target));
            return true;
        }

        return false;
    }

    private bool TrySetLongValue(object target, string name, long value)
    {
        if (target == null)
        {
            return false;
        }

        Type type = target.GetType();
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(target, Convert.ChangeType(value, field.FieldType));
            return true;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(target, Convert.ChangeType(value, property.PropertyType));
            return true;
        }

        return false;
    }
}
