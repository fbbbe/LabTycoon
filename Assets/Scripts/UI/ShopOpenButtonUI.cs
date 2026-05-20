using UnityEngine;

/// <summary>
/// 메인 화면의 상점 버튼을 눌렀을 때 상점 패널을 여는 스크립트.
/// 
/// 역할:
/// - 메인 화면 ShopButton 클릭
/// - EquipmentShopPanel 활성화
/// 
/// 이 스크립트는 상점 내부 로직을 처리하지 않는다.
/// 단순히 상점창을 열기만 한다.
/// </summary>
public class ShopOpenButtonUI : MonoBehaviour
{
    [Header("열 상점 패널")]
    [Tooltip("Canvas 아래의 EquipmentShopPanel 오브젝트를 연결합니다.")]
    public GameObject equipmentShopPanel;

    /// <summary>
    /// 상점 패널을 연다.
    /// 메인 화면 ShopButton의 OnClick에 연결한다.
    /// </summary>
    public void OpenShop()
    {
        if (equipmentShopPanel == null)
        {
            Debug.LogError("ShopOpenButtonUI: EquipmentShopPanel이 연결되지 않았습니다.");
            return;
        }

        equipmentShopPanel.SetActive(true);
    }
}