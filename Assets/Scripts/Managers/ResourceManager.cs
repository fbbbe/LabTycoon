using UnityEngine;

/// <summary>
/// 게임 전체 자원을 관리하는 스크립트.
/// 
/// 여기에는 전체 자원만 들어간다.
/// 인력마다 다른 연구력, 스트레스는 StaffWorker에서 관리한다.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("초기 자원")]
    public int startingMoney = 100000;
    public int startingResearchResult = 0;
    public int startingLabLevel = 1;

    [Header("현재 자원")]
    public int money;
    public int researchResult;
    public int labLevel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeResources();
    }

    /// <summary>
    /// 게임 시작 시 기본 자원을 세팅한다.
    /// </summary>
    private void InitializeResources()
    {
        money = startingMoney;
        researchResult = startingResearchResult;
        labLevel = startingLabLevel;
    }

    /// <summary>
    /// 돈이 충분한지 확인한다.
    /// </summary>
    public bool HasEnoughMoney(int amount)
    {
        return money >= amount;
    }

    /// <summary>
    /// 돈을 사용한다.
    /// 구매 확정, 배치 완료 시점에서 호출한다.
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (money < amount)
        {
            Debug.Log("돈이 부족합니다.");
            return false;
        }

        money -= amount;
        return true;
    }

    /// <summary>
    /// 돈을 추가한다.
    /// 과제 검사 완료 후 보상 지급 시 사용한다.
    /// </summary>
    public void AddMoney(int amount)
    {
        money += amount;

        if (money < 0)
        {
            money = 0;
        }
    }

    /// <summary>
    /// 연구성과를 추가한다.
    /// 과제 검사 완료 후 보상 지급 시 사용한다.
    /// </summary>
    public void AddResearchResult(int amount)
    {
        researchResult += amount;

        if (researchResult < 0)
        {
            researchResult = 0;
        }
    }
}