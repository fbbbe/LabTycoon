using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public MoneyPopup moneyPopupPrefab;

    [Header("효과음")]
    public AudioClip coinSound;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ShowMoneyPopup(Vector3 worldPosition, long amount)
    {
        Vector3 screenPos =
            Camera.main.WorldToScreenPoint(worldPosition);

        MoneyPopup popup =
            Instantiate(
                moneyPopupPrefab,
                transform
            );

        popup.transform.position = screenPos;

        popup.Setup(amount);

        PlayCoinSound();
    }

    private void PlayCoinSound()
    {
        if (coinSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(coinSound);
    }
}