using UnityEngine;

/// <summary>
/// 타일 위에 직접 설치하는 장비의 배치 모드를 관리한다.
/// 
/// 중요:
/// - 일반 타일 장비는 장비별 prefab을 만들지 않는다.
/// - 공통 TilePlaceableEquipment prefab 하나를 사용한다.
/// - 장비별 이미지는 EquipmentData.tilePlaceableData의 4방향 Sprite로 교체한다.
/// 
/// 예외:
/// - 책상 의자 세트는 Workstation prefab을 생성한다.
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
        UpdatePreviewPosition();
    }

    /// <summary>
    /// 타일 장비 배치 모드를 시작한다.
    /// 구매 버튼을 눌렀을 때 ShopPurchaseManager에서 호출된다.
    /// </summary>
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

        CreatePreview();

        Debug.Log("타일 장비 배치 모드 시작: " + equipmentData.equipmentName);
    }

    /// <summary>
    /// R키를 누르면 배치 방향을 회전한다.
    /// </summary>
    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentDirection = GetNextDirection(currentDirection);

            if (previewEquipmentObject != null)
            {
                previewEquipmentObject.ApplyDirection(currentDirection);
            }

            Debug.Log("배치 방향 변경: " + currentDirection);
        }
    }

    /// <summary>
    /// 마우스 위치를 따라 프리뷰를 이동시킨다.
    /// 현재는 마우스 월드 좌표를 그대로 따라가고,
    /// 타일 클릭 시 최종 위치가 타일 위치로 스냅된다.
    /// </summary>
    private void UpdatePreviewPosition()
    {
        if (previewObject == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        previewObject.transform.position = mouseWorldPosition;
    }

    /// <summary>
    /// 배치 프리뷰를 만든다.
    /// 책상 의자 세트는 Workstation prefab이라 프리뷰를 일단 생략한다.
    /// </summary>
    private void CreatePreview()
    {
        ClearPreview();

        if (pendingTileEquipment == null)
        {
            return;
        }

        // 책상 의자 세트는 Workstation prefab을 사용하는 특수 케이스.
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

        SetPreviewAlpha(previewObject, 0.5f);
    }

    /// <summary>
    /// 프리뷰는 반투명하게 보이도록 알파값을 낮춘다.
    /// </summary>
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

    /// <summary>
    /// 타일을 클릭했을 때 호출된다.
    /// LabTile.OnMouseDown()에서 이 함수를 호출해야 한다.
    /// </summary>
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

        RegisterEquipmentEffect(pendingTileEquipment, placedObject);

        Debug.Log("타일 장비 배치 완료: " + pendingTileEquipment.equipmentName);

        ClearPreview();

        pendingTileEquipment = null;
        isSelectingTile = false;

        if (ShopPurchaseManager.Instance != null)
        {
            ShopPurchaseManager.Instance.CancelPurchase();
        }
    }

    /// <summary>
    /// 실제 배치 오브젝트를 생성한다.
    /// 
    /// 책상 의자 세트:
    /// - Workstation prefab 생성
    /// 
    /// 일반 타일 장비:
    /// - 공통 TilePlaceableEquipment prefab 생성
    /// - EquipmentData의 4방향 Sprite 적용
    /// </summary>
    private GameObject CreatePlacedObject(LabTile tile)
    {
        if (pendingTileEquipment.equipmentName == "책상 의자 세트")
        {
            return CreateWorkstationObject(tile);
        }

        return CreateCommonTileEquipmentObject(tile);
    }

    /// <summary>
    /// 책상 의자 세트는 Workstation prefab으로 생성한다.
    /// </summary>
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
            tile.transform.position,
            Quaternion.identity
        );

        obj.name = pendingTileEquipment.equipmentName;

        return obj;
    }

    /// <summary>
    /// 일반 타일 장비는 공통 prefab으로 생성한다.
    /// </summary>
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
            tile.transform.position,
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

    /// <summary>
    /// 배치 완료 후 장비 효과를 등록한다.
    /// </summary>
    private void RegisterEquipmentEffect(EquipmentData equipmentData, GameObject placedObject)
    {
        if (equipmentData == null)
        {
            return;
        }

        // 책상 의자 세트는 Workstation 자체라서 전역 효과가 없다.
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

    /// <summary>
    /// 타일 장비 배치 취소.
    /// 돈 차감 없음.
    /// </summary>
    public void CancelPlacement()
    {
        if (pendingTileEquipment != null)
        {
            Debug.Log("타일 장비 배치 취소: " + pendingTileEquipment.equipmentName);
        }

        ClearPreview();

        pendingTileEquipment = null;
        isSelectingTile = false;

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