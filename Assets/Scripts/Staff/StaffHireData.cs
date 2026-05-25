using UnityEngine;

/// <summary>
/// 고용 가능한 연구생 한 명의 데이터.
/// 
/// 연구생 고용창에서 버튼을 눌렀을 때,
/// 이 데이터에 따라 가격, 연구력, prefab 경로를 결정한다.
/// </summary>
[System.Serializable]
public class StaffHireData
{
    public string staffName;
    public StaffType staffType;

    public int price;
    public int level;
    public int researchPower;
    public int initialStress;

    public string prefabResourcePath;

    public StaffHireData(
        string staffName,
        StaffType staffType,
        int price,
        int level,
        int researchPower,
        int initialStress,
        string prefabResourcePath
    )
    {
        this.staffName = staffName;
        this.staffType = staffType;
        this.price = price;
        this.level = level;
        this.researchPower = researchPower;
        this.initialStress = initialStress;
        this.prefabResourcePath = prefabResourcePath;
    }
}