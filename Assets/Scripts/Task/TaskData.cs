using UnityEngine;

/// <summary>
/// 과제 하나의 데이터.
/// 
/// 지금은 1차 구현용으로 F급 과제 하나만 사용한다.
/// 나중에는 과제 등급별로 여러 TaskData를 데이터베이스화할 수 있다.
/// </summary>
[System.Serializable]
public class TaskData
{
    [Header("과제 정보")]
    public string taskName = "F급 과제";

    [Tooltip("과제 수행 시간입니다.")]
    public float workTime = 7f;

    [Tooltip("검사 완료 후 받을 기본 돈 보상입니다.")]
    public int baseMoneyReward = 3000;

    [Tooltip("검사 완료 후 받을 기본 연구성과입니다.")]
    public int baseResearchResult = 15;

    [Tooltip("과제 수행으로 증가하는 기본 스트레스입니다.")]
    public int baseTaskStress = 2;

    [Tooltip("이 과제를 수행하기 위해 필요한 연구력입니다.")]
    public int requiredResearchPower = 10;
}