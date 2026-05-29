using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 연구실 확장 단계 데이터입니다.
///
/// 이 방식은 코드가 타일 위치를 계산해서 새로 만드는 방식이 아닙니다.
/// Unity Hierarchy 안에 사용자가 직접 배치해 둔 확장 구간 오브젝트를
/// 조건 충족 시 Active ON 하는 방식입니다.
///
/// 예시:
/// - 기본 9평은 처음부터 켜져 있음
/// - 9평 -> 18평 확장 시 stageRoot에 연결된 "추가 9평 오브젝트"만 켬
/// - 18평 -> 27평 확장 시 다음 "추가 9평 오브젝트"를 켬
/// </summary>
[Serializable]
public class LabExpansionData
{
    [Header("확장 정보")]
    public int currentArea;
    public int nextArea;

    [Header("조건")]
    public int requiredLabLevel;
    public long cost;

    [Header("Unity에서 직접 배치한 추가 구간")]
    [Tooltip("이 확장 단계에서 새로 켤 타일/벽지 묶음 오브젝트입니다. 사용자가 Unity에서 직접 배치합니다.")]
    public GameObject stageRoot;
}

public class LabExpansionManager : MonoBehaviour
{
    public static LabExpansionManager Instance;

    [Header("확장 단계 데이터")]
    public LabExpansionData[] expansionStages;

    [Header("현재 확장 상태")]
    [Tooltip("현재 완료된 확장 단계 인덱스입니다. -1이면 기본 9평 상태입니다.")]
    public int currentExpansionIndex = -1;

    [Tooltip("기본 연구실 평수입니다.")]
    public int baseArea = 9;

    [Tooltip("게임 시작 시 아직 확장되지 않은 stageRoot들을 자동으로 꺼둘지 여부입니다.")]
    public bool disableLockedStagesOnStart = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (expansionStages == null || expansionStages.Length == 0)
        {
            CreateDefaultExpansionStages();
        }

        ApplyStageActiveStates();
    }

    private void CreateDefaultExpansionStages()
    {
        expansionStages = new LabExpansionData[]
        {
            CreateStage(9, 18, 10, 150000),
            CreateStage(18, 27, 20, 600000),
            CreateStage(27, 36, 30, 1500000),
            CreateStage(36, 45, 40, 4000000),
            CreateStage(45, 54, 50, 9000000),
            CreateStage(54, 63, 60, 20000000),
            CreateStage(63, 72, 70, 45000000),
            CreateStage(72, 90, 80, 90000000),
            CreateStage(90, 108, 90, 180000000),
            CreateStage(108, 135, 100, 400000000)
        };
    }

    private LabExpansionData CreateStage(
        int currentArea,
        int nextArea,
        int requiredLabLevel,
        long cost
    )
    {
        LabExpansionData data = new LabExpansionData();
        data.currentArea = currentArea;
        data.nextArea = nextArea;
        data.requiredLabLevel = requiredLabLevel;
        data.cost = cost;
        data.stageRoot = null;

        return data;
    }

    public int GetCurrentArea()
    {
        if (currentExpansionIndex < 0)
        {
            return baseArea;
        }

        if (expansionStages == null || expansionStages.Length == 0)
        {
            return baseArea;
        }

        int clampedIndex = Mathf.Clamp(currentExpansionIndex, 0, expansionStages.Length - 1);
        LabExpansionData currentStage = expansionStages[clampedIndex];

        if (currentStage == null)
        {
            return baseArea;
        }

        return currentStage.nextArea;
    }

    public LabExpansionData GetNextExpansionData()
    {
        if (expansionStages == null || expansionStages.Length == 0)
        {
            return null;
        }

        int nextIndex = currentExpansionIndex + 1;

        if (nextIndex < 0 || nextIndex >= expansionStages.Length)
        {
            return null;
        }

        return expansionStages[nextIndex];
    }

    public bool TryExpandLab()
    {
        LabExpansionData data = GetNextExpansionData();

        if (data == null)
        {
            Debug.Log("더 이상 확장할 수 없습니다. 현재 평수: " + GetCurrentArea());
            return false;
        }

        int actualCurrentArea = GetCurrentArea();

        if (actualCurrentArea != data.currentArea)
        {
            Debug.LogWarning(
                "연구실 확장 데이터 불일치: 현재 평수 " + actualCurrentArea +
                " / 다음 확장 데이터의 시작 평수 " + data.currentArea +
                "입니다. currentExpansionIndex 또는 expansionStages 순서를 확인하세요."
            );
        }

        int currentLabLevel = GetCurrentLabLevel();

        if (currentLabLevel < data.requiredLabLevel)
        {
            Debug.Log(
                "연구실 확장 불가: 연구실 레벨 부족 Lv." +
                currentLabLevel + " / 필요 Lv." + data.requiredLabLevel
            );
            return false;
        }

        if (TrySpendMoney(data.cost) == false)
        {
            Debug.Log("연구실 확장 불가: 비용 부족 / 필요 금액 " + data.cost.ToString("N0") + "원");
            return false;
        }

        if (data.stageRoot == null)
        {
            Debug.LogWarning(
                "연구실 확장 실패: " + data.currentArea + "평 -> " + data.nextArea +
                "평 단계의 Stage Root가 비어 있습니다. Unity에서 직접 배치한 추가 타일/벽지 묶음을 연결하세요."
            );
            return false;
        }

        data.stageRoot.SetActive(true);
        currentExpansionIndex++;

        RefreshGridAfterManualExpansion();

        Debug.Log(
            "연구실 확장 성공: " +
            data.currentArea + "평 -> " + data.nextArea + "평"
        );

        return true;
    }

    private void ApplyStageActiveStates()
    {
        if (disableLockedStagesOnStart == false)
        {
            return;
        }

        if (expansionStages == null)
        {
            return;
        }

        for (int i = 0; i < expansionStages.Length; i++)
        {
            LabExpansionData data = expansionStages[i];

            if (data == null || data.stageRoot == null)
            {
                continue;
            }

            data.stageRoot.SetActive(i <= currentExpansionIndex);
        }
    }

    private void RefreshGridAfterManualExpansion()
    {
        if (LabGridManager.Instance == null)
        {
            return;
        }

        LabGridManager.Instance.SendMessage("RefreshTilesFromScene", SendMessageOptions.DontRequireReceiver);
        LabGridManager.Instance.SendMessage("RebuildTileCacheFromScene", SendMessageOptions.DontRequireReceiver);
        LabGridManager.Instance.SendMessage("RefreshLabTilesFromScene", SendMessageOptions.DontRequireReceiver);
        LabGridManager.Instance.SendMessage("ApplyEnvironmentFromCurrentLabLevel", SendMessageOptions.DontRequireReceiver);
        LabGridManager.Instance.SendMessage("HidePlacementGrid", SendMessageOptions.DontRequireReceiver);
    }

    private int GetCurrentLabLevel()
    {
        if (ResourceManager.Instance == null)
        {
            return 1;
        }

        return ResourceManager.Instance.labLevel;
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

        string[] methodNames =
        {
            "TrySpendMoney",
            "SpendMoney",
            "UseMoney",
            "PayMoney"
        };

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

        string[] moneyNames =
        {
            "money",
            "currentMoney",
            "Money",
            "CurrentMoney"
        };

        for (int i = 0; i < moneyNames.Length; i++)
        {
            FieldInfo field = type.GetField(moneyNames[i], BindingFlags.Public | BindingFlags.Instance);

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

            PropertyInfo property = type.GetProperty(moneyNames[i], BindingFlags.Public | BindingFlags.Instance);

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
}