using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log("청소하기 버튼 클릭됨. 다음 단계에서 청소 시스템 연결 예정.");
    }
}