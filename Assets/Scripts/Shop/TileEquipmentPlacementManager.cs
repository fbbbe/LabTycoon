using UnityEngine;

/// <summary>
/// 타일 위에 직접 설치하는 장비의 배치 모드를 관리한다.
///
/// 핵심 설계:
/// - 타일을 직접 클릭해서 자유 좌표에 놓지 않는다.
/// - LabGridManager가 생성한 배치 격자에 스냅해서 놓는다.
/// - 배치 모드 중에는 GridOverlay를 표시한다.
/// - 프리뷰는 항상 가장 가까운 격자 칸 중심으로 이동한다.
/// - R키로 방향을 바꾸면 Sprite 방향과 점유 타일 방향이 함께 바뀐다.
/// </summary>
public class TileEquipmentPlacementManager : MonoBehaviour
{
    public static TileEquipmentPlacementManager Instance;

    [Header("공통 타일 장비 Prefab")]
    [Tooltip("Assets/Resources/Prefabs/Common/TilePlaceableEquipment.prefab")]
    public string commonTileEquipmentPrefabPath = "Prefabs/Common/TilePlaceableEquipment";

    [Header("현재 배치 대기 중인 장비")]
    public EquipmentData pendingTileEquipment;

    [Header("타일 선택 모드 여부")]
    public bool isSelectingTile = false;

    [Header("현재 배치 방향")]
    public PlacementDirection currentDirection = PlacementDirection.RD;

    [Header("배치 프리뷰")]
    public GameObject previewObject;

    private TilePlaceableEquipmentObject previewEquipmentObject;
    private LabTile currentPreviewTile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (isSelectingTile == false)
        {
            return;
        }

        HandleRotationInput();
        UpdatePreviewByPlacementGrid();
        HandlePlacementClickInput();
        HandleCancelInput();
    }

    public void StartTileEquipmentPlacement(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            Debug.LogError("TileEquipmentPlacementManager: equipmentData가 없습니다.");
            return;
        }

        if (equipmentData.installType != EquipmentInstallType.TilePlaceable)
        {
            Debug.LogError("TileEquipmentPlacementManager: 타일 장비가 아닙니다: " + equipmentData.equipmentName);
            return;
        }

        pendingTileEquipment = equipmentData;
        isSelectingTile = true;
        currentDirection = PlacementDirection.RD;
        currentPreviewTile = null;

        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.ShowPlacementGrid();
        }

        CreatePreview();

        Debug.Log("타일 장비 배치 모드 시작: " + equipmentData.equipmentName);
    }

    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R) == false)
        {
            return;
        }

        currentDirection = GetNextDirection(currentDirection);

        if (previewEquipmentObject != null)
        {
            previewEquipmentObject.ApplyDirection(currentDirection);
        }

        Debug.Log("배치 방향 변경: " + currentDirection);
    }

    private void HandleCancelInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void UpdatePreviewByPlacementGrid()
    {
        if (LabGridManager.Instance == null)
        {
            return;
        }

        currentPreviewTile = LabGridManager.Instance.GetNearestTileFromMousePosition();

        if (currentPreviewTile == null)
        {
            return;
        }

        if (previewObject != null)
        {
            previewObject.transform.position = currentPreviewTile.GetCenterPosition();
        }

        bool canPlace = CanPlaceOnCurrentPreviewTile();
        SetPreviewAlpha(previewObject, canPlace ? 0.55f : 0.25f);
    }

    private void HandlePlacementClickInput()
    {
        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }

        if (currentPreviewTile == null)
        {
            Debug.Log("배치 실패: 현재 선택된 격자 칸이 없습니다.");
            return;
        }

        TryPlaceToTile(currentPreviewTile);
    }

    private void CreatePreview()
    {
        ClearPreview();

        if (pendingTileEquipment == null)
        {
            return;
        }

        if (pendingTileEquipment.equipmentName == "책상 의자 세트")
        {
            Debug.Log("책상 의자 세트는 Workstation prefab을 사용하므로 현재 프리뷰는 생략합니다.");
            return;
        }

        GameObject prefab = Resources.Load<GameObject>(commonTileEquipmentPrefabPath);

        if (prefab == null)
        {
            Debug.LogError("공통 타일 장비 prefab을 찾지 못했습니다: " + commonTileEquipmentPrefabPath);
            return;
        }

        previewObject = Instantiate(prefab);
        previewObject.name = "Preview_" + pendingTileEquipment.equipmentName;

        previewEquipmentObject = previewObject.GetComponent<TilePlaceableEquipmentObject>();

        if (previewEquipmentObject == null)
        {
            Debug.LogError("공통 prefab에 TilePlaceableEquipmentObject가 없습니다.");
            Destroy(previewObject);
            previewObject = null;
            return;
        }

        previewEquipmentObject.Initialize(
            pendingTileEquipment,
            null,
            currentDirection
        );

        SetPreviewAlpha(previewObject, 0.55f);
    }

    private void SetPreviewAlpha(GameObject target, float alpha)
    {
        if (target == null)
        {
            return;
        }

        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            Color color = renderers[i].color;
            color.a = alpha;
            renderers[i].color = color;
        }
    }

    private bool CanPlaceOnCurrentPreviewTile()
    {
        if (LabGridManager.Instance == null || pendingTileEquipment == null || currentPreviewTile == null)
        {
            return false;
        }

        return LabGridManager.Instance.CanPlaceEquipment(
            currentPreviewTile,
            pendingTileEquipment.spaceCost,
            currentDirection
        );
    }

    public void TryPlaceToTile(LabTile tile)
    {
        if (isSelectingTile == false)
        {
            return;
        }

        if (pendingTileEquipment == null)
        {
            Debug.LogError("배치 대기 중인 타일 장비가 없습니다.");
            CancelPlacement();
            return;
        }

        if (tile == null)
        {
            Debug.LogError("선택한 타일이 없습니다.");
            return;
        }

        if (LabGridManager.Instance == null)
        {
            Debug.LogError("LabGridManager.Instance가 없습니다.");
            return;
        }

        if (LabGridManager.Instance.CanPlaceEquipment(tile, pendingTileEquipment.spaceCost, currentDirection) == false)
        {
            Debug.Log("배치할 수 없는 위치입니다: " + pendingTileEquipment.equipmentName);
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance.HasEnoughMoney(pendingTileEquipment.price) == false)
        {
            Debug.Log("돈이 부족해서 배치할 수 없습니다: " + pendingTileEquipment.equipmentName);
            CancelPlacement();
            return;
        }

        GameObject placedObject = CreatePlacedObject(tile);

        if (placedObject == null)
        {
            Debug.LogError("장비 오브젝트 생성 실패: " + pendingTileEquipment.equipmentName);
            return;
        }

        bool spendSuccess = ResourceManager.Instance.SpendMoney(pendingTileEquipment.price);

        if (spendSuccess == false)
        {
            Destroy(placedObject);
            CancelPlacement();
            return;
        }

        LabGridManager.Instance.MarkEquipmentTilesOccupied(
            tile,
            pendingTileEquipment.spaceCost,
            currentDirection
        );

        RegisterEquipmentEffect(pendingTileEquipment, placedObject);

        Debug.Log("타일 장비 배치 완료: " + pendingTileEquipment.equipmentName);

        FinishPlacement();
    }

    private GameObject CreatePlacedObject(LabTile tile)
    {
        if (pendingTileEquipment.equipmentName == "책상 의자 세트")
        {
            return CreateWorkstationObject(tile);
        }

        return CreateCommonTileEquipmentObject(tile);
    }

    private GameObject CreateWorkstationObject(LabTile tile)
    {
        if (string.IsNullOrEmpty(pendingTileEquipment.placeablePrefabResourcePath))
        {
            Debug.LogError("책상 의자 세트의 Workstation prefab 경로가 비어 있습니다.");
            return null;
        }

        GameObject prefab = Resources.Load<GameObject>(pendingTileEquipment.placeablePrefabResourcePath);

        if (prefab == null)
        {
            Debug.LogError("Workstation prefab을 찾지 못했습니다: " + pendingTileEquipment.placeablePrefabResourcePath);
            return null;
        }

        GameObject obj = Instantiate(
            prefab,
            tile.GetCenterPosition(),
            Quaternion.identity
        );

        obj.name = pendingTileEquipment.equipmentName;

        return obj;
    }

    private GameObject CreateCommonTileEquipmentObject(LabTile tile)
    {
        GameObject prefab = Resources.Load<GameObject>(commonTileEquipmentPrefabPath);

        if (prefab == null)
        {
            Debug.LogError("공통 타일 장비 prefab을 찾지 못했습니다: " + commonTileEquipmentPrefabPath);
            return null;
        }

        GameObject obj = Instantiate(
            prefab,
            tile.GetCenterPosition(),
            Quaternion.identity
        );

        obj.name = pendingTileEquipment.equipmentName;

        TilePlaceableEquipmentObject equipmentObject = obj.GetComponent<TilePlaceableEquipmentObject>();

        if (equipmentObject == null)
        {
            Debug.LogError("생성된 공통 prefab에 TilePlaceableEquipmentObject가 없습니다.");
            Destroy(obj);
            return null;
        }

        equipmentObject.Initialize(
            pendingTileEquipment,
            tile,
            currentDirection
        );

        return obj;
    }

    private void RegisterEquipmentEffect(EquipmentData equipmentData, GameObject placedObject)
    {
        if (equipmentData == null)
        {
            return;
        }

        if (equipmentData.equipmentName == "책상 의자 세트")
        {
            Debug.Log("책상 의자 세트 배치 완료. 전역 효과 없음.");
            return;
        }

        if (EquipmentEffectManager.Instance != null)
        {
            EquipmentEffectManager.Instance.RegisterGlobalEquipment(equipmentData);
        }
    }

    private void FinishPlacement()
    {
        ClearPreview();

        pendingTileEquipment = null;
        isSelectingTile = false;
        currentPreviewTile = null;

        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.HidePlacementGrid();
        }

        if (ShopPurchaseManager.Instance != null)
        {
            ShopPurchaseManager.Instance.ClearPendingPurchase();
        }
    }

    public void CancelPlacement()
    {
        if (pendingTileEquipment != null)
        {
            Debug.Log("타일 장비 배치 취소: " + pendingTileEquipment.equipmentName);
        }

        ClearPreview();

        pendingTileEquipment = null;
        isSelectingTile = false;
        currentPreviewTile = null;

        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.HidePlacementGrid();
        }

        if (ShopPurchaseManager.Instance != null)
        {
            ShopPurchaseManager.Instance.CancelPurchase();
        }
    }

    private void ClearPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
            previewEquipmentObject = null;
        }
    }

    private PlacementDirection GetNextDirection(PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.RD:
                return PlacementDirection.RU;

            case PlacementDirection.RU:
                return PlacementDirection.LU;

            case PlacementDirection.LU:
                return PlacementDirection.LD;

            case PlacementDirection.LD:
                return PlacementDirection.RD;

            default:
                return PlacementDirection.RD;
        }
    }
}