using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 왼쪽 장비 목록 위에 올리는 투명 버튼에 붙는 스크립트.
/// 
/// 이 스크립트의 역할:
/// 1. 이 버튼이 어떤 장비를 의미하는지 equipmentName 문자열로 저장한다.
/// 2. 버튼을 누르면 EquipmentDatabase에서 같은 이름의 장비를 찾는다.
/// 3. 찾은 장비 데이터를 EquipmentShopPanelUI에 넘긴다.
/// 
/// 중요한 점:
/// - 가격, 효과, 해금 레벨, 차지 평수는 여기서 입력하지 않는다.
/// - 이 버튼에는 장비 이름만 입력한다.
/// </summary>
[RequireComponent(typeof(Button))]
public class ShopItemSlotButton : MonoBehaviour
{
    [Header("상점 패널 UI")]
    [Tooltip("EquipmentShopPanelUI가 붙어 있는 EquipmentPanel 오브젝트를 연결합니다.")]
    public EquipmentShopPanelUI shopPanelUI;

    [Header("이 버튼이 선택할 장비 이름")]
    [Tooltip("EquipmentDatabase.cs에 등록된 장비 이름과 정확히 일치해야 합니다.")]
    public string equipmentName;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        // 버튼 클릭 이벤트를 코드에서 자동 등록한다.
        // 그래서 Inspector의 OnClick에는 따로 함수 연결하지 않아도 된다.
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickSlot);
    }

    /// <summary>
    /// 투명 슬롯 버튼을 클릭했을 때 실행된다.
    /// </summary>
    private void OnClickSlot()
    {
        if (shopPanelUI == null)
        {
            Debug.LogError("ShopItemSlotButton: shopPanelUI가 연결되지 않았습니다.");
            return;
        }

        if (string.IsNullOrWhiteSpace(equipmentName))
        {
            Debug.LogError("ShopItemSlotButton: equipmentName이 비어 있습니다.");
            return;
        }

        if (EquipmentDatabase.Instance == null)
        {
            Debug.LogError("ShopItemSlotButton: EquipmentDatabase.Instance가 없습니다.");
            return;
        }

        // 장비 이름으로 EquipmentDatabase에서 실제 장비 데이터를 찾는다.
        EquipmentData equipmentData = EquipmentDatabase.Instance.FindByName(equipmentName);

        if (equipmentData == null)
        {
            return;
        }

        // 찾은 장비 데이터를 상점 패널에 전달한다.
        shopPanelUI.SelectEquipment(equipmentData);
    }
}