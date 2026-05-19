using UnityEngine;

/// <summary>
/// 배치 모드를 관리하는 스크립트.
/// 
/// 현재 조작 방식:
/// - 상점 버튼 클릭: 배치 모드 시작
/// - 마우스 이동: 가장 가까운 타일 중심으로 미리보기 스냅
/// - 좌클릭: 배치 확정
/// - 우클릭: 회전
/// - ESC: 배치 취소
/// 
/// WorkstationObject.cs가 붙어 있는 물건이면:
/// - 우클릭 시 WorkstationObject.RotateToNextDirection() 호출
/// 
/// WorkstationObject.cs가 없는 일반 물건이면:
/// - 지금은 회전하지 않음
/// - 나중에 일반 가구용 방향 시스템을 따로 붙일 수 있음
/// </summary>
public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("카메라")]
    [Tooltip("마우스 화면 좌표를 월드 좌표로 바꿀 때 사용할 카메라입니다.")]
    public Camera mainCamera;

    [Header("배치 상태")]
    [Tooltip("현재 배치 모드인지 여부입니다.")]
    public bool isPlacementMode = false;

    [Tooltip("현재 배치 중인 프리팹입니다.")]
    public GameObject currentPrefab;

    [Tooltip("현재 배치 중인 물건 가격입니다.")]
    public int currentPrice;

    [Tooltip("현재 화면에 떠 있는 미리보기 오브젝트입니다.")]
    public GameObject previewObject;

    private PlaceableObject previewPlaceable;
    private WorkstationObject previewWorkstation;
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
        if (isPlacementMode == false)
        {
            return;
        }

        UpdatePreviewPosition();
        HandlePlacementInput();
    }

    /// <summary>
    /// 배치 모드를 시작한다.
    /// 
    /// 상점에서 책상+의자 세트, 커피머신 등을 구매했을 때 호출된다.
    /// </summary>
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

        if (ResourceManager.Instance.money < price)
        {
            Debug.Log("돈이 부족해서 배치 모드에 들어갈 수 없습니다.");
            return;
        }

        // 이미 배치 모드 중이면 기존 미리보기를 취소한다.
        if (isPlacementMode)
        {
            CancelPlacement();
        }

        currentPrefab = prefab;
        currentPrice = price;
        isPlacementMode = true;

        // 배치 모드 시작 시 타일 격자를 보여준다.
        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.ShowPlacementGrid();
        }

        CreatePreviewObject();
    }

    /// <summary>
    /// 배치 미리보기 오브젝트를 생성한다.
    /// </summary>
    private void CreatePreviewObject()
    {
        previewObject = Instantiate(currentPrefab);
        previewObject.name = currentPrefab.name + "_Preview";

        previewPlaceable = previewObject.GetComponent<PlaceableObject>();

        if (previewPlaceable == null)
        {
            previewPlaceable = previewObject.AddComponent<PlaceableObject>();
        }

        previewWorkstation = previewObject.GetComponent<WorkstationObject>();

        // 미리보기 상태이므로 아직 배치 완료가 아니다.
        previewPlaceable.isPlaced = false;

        // 반투명하게 보여준다.
        previewPlaceable.SetAlpha(0.65f);
    }

    /// <summary>
    /// 마우스 위치에 따라 미리보기 오브젝트를 가장 가까운 타일 중심으로 이동시킨다.
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

        previewObject.transform.position = currentTargetTile.GetCenterPosition();

        bool canPlace = LabGridManager.Instance.CanPlaceOnTile(currentTargetTile);

        if (previewPlaceable != null)
        {
            previewPlaceable.SetPreviewColor(canPlace);
        }
    }

    /// <summary>
    /// 배치 모드 중 입력 처리.
    /// 
    /// 좌클릭 = 배치 확정
    /// 우클릭 = 회전
    /// ESC = 배치 취소
    /// </summary>
    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ConfirmPlacement();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RotatePreview();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    /// <summary>
    /// 미리보기 오브젝트 회전.
    /// 
    /// WorkstationObject가 붙어 있으면 Workstation의 방향을 바꾼다.
    /// 일반 오브젝트는 아직 회전하지 않는다.
    /// </summary>
    private void RotatePreview()
    {
        if (previewObject == null)
        {
            return;
        }

        if (previewWorkstation != null)
        {
            previewWorkstation.RotateToNextDirection();
            return;
        }

        Debug.Log("이 오브젝트는 아직 회전 기능이 없습니다.");
    }

    /// <summary>
    /// 현재 위치에 배치를 확정한다.
    /// </summary>
    private void ConfirmPlacement()
    {
        if (previewObject == null || currentTargetTile == null)
        {
            return;
        }

        if (LabGridManager.Instance.CanPlaceOnTile(currentTargetTile) == false)
        {
            Debug.Log("이미 사용 중인 타일입니다.");
            return;
        }

        bool success = ResourceManager.Instance.SpendMoney(currentPrice);

        if (success == false)
        {
            Debug.Log("돈이 부족해서 배치할 수 없습니다.");
            CancelPlacement();
            return;
        }

        // 타일 점유 처리
        LabGridManager.Instance.MarkTileOccupied(currentTargetTile);

        // PlaceableObject 배치 완료 처리
        if (previewPlaceable != null)
        {
            previewPlaceable.ConfirmPlacement(currentTargetTile);
        }

        // Workstation이면 배치된 타일 정보 저장
        if (previewWorkstation != null)
        {
            previewWorkstation.SetPlacedTile(currentTargetTile);
        }

        // 배치 완료된 오브젝트 이름 정리
        previewObject.name = currentPrefab.name;

        previewObject = null;
        previewPlaceable = null;
        previewWorkstation = null;
        currentTargetTile = null;
        currentPrefab = null;
        currentPrice = 0;

        EndPlacementMode();
    }

    /// <summary>
    /// 배치 취소.
    /// 
    /// 미리보기 오브젝트를 삭제하고 돈은 차감하지 않는다.
    /// </summary>
    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;
        previewPlaceable = null;
        previewWorkstation = null;
        currentTargetTile = null;
        currentPrefab = null;
        currentPrice = 0;

        EndPlacementMode();
    }

    /// <summary>
    /// 배치 모드를 종료한다.
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
    /// 마우스 위치를 월드 좌표로 변환한다.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 카메라가 Z -10에 있고 오브젝트가 Z 0에 있으므로,
        // 카메라에서 Z 0 평면까지의 거리를 넣는다.
        mouseScreenPosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }
}