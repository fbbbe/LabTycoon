using UnityEngine;

/// <summary>
/// 연구실 바닥 타일 하나를 담당하는 스크립트.
/// 
/// Tile.png 하나가 1평이므로,
/// LabTile 하나는 1평짜리 바닥 한 칸을 의미한다.
/// </summary>
public class LabTile : MonoBehaviour
{
    [Header("타일 좌표")]
    public int gridX;
    public int gridY;

    [Header("타일 점유 상태")]
    [Tooltip("이 타일 위에 장비가 배치되어 있으면 true입니다.")]
    public bool isOccupied = false;

    [Header("타일 이미지")]
    [Tooltip("평소에 보이는 Tile.png 렌더러입니다.")]
    public SpriteRenderer baseRenderer;

    [Tooltip("배치 모드에서만 보이는 Tile_Grid.png 렌더러입니다.")]
    public SpriteRenderer gridOverlayRenderer;

    /// <summary>
    /// 타일 좌표를 초기화한다.
    /// </summary>
    public void Initialize(int x, int y)
    {
        gridX = x;
        gridY = y;
        isOccupied = false;
    }

    /// <summary>
    /// 이 타일의 중심 월드 좌표를 반환한다.
    /// 장비 배치는 이 좌표를 기준으로 한다.
    /// </summary>
    public Vector3 GetCenterPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// 배치 모드용 테두리 이미지를 켜거나 끈다.
    /// </summary>
    public void SetGridOverlayVisible(bool visible)
    {
        if (gridOverlayRenderer != null)
        {
            gridOverlayRenderer.gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// 이 타일에 장비를 배치할 수 있는지 확인한다.
    /// </summary>
    public bool CanPlaceObject()
    {
        return isOccupied == false;
    }

    /// <summary>
    /// 이 타일의 점유 상태를 바꾼다.
    /// 장비가 놓이면 true, 장비가 제거되면 false.
    /// </summary>
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}