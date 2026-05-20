using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 패널 전체를 관리하는 스크립트.
/// 
/// 담당 기능:
/// 1. 상단 카테고리 버튼 클릭 시 왼쪽 장비 목록 그룹 변경
/// 2. 왼쪽 장비 버튼 클릭 시 오른쪽 상세 정보 갱신
/// 3. 구매 버튼 가격 텍스트 갱신
/// 4. 구매 버튼 클릭 시 현재 선택된 장비 구매 처리
/// </summary>
public class EquipmentShopPanelUI : MonoBehaviour
{
    [Header("카테고리별 왼쪽 목록 그룹")]
    [Tooltip("컴퓨터 장비 목록 버튼들이 들어있는 부모 오브젝트입니다.")]
    public GameObject computerItemGroup;

    [Tooltip("연구 장비 목록 버튼들이 들어있는 부모 오브젝트입니다.")]
    public GameObject researchItemGroup;

    [Tooltip("환경 장비 목록 버튼들이 들어있는 부모 오브젝트입니다.")]
    public GameObject environmentItemGroup;

    [Tooltip("커피 장비 목록 버튼들이 들어있는 부모 오브젝트입니다.")]
    public GameObject coffeeItemGroup;

    [Tooltip("청소 장비 목록 버튼들이 들어있는 부모 오브젝트입니다.")]
    public GameObject cleaningItemGroup;

    [Header("오른쪽 상세 정보 UI")]
    [Tooltip("선택한 장비 이미지를 표시하는 Image입니다.")]
    public Image detailItemImage;

    [Tooltip("선택한 장비 이름 텍스트입니다.")]
    public TextMeshProUGUI detailNameText;

    [Tooltip("선택한 장비 설명 텍스트입니다.")]
    public TextMeshProUGUI detailDescriptionText;

    [Tooltip("돈 보상 텍스트입니다.")]
    public TextMeshProUGUI moneyBonusText;

    [Tooltip("해금 레벨 텍스트입니다.")]
    public TextMeshProUGUI unlockLevelText;

    [Tooltip("가격 텍스트입니다.")]
    public TextMeshProUGUI priceText;

    [Tooltip("차지 평수 텍스트입니다.")]
    public TextMeshProUGUI spaceCostText;

    [Header("구매 버튼")]
    [Tooltip("초록색 구매 버튼입니다.")]
    public Button buyButton;

    [Tooltip("구매 버튼 안에 표시되는 가격 텍스트입니다.")]
    public TextMeshProUGUI buyButtonPriceText;

    [Header("패널 닫기")]
    [Tooltip("상점 패널 전체 오브젝트입니다.")]
    public GameObject panelRoot;

    [Header("현재 선택된 아이템")]
    public ShopItemData selectedItem;

    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuySelectedItem);
        }
    }

    private void OnEnable()
    {
        // 상점창이 열릴 때 기본으로 컴퓨터 장비 탭을 보여준다.
        ShowCategory(ShopCategory.Computer);
    }

    /// <summary>
    /// 컴퓨터 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowComputerCategory()
    {
        ShowCategory(ShopCategory.Computer);
    }

    /// <summary>
    /// 연구 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowResearchCategory()
    {
        ShowCategory(ShopCategory.Research);
    }

    /// <summary>
    /// 환경 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowEnvironmentCategory()
    {
        ShowCategory(ShopCategory.Environment);
    }

    /// <summary>
    /// 커피 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowCoffeeCategory()
    {
        ShowCategory(ShopCategory.Coffee);
    }

    /// <summary>
    /// 청소 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowCleaningCategory()
    {
        ShowCategory(ShopCategory.Cleaning);
    }

    /// <summary>
    /// 선택된 카테고리에 맞는 왼쪽 목록 그룹만 켜고 나머지는 끈다.
    /// </summary>
    public void ShowCategory(ShopCategory category)
    {
        SetActiveSafe(computerItemGroup, category == ShopCategory.Computer);
        SetActiveSafe(researchItemGroup, category == ShopCategory.Research);
        SetActiveSafe(environmentItemGroup, category == ShopCategory.Environment);
        SetActiveSafe(coffeeItemGroup, category == ShopCategory.Coffee);
        SetActiveSafe(cleaningItemGroup, category == ShopCategory.Cleaning);
    }

    /// <summary>
    /// 왼쪽 장비 목록 버튼을 클릭했을 때 호출된다.
    /// 오른쪽 상세 정보 UI를 선택한 아이템 데이터로 갱신한다.
    /// </summary>
    public void SelectItem(ShopItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        selectedItem = itemData;

        if (detailItemImage != null)
        {
            detailItemImage.sprite = itemData.detailImage;
        }

        if (detailNameText != null)
        {
            detailNameText.text = itemData.itemName;
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = itemData.description;
        }

        if (moneyBonusText != null)
        {
            moneyBonusText.text = itemData.GetMoneyBonusText();
        }

        if (unlockLevelText != null)
        {
            unlockLevelText.text = "Lv." + itemData.unlockLevel;
        }

        if (priceText != null)
        {
            priceText.text = itemData.GetPriceText();
        }

        if (spaceCostText != null)
        {
            spaceCostText.text = itemData.spaceCost + "평";
        }

        if (buyButtonPriceText != null)
        {
            buyButtonPriceText.text = itemData.GetPriceText() + " 구매";
        }
    }

    /// <summary>
    /// 구매 버튼을 눌렀을 때 실행된다.
    /// 현재 선택된 아이템의 구매 타입에 따라 처리 방식을 나눈다.
    /// </summary>
    public void BuySelectedItem()
    {
        if (selectedItem == null)
        {
            Debug.Log("선택된 장비가 없습니다.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("EquipmentShopPanelUI: ResourceManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance.money < selectedItem.price)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        switch (selectedItem.purchaseType)
        {
            case ShopPurchaseType.TilePlaceable:
                BuyTilePlaceableItem();
                break;

            case ShopPurchaseType.DeskEquipment:
                BuyDeskEquipmentItem();
                break;
        }
    }

    /// <summary>
    /// 타일 위에 직접 배치되는 아이템 구매 처리.
    /// 예: 책상+의자 세트, 커피머신
    /// </summary>
    private void BuyTilePlaceableItem()
    {
        if (selectedItem.placeablePrefab == null)
        {
            Debug.LogError(selectedItem.itemName + "의 placeablePrefab이 비어 있습니다.");
            return;
        }

        if (PlacementManager.Instance == null)
        {
            Debug.LogError("PlacementManager.Instance가 없습니다.");
            return;
        }

        // 상점 패널을 닫고 배치 모드로 들어간다.
        ClosePanel();

        PlacementManager.Instance.StartPlacement(
            selectedItem.placeablePrefab,
            selectedItem.price
        );
    }

    /// <summary>
    /// 책상 위에 설치하는 장비 구매 처리.
    /// 예: 낡은 노트북, 중고 컴퓨터
    /// 
    /// 아직 Workstation 선택 모드는 다음 단계에서 구현한다.
    /// 지금은 로그만 출력한다.
    /// </summary>
    private void BuyDeskEquipmentItem()
    {
        if (selectedItem.deskEquipmentData == null)
        {
            Debug.LogError(selectedItem.itemName + "의 deskEquipmentData가 비어 있습니다.");
            return;
        }

        Debug.Log("책상 위 장비 구매는 다음 단계에서 Workstation 선택 모드로 연결합니다: " + selectedItem.itemName);
    }

    /// <summary>
    /// 상점 패널 닫기.
    /// </summary>
    public void ClosePanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void SetActiveSafe(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}