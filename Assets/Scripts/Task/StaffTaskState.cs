/// <summary>
/// Workstation에 앉은 인력의 작업 상태.
/// 
/// 이 상태에 따라 머리 위 버튼이 바뀐다.
/// </summary>
public enum StaffTaskState
{
    Idle,                 // 아무 작업 안 함, 과제 가능
    Working,              // 과제 수행 중
    WaitingForInspection, // 과제 완료 후 검사 대기
    Inspecting,           // 검사 중
    NeedCleaning,         // 검사 완료 후 청소 필요
    Cleaning,             // 청소 중
    Resting               // 휴식 중
}