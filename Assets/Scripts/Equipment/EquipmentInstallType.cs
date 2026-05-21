/// <summary>
/// 장비가 어디에 설치되는지 구분한다.
/// </summary>
public enum EquipmentInstallType
{
    /// <summary>
    /// 타일 위에 직접 배치되는 장비.
    /// 예: 책상+의자 세트, 커피머신, 청소 장비 등
    /// </summary>
    TilePlaceable,

    /// <summary>
    /// 책상 위에만 설치되는 장비.
    /// 예: 낡은 노트북, 중고 컴퓨터, 기본 컴퓨터 등
    /// </summary>
    DeskEquipment
}