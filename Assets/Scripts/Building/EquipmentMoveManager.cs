using UnityEngine;

public class EquipmentMoveManager : MonoBehaviour
{
    public static EquipmentMoveManager Instance;

    private PlaceableObject movingObject;
    private TilePlaceableEquipmentObject movingTileEquipment;
    private WorkstationObject movingWorkstation;

    private LabTile currentTile;
    private bool canPlace = false;

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

        currentTile =
            LabGridManager.Instance.GetNearestTileFromMousePosition();

        if (currentTile == null)
        {
            return;
        }

        movingObject.transform.position =
            currentTile.GetCenterPosition();

        // 이동 중 R키 회전
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateMovingObject();
        }

        // 처음 클릭 떼기 전에는 배치 금지
        if (!canPlace)
        {
            if (Input.GetMouseButtonUp(0))
            {
                canPlace = true;
            }

            return;
        }

        // 좌클릭으로 배치 완료
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

        currentTile.SetOccupied(true);

        movingObject.placedTile = currentTile;

        Debug.Log("이동 완료");

        movingObject = null;
        movingTileEquipment = null;

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
}