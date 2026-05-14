using UnityEngine;

public class StaffWorker : MonoBehaviour
{
    [Header("생산량")]
    public int moneyPerTick = 0;
    public int researchPerTick = 1;
    public int stressPerTick = 1;

    [Header("작업 간격")]
    public float workInterval = 5f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= workInterval)
        {
            timer = 0f;
            Work();
        }
    }

    private void Work()
    {
        if (ResourceManager.Instance == null)
        {
            return;
        }

        ResourceManager.Instance.AddMoney(moneyPerTick);
        ResourceManager.Instance.AddResearch(researchPerTick);
        ResourceManager.Instance.AddStress(stressPerTick);
    }
}