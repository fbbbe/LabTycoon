using UnityEngine;

public class EquipmentMoveManager : MonoBehaviour
{
    public static EquipmentMoveManager Instance;

    private PlaceableObject movingObject;
    private TilePlaceableEquipmentObject movingTileEquipment;
    private WorkstationObject movingWorkstation;
    private Vector3 originalPosition;
    private LabTile originalTile;

    private LabTile currentTile; private bool canPlace = false;

    public bool IsMoving
    {
        get { return movingObject != null; }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
{
    if (movingObject == null)
    {
        return;
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
        CancelMove();
        return;
    }


    currentTile =
        LabGridManager.Instance.GetNearestTileFromMousePosition();

    if (currentTile == null)
    {
        return;
    }

    movingObject.transform.position =
        currentTile.GetCenterPosition()
        + new Vector3(0f, 0.2f, 0f);

    if (Input.GetKeyDown(KeyCode.R))
    {
        RotateMovingObject();
    }

    if (!canPlace)
    {
        if (Input.GetMouseButtonUp(0))
        {
            canPlace = true;
        }

        return;
    }

    if (Input.GetMouseButtonDown(0))
    {
        PlaceObject();
    }
}

    public void StartMove(PlaceableObject obj)
    {
        if (movingObject != null)
        {
            return;
        }

        originalPosition = obj.transform.position;
        originalTile = obj.placedTile;

        movingObject = obj;

        movingTileEquipment =
            obj.GetComponent<TilePlaceableEquipmentObject>();

        movingWorkstation =
            obj.GetComponent<WorkstationObject>();

        Debug.Log("movingTileEquipment = " + movingTileEquipment);

        if (obj.placedTile != null)
        {
            obj.placedTile.SetOccupied(false);
        }

        canPlace = false;

        Debug.Log("이동 시작");

        SpriteRenderer[] renderers =
            obj.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }
    }

   private void PlaceObject()
{
    if (movingObject == null)
    {
        return;
    }

    if (currentTile == null)
    {
        return;
    }

    if (!currentTile.CanPlaceObject())
    {
        Debug.Log("설치 불가능한 타일");
        return;
    }

    Debug.Log(
        $"Tile = {currentTile.name}, " +
        $"Role = {currentTile.tileRole}, " +
        $"Occupied = {currentTile.isOccupied}"
    );

    currentTile.SetOccupied(true);

    movingObject.placedTile = currentTile;

    if (movingTileEquipment != null)
    {
        movingObject.transform.position =
            currentTile.GetCenterPosition()
            + new Vector3(0f, 0.2f, 0f);
    }
    else
    {
        movingObject.transform.position =
            currentTile.GetCenterPosition();
    }

    SetAlpha(movingObject.gameObject, 1f);

    Debug.Log("이동 완료");

    movingObject = null;
    movingTileEquipment = null;
    movingWorkstation = null;

    currentTile = null;
    canPlace = false;
}

    private void RotateMovingObject()
    {
        if (movingTileEquipment != null)
        {
            movingTileEquipment.RotateToNextDirection();

            Debug.Log("타일 장비 방향 변경");
            return;
        }

        if (movingWorkstation != null)
        {
            PlacementDirection nextDirection =
                GetNextDirection(movingWorkstation.currentDirection);

            movingWorkstation.ApplyDirection(nextDirection);

            Debug.Log("책상 방향 변경");
        }
    }

    private PlacementDirection GetNextDirection(
        PlacementDirection direction)
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

    private void CancelMove()
{
    if (movingObject == null)
    {
        return;
    }

    movingObject.transform.position = originalPosition;
    movingObject.placedTile = originalTile;

    if (originalTile != null)
    {
        originalTile.SetOccupied(true);
    }

    SetAlpha(movingObject.gameObject, 1f);

    movingObject = null;
    movingTileEquipment = null;
    movingWorkstation = null;

    currentTile = null;
    canPlace = false;

    Debug.Log("이동 취소");
}

private void SetAlpha(GameObject target, float alpha)
{
    SpriteRenderer[] renderers =
        target.GetComponentsInChildren<SpriteRenderer>();

    foreach (SpriteRenderer renderer in renderers)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
}
}