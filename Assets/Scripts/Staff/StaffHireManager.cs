using UnityEngine;

/// <summary>
/// 연구생 고용 처리를 담당한다.
///
/// 흐름:
/// - 연구생 고용창에서 버튼 클릭
/// - StaffDatabase에서 연구생 데이터 조회
/// - 돈 확인
/// - 돈 차감
/// - 문 앞 StaffSpawnPoint에 연구생 prefab 생성
/// - StaffWorker 값을 고용 데이터 기준으로 초기화
/// </summary>
public class StaffHireManager : MonoBehaviour
{
    public static StaffHireManager Instance;

    [Header("연구생 스폰 위치")]
    [Tooltip("문 앞에 만든 StaffSpawnPoint 오브젝트를 연결한다.")]
    public Transform staffSpawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 연구생을 고용한다.
    /// ResearcherHirePanelUI에서 StaffType을 넘겨 호출한다.
    /// </summary>
    public bool HireStaff(StaffType staffType)
    {
        if (StaffDatabase.Instance == null)
        {
            Debug.LogError("StaffDatabase.Instance가 없습니다. Managers 오브젝트에 StaffDatabase를 추가했는지 확인하세요.");
            return false;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return false;
        }

        StaffHireData hireData = StaffDatabase.Instance.GetStaffData(staffType);

        if (hireData == null)
        {
            Debug.LogError("고용할 연구생 데이터를 찾지 못했습니다: " + staffType);
            return false;
        }

        if (ResourceManager.Instance.HasEnoughMoney(hireData.price) == false)
        {
            Debug.Log("돈이 부족해서 연구생을 고용할 수 없습니다: " + hireData.staffName);
            return false;
        }

        GameObject prefab = Resources.Load<GameObject>(hireData.prefabResourcePath);

        if (prefab == null)
        {
            Debug.LogError("연구생 prefab을 찾지 못했습니다: " + hireData.prefabResourcePath);
            return false;
        }

        bool spendSuccess = ResourceManager.Instance.SpendMoney(hireData.price);

        if (spendSuccess == false)
        {
            Debug.Log("연구생 고용 비용 차감에 실패했습니다: " + hireData.staffName);
            return false;
        }

        Vector3 spawnPosition = Vector3.zero;

        if (staffSpawnPoint != null)
        {
            spawnPosition = staffSpawnPoint.position;
        }
        else
        {
            Debug.LogWarning("StaffSpawnPoint가 연결되지 않았습니다. 임시로 (0,0,0)에 생성합니다.");
        }

        GameObject staffObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        staffObject.name = hireData.staffName;

        StaffWorker staffWorker = staffObject.GetComponent<StaffWorker>();

        if (staffWorker == null)
        {
            Debug.LogWarning("생성된 연구생 prefab에 StaffWorker가 없습니다: " + hireData.staffName);
            return true;
        }

        staffWorker.InitializeFromHireData(hireData);

        Debug.Log("연구생 고용 완료: " + hireData.staffName);
        return true;
    }
}
