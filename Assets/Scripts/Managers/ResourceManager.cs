using UnityEngine;

/// <summary>
/// 게임 전체 자원을 관리하는 스크립트.
/// 
/// 여기에는 "전체 자원"만 넣는다.
/// 인력마다 다른 연구력, 스트레스, 인력 레벨은 StaffWorker.cs에서 관리한다.
/// 
/// 현재 전체 자원:
/// - 돈
/// - 연구성과
/// - 연구실 전체 레벨
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("전체 자원")]
    [Tooltip("플레이어가 보유한 돈입니다. 장비 구매, 인력 고용, 연구실 확장 등에 사용됩니다.")]
    public int money = 100000;

    [Tooltip("과제 검사 완료 후 얻는 연구성과입니다. 연구실 전체 레벨업에 사용됩니다.")]
    public int researchResult = 0;

    [Tooltip("연구실 전체 레벨입니다. 과제 등급, 인력 고용, 연구실 확장 해금에 사용됩니다.")]
    public int level = 1;

    private void Awake()
    {
        // ResourceManager는 씬에 하나만 있어야 한다.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 돈을 사용한다.
    /// 돈이 부족하면 false를 반환한다.
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
    /// 과제 검사 완료 후 호출될 예정이다.
    /// </summary>
    public void AddResearchResult(int amount)
    {
        researchResult += amount;

        if (researchResult < 0)
        {
            researchResult = 0;
        }
    }

    /// <summary>
    /// 연구실 레벨을 올린다.
    /// 나중에 연구성과 요구량 조건을 추가할 예정이다.
    /// </summary>
    public void LevelUp()
    {
        level++;
    }
}