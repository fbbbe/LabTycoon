using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 패널 전체를 관리하는 스크립트.
/// 
/// 현재 구조:
/// - EquipmentShopPanel 자체가 상점 배경 Panel
/// - 카테고리별로 왼쪽 장비 목록 PNG가 바뀜
/// - 왼쪽 목록 위의 투명 버튼 슬롯으로 장비 선택
/// - 오른쪽 상세 정보는 선택된 ShopItemData로 갱신
/// - 페이지 버튼은 사용하지 않음
/// </summary>
public class EquipmentShopPanelUI : MonoBehaviour
{
    [Header("패널")]
    public GameObject panelRoot;

    [Header("왼쪽 장비 목록 이미지")]
    public Image itemListImage;

    [Header("카테고리별 왼쪽 목록 PNG")]
    public Sprite computerListSprite;
    public Sprite researchListSprite;
    public Sprite environmentListSprite;
    public Sprite coffeeListSprite;
    public Sprite cleaningListSprite;

    [Header("왼쪽 목록 투명 버튼 슬롯")]
    public Button[] itemButtons;

    [Header("카테고리별 아이템 데이터")]
    public ShopItemData[] computerItems;
    public ShopItemData[] researchItems;
    public ShopItemData[] environmentItems;
    public ShopItemData[] coffeeItems;
    public ShopItemData[] cleaningItems;

    [Header("오른쪽 상세 정보")]
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

    [Header("하단 보유 금액")]
    public TextMeshProUGUI ownedMoneyText;

    [Header("현재 선택 상태")]
    public ShopCategory currentCategory = ShopCategory.Computer;
    public ShopItemData selectedItem;

    private ShopItemData[] currentItems;

    private void Awake()
    {
        RegisterItemButtons();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuySelectedItem);
        }
    }

    private void OnEnable()
    {
        ShowComputerCategory();
    }

    private void RegisterItemButtons()
    {
        if (itemButtons == null)
        {
            return;
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            int localIndex = i;

            if (itemButtons[i] == null)
            {
                continue;
            }

            itemButtons[i].onClick.RemoveAllListeners();
            itemButtons[i].onClick.AddListener(() => SelectItemByIndex(localIndex));
        }
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

    private void ShowCategory(ShopCategory category)
    {
        currentCategory = category;

        switch (category)
        {
            case ShopCategory.Computer:
                currentItems = computerItems;
                SetItemListSprite(computerListSprite);
                break;

            case ShopCategory.Research:
                currentItems = researchItems;
                SetItemListSprite(researchListSprite);
                break;

            case ShopCategory.Environment:
                currentItems = environmentItems;
                SetItemListSprite(environmentListSprite);
                break;

            case ShopCategory.Coffee:
                currentItems = coffeeItems;
                SetItemListSprite(coffeeListSprite);
                break;

            case ShopCategory.Cleaning:
                currentItems = cleaningItems;
                SetItemListSprite(cleaningListSprite);
                break;
        }

        RefreshItemButtonStates();
        ClearDetailInfo();
        RefreshOwnedMoneyText();
    }

    private void SetItemListSprite(Sprite sprite)
    {
        if (itemListImage == null)
        {
            return;
        }

        itemListImage.sprite = sprite;
        itemListImage.enabled = sprite != null;
    }

    private void RefreshItemButtonStates()
    {
        if (itemButtons == null)
        {
            return;
        }

        int count = currentItems == null ? 0 : currentItems.Length;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i] == null)
            {
                continue;
            }

            itemButtons[i].gameObject.SetActive(i < count);
        }
    }

    private void SelectItemByIndex(int index)
    {
        if (currentItems == null)
        {
            return;
        }

        if (index < 0 || index >= currentItems.Length)
        {
            return;
        }

        SelectItem(currentItems[index]);
    }

    private void SelectItem(ShopItemData item)
    {
        if (item == null)
        {
            return;
        }

        selectedItem = item;

        if (detailItemImage != null)
        {
            detailItemImage.enabled = true;
            detailItemImage.sprite = item.detailImage;
        }

        if (detailNameText != null)
        {
            detailNameText.text = item.itemName;
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = item.description;
        }

        if (moneyBonusText != null)
        {
            moneyBonusText.text = item.GetMoneyBonusText();
        }

        if (unlockLevelText != null)
        {
            unlockLevelText.text = "Lv." + item.unlockLevel;
        }

        if (priceText != null)
        {
            priceText.text = item.GetPriceText();
        }

        if (spaceCostText != null)
        {
            spaceCostText.text = item.spaceCost + "평";
        }

        if (buyButtonPriceText != null)
        {
            buyButtonPriceText.text = item.GetBuyButtonText();
        }
    }

    private void ClearDetailInfo()
    {
        selectedItem = null;

        if (detailItemImage != null)
        {
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

    private void RefreshOwnedMoneyText()
    {
        if (ownedMoneyText == null)
        {
            return;
        }

        if (ResourceManager.Instance == null)
        {
            ownedMoneyText.text = "";
            return;
        }

        ownedMoneyText.text = ResourceManager.Instance.money.ToString("N0") + "원";
    }

    private void BuySelectedItem()
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
                BuyTilePlaceable();
                break;

            case ShopPurchaseType.DeskEquipment:
                BuyDeskEquipment();
                break;
        }
    }

    private void BuyTilePlaceable()
    {
        if (selectedItem.placeablePrefab == null)
        {
            Debug.LogError(selectedItem.itemName + "의 placeablePrefab이 없습니다.");
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

    private void BuyDeskEquipment()
    {
        if (selectedItem.deskEquipmentData == null)
        {
            Debug.LogError(selectedItem.itemName + "의 deskEquipmentData가 없습니다.");
            return;
        }

        Debug.Log("책상 위 장비 구매 선택: " + selectedItem.itemName);
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
}