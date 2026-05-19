using UnityEngine;

/// <summary>
/// 4방향 Sprite를 한 묶음으로 관리하는 데이터 클래스.
/// 
/// 예:
/// Desk_RD.png
/// Desk_RU.png
/// Desk_LD.png
/// Desk_LU.png
/// 
/// 이런 4개 이미지를 하나의 DirectionSpriteSet으로 묶어두면,
/// 현재 방향에 따라 알맞은 Sprite를 쉽게 가져올 수 있다.
/// </summary>
[System.Serializable]
public class DirectionSpriteSet
{
    [Header("4방향 Sprite")]
    [Tooltip("우측하단 방향 이미지입니다.")]
    public Sprite rightDown;

    [Tooltip("우측상단 방향 이미지입니다.")]
    public Sprite rightUp;

    [Tooltip("좌측하단 방향 이미지입니다.")]
    public Sprite leftDown;

    [Tooltip("좌측상단 방향 이미지입니다.")]
    public Sprite leftUp;

    /// <summary>
    /// 방향값에 맞는 Sprite를 반환한다.
    /// </summary>
    public Sprite GetSprite(PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.RD:
                return rightDown;

            case PlacementDirection.RU:
                return rightUp;

            case PlacementDirection.LD:
                return leftDown;

            case PlacementDirection.LU:
                return leftUp;

            default:
                return rightDown;
        }
    }

    /// <summary>
    /// 4방향 Sprite가 전부 연결되어 있는지 확인한다.
    /// 
    /// 개발 중에는 PNG가 아직 준비되지 않았을 수 있으므로,
    /// 임시로 같은 Sprite를 네 칸에 모두 넣어도 된다.
    /// </summary>
    public bool HasAllSprites()
    {
        return rightDown != null &&
               rightUp != null &&
               leftDown != null &&
               leftUp != null;
    }
}