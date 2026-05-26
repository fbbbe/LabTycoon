using UnityEngine;

/// <summary>
/// 인력 한 명의 정보를 관리하는 스크립트.
/// 
/// 인력별로 따로 관리해야 하는 값:
/// - 연구력
/// - 스트레스
/// - 인력 레벨
/// - 현재 앉은 Workstation
/// - 현재 행동 가능 여부
/// </summary>
public class StaffWorker : MonoBehaviour
{
    [Header("인력 기본 정보")]
    public string staffName = "학사생";
    public StaffType staffType = StaffType.Undergraduate;

    [Header("인력 능력치")]
    public int staffLevel = 1;

    /// <summary>
    /// 기존 코드에서 level이라는 이름으로 접근하는 경우를 위한 호환 프로퍼티입니다.
    /// 실제 저장 값은 staffLevel을 사용합니다.
    /// </summary>
    public int level
    {
        get { return staffLevel; }
        set { staffLevel = value; }
    }

    [Tooltip("이 인력의 연구력입니다. 과제 수행 가능 조건에 사용됩니다.")]
    public int researchPower = 10;

    [Header("스트레스")]
    public int stress = 0;

    /// <summary>
    /// 기존 코드에서 currentStress라는 이름으로 접근하는 경우를 위한 호환 프로퍼티입니다.
    /// 실제 저장 값은 stress를 사용합니다.
    /// </summary>
    public int currentStress
    {
        get { return stress; }
        set { stress = Mathf.Clamp(value, 0, 100); }
    }

    [Header("착석 상태")]
    public WorkstationObject currentWorkstation;
    public bool isSeated = false;

    [Header("개별 인력 데이터")]
    public StaffRuntimeData runtimeData;

    [Header("성장 정보")]
    public int completedTaskCount;

    public bool CanAct()
    {
        if (runtimeData != null)
        {
            return runtimeData.CanAct();
        }

        return currentStress < 100;
    }

    public void SetSeated(WorkstationObject workstation)
    {
        currentWorkstation = workstation;
        isSeated = true;
    }

    public void SetUnseated()
    {
        currentWorkstation = null;
        isSeated = false;
    }

    public void AddStress(int amount)
    {
        if (runtimeData == null)
        {
            runtimeData = new StaffRuntimeData();
        }

        runtimeData.AddStress(amount);

        currentStress = runtimeData.currentStress;
    }

    public void ReduceStress(int amount)
    {
        if (runtimeData == null)
        {
            runtimeData = new StaffRuntimeData();
        }

        runtimeData.ReduceStress(amount);

        currentStress = runtimeData.currentStress;
    }

    public void InitializeFromHireData(StaffHireData hireData)
    {
        if (hireData == null)
        {
            Debug.LogError("StaffWorker 초기화 실패: hireData가 null입니다.");
            return;
        }

        runtimeData = new StaffRuntimeData(hireData);

        staffName = runtimeData.staffName;
        staffType = runtimeData.staffType;
        staffLevel = runtimeData.level;
        researchPower = runtimeData.researchPower;
        stress = runtimeData.currentStress;
    }
}