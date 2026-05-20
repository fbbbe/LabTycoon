using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 패널 전체를 관리하는 스크립트.
/// 
/// 기능:
/// 1. 상단 카테고리 버튼을 누르면 왼쪽 장비 목록 그룹을 바꾼다.
/// 2. 왼쪽 장비 패널 버튼을 누르면 오른쪽 상세 정보가 바뀐다.
/// 3. 구매 버튼에는 현재 선택된 장비의 가격이 표시된다.
/// 4. 구매 버튼을 누르면 현재 선택된 장비의 구매 로직을 실행한다.
/// </summary>
public class EquipmentShopPanelUI : MonoBehaviour
{
    [Header("카테고리별 왼쪽 목록 그룹")]
    public GameObject computerItemGroup;
    public GameObject researchItemGroup;
    public GameObject environmentItemGroup;
    public GameObject coffeeItemGroup;
    public GameObject cleaningItemGroup;

    [Header("오른쪽 상세 정보 UI")]
    public Image detailItemImage;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescriptionText;
    public TextMeshProUGUI moneyBonusText;
    public TextMeshProUGUI unlockLevelText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI spaceCostText;

    [Header("구매 버튼")]
    public Button buyButton;
    public TextMeshProUGUI buyButtonPriceText;

    [Header("패널 루트")]
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
        ShowCategory(ShopCategory.Computer);
    }

    public void ShowComputerCategory()
    {
        ShowCategory(ShopCategory.Computer);
    }

    public void ShowResearchCategory()
    {
        ShowCategory(ShopCategory.Research);
    }

    public void ShowEnvironmentCategory()
    {
        ShowCategory(ShopCategory.Environment);
    }

    public void ShowCoffeeCategory()
    {
        ShowCategory(ShopCategory.Coffee);
    }

    public void ShowCleaningCategory()
    {
        ShowCategory(ShopCategory.Cleaning);
    }

    /// <summary>
    /// 선택한 카테고리의 장비 목록 그룹만 켜고 나머지는 끈다.
    /// </summary>
    public void ShowCategory(ShopCategory category)
    {
        SetActiveSafe(computerItemGroup, category == ShopCategory.Computer);
        SetActiveSafe(researchItemGroup, category == ShopCategory.Research);
        SetActiveSafe(environmentItemGroup, category == ShopCategory.Environment);
        SetActiveSafe(coffeeItemGroup, category == ShopCategory.Coffee);
        SetActiveSafe(cleaningItemGroup, category == ShopCategory.Cleaning);
        ClearDetailInfo();
    }

    /// <summary>
    /// 왼쪽 장비 목록 버튼을 눌렀을 때 호출된다.
    /// 이 함수가 오른쪽 상세 정보 Text와 Image를 실제로 바꾼다.
    /// </summary>
    public void SelectItem(ShopItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        // 현재 선택된 장비를 저장한다.
        // 구매 버튼을 눌렀을 때 이 selectedItem을 기준으로 구매 처리한다.
        selectedItem = itemData;

        if (detailItemImage != null)
        {
            detailItemImage.enabled = true;
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
    /// 현재 선택된 장비의 purchaseType에 따라 다르게 처리한다.
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
            Debug.LogError("ResourceManager.Instance가 없습니다.");
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
    /// 타일 위에 직접 배치하는 장비 구매.
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

        ClosePanel();

        PlacementManager.Instance.StartPlacement(
            selectedItem.placeablePrefab,
            selectedItem.price
        );
    }

    /// <summary>
    /// 책상 위에 설치하는 장비 구매.
    /// 예: 낡은 노트북, 중고 컴퓨터
    /// 
    /// 실제 Workstation 선택 모드는 다음 단계에서 구현한다.
    /// </summary>
    private void BuyDeskEquipmentItem()
    {
        if (selectedItem.deskEquipmentData == null)
        {
            Debug.LogError(selectedItem.itemName + "의 deskEquipmentData가 비어 있습니다.");
            return;
        }

        Debug.Log("책상 위 장비 구매 선택됨: " + selectedItem.itemName);
    }

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

    /// <summary>
    /// 오른쪽 상세 정보 영역을 비운다.
    /// 
    /// 카테고리를 바꾸거나 상점창을 처음 열 때,
    /// 이전에 선택했던 장비 정보가 남아 있지 않게 초기화한다.
    /// </summary>
    private void ClearDetailInfo()
    {
        // 현재 선택된 아이템이 없다는 뜻.
        selectedItem = null;

        if (detailItemImage != null)
        {
            // 아무 장비도 선택하지 않았을 때 흰 사각형이 보이지 않게 한다.
            detailItemImage.sprite = null;
            detailItemImage.enabled = false;
        }

        if (detailNameText != null)
        {
            detailNameText.text = "";
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = "";
        }

        if (moneyBonusText != null)
        {
            moneyBonusText.text = "";
        }

        if (unlockLevelText != null)
        {
            unlockLevelText.text = "";
        }

        if (priceText != null)
        {
            priceText.text = "";
        }

        if (spaceCostText != null)
        {
            spaceCostText.text = "";
        }

        if (buyButtonPriceText != null)
        {
            buyButtonPriceText.text = "";
        }
    }
}