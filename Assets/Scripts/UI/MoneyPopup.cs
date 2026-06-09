using TMPro;
using UnityEngine;

public class MoneyPopup : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private CanvasGroup canvasGroup;

    private float lifeTime = 1.2f; // 시간
    private float moveSpeed = 25f; // 속도

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();

        Debug.Log("TMP 찾음? " + (textUI != null));

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Setup(long amount)
    {
        Debug.Log("Popup 생성됨 : " + amount);

        textUI.text = "+" + amount.ToString("N0");
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        lifeTime -= Time.deltaTime;

        canvasGroup.alpha = lifeTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}