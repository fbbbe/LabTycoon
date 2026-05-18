using UnityEngine;

/// <summary>
/// 장비 배치 모드를 관리하는 스크립트.
/// 
/// 이 스크립트의 역할:
/// 1. 장비 구매 버튼을 누르면 배치 모드 시작
/// 2. 배치 모드 중 Tile_Grid 표시
/// 3. 장비 미리보기 오브젝트를 생성
/// 4. 마우스 위치에서 가장 가까운 타일 중심으로 장비 미리보기 이동
/// 5. 좌클릭 시 배치 확정
/// 6. 우클릭 또는 ESC 입력 시 배치 취소
/// </summary>
public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("카메라")]
    [Tooltip("마우스 화면 좌표를 월드 좌표로 변환할 때 사용할 카메라입니다. 비워두면 Main Camera를 자동으로 사용합니다.")]
    public Camera mainCamera;

    [Header("배치 상태")]
    [Tooltip("현재 배치 모드인지 여부입니다.")]
    public bool isPlacementMode = false;

    [Tooltip("현재 배치 중인 장비 프리팹입니다.")]
    public GameObject currentPrefab;

    [Tooltip("현재 배치 중인 장비 가격입니다.")]
    public int currentPrice;

    [Tooltip("현재 배치 미리보기 오브젝트입니다.")]
    public GameObject previewObject;

    private PlaceableObject previewPlaceable;
    private LabTile currentTargetTile;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // 배치 모드가 아닐 때는 아무것도 하지 않는다.
        if (isPlacementMode == false)
        {
            return;
        }

        UpdatePreviewPosition();
        HandlePlacementInput();
    }

    /// <summary>
    /// 장비 배치 모드를 시작한다.
    /// 
    /// EquipmentShop.cs에서 장비 구매 버튼을 눌렀을 때 이 함수를 호출하게 만들 것이다.
    /// </summary>
    /// <param name="prefab">배치할 장비 프리팹</param>
    /// <param name="price">장비 가격</param>
    public void StartPlacement(GameObject prefab, int price)
    {
        if (prefab == null)
        {
            Debug.LogError("PlacementManager: 배치할 프리팹이 없습니다.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("PlacementManager: ResourceManager.Instance가 없습니다.");
            return;
        }

        // 돈이 부족하면 배치 모드에 들어가지 않는다.
        // 실제 돈 차감은 배치 확정 시점에 한다.
        if (ResourceManager.Instance.money < price)
        {
            Debug.Log("돈이 부족해서 배치 모드에 들어갈 수 없습니다.");
            return;
        }

        // 이미 배치 모드 중이면 기존 미리보기를 정리한다.
        if (isPlacementMode)
        {
            CancelPlacement();
        }

        currentPrefab = prefab;
        currentPrice = price;
        isPlacementMode = true;

        // 배치 모드 시작 시 타일 격자 표시.
        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.ShowPlacementGrid();
        }

        // 장비 미리보기 생성.
        previewObject = Instantiate(currentPrefab);
        previewObject.name = currentPrefab.name + "_Preview";

        previewPlaceable = previewObject.GetComponent<PlaceableObject>();

        if (previewPlaceable == null)
        {
            previewPlaceable = previewObject.AddComponent<PlaceableObject>();
        }

        // 미리보기는 아직 배치 확정 전이므로 반투명하게 표시한다.
        previewPlaceable.isPlaced = false;
        previewPlaceable.SetAlpha(0.65f);

        // 미리보기 오브젝트는 실제 장비보다 위에 보이도록 정렬한다.
        SetPreviewSortingOrder(previewObject, 30);
    }

    /// <summary>
    /// 마우스 위치를 기준으로 장비 미리보기 위치를 갱신한다.
    /// 
    /// 마우스가 타일 사이에 있어도 가장 가까운 LabTile 중심으로 스냅된다.
    /// </summary>
    private void UpdatePreviewPosition()
    {
        if (previewObject == null || LabGridManager.Instance == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        currentTargetTile = LabGridManager.Instance.GetNearestTile(mouseWorldPosition);

        if (currentTargetTile == null)
        {
            return;
        }

        // 가장 가까운 타일의 중심 위치로 미리보기 이동.
        previewObject.transform.position = currentTargetTile.GetCenterPosition();

        bool canPlace = LabGridManager.Instance.CanPlaceOnTile(currentTargetTile);

        // 배치 가능하면 흰색 반투명, 불가능하면 빨간색 반투명.
        if (previewPlaceable != null)
        {
            previewPlaceable.SetPreviewColor(canPlace);
        }
    }

    /// <summary>
    /// 배치 모드 중 입력을 처리한다.
    /// 
    /// 좌클릭: 배치 확정
    /// 우클릭 또는 ESC: 배치 취소
    /// </summary>
    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ConfirmPlacement();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    /// <summary>
    /// 현재 미리보기 위치에 장비 배치를 확정한다.
    /// </summary>
    private void ConfirmPlacement()
    {
        if (previewObject == null || currentTargetTile == null)
        {
            return;
        }

        if (LabGridManager.Instance.CanPlaceOnTile(currentTargetTile) == false)
        {
            Debug.Log("이 타일에는 이미 장비가 있어서 배치할 수 없습니다.");
            return;
        }

        // 배치 확정 시점에 돈을 차감한다.
        bool success = ResourceManager.Instance.SpendMoney(currentPrice);

        if (success == false)
        {
            Debug.Log("돈이 부족해서 장비를 배치할 수 없습니다.");
            CancelPlacement();
            return;
        }

        // 현재 타일을 사용 중으로 표시한다.
        LabGridManager.Instance.MarkTileOccupied(currentTargetTile);

        // 미리보기 오브젝트를 실제 장비로 확정한다.
        if (previewPlaceable != null)
        {
            previewPlaceable.ConfirmPlacement(currentTargetTile);
        }

        // 실제 장비 정렬 순서로 되돌린다.
        SetPreviewSortingOrder(previewObject, 0);

        // 배치 완료 후 참조를 비운다.
        previewObject.name = currentPrefab.name;
        previewObject = null;
        previewPlaceable = null;
        currentTargetTile = null;
        currentPrefab = null;
        currentPrice = 0;

        EndPlacementMode();
    }

    /// <summary>
    /// 배치 취소.
    /// 
    /// 미리보기 오브젝트를 삭제하고,
    /// 돈은 차감하지 않는다.
    /// </summary>
    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;
        previewPlaceable = null;
        currentTargetTile = null;
        currentPrefab = null;
        currentPrice = 0;

        EndPlacementMode();
    }

    /// <summary>
    /// 배치 모드를 종료한다.
    /// 
    /// 격자를 숨기고, 배치 상태를 false로 바꾼다.
    /// </summary>
    private void EndPlacementMode()
    {
        isPlacementMode = false;

        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.HidePlacementGrid();
        }
    }

    /// <summary>
    /// 현재 마우스 위치를 Unity 월드 좌표로 변환한다.
    /// 
    /// 마우스 좌표는 화면 기준 픽셀 좌표이고,
    /// 장비 배치는 월드 좌표 기준이므로 변환이 필요하다.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2D 카메라는 보통 z = -10 위치에 있고, 오브젝트는 z = 0에 있다.
        // 따라서 카메라에서 z=0 평면까지의 거리를 넣어준다.
        mouseScreenPosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }

    /// <summary>
    /// 미리보기 또는 장비 오브젝트의 SpriteRenderer 정렬 순서를 바꾼다.
    /// 
    /// order가 높을수록 앞에 보인다.
    /// </summary>
    private void SetPreviewSortingOrder(GameObject targetObject, int order)
    {
        SpriteRenderer[] renderers = targetObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingOrder = order;
        }
    }
}