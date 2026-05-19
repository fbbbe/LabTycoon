using UnityEngine;

/// <summary>
/// 책상 위 장비 하나의 데이터를 담는 클래스.
/// 
/// 예:
/// - 낡은 노트북
/// - 중고 컴퓨터
/// - 기본 컴퓨터
/// - 사무용 컴퓨터
/// - 고사양 컴퓨터
/// 
/// 이 클래스는 MonoBehaviour가 아니다.
/// 즉, 오브젝트에 붙이는 스크립트가 아니라
/// WorkstationObject나 상점 데이터 안에서 사용하는 "데이터 묶음"이다.
/// </summary>
[System.Serializable]
public class DeskEquipmentData
{
    [Header("장비 기본 정보")]
    [Tooltip("장비 이름입니다. 예: 낡은 노트북")]
    public string equipmentName;

    [Tooltip("장비 종류입니다.")]
    public DeskEquipmentType equipmentType;

    [Tooltip("구매 가격입니다.")]
    public int price;

    [Header("장비 4방향 이미지")]
    [Tooltip("Workstation 방향 RD일 때 사용할 장비 이미지입니다.")]
    public Sprite rightDownSprite;

    [Tooltip("Workstation 방향 RU일 때 사용할 장비 이미지입니다.")]
    public Sprite rightUpSprite;

    [Tooltip("Workstation 방향 LD일 때 사용할 장비 이미지입니다.")]
    public Sprite leftDownSprite;

    [Tooltip("Workstation 방향 LU일 때 사용할 장비 이미지입니다.")]
    public Sprite leftUpSprite;

    [Header("효과 수치")]
    [Tooltip("과제 완료 시 돈 보상 증가율입니다. 예: 0.05 = 5% 증가")]
    public float moneyRewardBonusRate;

    [Tooltip("과제 완료 시 연구성과 증가율입니다. 예: 0.05 = 5% 증가")]
    public float researchRewardBonusRate;

    /// <summary>
    /// Workstation 방향에 맞는 장비 Sprite를 반환한다.
    /// 
    /// Workstation이 회전하면 책상 위 장비도 같은 방향 Sprite로 바뀌어야 한다.
    /// </summary>
    public Sprite GetSprite(PlacementDirection direction)
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

    /// <summary>
    /// 4방향 Sprite가 전부 들어갔는지 확인한다.
    /// 
    /// 개발 중에는 PNG가 부족할 수 있으므로,
    /// 임시로 같은 Sprite를 4칸에 넣어도 된다.
    /// </summary>
    public bool HasAllSprites()
    {
        return rightDownSprite != null &&
               rightUpSprite != null &&
               leftDownSprite != null &&
               leftUpSprite != null;
    }
}