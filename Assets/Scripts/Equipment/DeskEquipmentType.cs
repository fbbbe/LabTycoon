/// <summary>
/// 책상 위에 설치 가능한 장비 종류.
/// 
/// 이 장비들은 바닥 타일에 직접 배치되지 않는다.
/// 반드시 Workstation의 책상 위 EquipmentSlot에 장착된다.
/// </summary>
public enum DeskEquipmentType
{
    OldLaptop,       // 낡은 노트북
    UsedComputer,    // 중고 컴퓨터
    BasicComputer,   // 기본 컴퓨터
    OfficeComputer,  // 사무용 컴퓨터
    HighEndComputer  // 고사양 컴퓨터
}