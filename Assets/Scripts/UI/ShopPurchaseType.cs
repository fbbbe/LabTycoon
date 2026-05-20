/// <summary>
/// 상점 아이템 구매 방식.
/// 
/// TilePlaceable:
/// - 타일 위에 직접 배치하는 장비
/// - 예: 책상+의자 세트, 커피머신, 청소 도구함
/// 
/// DeskEquipment:
/// - 책상 위에만 설치하는 장비
/// - 예: 낡은 노트북, 중고 컴퓨터, 기본 컴퓨터
/// </summary>
public enum ShopPurchaseType
{
    TilePlaceable,
    DeskEquipment
}