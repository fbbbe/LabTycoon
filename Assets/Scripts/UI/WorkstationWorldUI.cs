using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Workstation 위에 뜨는 월드 UI.
/// 
/// 역할:
/// - 과제하기 버튼 표시
/// - 검사받기 버튼 표시
/// - 현재 Workstation 상태에 따라 버튼을 켜고 끔
/// 
/// 이 UI는 화면 고정 HUD가 아니라 Workstation 근처에 붙는 버튼이다.
/// </summary>
public class WorkstationWorldUI : MonoBehaviour
{
    [Header("버튼 오브젝트")]
    public Button taskButton;
    public Button inspectionButton;
    public Button cleaningButton;

    [Header("버튼 위치 기준")]
    public Vector3 localOffset = new Vector3(0f, 1.2f, 0f);

    [Header("연결된 작업 컨트롤러")]
    public WorkstationTaskController taskController;

    [Header("방향별 UI 위치 오프셋")]
    [Tooltip("오른쪽 아래 방향일 때 버튼 위치")]
    public Vector3 rightDownOffset = new Vector3(0f, 1.1f, 0f);

    [Tooltip("오른쪽 위 방향일 때 버튼 위치")]
    public Vector3 rightUpOffset = new Vector3(0f, 1.1f, 0f);

    [Tooltip("왼쪽 아래 방향일 때 버튼 위치")]
    public Vector3 leftDownOffset = new Vector3(0f, 1.1f, 0f);

    [Tooltip("왼쪽 위 방향일 때 버튼 위치")]
    public Vector3 leftUpOffset = new Vector3(0f, 1.1f, 0f);

    [Header("스트레스 숫자 UI")]
    public TextMeshProUGUI stressText;

    [Header("방향별 스트레스 숫자 위치 오프셋")]
    public Vector3 stressOffsetRightDown = new Vector3(0f, 1.35f, 0f);
    public Vector3 stressOffsetRightUp = new Vector3(0f, 1.35f, 0f);
    public Vector3 stressOffsetLeftDown = new Vector3(0f, 1.35f, 0f);
    public Vector3 stressOffsetLeftUp = new Vector3(0f, 1.35f, 0f);


    private void Awake()
    {
        ResolveTaskController();

        DisableStressTextRaycastTarget();

        if (taskButton != null)
        {
            taskButton.onClick.RemoveAllListeners();
            taskButton.onClick.AddListener(OnClickTaskButton);
        }

        if (inspectionButton != null)
        {
            inspectionButton.onClick.RemoveAllListeners();
            inspectionButton.onClick.AddListener(OnClickInspectionButton);
        }

        if (cleaningButton != null)
        {
            cleaningButton.onClick.RemoveAllListeners();
            cleaningButton.onClick.AddListener(OnClickCleaningButton);
        }
    }

    private void Start()
    {
        DisableStressTextRaycastTarget();
        HideAll();
        HideStressText();
    }
    /// <summary>
    /// 현재 상태에 따라 버튼과 스트레스 숫자를 갱신한다.
    /// </summary>
    public void Refresh(StaffTaskState state, WorkstationObject workstation)
    {
        HideAll();

        if (workstation == null)
        {
            HideStressText();
            return;
        }

        // Workstation에 실제로 앉아 있는 인력이 없으면 버튼과 스트레스 숫자를 모두 숨긴다.
        // 게임 시작 시 타일 위에 서 있는 초기 인력은 seatedStaff가 아니므로 여기서 표시되지 않는다.
        if (workstation.hasStaff == false || workstation.seatedStaff == null)
        {
            HideStressText();
            return;
        }

        switch (state)
        {
            case StaffTaskState.Idle:
                if (taskButton != null)
                {
                    taskButton.gameObject.SetActive(true);
                }
                break;

            case StaffTaskState.WaitingForInspection:
                if (inspectionButton != null)
                {
                    inspectionButton.gameObject.SetActive(true);
                }
                break;

            case StaffTaskState.NeedCleaning:
                if (cleaningButton != null)
                {
                    cleaningButton.gameObject.SetActive(true);
                }
                break;
        }

        RefreshStressText(state, workstation);
    }

    private void HideAll()
    {
        if (taskButton != null)
        {
            taskButton.gameObject.SetActive(false);
        }

        if (inspectionButton != null)
        {
            inspectionButton.gameObject.SetActive(false);
        }

        if (cleaningButton != null)
        {
            cleaningButton.gameObject.SetActive(false);
        }
    }

    private void HideStressText()
    {
        if (stressText != null)
        {
            stressText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 스트레스 숫자 Text가 버튼 클릭을 가로막지 않도록 Raycast Target을 끈다.
    /// TextMeshProUGUI는 기본적으로 Raycast Target이 켜져 있을 수 있어서,
    /// 버튼 위에 겹치면 과제하기/검사받기/청소하기 버튼 클릭이 먹지 않을 수 있다.
    /// </summary>
    private void DisableStressTextRaycastTarget()
    {
        if (stressText != null)
        {
            stressText.raycastTarget = false;
        }
    }

    /// <summary>
    /// 버튼 클릭 시 사용할 WorkstationTaskController를 찾는다.
    /// WorldUI가 Workstation의 자식이면 자동으로 찾고,
    /// 자동 탐색이 실패하면 Inspector에서 직접 연결해야 한다.
    /// </summary>
    private bool ResolveTaskController()
    {
        if (taskController != null)
        {
            return true;
        }

        taskController = GetComponentInParent<WorkstationTaskController>();

        if (taskController != null)
        {
            return true;
        }

        Debug.LogWarning("WorkstationWorldUI: WorkstationTaskController를 찾지 못했습니다. WorkstationWorldUI 컴포넌트의 Task Controller 필드에 해당 Workstation의 WorkstationTaskController를 직접 연결하세요.");
        return false;
    }

    private void OnClickTaskButton()
    {
        if (UIBlocker.Instance != null && UIBlocker.Instance.IsBlockingWorldInput())
        {
            return;
        }
        if (ResolveTaskController() == false)
        {
            return;
        }

        Debug.Log("과제하기 버튼 클릭됨");
        taskController.StartTask();
    }

    private void OnClickInspectionButton()
    {

        if (UIBlocker.Instance != null && UIBlocker.Instance.IsBlockingWorldInput())
        {
            return;
        }
        if (ResolveTaskController() == false)
        {
            return;
        }

        Debug.Log("검사받기 버튼 클릭됨");
        taskController.StartInspection();
    }

    private void OnClickCleaningButton()
    {
        if (ResolveTaskController() == false)
        {
            return;
        }

        if (UIBlocker.Instance != null && UIBlocker.Instance.IsBlockingWorldInput())
        {
            return;
        }

        Debug.Log("청소하기 버튼 클릭됨");
        taskController.StartCleaning();
    }

    /// <summary>
    /// Workstation 방향에 따라 과제/검사/청소 버튼 위치를 조정한다.
    /// </summary>
    public void ApplyDirection(PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.RD:
                transform.localPosition = rightDownOffset;
                break;

            case PlacementDirection.RU:
                transform.localPosition = rightUpOffset;
                break;

            case PlacementDirection.LD:
                transform.localPosition = leftDownOffset;
                break;

            case PlacementDirection.LU:
                transform.localPosition = leftUpOffset;
                break;
        }
    }

    /// <summary>
    /// 과제하기 버튼만 표시한다.
    /// 인력이 Workstation에 착석했을 때 호출된다.
    /// </summary>
    public void ShowTaskButton()
    {
        SetButtonGroupActiveByNamesForSeatStaff(
            true,
            "TaskButton",
            "Task",
            "TaskIcon",
            "TaskWorkButton",
            "과제하기"
        );

        SetButtonGroupActiveByNamesForSeatStaff(
            false,
            "InspectionButton",
            "Inspection",
            "InspectionIcon",
            "검사받기"
        );

        SetButtonGroupActiveByNamesForSeatStaff(
            false,
            "CleaningButton",
            "CleanButton",
            "Cleaning",
            "CleaningIcon",
            "청소하기"
        );
    }

    private void SetButtonGroupActiveByNamesForSeatStaff(bool active, params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            Transform child = FindDeepChildForSeatStaff(transform, names[i]);

            if (child != null)
            {
                child.gameObject.SetActive(active);
            }
        }
    }

    private Transform FindDeepChildForSeatStaff(Transform parent, string targetName)
    {
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.name == targetName)
            {
                return child;
            }

            Transform found = FindDeepChildForSeatStaff(child, targetName);

            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    public void HideAllButtons()
    {
        SetButtonGroupActiveByNamesForSeatStaff(
            false,
            "TaskButton",
            "Task",
            "TaskIcon",
            "TaskWorkButton",
            "과제하기"
        );

        SetButtonGroupActiveByNamesForSeatStaff(
            false,
            "InspectionButton",
            "Inspection",
            "InspectionIcon",
            "검사받기"
        );

        SetButtonGroupActiveByNamesForSeatStaff(
            false,
            "CleaningButton",
            "CleanButton",
            "Cleaning",
            "CleaningIcon",
            "청소하기"
        );
    }

    /// <summary>
    /// 기존 코드에서 taskState 없이 호출하는 경우를 위한 호환 함수입니다.
    /// 현재 WorkstationTaskController의 상태를 기준으로 스트레스 숫자를 갱신합니다.
    /// </summary>
    public void RefreshStressText(WorkstationObject workstation)
    {
        StaffTaskState state = StaffTaskState.Idle;

        if (taskController != null)
        {
            state = taskController.taskState;
        }

        RefreshStressText(state, workstation);
    }

    /// <summary>
    /// Workstation에 앉아 있는 인력의 스트레스 숫자를 표시한다.
    /// 
    /// 표시 조건:
    /// - Workstation에 seatedStaff가 있어야 함
    /// - 검사 중 상태가 아니어야 함
    /// - StaffWorker의 runtimeData 또는 currentStress 값을 읽음
    /// </summary>
    public void RefreshStressText(StaffTaskState taskState, WorkstationObject workstation)
    {
        if (stressText == null)
        {
            return;
        }

        if (workstation == null)
        {
            HideStressText();
            return;
        }

        // 무조건 Workstation에 실제로 앉아 있는 인력만 표시한다.
        // 타일 위에 서 있는 초기 인력은 여기서 표시되지 않는다.
        if (workstation.hasStaff == false || workstation.seatedStaff == null)
        {
            HideStressText();
            return;
        }

        // 검사 중에는 자리에서 빠져나간 연출이므로 스트레스 숫자를 숨긴다.
        if (taskState == StaffTaskState.Inspecting)
        {
            HideStressText();
            return;
        }

        StaffWorker staff = workstation.seatedStaff;
        int stressValue = staff.currentStress;

        if (staff.runtimeData != null)
        {
            stressValue = staff.runtimeData.currentStress;
        }

        DisableStressTextRaycastTarget();

        stressText.gameObject.SetActive(true);
        stressText.text = stressValue.ToString();
        stressText.color = GetStressColor(stressValue);

        ApplyStressTextOffset(workstation);
    }

    private Color GetStressColor(int stressValue)
    {
        if (stressValue <= 70)
        {
            return Color.green;
        }

        if (stressValue <= 80)
        {
            return Color.yellow;
        }

        if (stressValue <= 90)
        {
            return new Color(1f, 0.5f, 0f);
        }

        return Color.red;
    }

    private void ApplyStressTextOffset(WorkstationObject workstation)
    {
        if (stressText == null || workstation == null)
        {
            return;
        }

        RectTransform rectTransform = stressText.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            return;
        }

        Vector3 offset = stressOffsetRightDown;

        switch (workstation.currentDirection)
        {
            case PlacementDirection.RD:
                offset = stressOffsetRightDown;
                break;

            case PlacementDirection.RU:
                offset = stressOffsetRightUp;
                break;

            case PlacementDirection.LD:
                offset = stressOffsetLeftDown;
                break;

            case PlacementDirection.LU:
                offset = stressOffsetLeftUp;
                break;
        }

        rectTransform.localPosition = offset;
    }
}