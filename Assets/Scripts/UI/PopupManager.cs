using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public MoneyPopup moneyPopupPrefab;

    private void Awake()
    {
        Instance = this;
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
    }
}