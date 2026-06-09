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

    [Header("검사 연출 중 착석 이미지 교체")]
    [Tooltip("검사 연출 중 의자+인력 합성 이미지를 방향에 맞는 빈 의자 이미지로 교체하는 스크립트입니다.")]
    public WorkstationInspectionVisualSwitcher inspectionVisualSwitcher;

    private void Awake()
    {
        if (workstation == null)
        {
            workstation = GetComponent<WorkstationObject>();
        }

        if (worldUI == null)
        {
            worldUI = GetComponentInChildren<WorkstationWorldUI>(true);
        }

        if (worldUI != null)
        {
            worldUI.taskController = this;
        }
        else
        {
            Debug.LogWarning("WorkstationTaskController: 자식에서 WorkstationWorldUI를 찾지 못했습니다. Workstation 프리팹의 WorldUI 오브젝트를 World UI 필드에 직접 연결하세요.");
        }

        if (inspectionVisualSwitcher == null)
        {
            inspectionVisualSwitcher = GetComponent<WorkstationInspectionVisualSwitcher>();
        }

        if (inspectionVisualSwitcher == null)
        {
            Debug.LogWarning("WorkstationTaskController: WorkstationInspectionVisualSwitcher가 없습니다. 검사 연출 중 착석 이미지를 빈 의자로 바꾸려면 Workstation에 해당 컴포넌트를 추가해야 합니다.");
        }
    }

    private void Start()
    {
        if (worldUI != null)
        {
            worldUI.taskController = this;
        }

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

        // 과제 시작 직전에 인력 종류/레벨에 맞는 과제 데이터로 교체한다.
        currentTask = TaskGradeDatabase.CreateTaskDataForStaff(staff);

        if (staff.researchPower < currentTask.requiredResearchPower)
        {
            Debug.Log(
                "연구력이 부족해서 과제를 수행할 수 없습니다. " +
                "현재 연구력: " + staff.researchPower +
                " / 필요 연구력: " + currentTask.requiredResearchPower
            );
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTaskCompleteSound();
        }

        Debug.Log("과제 완료. 검사 대기 상태로 전환됨.");

        RefreshUI();
    }

    /// <summary>
    /// 외부에서 현재 상태에 맞게 UI를 다시 그리도록 요청한다.
    /// </summary>
    public void RefreshUI()
    {
        if (worldUI == null)
        {
            worldUI = GetComponentInChildren<WorkstationWorldUI>(true);
        }

        if (worldUI != null)
        {
            worldUI.taskController = this;
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

    /// <summary>
    /// 검사받기를 시작한다.
    /// 
    /// 검사받기 버튼을 눌렀을 때 호출된다.
    /// 실제 교수님 등장 연출은 InspectionManager가 담당한다.
    /// </summary>
    public void StartInspection()
    {
        if (taskState != StaffTaskState.WaitingForInspection)
        {
            Debug.Log("현재 검사받을 수 없는 상태입니다: " + taskState);
            return;
        }

        if (workstation == null || workstation.seatedStaff == null)
        {
            Debug.Log("검사받을 인력이 없습니다.");
            return;
        }

        if (InspectionManager.Instance == null)
        {
            Debug.LogError("InspectionManager.Instance가 없습니다. InspectionSystem 오브젝트에 InspectionManager를 붙였는지 확인하세요.");
            return;
        }

        Debug.Log("WorkstationTaskController: 검사 연출 시작 요청");

        bool inspectionStarted = InspectionManager.Instance.StartInspectionSequence(this);

        if (inspectionStarted == false)
        {
            // 이미 다른 검사가 진행 중인 경우에는 상태와 UI를 바꾸면 안 된다.
            // 그래야 기존 검사받기 버튼이 그대로 남는다.
            return;
        }

        taskState = StaffTaskState.Inspecting;
        RefreshUI();

        Debug.Log("WorkstationTaskController: 검사 연출 시작");
    }

    /// <summary>
    /// 교수님 검사 연출이 끝난 뒤 호출된다.
    /// 여기서 실제 보상과 스트레스를 적용한다.
    /// </summary>
    public void CompleteInspectionAfterSequence()
    {
        StaffWorker staff = workstation.seatedStaff;

        if (staff == null)
        {
            Debug.LogError("검사 완료 처리 실패: seatedStaff가 없습니다.");
            return;
        }

        int finalMoneyReward = RewardCalculator.CalculateMoneyReward(
            currentTask.baseMoneyReward,
            staff
        );

        int finalResearchResult = RewardCalculator.CalculateResearchResult(
            currentTask.baseResearchResult
        );

        int finalStress = StressCalculator.CalculateTaskStress(
            currentTask.baseTaskStress
        );

        

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddMoney(finalMoneyReward);

            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.ShowMoneyPopup(
                    workstation.seatPoint.position +
                    Vector3.up * 1.5f +
                    Vector3.right * 0.5f,
                    finalMoneyReward
                );
            }
            ResourceManager.Instance.AddResearchResult(finalResearchResult);
        }
        

        staff.AddStress(finalStress);

        if (StaffRestController.Instance != null)
        {
            StaffRestController.Instance.CheckStressOverload(staff);
        }



        if (staff.runtimeData != null)
        {
            staff.runtimeData.AddCompletedTaskCount(1);
            staff.completedTaskCount = staff.runtimeData.completedTaskCount;
        }
        Debug.Log(
            "검사 완료: 돈 +" + finalMoneyReward +
            ", 연구성과 +" + finalResearchResult +
            ", 스트레스 +" + finalStress
        );

        

        taskState = StaffTaskState.NeedCleaning;
        RefreshUI();
    }

    /// <summary>
    /// 검사 연출이 시작될 때 원래 자리에 보이는 의자+인력 합성 이미지를 빈 의자 이미지로 교체한다.
    ///
    /// 주의:
    /// 오브젝트를 SetActive(false)로 끄면 의자까지 사라진다.
    /// 따라서 SpriteRenderer의 Sprite만 방향에 맞는 빈 의자 Sprite로 교체한다.
    /// </summary>
    public void HideSeatedVisualForInspection()
    {
        if (inspectionVisualSwitcher == null)
        {
            Debug.LogWarning("inspectionVisualSwitcher가 연결되지 않았습니다. Workstation에 WorkstationInspectionVisualSwitcher를 추가하고 Chair Renderer와 빈 의자 4방향 Sprite를 연결하세요.");
            return;
        }

        inspectionVisualSwitcher.ShowChairOnlyForInspection();
    }

    /// <summary>
    /// 검사 연출이 끝나면 검사 전 저장해 둔 의자+인력 합성 이미지로 복구한다.
    /// </summary>
    public void RestoreSeatedVisualAfterInspection()
    {
        if (inspectionVisualSwitcher == null)
        {
            return;
        }

        inspectionVisualSwitcher.RestoreSeatedVisualAfterInspection();
    }

    /// <summary>
    /// 청소를 시작한다.
    /// 
    /// 청소하기 버튼을 눌렀을 때 호출된다.
    /// 청소가 끝나면 다시 과제 가능 상태로 돌아간다.
    /// </summary>
    public void StartCleaning()
    {
        
        if (taskState != StaffTaskState.NeedCleaning)
        {
            Debug.Log("현재 청소할 수 없는 상태입니다: " + taskState);
            return;
        }

        if (workstation == null || workstation.seatedStaff == null)
        {
            Debug.Log("청소할 인력이 없습니다.");
            return;
        }

        StaffWorker staff = workstation.seatedStaff;

        if (staff.CanAct() == false)
        {
            Debug.Log("스트레스가 너무 높아 청소할 수 없습니다.");
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCleaningSound();
        }

        taskState = StaffTaskState.Cleaning;
        RefreshUI();

        StartCoroutine(CleaningRoutine());
    }

    /// <summary>
    /// 청소 시간만큼 기다린 뒤 청소 완료 처리한다.
    /// 청소 시간은 청소 장비 효과를 반영한다.
    /// </summary>
    private IEnumerator CleaningRoutine()
    {
        float finalCleaningTime = StressCalculator.CalculateCleaningTime(
            currentTask.baseCleaningTime
        );

        Debug.Log("청소 시작. 청소 시간: " + finalCleaningTime + "초");

        yield return new WaitForSeconds(finalCleaningTime);

        CompleteCleaning();
    }

    /// <summary>
    /// 청소 완료 처리.
    /// 청소 스트레스를 적용하고 다시 과제 가능 상태로 되돌린다.
    /// </summary>
    private void CompleteCleaning()
    {
        if (workstation == null || workstation.seatedStaff == null)
        {
            Debug.LogError("청소 완료 처리 실패: seatedStaff가 없습니다.");
            return;
        }

        StaffWorker staff = workstation.seatedStaff;

        int finalCleaningStress = StressCalculator.CalculateCleaningStress(
            currentTask.baseCleaningStress
        );

        staff.AddStress(finalCleaningStress);

        if (StaffRestController.Instance != null)
        {
            StaffRestController.Instance.CheckStressOverload(staff);
        }

        Debug.Log("청소 완료: 스트레스 +" + finalCleaningStress);

        taskState = StaffTaskState.Idle;
        RefreshUI();
    }
}