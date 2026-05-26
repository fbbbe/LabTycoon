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

    [Tooltip("이 인력의 연구력입니다. 과제 수행 가능 조건에 사용됩니다.")]
    public int researchPower = 10;

    [Header("스트레스")]
    public int stress = 0;

    [Header("착석 상태")]
    public WorkstationObject currentWorkstation;
    public bool isSeated = false;

    public bool CanAct()
    {
        return stress < 100;
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
        stress += amount;

        if (stress > 100)
        {
            stress = 100;
        }
    }

    public void ReduceStress(int amount)
    {
        stress -= amount;

        if (stress < 0)
        {
            stress = 0;
        }
    }

    public void InitializeFromHireData(StaffHireData data)
    {
        if (data == null)
        {
            return;
        }

        staffName = data.staffName;
        staffType = data.staffType;
        staffLevel = data.level;
        researchPower = data.researchPower;
        stress = data.initialStress;
    }
}