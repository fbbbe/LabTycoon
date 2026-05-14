using UnityEngine;

public class EquipmentShop : MonoBehaviour
{
    [Header("장비 목록")]
    public EquipmentData[] equipmentList;

    [Header("장비 생성 위치")]
    public Transform[] spawnPoints;

    private int currentSpawnIndex = 0;

    public void BuyEquipment(int index)
    {
        if (index < 0 || index >= equipmentList.Length)
        {
            Debug.LogError("잘못된 장비 인덱스입니다.");
            return;
        }

        EquipmentData equipment = equipmentList[index];

        if (ResourceManager.Instance.SpendMoney(equipment.price) == false)
        {
            return;
        }

        ResourceManager.Instance.AddMoneyProduction(equipment.addMoneyPerTick);
        ResourceManager.Instance.AddResearchProduction(equipment.addResearchPerTick);
        ResourceManager.Instance.AddStressProduction(equipment.addStressPerTick);

        SpawnEquipment(equipment);

        Debug.Log(equipment.equipmentName + " 구매 완료");
    }

    private void SpawnEquipment(EquipmentData equipment)
    {
        if (equipment.equipmentPrefab == null)
        {
            Debug.LogWarning(equipment.equipmentName + " 프리팹이 없습니다.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("장비 생성 위치가 없습니다.");
            return;
        }

        if (currentSpawnIndex >= spawnPoints.Length)
        {
            Debug.LogWarning("더 이상 장비를 배치할 위치가 없습니다.");
            return;
        }

        Instantiate(
            equipment.equipmentPrefab,
            spawnPoints[currentSpawnIndex].position,
            Quaternion.identity
        );

        currentSpawnIndex++;
    }
}