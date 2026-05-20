using UnityEngine;

/// <summary>
/// 장비샵 패널을 열고 닫는 UI 스크립트.
/// 
/// 역할:
/// 1. ShopButton을 누르면 EquipmentShopPanel을 연다.
/// 2. CloseShopButton을 누르면 EquipmentShopPanel을 닫는다.
/// 
/// 여기서는 아직 실제 구매 기능을 처리하지 않는다.
/// 구매 기능은 EquipmentShop.cs와 연결해서 다음 단계에서 구현한다.
/// </summary>
public class ShopPanelUI : MonoBehaviour
{
    [Header("상점 패널")]
    [Tooltip("열고 닫을 장비샵 패널 오브젝트입니다.")]
    public GameObject equipmentShopPanel;

    /// <summary>
    /// 장비샵 패널을 연다.
    /// 
    /// ShopButton의 OnClick에 연결할 함수다.
    /// </summary>
    public void OpenShopPanel()
    {
        if (equipmentShopPanel == null)
        {
            Debug.LogError("ShopPanelUI: EquipmentShopPanel이 연결되지 않았습니다.");
            return;
        }

        equipmentShopPanel.SetActive(true);
    }

    /// <summary>
    /// 장비샵 패널을 닫는다.
    /// 
    /// CloseShopButton의 OnClick에 연결할 함수다.
    /// </summary>
    public void CloseShopPanel()
    {
        if (equipmentShopPanel == null)
        {
            Debug.LogError("ShopPanelUI: EquipmentShopPanel이 연결되지 않았습니다.");
            return;
        }

        equipmentShopPanel.SetActive(false);
    }

    /// <summary>
    /// 열려 있으면 닫고, 닫혀 있으면 여는 함수.
    /// 필요하면 나중에 사용한다.
    /// </summary>
    public void ToggleShopPanel()
    {
        if (equipmentShopPanel == null)
        {
            Debug.LogError("ShopPanelUI: EquipmentShopPanel이 연결되지 않았습니다.");
            return;
        }

        equipmentShopPanel.SetActive(!equipmentShopPanel.activeSelf);
    }
}