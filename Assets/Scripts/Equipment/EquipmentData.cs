using UnityEngine;

[System.Serializable]
public class EquipmentData
{
    public string equipmentName;
    public int price;

    [Header("구매 시 생산량 변화")]
    public int addMoneyPerTick;
    public int addResearchPerTick;
    public int addStressPerTick;

    [Header("생성할 프리팹")]
    public GameObject equipmentPrefab;
}