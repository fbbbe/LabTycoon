using UnityEngine;

/// <summary>
/// 연구생 고용 요청과 고용 확정을 담당한다.
/// 
/// 고용 버튼을 누른 순간에는 돈을 차감하지 않는다.
/// Workstation 선택이 완료된 순간 돈을 차감하고 Staff를 생성한다.
/// </summary>
public class StaffHireManager : MonoBehaviour
{
    public static StaffHireManager Instance;

    private StaffHireData pendingHireData;

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
    /// 연구생 고용 버튼을 눌렀을 때 호출된다.
    /// 바로 고용하지 않고 인력 배치 모드로 들어간다.
    /// 
    /// 반환값:
    /// - true: 인력 배치 모드 진입 성공
    /// - false: 돈 부족, 자리 없음, 데이터 없음 등으로 실패
    /// </summary>
    public bool StartHirePlacement(StaffType staffType)
    {
        if (StaffDatabase.Instance == null)
        {
            Debug.LogError("StaffDatabase.Instance가 없습니다.");
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
            Debug.LogError("고용 데이터를 찾지 못했습니다: " + staffType);
            return false;
        }

        if (ResourceManager.Instance.HasEnoughMoney(hireData.price) == false)
        {
            Debug.Log("돈이 부족해서 연구생을 고용할 수 없습니다: " + hireData.staffName);
            return false;
        }

        if (StaffPlacementManager.Instance == null)
        {
            Debug.LogError("StaffPlacementManager.Instance가 없습니다.");
            return false;
        }

        if (StaffPlacementManager.Instance.HasAvailableWorkstationForHire() == false)
        {
            Debug.Log("고용 가능한 Workstation이 없습니다. 컴퓨터 장비가 설치된 빈 Workstation이 필요합니다.");
            return false;
        }

        // 중요:
        // StartHirePlacementMode 내부에서 기존 배치 모드를 정리하면서
        // pendingHireData가 지워질 수 있으므로, 배치 모드 시작 후에 다시 저장한다.
        StaffPlacementManager.Instance.StartHirePlacementMode(hireData);

        pendingHireData = hireData;

        Debug.Log("인력 배치 모드 시작: " + hireData.staffName);
        return true;
    }
    /// <summary>
    /// Workstation 선택이 완료되었을 때 호출된다.
    /// 이 순간에 돈을 차감하고 Staff prefab을 생성한 뒤 착석시킨다.
    /// </summary>
    public bool ConfirmHireToWorkstation(WorkstationObject targetWorkstation)
    {
        if (pendingHireData == null)
        {
            Debug.LogError("대기 중인 고용 데이터가 없습니다.");
            return false;
        }

        if (targetWorkstation == null)
        {
            Debug.LogError("선택한 Workstation이 없습니다.");
            return false;
        }

        if (targetWorkstation.CanSeatNewStaff() == false)
        {
            Debug.Log("이 Workstation에는 인력을 배치할 수 없습니다.");
            return false;
        }

        if (ResourceManager.Instance.HasEnoughMoney(pendingHireData.price) == false)
        {
            Debug.Log("돈이 부족해서 연구생을 고용할 수 없습니다: " + pendingHireData.staffName);
            return false;
        }

        GameObject prefab = Resources.Load<GameObject>(pendingHireData.prefabResourcePath);

        if (prefab == null)
        {
            Debug.LogError("연구생 prefab을 찾지 못했습니다: " + pendingHireData.prefabResourcePath);
            return false;
        }

        bool spendSuccess = ResourceManager.Instance.SpendMoney(pendingHireData.price);

        if (spendSuccess == false)
        {
            return false;
        }

        GameObject staffObject = Instantiate(prefab);
        staffObject.name = pendingHireData.staffName;

        StaffWorker staffWorker = staffObject.GetComponent<StaffWorker>();

        if (staffWorker == null)
        {
            Debug.LogError("생성된 연구생 prefab에 StaffWorker가 없습니다.");
            Destroy(staffObject);
            return false;
        }

        staffWorker.InitializeFromHireData(pendingHireData);

        targetWorkstation.SeatStaff(staffWorker);

        pendingHireData = null;

        Debug.Log("연구생 고용 및 착석 완료: " + staffWorker.staffName);
        return true;
    }

    public void CancelPendingHire()
    {
        pendingHireData = null;
    }
}