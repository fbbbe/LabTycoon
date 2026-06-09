using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabInfoPanelUI : MonoBehaviour
{
    public static LabInfoPanelUI Instance;

    [Header("연구실 정보 패널")]
    public GameObject panelRoot;

    [Header("기본 정보 텍스트")]
    public TextMeshProUGUI labNameText;
    public TextMeshProUGUI labGradeText;
    public TextMeshProUGUI labAreaText;
    public TextMeshProUGUI requiredLevelText;
    public TextMeshProUGUI expansionCostText;

    [Header("연구실 이름")]
    public string labName = "연구실";

    [Header("버튼")]
    public Button expandButton;
    public Button closeButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (expandButton != null)
        {
            expandButton.onClick.RemoveAllListeners();
            expandButton.onClick.AddListener(OnClickExpandButton);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        Close();
    }

    public void Open()
    {
        Refresh();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    public void Close()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Refresh()
    {
        int currentLabLevel = GetCurrentLabLevel();
        int currentArea = GetCurrentArea();

        LabExpansionData nextExpansionData = null;

        if (LabExpansionManager.Instance != null)
        {
            nextExpansionData = LabExpansionManager.Instance.GetNextExpansionData();
        }

        if (labNameText != null)
        {
            labNameText.text = labName;
        }

        if (labGradeText != null)
        {
            labGradeText.text = GetLabGradeName(currentLabLevel);
        }

        if (labAreaText != null)
        {
            labAreaText.text = currentArea + "m²";
        }

        if (requiredLevelText != null)
        {
            if (nextExpansionData == null)
            {
                requiredLevelText.text = "0";
            }
            else
            {
                int remainingLevel = nextExpansionData.requiredLabLevel - currentLabLevel;

                if (remainingLevel < 0)
                {
                    remainingLevel = 0;
                }

                requiredLevelText.text = remainingLevel.ToString();
            }
        }

        if (expansionCostText != null)
        {
            if (nextExpansionData == null)
            {
                expansionCostText.text = "최대 확장";
            }
            else
            {
                expansionCostText.text = nextExpansionData.cost.ToString("N0") + "$";
            }
        }
    }

    private void OnClickExpandButton()
    {
        if (LabExpansionManager.Instance == null)
        {
            Debug.LogWarning("LabExpansionManager.Instance가 없습니다.");
            return;
        }

        bool success = LabExpansionManager.Instance.TryExpandLab();

        if (success)
        {
            Refresh();
        }
    }

    private int GetCurrentLabLevel()
    {
        if (ResourceManager.Instance == null)
        {
            return 1;
        }

        return ResourceManager.Instance.labLevel;
    }

    private int GetCurrentArea()
    {
        if (LabExpansionManager.Instance != null)
        {
            return LabExpansionManager.Instance.GetCurrentArea();
        }

        if (LabGridManager.Instance != null)
        {
            return LabGridManager.Instance.GetCurrentArea();
        }

        return 9;
    }

    private string GetLabGradeName(int labLevel)
    {
        if (labLevel >= 1 && labLevel <= 10)
        {
            return "Underground Lab";
        }

        if (labLevel >= 11 && labLevel <= 25)
        {
            return "Ground";
        }

        if (labLevel >= 26 && labLevel <= 40)
        {
            return "Ground Lab";
        }

        if (labLevel >= 41 && labLevel <= 60)
        {
            return "Basic Lab";
        }

        if (labLevel >= 61 && labLevel <= 75)
        {
            return "Big Lab";
        }

        if (labLevel >= 76 && labLevel <= 90)
        {
            return "Research Center";
        }

        return "Sky Lab";
    }
}