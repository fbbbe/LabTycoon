using UnityEngine;

/// <summary>
/// 타일 위에 직접 배치되는 장비의 방향별 Sprite 데이터.
/// 
/// 대상:
/// - GPU 서버
/// - AI 연구 서버
/// - 연구 장비
/// - 환경 장비
/// - 커피 장비
/// - 청소 장비
/// - 책상 의자 세트는 예외적으로 Workstation prefab을 사용
/// </summary>
[System.Serializable]
public class TilePlaceableEquipmentData
{
    public string equipmentName;

    public Sprite rightDownSprite;
    public Sprite rightUpSprite;
    public Sprite leftDownSprite;
    public Sprite leftUpSprite;

    public Sprite GetSpriteByDirection(PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.RD:
                return rightDownSprite;

            case PlacementDirection.RU:
                return rightUpSprite;

            case PlacementDirection.LD:
                return leftDownSprite;

            case PlacementDirection.LU:
                return leftUpSprite;

            default:
                return rightDownSprite;
        }
    }
}