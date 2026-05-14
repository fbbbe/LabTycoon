using UnityEngine;

public class StaffShop : MonoBehaviour
{
    [Header("인력 목록")]
    public StaffData[] staffList;

    [Header("인력 생성 위치")]
    public Transform[] spawnPoints;

    private int currentSpawnIndex = 0;

    public void HireStaff(int index)
    {
        if (index < 0 || index >= staffList.Length)
        {
            Debug.LogError("잘못된 인력 인덱스입니다.");
            return;
        }

        StaffData staff = staffList[index];

        if (ResourceManager.Instance.SpendMoney(staff.hireCost) == false)
        {
            return;
        }

        SpawnStaff(staff);

        Debug.Log(staff.staffName + " 고용 완료");
    }

    private void SpawnStaff(StaffData staff)
    {
        if (staff.staffPrefab == null)
        {
            Debug.LogWarning(staff.staffName + " 프리팹이 없습니다.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("인력 생성 위치가 없습니다.");
            return;
        }

        if (currentSpawnIndex >= spawnPoints.Length)
        {
            Debug.LogWarning("더 이상 인력을 배치할 위치가 없습니다.");
            return;
        }

        GameObject staffObject = Instantiate(
            staff.staffPrefab,
            spawnPoints[currentSpawnIndex].position,
            Quaternion.identity
        );

        StaffWorker worker = staffObject.GetComponent<StaffWorker>();

        if (worker != null)
        {
            worker.moneyPerTick = staff.moneyPerTick;
            worker.researchPerTick = staff.researchPerTick;
            worker.stressPerTick = staff.stressPerTick;
        }

        currentSpawnIndex++;
    }
}