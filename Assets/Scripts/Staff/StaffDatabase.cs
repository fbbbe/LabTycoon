using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인력 레벨업에 필요한 조건 데이터입니다.
///
/// 예시:
/// 학사생 Lv.1 -> Lv.2
/// - requiredCompletedTaskCount = 10
/// - cost = 50000
/// - requiredLabLevel = 8
/// </summary>
[System.Serializable]
public class StaffLevelUpData
{
    public StaffType staffType;
    public int currentLevel;
    public int nextLevel;
    public int requiredCompletedTaskCount;
    public long cost;
    public int requiredLabLevel;
}

/// <summary>
/// 인력 진화에 필요한 조건 데이터입니다.
///
/// 예시:
/// 학사생 Lv.3 -> 석사생 Lv.1
/// </summary>
[System.Serializable]
public class StaffEvolutionData
{
    public StaffType currentStaffType;
    public int currentLevel;
    public StaffType nextStaffType;
    public int nextLevel;
    public int requiredCompletedTaskCount;
    public long cost;
    public int requiredLabLevel;
}

/// <summary>
/// 고용 가능한 연구생 데이터와 레벨별 연구생 데이터를 코드에서 관리한다.
///
/// 기존에는 학사/석사/박사 Lv.1 데이터만 있었지만,
/// 이제 학사 Lv.1~3, 석사 Lv.1~3, 박사 Lv.1~5 데이터를 모두 관리한다.
/// </summary>
public class StaffDatabase : MonoBehaviour
{
    public static StaffDatabase Instance;

    private Dictionary<StaffType, StaffHireData> staffDataMap =
        new Dictionary<StaffType, StaffHireData>();

    private Dictionary<string, StaffHireData> staffLevelDataMap =
        new Dictionary<string, StaffHireData>();

    private Dictionary<string, StaffLevelUpData> staffLevelUpDataMap =
        new Dictionary<string, StaffLevelUpData>();

    private Dictionary<string, StaffEvolutionData> staffEvolutionDataMap =
        new Dictionary<string, StaffEvolutionData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateStaffData();
        CreateLevelUpData();
        CreateEvolutionData();
    }

    private void CreateStaffData()
    {
        staffDataMap.Clear();
        staffLevelDataMap.Clear();

        // 학사생: Lv.1~Lv.3
        AddStaffLevelData(
            staffName: "학사생",
            staffType: StaffType.Undergraduate,
            price: 30000,
            level: 1,
            researchPower: 10,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/UndergraduateStaff",
            unlockLevel: 1
        );

        AddStaffLevelData(
            staffName: "학사생",
            staffType: StaffType.Undergraduate,
            price: 30000,
            level: 2,
            researchPower: 15,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/UndergraduateStaff",
            unlockLevel: 1
        );

        AddStaffLevelData(
            staffName: "학사생",
            staffType: StaffType.Undergraduate,
            price: 30000,
            level: 3,
            researchPower: 20,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/UndergraduateStaff",
            unlockLevel: 1
        );

        // 석사생: Lv.1~Lv.3
        AddStaffLevelData(
            staffName: "석사생",
            staffType: StaffType.Master,
            price: 150000,
            level: 1,
            researchPower: 30,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/MasterStaff",
            unlockLevel: 25
        );

        AddStaffLevelData(
            staffName: "석사생",
            staffType: StaffType.Master,
            price: 150000,
            level: 2,
            researchPower: 40,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/MasterStaff",
            unlockLevel: 25
        );

        AddStaffLevelData(
            staffName: "석사생",
            staffType: StaffType.Master,
            price: 150000,
            level: 3,
            researchPower: 50,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/MasterStaff",
            unlockLevel: 25
        );

        // 박사생: Lv.1~Lv.5
        AddStaffLevelData(
            staffName: "박사생",
            staffType: StaffType.PhD,
            price: 500000,
            level: 1,
            researchPower: 60,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/PhDStaff",
            unlockLevel: 60
        );

        AddStaffLevelData(
            staffName: "박사생",
            staffType: StaffType.PhD,
            price: 500000,
            level: 2,
            researchPower: 70,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/PhDStaff",
            unlockLevel: 60
        );

        AddStaffLevelData(
            staffName: "박사생",
            staffType: StaffType.PhD,
            price: 500000,
            level: 3,
            researchPower: 80,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/PhDStaff",
            unlockLevel: 60
        );

        AddStaffLevelData(
            staffName: "박사생",
            staffType: StaffType.PhD,
            price: 500000,
            level: 4,
            researchPower: 90,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/PhDStaff",
            unlockLevel: 60
        );

        AddStaffLevelData(
            staffName: "박사생",
            staffType: StaffType.PhD,
            price: 500000,
            level: 5,
            researchPower: 100,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/PhDStaff",
            unlockLevel: 60
        );
    }

    private void AddStaffLevelData(
        string staffName,
        StaffType staffType,
        long price,
        int level,
        int researchPower,
        int initialStress,
        string prefabResourcePath,
        int unlockLevel
    )
    {
        StaffHireData data = new StaffHireData(
            staffName: staffName,
            staffType: staffType,
            price: price,
            level: level,
            researchPower: researchPower,
            initialStress: initialStress,
            prefabResourcePath: prefabResourcePath
        );

        data.unlockLevel = unlockLevel;

        staffLevelDataMap[CreateStaffLevelKey(staffType, level)] = data;

        // 기존 고용 로직은 StaffType만으로 Lv.1 데이터를 가져간다.
        // 따라서 StaffType 기본 데이터는 Lv.1만 등록한다.
        if (level == 1)
        {
            staffDataMap[staffType] = data;
        }
    }

    private void CreateLevelUpData()
    {
        staffLevelUpDataMap.Clear();

        // 학사생 레벨업
        AddLevelUpData(StaffType.Undergraduate, 1, 2, 10, 50000, 8);
        AddLevelUpData(StaffType.Undergraduate, 2, 3, 25, 200000, 18);

        // 석사생 레벨업
        AddLevelUpData(StaffType.Master, 1, 2, 20, 800000, 38);
        AddLevelUpData(StaffType.Master, 2, 3, 50, 2500000, 50);

        // 박사생 레벨업
        AddLevelUpData(StaffType.PhD, 1, 2, 30, 5000000, 70);
        AddLevelUpData(StaffType.PhD, 2, 3, 60, 12000000, 80);
        AddLevelUpData(StaffType.PhD, 3, 4, 100, 30000000, 90);
        AddLevelUpData(StaffType.PhD, 4, 5, 150, 80000000, 97);
    }

    private void AddLevelUpData(
        StaffType staffType,
        int currentLevel,
        int nextLevel,
        int requiredCompletedTaskCount,
        long cost,
        int requiredLabLevel
    )
    {
        StaffLevelUpData data = new StaffLevelUpData();
        data.staffType = staffType;
        data.currentLevel = currentLevel;
        data.nextLevel = nextLevel;
        data.requiredCompletedTaskCount = requiredCompletedTaskCount;
        data.cost = cost;
        data.requiredLabLevel = requiredLabLevel;

        staffLevelUpDataMap[CreateStaffLevelKey(staffType, currentLevel)] = data;
    }

    private void CreateEvolutionData()
    {
        staffEvolutionDataMap.Clear();

        // 진화 조건은 이미지에 나온 예시 기준으로 먼저 등록한다.
        // 학사생 Lv.3 -> 석사생 Lv.1
        AddEvolutionData(
            currentStaffType: StaffType.Undergraduate,
            currentLevel: 3,
            nextStaffType: StaffType.Master,
            nextLevel: 1,
            requiredCompletedTaskCount: 15,
            cost: 0,
            requiredLabLevel: 25
        );

        // 석사생 Lv.3 -> 박사생 Lv.1
        AddEvolutionData(
            currentStaffType: StaffType.Master,
            currentLevel: 3,
            nextStaffType: StaffType.PhD,
            nextLevel: 1,
            requiredCompletedTaskCount: 20,
            cost: 0,
            requiredLabLevel: 60
        );
    }

    private void AddEvolutionData(
        StaffType currentStaffType,
        int currentLevel,
        StaffType nextStaffType,
        int nextLevel,
        int requiredCompletedTaskCount,
        long cost,
        int requiredLabLevel
    )
    {
        StaffEvolutionData data = new StaffEvolutionData();
        data.currentStaffType = currentStaffType;
        data.currentLevel = currentLevel;
        data.nextStaffType = nextStaffType;
        data.nextLevel = nextLevel;
        data.requiredCompletedTaskCount = requiredCompletedTaskCount;
        data.cost = cost;
        data.requiredLabLevel = requiredLabLevel;

        staffEvolutionDataMap[CreateStaffLevelKey(currentStaffType, currentLevel)] = data;
    }

    public StaffHireData GetStaffData(StaffType staffType)
    {
        if (staffDataMap.ContainsKey(staffType))
        {
            return staffDataMap[staffType];
        }

        Debug.LogError("연구생 데이터를 찾지 못했습니다: " + staffType);
        return null;
    }

    public StaffHireData GetStaffData(StaffType staffType, int level)
    {
        string key = CreateStaffLevelKey(staffType, level);

        if (staffLevelDataMap.ContainsKey(key))
        {
            return staffLevelDataMap[key];
        }

        Debug.LogError("연구생 레벨 데이터를 찾지 못했습니다: " + staffType + " Lv." + level);
        return null;
    }

    public StaffHireData GetNextLevelStaffData(StaffType staffType, int currentLevel)
    {
        return GetStaffData(staffType, currentLevel + 1);
    }

    public StaffLevelUpData GetLevelUpData(StaffType staffType, int currentLevel)
    {
        string key = CreateStaffLevelKey(staffType, currentLevel);

        if (staffLevelUpDataMap.ContainsKey(key))
        {
            return staffLevelUpDataMap[key];
        }

        return null;
    }

    public StaffEvolutionData GetEvolutionData(StaffType staffType, int currentLevel)
    {
        string key = CreateStaffLevelKey(staffType, currentLevel);

        if (staffEvolutionDataMap.ContainsKey(key))
        {
            return staffEvolutionDataMap[key];
        }

        return null;
    }

    public bool HasNextLevel(StaffType staffType, int currentLevel)
    {
        return staffLevelDataMap.ContainsKey(CreateStaffLevelKey(staffType, currentLevel + 1));
    }

    public bool HasEvolution(StaffType staffType, int currentLevel)
    {
        return staffEvolutionDataMap.ContainsKey(CreateStaffLevelKey(staffType, currentLevel));
    }

    private string CreateStaffLevelKey(StaffType staffType, int level)
    {
        return staffType.ToString() + "_Lv" + level;
    }
}