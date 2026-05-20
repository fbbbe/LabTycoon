using System.Collections;
using UnityEngine;

/// <summary>
/// 게임 시작 시 기본 자원을 세팅하는 스크립트.
/// 
/// 초기 자원:
/// 1. 학사생 1명
/// 2. 책상+의자 세트 1개
/// 3. 낡은 노트북 1개
/// 4. 소정의 돈
/// 5. 9평 연구실
/// 
/// 주의:
/// - 9평 연구실 타일은 LabGridManager.cs가 생성한다.
/// - 이 스크립트는 그 위에 Workstation과 인력을 생성한다.
/// - 낡은 노트북은 바닥에 직접 생성하지 않고 Workstation 안의 DeskEquipment로 장착한다.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("초기 돈")]
    [Tooltip("게임 시작 시 지급할 돈입니다.")]
    public int startingMoney = 100000;

    [Header("초기 Workstation")]
    [Tooltip("책상+의자 세트 프리팹입니다. Workstation.prefab을 넣습니다.")]
    public GameObject workstationPrefab;

    [Tooltip("초기 Workstation을 배치할 타일 X 좌표입니다.")]
    public int workstationTileX = 1;

    [Tooltip("초기 Workstation을 배치할 타일 Y 좌표입니다.")]
    public int workstationTileY = 1;

    [Tooltip("초기 Workstation 방향입니다.")]
    public PlacementDirection initialWorkstationDirection = PlacementDirection.RD;

    [Header("초기 책상 위 장비")]
    [Tooltip("초기 장비 데이터입니다. 낡은 노트북 정보를 넣습니다.")]
    public DeskEquipmentData initialDeskEquipment;

    [Header("초기 인력")]
    [Tooltip("게임 시작 시 생성할 학사생 프리팹입니다.")]
    public GameObject undergraduatePrefab;

    [Tooltip("학사생이 처음 등장할 문 앞 위치입니다.")]
    public Transform doorSpawnPoint;

    [Header("초기화 상태")]
    [Tooltip("초기화가 이미 완료되었는지 확인하는 값입니다.")]
    public bool initialized = false;

    [Header("초기 인력 생성 타일")]
    [Tooltip("초기 학사생을 생성할 타일 X 좌표입니다.")]
    public int undergraduateSpawnTileX = 1;

    [Tooltip("초기 학사생을 생성할 타일 Y 좌표입니다.")]
    public int undergraduateSpawnTileY = 2;

    [Tooltip("타일 중심에서 인력을 얼마나 이동시켜 생성할지 정합니다.")]
    public Vector3 undergraduateSpawnOffset = Vector3.zero;

    private IEnumerator Start()
    {
        // 한 프레임 기다린다.
        // 이유:
        // LabGridManager도 Start()에서 타일을 생성하므로,
        // GameInitializer가 너무 빨리 실행되면 아직 타일이 없을 수 있다.
        yield return null;

        InitializeGame();
    }

    /// <summary>
    /// 게임 시작 상태를 세팅한다.
    /// </summary>
    public void InitializeGame()
    {
        // 중복 실행 방지.
        // 실수로 두 번 실행되면 책상, 학사생, 노트북이 두 번 생긴다.
        if (initialized)
        {
            return;
        }

        initialized = true;

        SetInitialMoney();
        CreateInitialWorkstationWithLaptop();
        SpawnInitialUndergraduate();
    }

    /// <summary>
    /// 초기 돈을 설정한다.
    /// </summary>
    private void SetInitialMoney()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("GameInitializer: ResourceManager.Instance가 없습니다.");
            return;
        }

        ResourceManager.Instance.money = startingMoney;
    }

    /// <summary>
    /// 초기 책상+의자 세트를 생성하고,
    /// 그 책상 위에 낡은 노트북을 장착한다.
    /// </summary>
    private void CreateInitialWorkstationWithLaptop()
    {
        if (LabGridManager.Instance == null)
        {
            Debug.LogError("GameInitializer: LabGridManager.Instance가 없습니다.");
            return;
        }

        if (workstationPrefab == null)
        {
            Debug.LogError("GameInitializer: Workstation Prefab이 연결되지 않았습니다.");
            return;
        }

        // 지정한 좌표의 타일을 가져온다.
        LabTile targetTile = LabGridManager.Instance.GetTile(workstationTileX, workstationTileY);

        if (targetTile == null)
        {
            Debug.LogError("GameInitializer: Workstation을 배치할 타일을 찾지 못했습니다.");
            return;
        }

        if (targetTile.CanPlaceObject() == false)
        {
            Debug.LogError("GameInitializer: 초기 Workstation을 놓을 타일이 이미 사용 중입니다.");
            return;
        }

        // Workstation을 타일 중심에 생성한다.
        GameObject workstationObject = Instantiate(
            workstationPrefab,
            targetTile.GetCenterPosition(),
            Quaternion.identity
        );

        workstationObject.name = "Initial_Workstation";

        WorkstationObject workstation = workstationObject.GetComponent<WorkstationObject>();

        if (workstation == null)
        {
            Debug.LogError("GameInitializer: WorkstationObject.cs가 Workstation 프리팹에 붙어 있지 않습니다.");
            return;
        }

        // Workstation 방향 설정.
        // 방향에 따라 책상/의자/장비 위치와 정렬 순서가 바뀐다.
        workstation.ApplyDirection(initialWorkstationDirection);

        // 이 Workstation이 어느 타일에 배치되었는지 저장한다.
        workstation.SetPlacedTile(targetTile);

        // 타일을 사용 중으로 표시한다.
        // 이렇게 해야 같은 타일에 다른 가구를 중복 배치하지 못한다.
        LabGridManager.Instance.MarkTileOccupied(targetTile);

        PlaceableObject placeable = workstationObject.GetComponent<PlaceableObject>();

        if (placeable != null)
        {
            placeable.ConfirmPlacement(targetTile);
        }

        // 낡은 노트북을 Workstation 책상 위에 장착한다.
        InstallInitialDeskEquipment(workstation);
    }

    /// <summary>
    /// 초기 Workstation에 낡은 노트북을 장착한다.
    /// 
    /// 낡은 노트북은 바닥에 직접 생성하지 않는다.
    /// Workstation 내부의 DeskEquipmentObject가 이미지를 표시한다.
    /// </summary>
    private void InstallInitialDeskEquipment(WorkstationObject workstation)
    {
        if (workstation == null)
        {
            return;
        }

        if (initialDeskEquipment == null)
        {
            Debug.LogError("GameInitializer: Initial Desk Equipment가 비어 있습니다. 낡은 노트북 데이터를 넣어야 합니다.");
            return;
        }

        bool installed = workstation.InstallDeskEquipment(initialDeskEquipment);

        if (installed == false)
        {
            Debug.LogError("GameInitializer: 초기 낡은 노트북 설치에 실패했습니다.");
        }
    }

    /// <summary>
    /// 게임 시작 시 학사생 1명을 타일 좌표 기준으로 생성한다.
    /// 
    /// 예전 방식:
    /// - DoorSpawnPoint라는 빈 오브젝트 위치에 생성
    /// 
    /// 수정 방식:
    /// - LabGridManager에서 특정 타일을 가져옴
    /// - 그 타일 중심 좌표에 생성
    /// - 필요하면 undergraduateSpawnOffset으로 살짝 보정
    /// 
    /// 이렇게 하면 나중에 연구실 크기가 바뀌거나 타일 위치가 바뀌어도
    /// 초기 인력 위치를 타일 기준으로 관리할 수 있다.
    /// </summary>
    private void SpawnInitialUndergraduate()
    {
        if (undergraduatePrefab == null)
        {
            Debug.LogError("GameInitializer: Undergraduate Prefab이 연결되지 않았습니다.");
            return;
        }

        if (LabGridManager.Instance == null)
        {
            Debug.LogError("GameInitializer: LabGridManager.Instance가 없습니다.");
            return;
        }

        LabTile spawnTile = LabGridManager.Instance.GetTile(
            undergraduateSpawnTileX,
            undergraduateSpawnTileY
        );

        if (spawnTile == null)
        {
            Debug.LogError("GameInitializer: 학사생을 생성할 타일을 찾지 못했습니다.");
            return;
        }

        Vector3 spawnPosition = spawnTile.GetCenterPosition() + undergraduateSpawnOffset;

        GameObject staffObject = Instantiate(
            undergraduatePrefab,
            spawnPosition,
            Quaternion.identity
        );

        staffObject.name = "Initial_Undergraduate";

        // 인력도 타일 기준으로 앞뒤 정렬을 맞춘다.
        SpriteRenderer staffRenderer = staffObject.GetComponent<SpriteRenderer>();

        if (staffRenderer != null)
        {
            int tileOrder = LabGridManager.Instance.GetSortingOrderByTile(spawnTile);

            // 인력은 같은 타일의 가구보다 앞에 보이는 편이 자연스러우므로 +20 정도를 준다.
            staffRenderer.sortingOrder = tileOrder + 20;
        }
    }
}