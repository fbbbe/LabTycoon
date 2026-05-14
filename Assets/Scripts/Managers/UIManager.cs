using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("자원 UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI researchText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timeText;

    [Header("생산량 UI")]
    public TextMeshProUGUI moneyProductionText;
    public TextMeshProUGUI researchProductionText;
    public TextMeshProUGUI stressProductionText;

    private void Update()
    {
        if (ResourceManager.Instance == null)
        {
            return;
        }

        moneyText.text = "돈: " + ResourceManager.Instance.money.ToString("N0") + "원";
        researchText.text = "연구력: " + ResourceManager.Instance.research + " / " + ResourceManager.Instance.researchRequiredForLevelUp;
        stressText.text = "스트레스: " + ResourceManager.Instance.stress + " / 100";
        levelText.text = "레벨: " + ResourceManager.Instance.level;

        moneyProductionText.text = "돈 생산량: +" + ResourceManager.Instance.moneyPerTick + " / 틱";
        researchProductionText.text = "연구력 생산량: +" + ResourceManager.Instance.researchPerTick + " / 틱";
        stressProductionText.text = "스트레스 변화량: " + ResourceManager.Instance.stressPerTick + " / 틱";

        if (TimeManager.Instance != null)
        {
            timeText.text = TimeManager.Instance.GetTimeText();
        }
    }
}