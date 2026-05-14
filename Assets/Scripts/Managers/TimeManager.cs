using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("게임 시간")]
    public int day = 1;
    public int hour = 9;
    public int minute = 0;

    [Header("시간 흐름 설정")]
    public float realSecondsPerGameTick = 1f;
    public int minutesPerTick = 10;

    private float timer = 0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerGameTick)
        {
            timer = 0f;
            AddGameTime(minutesPerTick);
            ProduceResources();
        }
    }

    private void AddGameTime(int minutes)
    {
        minute += minutes;

        while (minute >= 60)
        {
            minute -= 60;
            hour++;
        }

        while (hour >= 24)
        {
            hour -= 24;
            day++;
        }
    }

    private void ProduceResources()
    {
        if (ResourceManager.Instance == null)
        {
            return;
        }

        ResourceManager.Instance.AddMoney(ResourceManager.Instance.moneyPerTick);
        ResourceManager.Instance.AddResearch(ResourceManager.Instance.researchPerTick);
        ResourceManager.Instance.AddStress(ResourceManager.Instance.stressPerTick);
    }

    public string GetTimeText()
    {
        return "Day " + day + " / " + hour.ToString("00") + ":" + minute.ToString("00");
    }
}