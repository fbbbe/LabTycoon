using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이 화면에서 인력을 클릭했을 때 나오는 인력 스탯 패널.
/// 
/// 패널 배경 PNG는 인력 종류에 따라 바뀐다.
/// 실제 스탯 값은 selectedStaff의 StaffRuntimeData를 기준으로 표시한다.
/// </summary>
public class StaffStatusPanelUI : MonoBehaviour
{
    public static StaffStatusPanelUI Instance;

    [Header("패널")]
    public GameObject panelRoot;
    public Image panelBackgroundImage;

    [Header("인력 종류별 패널 PNG")]
    public Sprite undergraduatePanelSprite;
    public Sprite masterPanelSprite;
    public Sprite phdPanelSprite;

    [Header("스탯 텍스트")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI researchPowerText;
    public TextMeshProUGUI taskGradeText;
    public TextMeshProUGUI dailyPayText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI taskStressText;

    [Header("투명 버튼")]
    public Button reduceStressButton;
    public Button levelUpButton;
    public Button evolveButton;
    public Button closeButton;

    private StaffWorker selectedStaff;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (reduceStressButton != null)
        {
            reduceStressButton.onClick.RemoveAllListeners();
            reduceStressButton.onClick.AddListener(OnClickReduceStressButton);
        }

        if (levelUpButton != null)
        {
            levelUpButton.onClick.RemoveAllListeners();
            levelUpButton.onClick.AddListener(OnClickLevelUpButton);
        }

        if (evolveButton != null)
        {
            evolveButton.onClick.RemoveAllListeners();
            evolveButton.onClick.AddListener(OnClickEvolveButton);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        Close();
    }

    public void Open(StaffWorker staff)
    {
        if (staff == null)
        {
            Debug.LogWarning("StaffStatusPanelUI: 표시할 인력이 없습니다.");
            return;
        }

        selectedStaff = staff;

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        Refresh();
    }

    public void Close()
    {
        selectedStaff = null;

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (selectedStaff == null)
        {
            Close();
            return;
        }

        StaffRuntimeData data = selectedStaff.runtimeData;

        string staffName = selectedStaff.staffName;
        StaffType staffType = selectedStaff.staffType;
        int level = selectedStaff.level;
        int researchPower = selectedStaff.researchPower;
        int currentStress = selectedStaff.currentStress;

        if (data != null)
        {
            staffName = data.staffName;
            staffType = data.staffType;
            level = data.level;
            researchPower = data.researchPower;
            currentStress = data.currentStress;
        }

        ApplyPanelSprite(staffType);

        TaskGrade grade = TaskGradeDatabase.GetTaskGradeByStaff(selectedStaff);
        TaskData taskData = TaskGradeDatabase.CreateTaskData(grade);

        if (levelText != null)
        {
            levelText.text = "Lv." + level;
        }

        if (researchPowerText != null)
        {
            researchPowerText.text = researchPower.ToString();
        }

        if (taskGradeText != null)
        {
            taskGradeText.text = GetTaskGradeName(grade);
        }

        if (dailyPayText != null)
        {
            dailyPayText.text = taskData.baseMoneyReward.ToString("N0") + "$";
        }

        if (stressText != null)
        {
            stressText.text = currentStress + " / 100";
        }

        if (taskStressText != null)
        {
            taskStressText.text = "+" + taskData.baseTaskStress;
        }
    }

    private void ApplyPanelSprite(StaffType staffType)
    {
        if (panelBackgroundImage == null)
        {
            return;
        }

        switch (staffType)
        {
            case StaffType.Undergraduate:
                panelBackgroundImage.sprite = undergraduatePanelSprite;
                break;

            case StaffType.Master:
                panelBackgroundImage.sprite = masterPanelSprite;
                break;

            case StaffType.PhD:
                panelBackgroundImage.sprite = phdPanelSprite;
                break;
        }
    }

    private string GetTaskGradeName(TaskGrade grade)
    {
        switch (grade)
        {
            case TaskGrade.F:
                return "F";

            case TaskGrade.E:
                return "E";

            case TaskGrade.D:
                return "D";

            case TaskGrade.C:
                return "C";

            case TaskGrade.B:
                return "B";

            case TaskGrade.A:
                return "A";

            case TaskGrade.S:
                return "S";

            case TaskGrade.SS:
                return "SS";

            case TaskGrade.SSS:
                return "SSS";

            case TaskGrade.National:
                return "National Grade";

            case TaskGrade.World:
                return "World Grade";

            default:
                return "F";
        }
    }

    private void OnClickReduceStressButton()
    {
        if (selectedStaff == null)
        {
            return;
        }

        Debug.Log("스트레스 감소시키기 버튼 클릭: " + selectedStaff.staffName);

        // 나중에 스트레스 감소 창 열기로 연결.
        // 예: StressRestPanelUI.Instance.Open(selectedStaff);
    }

    private void OnClickLevelUpButton()
    {
        if (selectedStaff == null)
        {
            return;
        }

        Debug.Log("레벨업 버튼 클릭: " + selectedStaff.staffName);

        // 다음 단계에서 레벨업 조건/비용/과제 수행 횟수 검사 구현.
    }

    private void OnClickEvolveButton()
    {
        if (selectedStaff == null)
        {
            return;
        }

        Debug.Log("진화 버튼 클릭: " + selectedStaff.staffName);

        // 다음 단계에서 학사 → 석사, 석사 → 박사 진화 조건 구현.
    }
}