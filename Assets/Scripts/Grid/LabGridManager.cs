using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 연구실 레벨 구간에 따라 적용할 바닥/벽/문 벽지 세트입니다.
///
/// 핵심 원칙:
/// - 벽 오브젝트를 새로 만들거나 위치를 다시 계산하지 않는다.
/// - 기존에 생성된 벽 SpriteRenderer의 sprite만 교체한다.
/// - 맨 위 타일의 오른쪽 벽지만 doorWallRightSprite로 교체한다.
/// </summary>
[System.Serializable]
public class LabEnvironmentTheme
{
    [Header("적용 레벨 구간")]
    public int minLabLevel = 1;
    public int maxLabLevel = 10;

    [Header("테마 Sprite")]
    public Sprite tileSprite;
    public Sprite wallLeftSprite;
    public Sprite wallRightSprite;
    public Sprite doorWallRightSprite;
    public Sprite backgroundSprite;
}

/// <summary>
/// 연구실 바닥 타일 전체를 관리하는 스크립트.
///
/// 현재 게임 규칙:
/// - Tile.png 하나 = 1평
/// - 게임 시작 시 기본 연구실 = 9평
/// - 9평 = 3 x 3 타일
///
/// 이 스크립트의 역할:
/// 1. 게임이 시작되면 Tile.png를 9개 생성해서 연구실 바닥을 만든다.
/// 2. 각 타일마다 LabTile.cs를 붙여서 좌표와 점유 상태를 관리한다.
/// 3. 배치 모드일 때만 Tile_Grid.png를 타일 위에 표시한다.
/// 4. 연구실 레벨에 따라 바닥 타일/일반 벽지/문 포함 벽지 Sprite만 교체한다.
/// </summary>
public class LabGridManager : MonoBehaviour
{
    public static LabGridManager Instance;

    [Header("연구실 레벨별 환경 테마")]
    [Tooltip("1~10, 11~90, 91~100 같은 레벨 구간별 바닥/벽/문 벽지 세트입니다.")]
    public LabEnvironmentTheme[] environmentThemes;

    [Tooltip("게임 시작 시 적용할 기본 연구실 레벨입니다. ResourceManager와 연결하기 전까지는 이 값을 사용합니다.")]
    public int startLabLevelForTheme = 1;

    [Header("게임 배경 설정")]
    [Tooltip("타일/벽지가 아니라 카메라 뒤에 깔리는 전체 배경 SpriteRenderer입니다.")]
    public SpriteRenderer backgroundRenderer;

    [Tooltip("배경을 기준으로 맞출 카메라입니다. 비워두면 Main Camera를 자동으로 사용합니다.")]
    public Camera backgroundTargetCamera;

    [Tooltip("배경이 화면보다 살짝 더 크게 보이도록 추가 배율을 줍니다.")]
    public float backgroundExtraScaleMultiplier = 1.05f;

    [Tooltip("배경의 Sorting Order입니다. 타일보다 훨씬 뒤에 있어야 합니다.")]
    public int backgroundOrder = -10000;

    [Header("문 포함 벽지 PNG 설정")]
    [Tooltip("기존 오른쪽 벽지 중 하나를 이 Sprite로 대체합니다. 새 오브젝트를 만들지 않습니다.")]
    public Sprite doorWallRightSprite;

    [Tooltip("문 포함 벽지로 대체할 기존 오른쪽 벽지 인덱스입니다. 3x3 기준 0, 1, 2 중 하나입니다.")]
    public int doorWallRightIndex = 0;

    [Header("벽지 PNG 설정")]
    [Tooltip("왼쪽 상단 벽에 사용할 PNG입니다. Wall_Left.png를 넣습니다.")]
    public Sprite wallLeftSprite;

    [Tooltip("오른쪽 상단 벽에 사용할 PNG입니다. Wall_Right.png를 넣습니다.")]
    public Sprite wallRightSprite;

    [Header("벽지 크기 설정")]
    [Tooltip("벽지 PNG의 크기 비율입니다. 1이면 원래 크기, 0.5면 절반 크기입니다.")]
    public float wallScale = 1f;

    [Header("벽지 위치 보정")]
    [Tooltip("Wall_Left.png가 타일 중심에서 얼마나 이동할지 정합니다. 타일과 안 맞으면 이 값을 조절합니다.")]
    public Vector3 wallLeftOffset = new Vector3(-0.52f, 0.28f, 0f);

    [Tooltip("Wall_Right.png가 타일 중심에서 얼마나 이동할지 정합니다. 타일과 안 맞으면 이 값을 조절합니다.")]
    public Vector3 wallRightOffset = new Vector3(0.52f, 0.28f, 0f);

    [Header("벽지 정렬 순서")]
    [Tooltip("벽지의 Order in Layer입니다. 타일보다 앞, 장비보다 뒤에 두는 것이 기본입니다.")]
    public int wallOrder = -6;

    [Header("생성된 벽지 부모")]
    [Tooltip("자동 생성된 벽지들을 담아둘 부모 오브젝트입니다. 비워두면 자동 생성합니다.")]
    public Transform wallParent;

    [Header("연구실 크기 설정")]
    [Tooltip("연구실 가로 칸 수입니다. 3이면 가로 3칸입니다.")]
    public int width = 3;

    [Tooltip("연구실 세로 칸 수입니다. 3이면 세로 3칸입니다.")]
    public int height = 3;

    [Header("타일 PNG 설정")]
    [Tooltip("평소에 보이는 1평 바닥 타일 PNG입니다. Tile.png를 넣습니다.")]
    public Sprite tileSprite;

    [Tooltip("배치 모드에서만 보이는 흰색 테두리 타일 PNG입니다. Tile_Grid.png를 넣습니다.")]
    public Sprite gridOverlaySprite;

    [Header("타일 크기 설정")]
    [Tooltip("타일 이미지 크기 비율입니다. 1이면 원래 크기, 0.5면 절반 크기입니다.")]
    public float tileScale = 1f;

    [Header("아이소메트릭 타일 간격")]
    [Tooltip("자동 계산을 사용할지 여부입니다. 일단 false 추천. 직접 눈으로 맞추는 게 더 확실합니다.")]
    public bool autoCalculateTileSpacing = true;

    [Tooltip("타일 중심 간 X 간격입니다. 타일이 좌우로 벌어지거나 겹치면 이 값을 조절합니다.")]
    public float manualTileHalfWidth = 1f;

    [Tooltip("타일 중심 간 Y 간격입니다. 타일이 위아래로 벌어지거나 겹치면 이 값을 조절합니다.")]
    public float manualTileHalfHeight = 0.5f;

    [Header("연구실 위치")]
    [Tooltip("연구실 전체의 기준 위치입니다. 보통 0,0,0으로 둡니다.")]
    public Vector3 originPosition = Vector3.zero;

    [Header("이미지 앞뒤 순서")]
    [Tooltip("기본 바닥 타일의 표시 순서입니다. 낮을수록 뒤에 보입니다.")]
    public int baseTileOrder = -10;

    [Tooltip("배치 모드용 흰색 테두리 타일의 표시 순서입니다. 기본 타일보다 앞에 보여야 합니다.")]
    public int gridOverlayOrder = -5;

    [Header("생성된 타일 부모")]
    [Tooltip("자동 생성된 타일들을 담아둘 부모 오브젝트입니다. 비워두면 자동 생성합니다.")]
    public Transform tileParent;

    // 생성된 타일들을 좌표로 관리하기 위한 2차원 배열.
    private LabTile[,] tiles;

    // foreach로 전체 타일을 안정적으로 순회하기 위한 리스트.
    private readonly List<LabTile> allTiles = new List<LabTile>();

    // 생성된 벽 SpriteRenderer 목록. 연구실 레벨에 따라 벽지를 교체할 때 사용한다.
    private readonly List<SpriteRenderer> leftWallRenderers = new List<SpriteRenderer>();
    private readonly List<SpriteRenderer> rightWallRenderers = new List<SpriteRenderer>();

    // 맨 위 타일 오른쪽 윗면 벽지 Renderer.
    // 이 Renderer의 sprite만 문 포함 벽지로 교체한다.
    private SpriteRenderer doorWallRightRenderer;

    [Header("검사 연출 전용 타일")]
    [Tooltip("맨 위 타일입니다. 검사 연출용으로 예약되며 장비를 배치할 수 없습니다.")]
    public LabTile inspectionTile;

    // 실제 계산에 사용할 타일 중심 간격.
    private float tileHalfWidth;
    private float tileHalfHeight;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CalculateTileSpacing();
        GenerateInitialLab();
        MarkInspectionTile();
        GenerateInitialWalls();

        ApplyEnvironmentFromCurrentLabLevel();
        Invoke(nameof(ApplyEnvironmentFromCurrentLabLevel), 0.05f);

        FitBackgroundToCamera();

        HidePlacementGrid();
    }

    private void LateUpdate()
    {
        FitBackgroundToCamera();
    }
    private void CalculateTileSpacing()
    {
        if (autoCalculateTileSpacing && tileSprite != null)
        {
            Vector2 spriteSize = tileSprite.bounds.size;
            tileHalfWidth = spriteSize.x * tileScale * 0.5f;
            tileHalfHeight = spriteSize.y * tileScale * 0.5f;
        }
        else
        {
            tileHalfWidth = manualTileHalfWidth;
            tileHalfHeight = manualTileHalfHeight;
        }
    }

    public void GenerateInitialLab()
    {
        if (tileSprite == null)
        {
            Debug.LogError("LabGridManager: Tile Sprite가 비어 있습니다. Tile.png를 연결하세요.");
            return;
        }

        if (gridOverlaySprite == null)
        {
            Debug.LogError("LabGridManager: Grid Overlay Sprite가 비어 있습니다. Tile_Grid.png를 연결하세요.");
            return;
        }

        if (tiles != null)
        {
            Debug.LogWarning("LabGridManager: 이미 타일이 생성되어 있습니다.");
            return;
        }

        if (tileParent == null)
        {
            GameObject parentObject = new GameObject("GeneratedLabTiles");
            parentObject.transform.SetParent(transform);
            parentObject.transform.localPosition = Vector3.zero;
            tileParent = parentObject.transform;
        }

        tiles = new LabTile[width, height];
        allTiles.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y)
    {
        Vector3 tileWorldPosition = GridToWorldPosition(x, y);


        GameObject tileObject = new GameObject("Tile_" + x + "_" + y);
        tileObject.transform.SetParent(tileParent);
        tileObject.transform.position = tileWorldPosition;

        LabTile labTile = tileObject.AddComponent<LabTile>();

        GameObject baseTileObject = new GameObject("BaseTile");
        baseTileObject.transform.SetParent(tileObject.transform);
        baseTileObject.transform.localPosition = Vector3.zero;
        baseTileObject.transform.localScale = Vector3.one * tileScale;

        SpriteRenderer baseRenderer = baseTileObject.AddComponent<SpriteRenderer>();
        baseRenderer.sprite = tileSprite;
        baseRenderer.sortingOrder = baseTileOrder;

        GameObject gridOverlayObject = new GameObject("GridOverlay");
        gridOverlayObject.transform.SetParent(tileObject.transform);
        gridOverlayObject.transform.localPosition = Vector3.zero;
        gridOverlayObject.transform.localScale = Vector3.one * tileScale;

        SpriteRenderer gridOverlayRenderer = gridOverlayObject.AddComponent<SpriteRenderer>();
        gridOverlayRenderer.sprite = gridOverlaySprite;
        gridOverlayRenderer.sortingOrder = gridOverlayOrder;

        labTile.baseRenderer = baseRenderer;
        labTile.gridOverlayRenderer = gridOverlayRenderer;
        labTile.Initialize(x, y);

        labTile.SetGridOverlayVisible(false);

        tiles[x, y] = labTile;
        allTiles.Add(labTile);
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        float worldX = (x - y) * tileHalfWidth;
        float worldY = -(x + y) * tileHalfHeight;

        return originPosition + new Vector3(worldX, worldY, 0f);
    }

    public void ShowPlacementGrid()
    {
        SetPlacementGridVisible(true);
    }

    public void HidePlacementGrid()
    {
        SetPlacementGridVisible(false);
    }

    private void SetPlacementGridVisible(bool visible)
    {
        if (tiles == null)
        {
            return;
        }

        foreach (LabTile tile in tiles)
        {
            if (tile != null)
            {
                tile.SetGridOverlayVisible(visible);
            }
        }
    }

    public LabTile GetNearestTile(Vector3 worldPosition)
    {
        if (tiles == null)
        {
            return null;
        }

        LabTile nearestTile = null;
        float nearestDistance = float.MaxValue;

        foreach (LabTile tile in tiles)
        {
            if (tile == null)
            {
                continue;
            }

            float distance = Vector3.Distance(worldPosition, tile.GetCenterPosition());

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTile = tile;
            }
        }

        return nearestTile;
    }

    public Vector3 GetNearestTileCenterPosition(Vector3 worldPosition)
    {
        LabTile nearestTile = GetNearestTile(worldPosition);

        if (nearestTile == null)
        {
            return worldPosition;
        }

        return nearestTile.GetCenterPosition();
    }

    public bool CanPlaceOnTile(LabTile tile)
    {
        if (tile == null)
        {
            return false;
        }

        return tile.CanPlaceObject();
    }

    public LabTile GetNearestTileFromMousePosition()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("LabGridManager: Main Camera가 없습니다.");
            return null;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        return GetNearestTile(mouseWorldPosition);
    }

    public List<LabTile> GetPlacementTiles(LabTile originTile, int spaceCost, PlacementDirection direction)
    {
        List<LabTile> result = new List<LabTile>();

        if (originTile == null)
        {
            return result;
        }

        int finalSpaceCost = Mathf.Max(1, spaceCost);
        Vector2Int step = GetPlacementDirectionStep(direction);


        for (int i = 0; i < finalSpaceCost; i++)
        {
            int targetX = originTile.gridX + step.x * i;
            int targetY = originTile.gridY + step.y * i;

            Debug.Log(
                $"origin={originTile.gridX},{originTile.gridY} " +
                $"dir={direction} " +
                $"tile={targetX},{targetY}"
            );

            LabTile tile = GetTile(targetX, targetY);

            if (tile == null)
            {
                result.Clear();
                return result;
            }

            result.Add(tile);
        }

        return result;
    }

    public bool CanPlaceEquipment(LabTile originTile, int spaceCost, PlacementDirection direction)
    {

        Debug.Log(
            $"origin={originTile.gridX},{originTile.gridY} " +
            $"direction={direction}"
        );

        List<LabTile> placementTiles = GetPlacementTiles(originTile, spaceCost, direction);


        if (placementTiles.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < placementTiles.Count; i++)
        {
            if (placementTiles[i] == null || placementTiles[i].CanPlaceObject() == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool CanPlaceArea(
        LabTile originTile,
        int sizeX,
        int sizeY,
        PlacementDirection direction
    )
    {
        return true;
    }

    public void MarkEquipmentTilesOccupied(LabTile originTile, int spaceCost, PlacementDirection direction)
    {
        List<LabTile> placementTiles = GetPlacementTiles(originTile, spaceCost, direction);

        Debug.Log("==== 점유 시작 ====");

        for (int i = 0; i < placementTiles.Count; i++)
        {
            Debug.Log(
                $"occupy : {placementTiles[i].gridX}," +
                $"{placementTiles[i].gridY}"
            );

            placementTiles[i].SetOccupied(true);
        }

        for (int i = 0; i < placementTiles.Count; i++)
        {
            if (placementTiles[i] != null)
            {
                placementTiles[i].SetOccupied(true);
            }
        }

    }

    private Vector2Int GetPlacementDirectionStep(PlacementDirection direction)
    {
        
        switch (direction)
        {
            case PlacementDirection.RD:
                return new Vector2Int(1, 0);

            case PlacementDirection.LD:
                return new Vector2Int(0, 1);

            case PlacementDirection.RU:
                return new Vector2Int(-1, 0);

            case PlacementDirection.LU:
                return new Vector2Int(0, -1);

            default:
                return new Vector2Int(1, 0);
        }
    }

    public void MarkTileOccupied(LabTile tile)
    {
        if (tile == null)
        {
            return;
        }

        tile.SetOccupied(true);
    }

    public LabTile GetTile(int x, int y)
    {
        if (tiles == null)
        {
            return null;
        }

        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return null;
        }

        return tiles[x, y];
    }

    /// <summary>
    /// 게임 시작 시 벽지를 한 번만 생성한다.
    /// 이후 레벨 테마가 바뀌어도 벽 오브젝트는 다시 만들지 않고 Sprite만 교체한다.
    /// </summary>
    public void GenerateInitialWalls()
    {
        if (tiles == null)
        {
            Debug.LogError("LabGridManager: 타일이 아직 생성되지 않아서 벽지를 만들 수 없습니다.");
            return;
        }

        if (wallLeftSprite == null)
        {
            Debug.LogError("LabGridManager: Wall_Left Sprite가 비어 있습니다.");
            return;
        }

        if (wallRightSprite == null)
        {
            Debug.LogError("LabGridManager: Wall_Right Sprite가 비어 있습니다.");
            return;
        }

        if (doorWallRightSprite == null)
        {
            doorWallRightSprite = wallRightSprite;
        }

        if (wallParent == null)
        {
            GameObject parentObject = new GameObject("GeneratedLabWalls");
            parentObject.transform.SetParent(transform);
            parentObject.transform.localPosition = Vector3.zero;
            wallParent = parentObject.transform;
        }

        leftWallRenderers.Clear();
        rightWallRenderers.Clear();
        doorWallRightRenderer = null;

        for (int y = 0; y < height; y++)
        {
            LabTile tile = GetTile(0, y);

            if (tile != null)
            {
                Vector3 wallPosition = GetTileTopLeftEdgeCenter(tile) + wallLeftOffset;

                SpriteRenderer wallRenderer = CreateWall(
                    "Wall_Left_" + y,
                    wallLeftSprite,
                    wallPosition
                );

                if (wallRenderer != null)
                {
                    leftWallRenderers.Add(wallRenderer);
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            LabTile tile = GetTile(x, 0);

            if (tile != null)
            {
                Vector3 wallPosition = GetTileTopRightEdgeCenter(tile) + wallRightOffset;

                SpriteRenderer wallRenderer = CreateWall(
                    "Wall_Right_" + x,
                    wallRightSprite,
                    wallPosition
                );

                if (wallRenderer != null)
                {
                    // 기존 오른쪽 벽지 중 doorWallRightIndex에 해당하는 벽지만 문 포함 벽지로 대체한다.
                    // 새 문 벽지를 추가로 생성하지 않는다.
                    if (x == doorWallRightIndex)
                    {
                        doorWallRightRenderer = wallRenderer;
                        doorWallRightRenderer.sprite = doorWallRightSprite != null ? doorWallRightSprite : wallRightSprite;
                    }
                    else
                    {
                        rightWallRenderers.Add(wallRenderer);
                    }
                }
            }
        }
    }

    private SpriteRenderer CreateWall(string wallName, Sprite wallSprite, Vector3 worldPosition)
    {
        GameObject wallObject = new GameObject(wallName);
        wallObject.transform.SetParent(wallParent);
        wallObject.transform.position = worldPosition;
        wallObject.transform.localScale = Vector3.one * wallScale;

        SpriteRenderer spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = wallSprite;
        spriteRenderer.sortingOrder = wallOrder;

        return spriteRenderer;
    }

    public Vector3 GetTileTopLeftEdgeCenter(LabTile tile)
    {
        if (tile == null)
        {
            return Vector3.zero;
        }

        Vector3 tileCenter = tile.GetCenterPosition();

        Vector3 edgeOffset = new Vector3(
            -manualTileHalfWidth * 0.5f,
            manualTileHalfHeight * 0.5f,
            0f
        );

        return tileCenter + edgeOffset;
    }

    public Vector3 GetTileTopRightEdgeCenter(LabTile tile)
    {
        if (tile == null)
        {
            return Vector3.zero;
        }

        Vector3 tileCenter = tile.GetCenterPosition();

        Vector3 edgeOffset = new Vector3(
            manualTileHalfWidth * 0.5f,
            manualTileHalfHeight * 0.5f,
            0f
        );

        return tileCenter + edgeOffset;
    }

    public int GetSortingOrderByTile(int tileX, int tileY)
    {
        int baseOrder = 1000;

        return baseOrder - tileY * 100 - tileX;
    }

    public int GetSortingOrderByTile(LabTile tile)
    {
        if (tile == null)
        {
            return 0;
        }

        return GetSortingOrderByTile(tile.gridX, tile.gridY);
    }

    private LabTile FindTopTile()
    {
        LabTile topTile = null;
        float highestWorldY = float.MinValue;

        for (int i = 0; i < allTiles.Count; i++)
        {
            LabTile tile = allTiles[i];

            if (tile == null)
            {
                continue;
            }

            float tileWorldY = tile.GetCenterPosition().y;

            if (topTile == null || tileWorldY > highestWorldY)
            {
                topTile = tile;
                highestWorldY = tileWorldY;
            }
        }

        return topTile;
    }

    private void MarkInspectionTile()
    {
        inspectionTile = FindTopTile();

        if (inspectionTile == null)
        {
            Debug.LogWarning("LabGridManager: 검사 연출용 타일을 찾지 못했습니다.");
            return;
        }

        inspectionTile.tileRole = LabTileRole.InspectionZone;
        inspectionTile.SetOccupied(false);

        Debug.Log("검사 연출용 타일 지정: " + inspectionTile.name);
    }

    public LabEnvironmentTheme GetThemeByLabLevel(int labLevel)
    {
        if (environmentThemes == null || environmentThemes.Length == 0)
        {
            return null;
        }

        for (int i = 0; i < environmentThemes.Length; i++)
        {
            LabEnvironmentTheme theme = environmentThemes[i];

            if (theme == null)
            {
                continue;
            }

            if (labLevel >= theme.minLabLevel && labLevel <= theme.maxLabLevel)
            {
                return theme;
            }
        }

        return environmentThemes[environmentThemes.Length - 1];
    }

    public void ApplyEnvironmentByLabLevel(int labLevel)
    {
        LabEnvironmentTheme theme = GetThemeByLabLevel(labLevel);

        if (theme == null)
        {
            return;
        }

        if (theme.tileSprite != null)
        {
            tileSprite = theme.tileSprite;
            ApplyTileSprites(theme.tileSprite);
        }

        if (theme.wallLeftSprite != null)
        {
            wallLeftSprite = theme.wallLeftSprite;
        }

        if (theme.wallRightSprite != null)
        {
            wallRightSprite = theme.wallRightSprite;
        }

        if (theme.doorWallRightSprite != null)
        {
            doorWallRightSprite = theme.doorWallRightSprite;
        }

        if (theme.backgroundSprite != null)
        {
            ApplyBackgroundSprite(theme.backgroundSprite);
        }

        ApplyWallSprites(wallLeftSprite, wallRightSprite, doorWallRightSprite);
    }

    private void ApplyTileSprites(Sprite newTileSprite)
    {
        if (newTileSprite == null)
        {
            return;
        }

        for (int i = 0; i < allTiles.Count; i++)
        {
            LabTile tile = allTiles[i];

            if (tile != null && tile.baseRenderer != null)
            {
                tile.baseRenderer.sprite = newTileSprite;
            }
        }
    }

    private void ApplyWallSprites(Sprite newLeftWallSprite, Sprite newRightWallSprite, Sprite newDoorWallRightSprite)
    {
        for (int i = 0; i < leftWallRenderers.Count; i++)
        {
            if (leftWallRenderers[i] != null && newLeftWallSprite != null)
            {
                leftWallRenderers[i].sprite = newLeftWallSprite;
            }
        }

        for (int i = 0; i < rightWallRenderers.Count; i++)
        {
            if (rightWallRenderers[i] != null && newRightWallSprite != null)
            {
                rightWallRenderers[i].sprite = newRightWallSprite;
            }
        }

        if (doorWallRightRenderer != null)
        {
            // 기존 오른쪽 벽지 하나의 Sprite만 문 포함 벽지로 대체한다.
            // 위치, 스케일, 오브젝트 개수는 건드리지 않는다.
            if (newDoorWallRightSprite != null)
            {
                doorWallRightRenderer.sprite = newDoorWallRightSprite;
            }
            else if (newRightWallSprite != null)
            {
                doorWallRightRenderer.sprite = newRightWallSprite;
            }
        }
        else
        {
            Debug.LogWarning("LabGridManager: 문 벽지로 대체할 오른쪽 벽 Renderer가 없습니다. Door Wall Right Index 값을 확인하세요.");
        }
    }

    private void ApplyBackgroundSprite(Sprite newBackgroundSprite)
    {
        if (backgroundRenderer == null)
        {
            return;
        }

        backgroundRenderer.sprite = newBackgroundSprite;
        backgroundRenderer.sortingOrder = backgroundOrder;
        FitBackgroundToCamera();
    }

    public void FitBackgroundToCamera()
    {
        if (backgroundRenderer == null || backgroundRenderer.sprite == null)
        {
            return;
        }

        if (backgroundTargetCamera == null)
        {
            backgroundTargetCamera = Camera.main;
        }

        if (backgroundTargetCamera == null)
        {
            return;
        }

        if (backgroundTargetCamera.orthographic == false)
        {
            Debug.LogWarning("LabGridManager: 배경 자동 맞춤은 Orthographic Camera 기준입니다.");
            return;
        }

        float cameraHeight = backgroundTargetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * backgroundTargetCamera.aspect;

        float spriteWidth = backgroundRenderer.sprite.bounds.size.x;
        float spriteHeight = backgroundRenderer.sprite.bounds.size.y;

        if (spriteWidth <= 0f || spriteHeight <= 0f)
        {
            return;
        }

        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;
        float finalScale = Mathf.Max(scaleX, scaleY) * backgroundExtraScaleMultiplier;

        backgroundRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);

        Vector3 cameraPosition = backgroundTargetCamera.transform.position;
        backgroundRenderer.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0f);
        backgroundRenderer.sortingOrder = backgroundOrder;
    }

    public void ApplyEnvironmentFromCurrentLabLevel()
    {
        int currentLabLevel = GetCurrentLabLevelForTheme();
        ApplyEnvironmentByLabLevel(currentLabLevel);
        Debug.Log("연구실 환경 테마 적용: Lv." + currentLabLevel);
    }

    private int GetCurrentLabLevelForTheme()
    {
        if (ResourceManager.Instance != null)
        {
            return ResourceManager.Instance.labLevel;
        }

        return startLabLevelForTheme;
    }


    /// <summary>
    /// 현재 생성되어 있는 LabTile 개수를 기준으로 연구실 평수를 반환한다.
    /// Tile 1개 = 1평 기준이다.
    /// </summary>
    public int GetCurrentArea()
    {
        return allTiles.Count;
    }

    /// <summary>
    /// 지정된 grid 좌표 목록에 타일을 추가한다.
    /// 기존 타일은 유지하고, 없는 좌표에만 새 타일을 생성한다.
    /// </summary>
    public void ExpandGridByCoordinates(Vector2Int[] gridPositions)
    {
        if (gridPositions == null || gridPositions.Length == 0)
        {
            Debug.LogWarning("LabGridManager: 추가할 타일 좌표가 없습니다.");
            return;
        }

        int maxX = width - 1;
        int maxY = height - 1;

        for (int i = 0; i < gridPositions.Length; i++)
        {
            Vector2Int position = gridPositions[i];

            if (position.x < 0 || position.y < 0)
            {
                Debug.LogWarning("LabGridManager: 음수 좌표는 사용할 수 없습니다. " + position);
                continue;
            }

            if (position.x > maxX)
            {
                maxX = position.x;
            }

            if (position.y > maxY)
            {
                maxY = position.y;
            }
        }

        EnsureTileArraySize(maxX + 1, maxY + 1);

        if (inspectionTile != null)
        {
            inspectionTile.tileRole = LabTileRole.Normal;
        }

        int createdCount = 0;

        for (int i = 0; i < gridPositions.Length; i++)
        {
            Vector2Int position = gridPositions[i];

            if (position.x < 0 || position.y < 0)
            {
                continue;
            }

            if (tiles[position.x, position.y] != null)
            {
                continue;
            }

            CreateTile(position.x, position.y);
            createdCount++;
        }

        RebuildWallsAfterExpansion();
        MarkInspectionTile();
        ApplyEnvironmentFromCurrentLabLevel();
        HidePlacementGrid();

        Debug.Log("연구실 타일 추가 완료: +" + createdCount + "개 / 현재 " + GetCurrentArea() + "평");
    }

    private void EnsureTileArraySize(int newWidth, int newHeight)
    {
        if (tiles == null)
        {
            width = Mathf.Max(width, newWidth);
            height = Mathf.Max(height, newHeight);
            tiles = new LabTile[width, height];
            return;
        }

        if (newWidth <= width && newHeight <= height)
        {
            return;
        }

        int finalWidth = Mathf.Max(width, newWidth);
        int finalHeight = Mathf.Max(height, newHeight);

        LabTile[,] newTiles = new LabTile[finalWidth, finalHeight];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newTiles[x, y] = tiles[x, y];
            }
        }

        tiles = newTiles;
        width = finalWidth;
        height = finalHeight;
    }

    private void RebuildWallsAfterExpansion()
    {
        ClearGeneratedWalls();
        GenerateInitialWalls();
    }

    private void ClearGeneratedWalls()
    {
        leftWallRenderers.Clear();
        rightWallRenderers.Clear();
        doorWallRightRenderer = null;

        if (wallParent == null)
        {
            return;
        }

        for (int i = wallParent.childCount - 1; i >= 0; i--)
        {
            Transform child = wallParent.GetChild(i);

            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}