using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 왼쪽 장비 목록 버튼에 붙는 스크립트.
/// 
/// 각 장비 패널은 PNG 버튼으로 되어 있고,
/// 이 스크립트는 해당 버튼이 어떤 ShopItemData를 의미하는지 저장한다.
/// 
/// 클릭하면 EquipmentShopPanelUI에게
/// "이 장비가 선택되었다"고 알려준다.
/// </summary>
public class ShopItemButton : MonoBehaviour
{
    [Header("상점 패널")]
    [Tooltip("상점 전체를 관리하는 UI 스크립트입니다.")]
    public EquipmentShopPanelUI shopPanelUI;

    [Header("연결된 아이템 데이터")]
    [Tooltip("이 버튼이 의미하는 장비 데이터입니다.")]
    public ShopItemData itemData;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnClickItem);
        }
    }

    /// <summary>
    /// 왼쪽 장비 목록 버튼을 클릭했을 때 실행된다.
    /// </summary>
    private void OnClickItem()
    {
        if (shopPanelUI == null)
        {
            Debug.LogError("ShopItemButton: shopPanelUI가 연결되지 않았습니다.");
            return;
        }

        if (itemData == null)
        {
            Debug.LogError("ShopItemButton: itemData가 연결되지 않았습니다.");
            return;
        }

        shopPanelUI.SelectItem(itemData);
    }
}