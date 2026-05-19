/// <summary>
/// 배치 가능한 물체의 방향을 나타내는 enum.
/// 
/// 이 게임은 마름모형 아이소메트릭 바닥을 사용하므로,
/// 물체를 단순히 0도, 90도 회전시키지 않고
/// 방향별 PNG를 바꿔 끼우는 방식으로 처리한다.
/// 
/// RD = Right Down  = 우측하단
/// RU = Right Up    = 우측상단
/// LD = Left Down   = 좌측하단
/// LU = Left Up     = 좌측상단
/// </summary>
public enum PlacementDirection
{
    RD,
    RU,
    LD,
    LU
}