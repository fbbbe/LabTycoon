using TMPro;
using UnityEngine;

/// <summary>
/// 메인 화면 HUD를 갱신하는 스크립트.
/// 
/// 메인 HUD에는 전체 자원만 표시한다.
/// 인력별 연구력/스트레스/상태는 나중에 StaffInfoPanel에서 표시한다.
/// </summary>
public class MainHUDUI : MonoBehaviour
{
    [Header("자원 텍스트")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI researchResultText;
    public TextMeshProUGUI levelText;

    private void Update()
    {
        if (ResourceManager.Instance == null)
        {
            return;
        }

        if (moneyText != null)
        {
            moneyText.text = ResourceManager.Instance.money.ToString("N0") + " G";
        }

        if (researchResultText != null)
        {
            researchResultText.text = ResourceManager.Instance.researchResult.ToString("N0");
        }

        if (levelText != null)
        {
            levelText.text = "LV. " + ResourceManager.Instance.level;
        }
    }
}