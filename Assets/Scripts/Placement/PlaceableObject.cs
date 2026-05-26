using UnityEngine;

/// <summary>
/// 타일 위에 배치 가능한 오브젝트를 나타내는 스크립트.
/// 
/// 예:
/// - 책상
/// - 컴퓨터
/// - 커피머신
/// - 낡은 노트북
/// 
/// 이 스크립트의 역할:
/// 1. 이 오브젝트가 배치 가능한 물건임을 표시한다.
/// 2. 현재 어떤 LabTile 위에 놓였는지 저장한다.
/// 3. 배치가 확정되었는지 여부를 저장한다.
/// 4. 배치 미리보기 상태일 때 투명도를 조절할 수 있게 한다.
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Header("배치 상태")]
    [Tooltip("이 오브젝트가 현재 어느 타일 위에 배치되었는지 저장합니다.")]
    public LabTile placedTile;

    [Tooltip("배치가 확정되었으면 true입니다. 미리보기 상태면 false입니다.")]
    public bool isPlaced = false;

    [Header("차지하는 칸 수")]
    [Tooltip("가로로 차지하는 타일 수입니다. 지금은 모든 장비를 1칸으로 사용합니다.")]
    public int sizeX = 1;

    [Tooltip("세로로 차지하는 타일 수입니다. 지금은 모든 장비를 1칸으로 사용합니다.")]
    public int sizeY = 1;

    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        // 이 오브젝트와 자식 오브젝트에 있는 SpriteRenderer를 모두 가져온다.
        // 장비 PNG가 자식에 붙어 있어도 투명도 조절이 가능하게 하기 위함이다.
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// 배치 미리보기 상태의 투명도를 설정한다.
    /// 
    /// alpha = 1   → 완전히 불투명
    /// alpha = 0.5 → 반투명
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (spriteRenderers == null)
        {
            return;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == null)
            {
                continue;
            }

            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    /// <summary>
    /// 배치 가능한 상태인지에 따라 색을 바꾼다.
    /// 
    /// canPlace = true  → 흰색 정상 표시
    /// canPlace = false → 붉은색 표시
    /// 
    /// 이 기능은 미리보기 중 현재 위치에 배치 가능한지 사용자에게 알려주는 역할이다.
    /// </summary>
    public void SetPreviewColor(bool canPlace)
    {
        if (spriteRenderers == null)
        {
            return;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == null)
            {
                continue;
            }

            if (canPlace)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.65f);
            }
            else
            {
                spriteRenderer.color = new Color(1f, 0.35f, 0.35f, 0.65f);
            }
        }
    }

    /// <summary>
    /// 배치가 확정되었을 때 호출한다.
    /// 
    /// placedTile에 현재 타일을 저장하고,
    /// 오브젝트를 완전히 불투명하게 바꾸고,
    /// isPlaced를 true로 바꾼다.
    /// </summary>
    public void ConfirmPlacement(LabTile tile)
    {
        placedTile = tile;
        isPlaced = true;

        SetAlpha(1f);
    }
}