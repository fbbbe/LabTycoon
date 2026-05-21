using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 패널 전체를 관리하는 스크립트.
/// 
/// 현재 구조:
/// - EquipmentPanel 자체가 상점 배경 PNG를 가진다.
/// - 상단 카테고리 버튼을 누르면 왼쪽 장비 목록 PNG가 교체된다.
/// - 왼쪽 목록 위의 투명 버튼을 누르면 장비 이름으로 EquipmentDatabase에서 데이터를 찾는다.
/// - 오른쪽 상세 정보는 장비별 DetailCard PNG로 통째로 교체된다.
/// - 구매 버튼을 누르면 현재 선택된 장비의 구매 방식에 따라 처리한다.
/// 
/// 이 스크립트는 장비 가격/효과 정보를 직접 들고 있지 않는다.
/// 장비 정보는 EquipmentDatabase.cs가 담당한다.
/// </summary>
public class EquipmentShopPanelUI : MonoBehaviour
{
    [Header("패널")]
    [Tooltip("상점 패널 전체 오브젝트입니다. 보통 자기 자신 EquipmentPanel을 넣습니다.")]
    public GameObject panelRoot;

    [Header("왼쪽 장비 목록 이미지")]
    [Tooltip("카테고리에 따라 바뀌는 왼쪽 장비 목록 Image입니다.")]
    public Image itemListImage;

    [Header("카테고리별 왼쪽 목록 PNG")]
    [Tooltip("컴퓨터 장비 목록 PNG입니다.")]
    public Sprite computerListSprite;

    [Tooltip("연구 장비 목록 PNG입니다.")]
    public Sprite researchListSprite;

    [Tooltip("환경 장비 목록 PNG입니다.")]
    public Sprite environmentListSprite;

    [Tooltip("커피 장비 목록 PNG입니다.")]
    public Sprite coffeeListSprite;

    [Tooltip("청소 장비 목록 PNG입니다.")]
    public Sprite cleaningListSprite;

    [Header("카테고리별 투명 버튼 그룹")]
    [Tooltip("컴퓨터 장비 투명 버튼들이 들어 있는 부모 오브젝트입니다.")]
    public GameObject computerButtonsRoot;

    [Tooltip("연구 장비 투명 버튼들이 들어 있는 부모 오브젝트입니다.")]
    public GameObject researchButtonsRoot;

    [Tooltip("환경 장비 투명 버튼들이 들어 있는 부모 오브젝트입니다.")]
    public GameObject environmentButtonsRoot;

    [Tooltip("커피 장비 투명 버튼들이 들어 있는 부모 오브젝트입니다.")]
    public GameObject coffeeButtonsRoot;

    [Tooltip("청소 장비 투명 버튼들이 들어 있는 부모 오브젝트입니다.")]
    public GameObject cleaningButtonsRoot;

    [Header("오른쪽 상세 카드")]
    [Tooltip("선택한 장비의 상세 카드 PNG를 표시하는 Image입니다.")]
    public Image detailCardImage;

    [Header("구매 버튼")]
    [Tooltip("구매 버튼입니다.")]
    public Button buyButton;

    [Header("현재 선택 상태")]
    [Tooltip("현재 선택된 장비 데이터입니다.")]
    public EquipmentData selectedEquipment;

    private EquipmentCategory currentCategory = EquipmentCategory.Computer;

    private void Awake()
    {
        if (buyButton != null)
        {
            // 구매 버튼은 항상 현재 selectedEquipment를 기준으로 처리한다.
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuySelectedEquipment);
        }
    }

    private void OnEnable()
    {
        // 상점창이 열리면 기본으로 컴퓨터 장비 탭을 보여준다.
        ShowComputerCategory();
    }

    /// <summary>
    /// 컴퓨터 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowComputerCategory()
    {
        ShowCategory(EquipmentCategory.Computer);
    }

    /// <summary>
    /// 연구 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowResearchCategory()
    {
        ShowCategory(EquipmentCategory.Research);
    }

    /// <summary>
    /// 환경 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowEnvironmentCategory()
    {
        ShowCategory(EquipmentCategory.Environment);
    }

    /// <summary>
    /// 커피 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowCoffeeCategory()
    {
        ShowCategory(EquipmentCategory.Coffee);
    }

    /// <summary>
    /// 청소 장비 탭 버튼에 연결할 함수.
    /// </summary>
    public void ShowCleaningCategory()
    {
        ShowCategory(EquipmentCategory.Cleaning);
    }

    /// <summary>
    /// 카테고리를 변경한다.
    /// 
    /// 변경되는 것:
    /// 1. 왼쪽 목록 PNG
    /// 2. 해당 카테고리의 투명 버튼 그룹
    /// 3. 오른쪽 상세 카드 초기화
    /// </summary>
    private void ShowCategory(EquipmentCategory category)
    {
        currentCategory = category;

        switch (category)
        {
            case EquipmentCategory.Computer:
                SetItemListSprite(computerListSprite);
                break;

            case EquipmentCategory.Research:
                SetItemListSprite(researchListSprite);
                break;

            case EquipmentCategory.Environment:
                SetItemListSprite(environmentListSprite);
                break;

            case EquipmentCategory.Coffee:
                SetItemListSprite(coffeeListSprite);
                break;

            case EquipmentCategory.Cleaning:
                SetItemListSprite(cleaningListSprite);
                break;
        }

        SetButtonGroup(category);
        ClearSelection();
    }

    /// <summary>
    /// 왼쪽 장비 목록 PNG를 교체한다.
    /// </summary>
    private void SetItemListSprite(Sprite listSprite)
    {
        if (itemListImage == null)
        {
            return;
        }

        itemListImage.sprite = listSprite;
        itemListImage.enabled = listSprite != null;
    }

    /// <summary>
    /// 현재 카테고리에 맞는 투명 버튼 그룹만 켜고 나머지는 끈다.
    /// 
    /// 각 카테고리마다 장비 개수가 다를 수 있으므로,
    /// 카테고리별로 버튼 그룹을 따로 관리한다.
    /// </summary>
    private void SetButtonGroup(EquipmentCategory category)
    {
        SetActiveSafe(computerButtonsRoot, category == EquipmentCategory.Computer);
        SetActiveSafe(researchButtonsRoot, category == EquipmentCategory.Research);
        SetActiveSafe(environmentButtonsRoot, category == EquipmentCategory.Environment);
        SetActiveSafe(coffeeButtonsRoot, category == EquipmentCategory.Coffee);
        SetActiveSafe(cleaningButtonsRoot, category == EquipmentCategory.Cleaning);
    }

    /// <summary>
    /// 왼쪽 투명 버튼이 장비를 선택했을 때 호출된다.
    /// 
    /// 이 함수는 ShopItemSlotButton이 호출한다.
    /// </summary>
    public void SelectEquipment(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        selectedEquipment = equipmentData;

        if (detailCardImage != null)
        {
            detailCardImage.enabled = true;
            detailCardImage.sprite = selectedEquipment.LoadDetailCardSprite();
        }

        Debug.Log("상점 장비 선택: " + selectedEquipment.equipmentName);
    }

    /// <summary>
    /// 선택 상태를 비운다.
    /// 카테고리를 바꾸면 이전 상세 카드가 남아 있지 않도록 초기화한다.
    /// </summary>
    private void ClearSelection()
    {
        selectedEquipment = null;

        if (detailCardImage != null)
        {
            detailCardImage.sprite = null;
            detailCardImage.enabled = false;
        }
    }

    /// <summary>
    /// 구매 버튼을 눌렀을 때 실행된다.
    /// 
    /// 주의:
    /// 여기서 돈 차감/효과 적용을 하지 않는다.
    /// 돈 차감과 효과 적용은 배치가 끝난 뒤 해야 한다.
    /// </summary>
    private void BuySelectedEquipment()
    {
        if (selectedEquipment == null)
        {
            Debug.Log("선택된 장비가 없습니다.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance.money < selectedEquipment.price)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        switch (selectedEquipment.installType)
        {
            case EquipmentInstallType.TilePlaceable:
                StartTilePlacement();
                break;

            case EquipmentInstallType.DeskEquipment:
                StartDeskEquipmentPlacement();
                break;
        }
    }

    /// <summary>
    /// 타일 위에 직접 배치되는 장비 구매 처리.
    /// 
    /// 구매 버튼 클릭 시:
    /// - 상점창 닫기
    /// - 배치 모드 진입
    /// 
    /// 돈 차감과 효과 적용은 배치 완료 시점에 해야 한다.
    /// </summary>
    private void StartTilePlacement()
    {
        GameObject prefab = selectedEquipment.LoadPlaceablePrefab();

        if (prefab == null)
        {
            return;
        }

        if (PlacementManager.Instance == null)
        {
            Debug.LogError("PlacementManager.Instance가 없습니다.");
            return;
        }

        ClosePanel();

        // 지금 PlacementManager는 prefab과 price를 받는 구조다.
        // 다음 단계에서 EquipmentData 전체를 넘기도록 수정해야 한다.
        PlacementManager.Instance.StartPlacement(prefab, selectedEquipment.price);
    }

    /// <summary>
    /// 책상 위 장비 구매 처리.
    /// 
    /// 아직 Workstation 선택 모드는 다음 단계에서 구현한다.
    /// </summary>
    private void StartDeskEquipmentPlacement()
    {
        if (selectedEquipment.deskEquipmentData == null)
        {
            Debug.LogError(selectedEquipment.equipmentName + "의 deskEquipmentData가 없습니다.");
            return;
        }

        ClosePanel();

        Debug.Log("책상 위 장비 선택 모드 시작 예정: " + selectedEquipment.equipmentName);

        // TODO:
        // 다음 단계에서 Workstation 선택 모드를 만들고,
        // 빈 장비 슬롯이 있는 Workstation을 클릭하면
        // 돈 차감 + 장비 설치 + 효과 적용을 하도록 구현한다.
    }

    /// <summary>
    /// 상점 패널을 닫는다.
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