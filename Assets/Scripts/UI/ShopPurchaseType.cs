/// <summary>
/// 상점 아이템을 구매했을 때 어떤 방식으로 처리할지 구분한다.
/// 
/// TilePlaceable:
/// - 타일 위에 직접 배치되는 물건
/// - 예: 책상+의자 세트, 커피머신, 청소도구함
/// 
/// DeskEquipment:
/// - 책상 위에만 장착되는 장비
/// - 예: 낡은 노트북, 중고 컴퓨터, 기본 컴퓨터
/// </summary>
public enum ShopPurchaseType
{
    TilePlaceable,
    DeskEquipment
}