using UnityEngine;

[System.Serializable]
public class StaffData
{
    public string staffName;
    public int hireCost;

    [Header("인력 생산량")]
    public int moneyPerTick;
    public int researchPerTick;
    public int stressPerTick;

    [Header("생성할 프리팹")]
    public GameObject staffPrefab;
}