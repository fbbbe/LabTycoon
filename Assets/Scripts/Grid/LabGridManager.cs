using UnityEngine;

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
/// 4. 나중에 장비를 배치할 때 가장 가까운 타일 중심 좌표를 제공한다.
/// </summary>
public class LabGridManager : MonoBehaviour
{
    public static LabGridManager Instance;

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
    public bool autoCalculateTileSpacing = false;

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

    // 실제 계산에 사용할 타일 중심 간격.
    private float tileHalfWidth;
    private float tileHalfHeight;

    private void Awake()
    {
        // 싱글톤 설정.
        // 다른 스크립트에서 LabGridManager.Instance로 접근할 수 있게 한다.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // 타일 간격을 계산한다.
        CalculateTileSpacing();

        // 게임 시작 시 기본 9평 연구실 바닥을 생성한다.
        GenerateInitialLab();

        // 바닥 타일이 생성된 뒤, 그 타일 좌표를 기준으로 벽지를 생성한다.
        GenerateInitialWalls();

        // 게임 시작 상태는 배치 모드가 아니므로 격자 테두리는 숨긴다.
        HidePlacementGrid();
    }

    /// <summary>
    /// 타일 사이 간격을 계산한다.
    /// 
    /// autoCalculateTileSpacing이 true면 Sprite 크기를 기준으로 자동 계산한다.
    /// 그런데 PNG 여백이나 아이소메트릭 형태에 따라 자동 계산이 어긋날 수 있어서,
    /// 지금 프로젝트에서는 수동값을 추천한다.
    /// </summary>
    private void CalculateTileSpacing()
    {
        if (autoCalculateTileSpacing && tileSprite != null)
        {
            // Sprite의 Unity 월드 기준 크기를 가져온다.
            Vector2 spriteSize = tileSprite.bounds.size;

            // 아이소메트릭 타일은 보통 중심 간격이 이미지 크기의 절반 정도다.
            tileHalfWidth = spriteSize.x * tileScale * 0.5f;
            tileHalfHeight = spriteSize.y * tileScale * 0.5f;
        }
        else
        {
            // 수동으로 입력한 값을 사용한다.
            tileHalfWidth = manualTileHalfWidth;
            tileHalfHeight = manualTileHalfHeight;
        }
    }

    /// <summary>
    /// 게임 시작 시 기본 연구실을 생성한다.
    /// 
    /// 현재 기본값:
    /// width = 3
    /// height = 3
    /// 따라서 Tile.png 9개가 생성된다.
    /// </summary>
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

        // 이미 생성된 타일이 있으면 중복 생성하지 않는다.
        if (tiles != null)
        {
            Debug.LogWarning("LabGridManager: 이미 타일이 생성되어 있습니다.");
            return;
        }

        // 생성된 타일을 정리해서 담아둘 부모 오브젝트를 만든다.
        if (tileParent == null)
        {
            GameObject parentObject = new GameObject("GeneratedLabTiles");
            parentObject.transform.SetParent(transform);
            parentObject.transform.localPosition = Vector3.zero;
            tileParent = parentObject.transform;
        }

        tiles = new LabTile[width, height];

        // 3 x 3 타일 생성.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateTile(x, y);
            }
        }
    }

    /// <summary>
    /// 1평짜리 타일 하나를 생성한다.
    /// 
    /// 생성되는 구조:
    /// Tile_0_0
    /// ├── BaseTile     : Tile.png 표시
    /// └── GridOverlay  : Tile_Grid.png 표시, 평소에는 숨김
    /// </summary>
    private void CreateTile(int x, int y)
    {
        // 격자 좌표를 실제 Unity 월드 좌표로 변환한다.
        Vector3 tileWorldPosition = GridToWorldPosition(x, y);

        // 타일 루트 오브젝트 생성.
        GameObject tileObject = new GameObject("Tile_" + x + "_" + y);
        tileObject.transform.SetParent(tileParent);
        tileObject.transform.position = tileWorldPosition;

        // 타일 상태를 관리하는 LabTile.cs 추가.
        LabTile labTile = tileObject.AddComponent<LabTile>();

        // 기본 바닥 이미지 오브젝트 생성.
        GameObject baseTileObject = new GameObject("BaseTile");
        baseTileObject.transform.SetParent(tileObject.transform);
        baseTileObject.transform.localPosition = Vector3.zero;
        baseTileObject.transform.localScale = Vector3.one * tileScale;

        SpriteRenderer baseRenderer = baseTileObject.AddComponent<SpriteRenderer>();
        baseRenderer.sprite = tileSprite;
        baseRenderer.sortingOrder = baseTileOrder;

        // 배치 모드용 흰색 테두리 오버레이 생성.
        GameObject gridOverlayObject = new GameObject("GridOverlay");
        gridOverlayObject.transform.SetParent(tileObject.transform);
        gridOverlayObject.transform.localPosition = Vector3.zero;
        gridOverlayObject.transform.localScale = Vector3.one * tileScale;

        SpriteRenderer gridOverlayRenderer = gridOverlayObject.AddComponent<SpriteRenderer>();
        gridOverlayRenderer.sprite = gridOverlaySprite;
        gridOverlayRenderer.sortingOrder = gridOverlayOrder;

        // LabTile.cs에 렌더러 연결.
        labTile.baseRenderer = baseRenderer;
        labTile.gridOverlayRenderer = gridOverlayRenderer;

        // 좌표 초기화.
        labTile.Initialize(x, y);

        // 처음에는 반드시 GridOverlay를 숨긴다.
        // 즉 게임 시작 시 Tile.png만 보이고 Tile_Grid.png는 보이지 않아야 한다.
        labTile.SetGridOverlayVisible(false);

        // 배열에 저장.
        tiles[x, y] = labTile;
    }

    /// <summary>
    /// 격자 좌표를 Unity 월드 좌표로 바꾼다.
    /// 
    /// 아이소메트릭 타일 배치 공식:
    /// worldX = (x - y) 곱하기 tileHalfWidth
    /// worldY = -(x + y) 곱하기 tileHalfHeight
    /// 
    /// x가 증가하면 오른쪽 아래로,
    /// y가 증가하면 왼쪽 아래로 타일이 이어진다.
    /// </summary>
    public Vector3 GridToWorldPosition(int x, int y)
    {
        float worldX = (x - y) * tileHalfWidth;
        float worldY = -(x + y) * tileHalfHeight;

        return originPosition + new Vector3(worldX, worldY, 0f);
    }

    /// <summary>
    /// 배치 모드용 격자 테두리를 보여준다.
    /// 
    /// 장비 구매 후 배치 모드에 들어갈 때 호출할 함수다.
    /// </summary>
    public void ShowPlacementGrid()
    {
        SetPlacementGridVisible(true);
    }

    /// <summary>
    /// 배치 모드용 격자 테두리를 숨긴다.
    /// 
    /// 게임 시작 상태, 배치 완료, 배치 취소 시 호출한다.
    /// </summary>
    public void HidePlacementGrid()
    {
        SetPlacementGridVisible(false);
    }

    /// <summary>
    /// 모든 타일의 GridOverlay 표시 여부를 바꾼다.
    /// 
    /// true  = Tile_Grid.png 보임
    /// false = Tile_Grid.png 숨김
    /// </summary>
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

    /// <summary>
    /// 월드 좌표에서 가장 가까운 타일을 찾는다.
    /// 
    /// 나중에 장비 미리보기를 마우스 위치에 따라 움직일 때 사용한다.
    /// </summary>
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

    /// <summary>
    /// 특정 위치에서 가장 가까운 타일 중심 좌표를 반환한다.
    /// 
    /// 장비를 이 위치에 놓으면 타일 중심에 딱 맞게 배치된다.
    /// </summary>
    public Vector3 GetNearestTileCenterPosition(Vector3 worldPosition)
    {
        LabTile nearestTile = GetNearestTile(worldPosition);

        if (nearestTile == null)
        {
            return worldPosition;
        }

        return nearestTile.GetCenterPosition();
    }

    /// <summary>
    /// 특정 타일에 장비를 놓을 수 있는지 검사한다.
    /// 현재는 이미 점유된 타일인지 아닌지만 확인한다.
    /// </summary>
    public bool CanPlaceOnTile(LabTile tile)
    {
        if (tile == null)
        {
            return false;
        }

        return tile.CanPlaceObject();
    }

    /// <summary>
    /// 장비 배치가 확정된 타일을 사용 중으로 표시한다.
    /// </summary>
    public void MarkTileOccupied(LabTile tile)
    {
        if (tile == null)
        {
            return;
        }

        tile.SetOccupied(true);
    }

    /// <summary>
    /// 격자 좌표로 특정 타일을 가져온다.
    /// 
    /// 예:
    /// GetTile(1, 1)
    /// → 3x3 연구실의 가운데 타일을 가져온다.
    /// 
    /// 이 함수는 게임 시작 시 낡은 노트북을 특정 위치에 배치하거나,
    /// 나중에 저장 데이터를 불러와 장비를 복원할 때 사용한다.
    /// </summary>
    public LabTile GetTile(int x, int y)
    {
        // 아직 타일 배열이 생성되지 않았다면 null 반환
        if (tiles == null)
        {
            return null;
        }

        // 범위 밖 좌표면 null 반환
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return null;
        }

        return tiles[x, y];
    }
    /// <summary>
    /// 게임 시작 시 연구실 벽지를 생성한다.
    /// 
    /// 현재 규칙:
    /// - 왼쪽 벽지는 x = 0 라인의 타일 윗왼쪽 변에 붙인다.
    /// - 오른쪽 벽지는 y = 0 라인의 타일 윗오른쪽 변에 붙인다.
    /// - 벽지 PNG는 타일보다 커도 된다.
    /// - 중요한 것은 벽지의 밑면이 타일의 윗변과 맞는 것이다.
    /// 
    /// 벽지 위치는:
    /// 타일 윗변 기준점 + wallLeftOffset / wallRightOffset
    /// 으로 조정한다.
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

        if (wallParent == null)
        {
            GameObject parentObject = new GameObject("GeneratedLabWalls");
            parentObject.transform.SetParent(transform);
            parentObject.transform.localPosition = Vector3.zero;
            wallParent = parentObject.transform;
        }

        // 왼쪽 상단 벽 생성
        // x = 0 라인: (0,0), (0,1), (0,2)
        for (int y = 0; y < height; y++)
        {
            LabTile tile = GetTile(0, y);

            if (tile != null)
            {
                // 타일의 왼쪽 윗변 기준점에 Wall_Left 전용 위치 보정을 더한다.
                Vector3 wallPosition = GetTileTopLeftEdgeCenter(tile) + wallLeftOffset;

                CreateWall(
                    "Wall_Left_" + y,
                    wallLeftSprite,
                    wallPosition
                );
            }
        }

        // 오른쪽 상단 벽 생성
        // y = 0 라인: (0,0), (1,0), (2,0)
        for (int x = 0; x < width; x++)
        {
            LabTile tile = GetTile(x, 0);

            if (tile != null)
            {
                // 타일의 오른쪽 윗변 기준점에 Wall_Right 전용 위치 보정을 더한다.
                Vector3 wallPosition = GetTileTopRightEdgeCenter(tile) + wallRightOffset;

                CreateWall(
                    "Wall_Right_" + x,
                    wallRightSprite,
                    wallPosition
                );
            }
        }
    }

    /// <summary>
    /// 벽지 오브젝트 하나를 생성한다.
    /// 
    /// wallSprite는 Wall_Left.png 또는 Wall_Right.png이고,
    /// worldPosition은 타일 끝선을 기준으로 계산된 위치다.
    /// </summary>
    private void CreateWall(string wallName, Sprite wallSprite, Vector3 worldPosition)
    {
        GameObject wallObject = new GameObject(wallName);
        wallObject.transform.SetParent(wallParent);
        wallObject.transform.position = worldPosition;

        // 벽지 크기.
        // 벽지 PNG 밑면이 타일 길이와 맞게 제작되어 있다면 1로 둔다.
        wallObject.transform.localScale = Vector3.one * wallScale;

        SpriteRenderer spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = wallSprite;
        spriteRenderer.sortingOrder = wallOrder;
    }
    /// <summary>
    /// 특정 타일의 왼쪽 윗변 중앙 위치를 반환한다.
    /// 
    /// Wall_Left.png를 붙일 기준점이다.
    /// 여기서 반환되는 위치는 "타일 중심"이 아니라
    /// 타일 왼쪽 위 모서리 방향의 변 중앙이다.
    /// </summary>
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

    /// <summary>
    /// 특정 타일의 오른쪽 윗변 중앙 위치를 반환한다.
    /// 
    /// Wall_Right.png를 붙일 기준점이다.
    /// </summary>
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

    /// <summary>
    /// 타일 좌표를 기준으로 SpriteRenderer의 Sorting Order를 계산한다.
    /// 
    /// Unity 2D에서는 sortingOrder 값이 클수록 화면 앞에 보인다.
    /// 
    /// 현재 정렬 규칙:
    /// 1. y값이 작은 타일에 있는 물체가 더 앞에 온다.
    /// 2. y값이 같다면 x값이 작은 타일에 있는 물체가 더 앞에 온다.
    /// 3. x와 y 기준이 충돌하면 y값 기준을 우선한다.
    /// 
    /// 예:
    /// (0, 0) → 가장 앞쪽 우선
    /// (1, 0) → 그다음
    /// (0, 1) → y가 더 크므로 뒤쪽
    /// 
    /// y에 100을 곱하는 이유:
    /// x 차이보다 y 차이를 훨씬 크게 반영하기 위해서다.
    /// 즉, y 기준이 x 기준보다 우선된다.
    /// </summary>
    public int GetSortingOrderByTile(int tileX, int tileY)
    {
        int baseOrder = 1000;

        return baseOrder - tileY * 100 - tileX;
    }

    /// <summary>
    /// LabTile을 직접 받아서 Sorting Order를 계산한다.
    /// </summary>
    public int GetSortingOrderByTile(LabTile tile)
    {
        if (tile == null)
        {
            return 0;
        }

        return GetSortingOrderByTile(tile.gridX, tile.gridY);
    }
}