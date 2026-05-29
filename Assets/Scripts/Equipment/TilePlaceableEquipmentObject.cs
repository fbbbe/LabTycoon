using UnityEngine;

/// <summary>
/// 타일 위에 설치되는 공통 장비 오브젝트.
/// 
/// 장비마다 prefab을 따로 만들지 않고,
/// 하나의 공통 prefab에 EquipmentData를 주입해서 방향별 Sprite를 표시한다.
/// </summary>
public class TilePlaceableEquipmentObject : MonoBehaviour
{
    [Header("렌더러")]
    public SpriteRenderer spriteRenderer;

    [Header("현재 장비 데이터")]
    public EquipmentData equipmentData;

    [Header("현재 방향")]
    public PlacementDirection currentDirection = PlacementDirection.RD;

    [Header("배치된 타일")]
    public LabTile placedTile;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void Initialize(EquipmentData data, LabTile tile, PlacementDirection direction)
    {
        equipmentData = data;
        placedTile = tile;
        currentDirection = direction;

        if (tile != null)
        {
            transform.position = tile.transform.position
                       + new Vector3(0f, 0.2f, 0f);
        }

        ApplyDirection(direction);
    }

    public void ApplyDirection(PlacementDirection direction)
    {
        currentDirection = direction;

        if (spriteRenderer == null)
        {
            Debug.LogError("TilePlaceableEquipmentObject: SpriteRenderer가 없습니다.");
            return;
        }

        if (equipmentData == null || equipmentData.tilePlaceableData == null)
        {
            Debug.LogError("TilePlaceableEquipmentObject: tilePlaceableData가 없습니다.");
            return;
        }

        spriteRenderer.sprite = equipmentData.tilePlaceableData.GetSpriteByDirection(direction);

        if (LabGridManager.Instance != null && placedTile != null)
        {
            spriteRenderer.sortingOrder = LabGridManager.Instance.GetSortingOrderByTile(placedTile) + 20;
        }
    }

    public void RotateToNextDirection()
    {
        currentDirection = GetNextDirection(currentDirection);
        ApplyDirection(currentDirection);
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

    private void OnMouseDown()
    {
        if (EquipmentMoveManager.Instance != null &&
            EquipmentMoveManager.Instance.IsMoving)
        {
            return;
        }

        Debug.Log("가구 클릭");

        if (BuildingEditManager.Instance == null)
        {
            return;
        }

        if (!BuildingEditManager.Instance.IsEditMode)
        {
            return;
        }

        if (EquipmentMoveManager.Instance == null)
        {
            return;
        }

        PlaceableObject placeable = GetComponent<PlaceableObject>();

        if (placeable == null)
        {
            placeable = gameObject.AddComponent<PlaceableObject>();
            placeable.placedTile = placedTile;
            placeable.isPlaced = true;
        }

        EquipmentMoveManager.Instance.StartMove(placeable);
    }
}