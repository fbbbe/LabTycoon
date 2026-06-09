using System.Collections;
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

    private long displayedMoney;

    private Coroutine moneyAnimationCoroutine;
    private Coroutine moneyPunchCoroutine;

    private Vector3 originalMoneyScale;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += Refresh;
            displayedMoney = ResourceManager.Instance.money;
        }

        if (moneyText != null)
        {
            originalMoneyScale = moneyText.transform.localScale;
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
            StartMoneyAnimation(ResourceManager.Instance.money);
        }

        if (researchResultText != null)
        {
            researchResultText.text =
                ResourceManager.Instance.researchResult.ToString();
        }

        if (labLevelText != null)
        {
            labLevelText.text =
                "Lv." + ResourceManager.Instance.labLevel;
        }
    }

    private void StartMoneyAnimation(long targetMoney)
    {
        // 돈이 증가한 경우에만 튕김 효과
        if (targetMoney > displayedMoney)
        {
            if (moneyPunchCoroutine != null)
            {
                StopCoroutine(moneyPunchCoroutine);
            }

            moneyPunchCoroutine =
                StartCoroutine(PunchMoneyText());
        }

        if (moneyAnimationCoroutine != null)
        {
            StopCoroutine(moneyAnimationCoroutine);
        }

        moneyAnimationCoroutine =
            StartCoroutine(AnimateMoney(targetMoney));
    }

    private IEnumerator AnimateMoney(long targetMoney)
    {
        long startMoney = displayedMoney;

        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            displayedMoney = (long)Mathf.Lerp(
                startMoney,
                targetMoney,
                t
            );

            if (moneyText != null)
            {
                moneyText.text =
                    FormatMoney(displayedMoney);
            }

            yield return null;
        }

        displayedMoney = targetMoney;

        if (moneyText != null)
        {
            moneyText.text =
                FormatMoney(displayedMoney);
        }
    }

    /// <summary>
    /// 돈 획득 시 HUD가 살짝 커졌다가 원래 크기로 돌아온다.
    /// </summary>
    private IEnumerator PunchMoneyText()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            float scale = Mathf.Lerp(
                1.15f,
                1f,
                t
            );

            moneyText.transform.localScale =
                originalMoneyScale * scale;

            yield return null;
        }

        moneyText.transform.localScale =
            originalMoneyScale;
    }

    private string FormatMoney(long value)
    {
        return value.ToString("N0") + "$";
    }
}