using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class LabExpansionData
{
    [Header("확장 정보")]
    public int currentArea;
    public int nextArea;

    [Header("조건")]
    public int requiredLabLevel;
    public long cost;

    [Header("추가할 타일 좌표")]
    [Tooltip("확장 시 새로 생성할 LabTile grid 좌표 목록입니다. 내부 좌표는 0부터 시작합니다.")]
    public Vector2Int[] tilesToAdd;
}

public class LabExpansionManager : MonoBehaviour
{
    public static LabExpansionManager Instance;

    [Header("확장 단계 데이터")]
    public LabExpansionData[] expansionStages;

    [Header("현재 확장 상태")]
    [Tooltip("-1이면 기본 9평 상태입니다.")]
    public int currentExpansionIndex = -1;

    [Tooltip("기본 연구실 평수입니다.")]
    public int baseArea = 9;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateDefaultExpansionStages();
    }

    private void CreateDefaultExpansionStages()
    {
        expansionStages = new LabExpansionData[]
        {
            // 9평 -> 18평
            CreateStage(
                9,
                18,
                10,
                150000,
                Rect1Based(1, 3, 4, 6)
            ),

            // 18평 -> 27평
            CreateStage(
                18,
                27,
                20,
                600000,
                Rect1Based(4, 6, 4, 6)
            ),

            // 27평 -> 36평
            CreateStage(
                27,
                36,
                30,
                1500000,
                Rect1Based(4, 6, 1, 3)
            ),

            // 36평 -> 45평
            CreateStage(
                36,
                45,
                40,
                4000000,
                Rect1Based(1, 3, 7, 9)
            ),

            // 45평 -> 54평
            CreateStage(
                45,
                54,
                50,
                9000000,
                Rect1Based(4, 6, 7, 9)
            ),

            // 54평 -> 63평
            CreateStage(
                54,
                63,
                60,
                20000000,
                Rect1Based(7, 9, 7, 9)
            ),

            // 63평 -> 72평
            CreateStage(
                63,
                72,
                70,
                45000000,
                Rect1Based(7, 9, 4, 6)
            ),

            // 72평 -> 90평
            // 네가 준 좌표 중 7~9,1~3 9개 + 1~3,10~12 9개를 먼저 추가
            CreateStage(
                72,
                90,
                80,
                90000000,
                Combine(
                    Rect1Based(7, 9, 1, 3),
                    Rect1Based(1, 3, 10, 12)
                )
            ),

            // 90평 -> 117평
            // 남은 4~9,10~12 18개 + 10~12,10~12 9개
            CreateStage(
                90,
                117,
                90,
                180000000,
                Combine(
                    Rect1Based(4, 9, 10, 12),
                    Rect1Based(10, 12, 10, 12)
                )
            ),

            // 117평 -> 144평
            // 10~12,1~9 27개
            CreateStage(
                117,
                144,
                100,
                400000000,
                Rect1Based(10, 12, 1, 9)
            )
        };
    }

    private LabExpansionData CreateStage(
        int currentArea,
        int nextArea,
        int requiredLabLevel,
        long cost,
        Vector2Int[] tilesToAdd
    )
    {
        LabExpansionData data = new LabExpansionData();
        data.currentArea = currentArea;
        data.nextArea = nextArea;
        data.requiredLabLevel = requiredLabLevel;
        data.cost = cost;
        data.tilesToAdd = tilesToAdd;

        return data;
    }

    private Vector2Int[] Rect1Based(int xMin, int xMax, int yMin, int yMax)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                // 사용자가 준 좌표는 1부터 시작.
                // LabGridManager 내부 좌표는 0부터 시작하므로 -1 처리.
                positions.Add(new Vector2Int(x - 1, y - 1));
            }
        }

        return positions.ToArray();
    }

    private Vector2Int[] Combine(params Vector2Int[][] groups)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] == null)
            {
                continue;
            }

            for (int j = 0; j < groups[i].Length; j++)
            {
                result.Add(groups[i][j]);
            }
        }

        return result.ToArray();
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
        if (LabGridManager.Instance == null)
        {
            Debug.LogWarning("LabExpansionManager: LabGridManager.Instance가 없습니다.");
            return false;
        }

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
                "연구실 확장 데이터 불일치: 현재 평수 " +
                actualCurrentArea + " / 다음 확장 데이터 시작 평수 " +
                data.currentArea
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

        if (data.tilesToAdd == null || data.tilesToAdd.Length == 0)
        {
            Debug.LogWarning("연구실 확장 실패: 추가할 타일 좌표가 없습니다.");
            return false;
        }

        LabGridManager.Instance.ExpandGridByCoordinates(data.tilesToAdd);

        currentExpansionIndex++;

        Debug.Log(
            "연구실 확장 성공: " +
            data.currentArea + "평 -> " + data.nextArea + "평"
        );

        return true;
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