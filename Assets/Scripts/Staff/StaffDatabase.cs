using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 고용 가능한 연구생 데이터를 코드에서 관리한다.
/// 
/// Inspector에서 연구생 정보를 직접 입력하지 않고,
/// StaffType 기준으로 필요한 데이터를 찾아 사용한다.
/// </summary>
public class StaffDatabase : MonoBehaviour
{
    public static StaffDatabase Instance;

    private Dictionary<StaffType, StaffHireData> staffDataMap =
        new Dictionary<StaffType, StaffHireData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateStaffData();
    }

    private void CreateStaffData()
    {
        staffDataMap.Clear();

        StaffHireData undergraduate = new StaffHireData(
            staffName: "학사생",
            staffType: StaffType.Undergraduate,
            price: 30000,
            level: 1,
            researchPower: 10,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/UndergraduateStaff"
        );
        undergraduate.unlockLevel = 1;
        staffDataMap.Add(StaffType.Undergraduate, undergraduate);

        StaffHireData master = new StaffHireData(
            staffName: "석사생",
            staffType: StaffType.Master,
            price: 150000,
            level: 1,
            researchPower: 30,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/MasterStaff"
        );
        master.unlockLevel = 25;
        staffDataMap.Add(StaffType.Master, master);

        StaffHireData phd = new StaffHireData(
            staffName: "박사생",
            staffType: StaffType.PhD,
            price: 500000,
            level: 1,
            researchPower: 60,
            initialStress: 0,
            prefabResourcePath: "Prefabs/Staff/PhDStaff"
        );
        phd.unlockLevel = 60;
        staffDataMap.Add(StaffType.PhD, phd);
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
}