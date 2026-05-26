using UnityEngine;

/// <summary>
/// Workstation의 특정 방향 하나에 대한 설정값.
/// 
/// 예:
/// RD 방향일 때
/// - 책상 Sprite는 무엇인지
/// - 빈 의자 Sprite는 무엇인지
/// - 학사생이 앉은 의자 Sprite는 무엇인지
/// - 책상 위치는 어디인지
/// - 의자 위치는 어디인지
/// - 장비 슬롯 위치는 어디인지
/// - 어떤 이미지가 앞에 와야 하는지
/// 
/// 이런 값을 한 번에 묶어서 관리한다.
/// </summary>
[System.Serializable]
public class WorkstationDirectionSetting
{
    [Header("방향")]
    [Tooltip("이 설정이 어떤 방향에 대한 설정인지 나타냅니다.")]
    public PlacementDirection direction;

    [Header("책상 이미지")]
    [Tooltip("이 방향에서 사용할 책상 Sprite입니다.")]
    public Sprite deskSprite;

    [Header("빈 의자 이미지")]
    [Tooltip("이 방향에서 사용할 빈 의자 Sprite입니다.")]
    public Sprite emptyChairSprite;

    [Header("인력이 앉은 의자 이미지")]
    [Tooltip("학사생이 앉아 있는 의자 이미지입니다.")]
    public Sprite undergraduateSeatedChairSprite;

    [Tooltip("석사생이 앉아 있는 의자 이미지입니다.")]
    public Sprite masterSeatedChairSprite;

    [Tooltip("박사생이 앉아 있는 의자 이미지입니다.")]
    public Sprite phdSeatedChairSprite;

    [Header("방향별 위치")]
    [Tooltip("Workstation 중심 기준 책상 위치입니다.")]
    public Vector3 deskLocalPosition;

    [Tooltip("Workstation 중심 기준 의자 위치입니다.")]
    public Vector3 chairLocalPosition;

    [Tooltip("Workstation 중심 기준 책상 위 장비 위치입니다.")]
    public Vector3 equipmentSlotLocalPosition;

    [Tooltip("Workstation 중심 기준 인력 착석 기준 위치입니다.")]
    public Vector3 seatPointLocalPosition;

    [Header("방향별 앞뒤 순서")]
    [Tooltip("책상의 Sorting Order입니다. 값이 클수록 앞에 보입니다.")]
    public int deskSortingOrder;

    [Tooltip("의자 또는 앉은 인력 이미지의 Sorting Order입니다. 값이 클수록 앞에 보입니다.")]
    public int chairSortingOrder;

    [Tooltip("책상 위 장비의 Sorting Order입니다. 값이 클수록 앞에 보입니다.")]
    public int deskEquipmentSortingOrder;

    /// <summary>
    /// 인력 종류에 맞는 앉은 의자 Sprite를 반환한다.
    /// </summary>
    public Sprite GetSeatedChairSprite(StaffType staffType)
    {
        switch (staffType)
        {
            case StaffType.Undergraduate:
                return undergraduateSeatedChairSprite;

            case StaffType.Master:
                return masterSeatedChairSprite;

            case StaffType.PhD:
                return phdSeatedChairSprite;

            default:
                return undergraduateSeatedChairSprite;
        }
    }
}