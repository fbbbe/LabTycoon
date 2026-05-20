using UnityEngine;

/// <summary>
/// 인력 한 명의 기본 데이터를 관리하는 스크립트.
/// 
/// 예:
/// - 학사생
/// - 석사생
/// - 박사생
/// 
/// 지금 단계에서는:
/// 1. 인력 종류 저장
/// 2. 인력 이름 저장
/// 3. 스트레스 저장
/// 4. 현재 앉아 있는 Workstation 저장
/// 
/// 나중에는 여기에 과제 상태, 레벨, 연구력, 휴식 상태 등을 추가한다.
/// </summary>
public class StaffWorker : MonoBehaviour
{
    [Header("인력 기본 정보")]
    [Tooltip("인력 이름입니다. 예: 학사생")]
    public string staffName = "학사생";

    [Tooltip("인력 종류입니다. 학사생/석사생/박사생을 구분합니다.")]
    public StaffType staffType = StaffType.Undergraduate;

    [Header("인력 능력치")]
    [Tooltip("이 인력의 레벨입니다.")]
    public int level = 1;

    [Tooltip("이 인력의 연구력입니다. 과제 수행 가능 여부에 사용됩니다.")]
    public int researchPower = 10;

    [Header("스트레스")]
    [Tooltip("현재 스트레스 수치입니다. 0~100 범위로 사용합니다.")]
    public int stress = 0;

    [Header("착석 상태")]
    [Tooltip("현재 이 인력이 앉아 있는 Workstation입니다. 아직 안 앉았으면 null입니다.")]
    public WorkstationObject currentWorkstation;

    [Tooltip("현재 의자에 앉아 있는지 여부입니다.")]
    public bool isSeated = false;

    /// <summary>
    /// 인력을 특정 Workstation에 앉은 상태로 기록한다.
    /// 
    /// 실제 화면 이미지는 Workstation 쪽에서
    /// 빈 의자 이미지 → 인력이 앉은 의자 이미지로 바뀐다.
    /// </summary>
    public void SetSeated(WorkstationObject workstation)
    {
        currentWorkstation = workstation;
        isSeated = true;
    }

    /// <summary>
    /// 인력을 자리에서 뺀 상태로 기록한다.
    /// 나중에 휴식/이동/퇴근 같은 기능에서 사용할 수 있다.
    /// </summary>
    public void SetUnseated()
    {
        currentWorkstation = null;
        isSeated = false;
    }

    /// <summary>
    /// 스트레스를 증가시킨다.
    /// </summary>
    public void AddStress(int amount)
    {
        stress += amount;

        if (stress > 100)
        {
            stress = 100;
        }
    }

    /// <summary>
    /// 스트레스를 감소시킨다.
    /// </summary>
    public void ReduceStress(int amount)
    {
        stress -= amount;

        if (stress < 0)
        {
            stress = 0;
        }
    }
}