using System.Collections;
using UnityEngine;

/// <summary>
/// Workstation 하나의 과제 수행 상태를 관리하는 스크립트.
/// 
/// 역할:
/// - 앉아 있는 인력이 과제를 시작할 수 있는지 확인
/// - 과제 수행 타이머 실행
/// - 과제 완료 후 검사 대기 상태로 변경
/// - UI 갱신 요청
/// </summary>
public class WorkstationTaskController : MonoBehaviour
{
    [Header("연결")]
    public WorkstationObject workstation;
    public WorkstationWorldUI worldUI;

    [Header("현재 과제")]
    public TaskData currentTask = new TaskData();

    [Header("현재 작업 상태")]
    public StaffTaskState taskState = StaffTaskState.Idle;

    private Coroutine taskCoroutine;

    private void Awake()
    {
        if (workstation == null)
        {
            workstation = GetComponent<WorkstationObject>();
        }

        if (worldUI == null)
        {
            worldUI = GetComponentInChildren<WorkstationWorldUI>();
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// 과제를 시작한다.
    /// 
    /// 이 함수는 WorkstationWorldUI의 과제하기 버튼에서 호출된다.
    /// </summary>
    public void StartTask()
    {
        if (workstation == null)
        {
            Debug.LogError("WorkstationTaskController: WorkstationObject가 없습니다.");
            return;
        }

        if (workstation.hasStaff == false || workstation.seatedStaff == null)
        {
            Debug.Log("인력이 앉아 있지 않아 과제를 시작할 수 없습니다.");
            return;
        }

        StaffWorker staff = workstation.seatedStaff;

        if (staff.CanAct() == false)
        {
            Debug.Log("스트레스가 너무 높아 행동할 수 없습니다.");
            return;
        }

        if (staff.researchPower < currentTask.requiredResearchPower)
        {
            Debug.Log("연구력이 부족해서 과제를 수행할 수 없습니다.");
            return;
        }

        if (taskState != StaffTaskState.Idle)
        {
            Debug.Log("현재 과제를 시작할 수 없는 상태입니다: " + taskState);
            return;
        }

        taskState = StaffTaskState.Working;
        RefreshUI();

        taskCoroutine = StartCoroutine(TaskRoutine());
    }

    /// <summary>
    /// 과제 수행 시간만큼 기다린 뒤 검사 대기 상태로 바꾼다.
    /// </summary>
    private IEnumerator TaskRoutine()
    {
        yield return new WaitForSeconds(currentTask.workTime);

        CompleteTask();
    }

    /// <summary>
    /// 과제 수행 완료 처리.
    /// 
    /// 아직 보상은 지급하지 않는다.
    /// 보상은 나중에 검사 완료 단계에서 지급한다.
    /// </summary>
    private void CompleteTask()
    {
        taskState = StaffTaskState.WaitingForInspection;

        Debug.Log("과제 완료. 검사 대기 상태로 전환됨.");

        RefreshUI();
    }

    /// <summary>
    /// 외부에서 현재 상태에 맞게 UI를 다시 그리도록 요청한다.
    /// </summary>
    public void RefreshUI()
    {
        if (worldUI != null)
        {
            worldUI.Refresh(taskState, workstation);
        }
    }

    /// <summary>
    /// 테스트용 상태 초기화.
    /// 나중에 청소 완료 후 Idle로 돌아갈 때 사용한다.
    /// </summary>
    public void SetIdle()
    {
        taskState = StaffTaskState.Idle;
        RefreshUI();
    }
}