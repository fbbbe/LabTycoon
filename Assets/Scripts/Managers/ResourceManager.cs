using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("기본 자원")]
    public int money = 100000;
    public int research = 0;
    public int stress = 0;
    public int level = 1;

    [Header("생산량")]
    public int researchPerTick = 0;
    public int moneyPerTick = 0;
    public int stressPerTick = 0;

    [Header("레벨업 설정")]
    public int researchRequiredForLevelUp = 100;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

    public void AddMoney(int amount)
    {
        money += amount;

        if (money < 0)
        {
            money = 0;
        }
    }

    public void AddResearch(int amount)
    {
        research += amount;

        if (research < 0)
        {
            research = 0;
        }

        CheckLevelUp();
    }

    public void AddStress(int amount)
    {
        stress += amount;

        if (stress < 0)
        {
            stress = 0;
        }

        if (stress > 100)
        {
            stress = 100;
        }
    }

    public void AddResearchProduction(int amount)
    {
        researchPerTick += amount;

        if (researchPerTick < 0)
        {
            researchPerTick = 0;
        }
    }

    public void AddMoneyProduction(int amount)
    {
        moneyPerTick += amount;

        if (moneyPerTick < 0)
        {
            moneyPerTick = 0;
        }
    }

    public void AddStressProduction(int amount)
    {
        stressPerTick += amount;
    }

    private void CheckLevelUp()
    {
        while (research >= researchRequiredForLevelUp)
        {
            research -= researchRequiredForLevelUp;
            level++;

            researchRequiredForLevelUp += 100;

            Debug.Log("레벨업! 현재 레벨: " + level);
        }
    }
}