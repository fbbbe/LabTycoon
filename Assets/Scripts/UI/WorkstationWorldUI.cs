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

    private WorkstationTaskController taskController;

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


    private void Awake()
    {
        taskController = GetComponentInParent<WorkstationTaskController>();

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
        HideAll();
    }
    /// <summary>
    /// 현재 상태에 따라 버튼을 갱신한다.
    /// </summary>
    public void Refresh(StaffTaskState state, WorkstationObject workstation)
    {
        HideAll();

        if (workstation == null)
        {
            return;
        }

        // 인력이 앉아 있지 않으면 버튼을 보여주지 않는다.
        if (workstation.hasStaff == false)
        {
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

        RefreshStressText(workstation);
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

    private void OnClickTaskButton()
    {
        if (taskController != null)
        {
            taskController.StartTask();
        }
    }

    private void OnClickInspectionButton()
    {
        if (taskController != null)
        {
            taskController.StartInspection();
        }
    }

    private void OnClickCleaningButton()
    {
        if (taskController != null)
        {
            taskController.StartCleaning();
        }
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

    public void RefreshStressText(WorkstationObject workstation)
    {
        if (stressText == null)
        {
            return;
        }

        if (workstation == null || workstation.seatedStaff == null)
        {
            stressText.gameObject.SetActive(false);
            return;
        }

        StaffWorker staff = workstation.seatedStaff;

        int stressValue = staff.currentStress;

        if (staff.runtimeData != null)
        {
            stressValue = staff.runtimeData.currentStress;
        }

        stressText.gameObject.SetActive(true);
        stressText.text = stressValue.ToString();
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
}