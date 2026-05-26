using TMPro;
using UnityEngine;

/// <summary>
/// 메인 HUD의 돈, 연구성과, 연구실 레벨 Text를 ResourceManager와 연결한다.
/// </summary>
public class MainHUDUI : MonoBehaviour
{
    [Header("돈 HUD")]
    public TextMeshProUGUI moneyText;

    [Header("연구성과 HUD")]
    public TextMeshProUGUI researchResultText;

    [Header("전체 레벨 HUD")]
    public TextMeshProUGUI labLevelText;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += Refresh;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= Refresh;
        }
    }

    /// <summary>
    /// ResourceManager의 현재 값을 HUD Text에 반영한다.
    /// </summary>
    public void Refresh()
    {
        if (ResourceManager.Instance == null)
        {
            return;
        }

        if (moneyText != null)
        {
            moneyText.text = FormatMoney(ResourceManager.Instance.money);
        }

        if (researchResultText != null)
        {
            researchResultText.text = ResourceManager.Instance.researchResult.ToString();
        }

        if (labLevelText != null)
        {
            labLevelText.text = "Lv." + ResourceManager.Instance.labLevel;
        }
    }

    private string FormatMoney(long value)
    {
        return value.ToString("N0") + "$";
    }
}