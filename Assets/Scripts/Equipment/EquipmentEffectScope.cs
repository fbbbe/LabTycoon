/// <summary>
/// 장비 효과가 적용되는 범위.
/// </summary>
public enum EquipmentEffectScope
{
    /// <summary>
    /// 특정 인력에게만 적용.
    /// 예: 컴퓨터 장비 효과.
    /// </summary>
    PersonalStaff,

    /// <summary>
    /// 전체 돈 보상 계산에 적용.
    /// 예: 커피 장비의 돈 보상 증가.
    /// </summary>
    GlobalMoneyReward,

    /// <summary>
    /// 전체 연구성과 계산에 적용.
    /// 예: 연구 장비.
    /// </summary>
    GlobalResearchResult,

    /// <summary>
    /// 전체 스트레스 계산에 적용.
    /// 예: 환경 장비.
    /// </summary>
    GlobalStress,

    /// <summary>
    /// 모든 인력에게 패널티로 적용.
    /// 예: 커피 장비의 추가 스트레스.
    /// </summary>
    AllStaffPenalty,

    /// <summary>
    /// 전체 청소 시스템에 적용.
    /// 예: 청소 시간 감소, 자동 청소.
    /// </summary>
    GlobalCleaning
}