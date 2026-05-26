using System;
using UnityEngine;

/// <summary>
/// 실제 고용된 인력 한 명의 개별 데이터를 저장하는 클래스.
/// 
/// 같은 학사생이라도 각각 다른 스트레스, 레벨, 연구력, 과제 완료 횟수를 가져야 하므로
/// StaffHireData와 분리해서 관리한다.
/// </summary>
[Serializable]
public class StaffRuntimeData
{
    [Header("고유 식별")]
    public string staffId;

    [Header("기본 정보")]
    public string staffName;
    public StaffType staffType;

    [Header("성장 정보")]
    public int level;
    public int completedTaskCount;

    [Header("능력치")]
    public int researchPower;

    [Header("스트레스")]
    public int currentStress;
    public int maxStress;

    public StaffRuntimeData()
    {
        staffId = Guid.NewGuid().ToString();

        staffName = "인력";
        staffType = StaffType.Undergraduate;

        level = 1;
        completedTaskCount = 0;

        researchPower = 10;

        currentStress = 0;
        maxStress = 100;
    }

    public StaffRuntimeData(StaffHireData hireData)
    {
        staffId = Guid.NewGuid().ToString();

        staffName = hireData.staffName;
        staffType = hireData.staffType;

        level = hireData.level;
        completedTaskCount = 0;

        researchPower = hireData.researchPower;

        currentStress = hireData.initialStress;
        maxStress = 100;
    }

    public void AddStress(int amount)
    {
        currentStress += amount;
        currentStress = Mathf.Clamp(currentStress, 0, maxStress);
    }

    public void ReduceStress(int amount)
    {
        currentStress -= amount;
        currentStress = Mathf.Clamp(currentStress, 0, maxStress);
    }

    public void AddCompletedTaskCount(int amount = 1)
    {
        completedTaskCount += amount;
    }

    public bool CanAct()
    {
        return currentStress < maxStress;
    }
}