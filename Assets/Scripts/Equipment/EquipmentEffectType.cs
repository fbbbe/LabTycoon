/// <summary>
/// 장비 효과 종류.
/// 
/// 장비 효과는 돈 보상만 있는 게 아니다.
/// PDF 기준으로 돈 보상, 연구성과, 스트레스, 청소 시간, 자동화 효과를 모두 고려한다.
/// </summary>
public enum EquipmentEffectType
{
    None,

    /// <summary>
    /// 과제 보상으로 얻는 돈 증가.
    /// 예: 돈 보상 +2%라면 value = 0.02
    /// </summary>
    MoneyRewardBonus,

    /// <summary>
    /// 과제 완료 후 얻는 연구성과 증가.
    /// 예: 연구성과 +3%라면 value = 0.03
    /// </summary>
    ResearchResultBonus,

    /// <summary>
    /// 과제 수행 및 청소로 인한 스트레스 증가량 감소.
    /// 예: 스트레스 증가량 -5%라면 value = 0.05
    /// </summary>
    StressIncreaseReduction,

    /// <summary>
    /// 커피 장비류처럼 스트레스를 추가로 증가시키는 효과.
    /// 예: 스트레스 +1이라면 value = 1
    /// </summary>
    ExtraStressIncrease,

    /// <summary>
    /// 스트레스 회복 행동 효과 증가.
    /// 예: 회복 효과 +20%라면 value = 0.20
    /// </summary>
    RecoveryEffectBonus,

    /// <summary>
    /// 청소 시간 감소.
    /// 예: 청소 시간 5초 → 4초라면 value = 1
    /// </summary>
    CleaningTimeReduction,

    /// <summary>
    /// 일정 확률로 청소 자동 처리.
    /// value는 확률로 사용 가능. 예: 0.2 = 20%
    /// </summary>
    AutoCleaningChance,

    /// <summary>
    /// 청소 완료 후 자동으로 자리 복구.
    /// value는 보통 1로 사용.
    /// </summary>
    AutoSeatRestoreAfterCleaning
}