using UnityEngine;

/// <summary>
/// 과제 등급.
/// 인력 종류/레벨에 따라 자동 선택된다.
/// </summary>
public enum TaskGrade
{
    F,
    E,
    D,
    C,
    B,
    A,
    S,
    SS,
    SSS,
    National,
    World
}

/// <summary>
/// 과제 등급별 데이터를 제공하는 정적 데이터베이스.
/// </summary>
public static class TaskGradeDatabase
{
    public static TaskGrade GetTaskGradeByStaff(StaffWorker staff)
    {
        if (staff == null)
        {
            return TaskGrade.F;
        }

        int level = staff.level;

        if (staff.runtimeData != null)
        {
            level = staff.runtimeData.level;
        }

        switch (staff.staffType)
        {
            case StaffType.Undergraduate:
                if (level >= 3)
                {
                    return TaskGrade.D;
                }

                if (level >= 2)
                {
                    return TaskGrade.E;
                }

                return TaskGrade.F;

            case StaffType.Master:
                if (level >= 3)
                {
                    return TaskGrade.A;
                }

                if (level >= 2)
                {
                    return TaskGrade.B;
                }

                return TaskGrade.C;

            case StaffType.PhD:
                if (level >= 5)
                {
                    return TaskGrade.World;
                }

                if (level >= 4)
                {
                    return TaskGrade.National;
                }

                if (level >= 3)
                {
                    return TaskGrade.SSS;
                }

                if (level >= 2)
                {
                    return TaskGrade.SS;
                }

                return TaskGrade.S;

            default:
                return TaskGrade.F;
        }
    }

    public static TaskData CreateTaskDataForStaff(StaffWorker staff)
    {
        TaskGrade grade = GetTaskGradeByStaff(staff);
        return CreateTaskData(grade);
    }

    public static TaskData CreateTaskData(TaskGrade grade)
    {
        TaskData taskData = new TaskData();

        taskData.workTime = 7f;

        switch (grade)
        {
            case TaskGrade.F:
                taskData.requiredResearchPower = 10;
                taskData.baseMoneyReward = 3000;
                taskData.baseResearchResult = 15;
                taskData.baseTaskStress = 2;
                break;

            case TaskGrade.E:
                taskData.requiredResearchPower = 15;
                taskData.baseMoneyReward = 3300;
                taskData.baseResearchResult = 45;
                taskData.baseTaskStress = 3;
                break;

            case TaskGrade.D:
                taskData.requiredResearchPower = 20;
                taskData.baseMoneyReward = 3600;
                taskData.baseResearchResult = 120;
                taskData.baseTaskStress = 4;
                break;

            case TaskGrade.C:
                taskData.requiredResearchPower = 30;
                taskData.baseMoneyReward = 4000;
                taskData.baseResearchResult = 300;
                taskData.baseTaskStress = 5;
                break;

            case TaskGrade.B:
                taskData.requiredResearchPower = 40;
                taskData.baseMoneyReward = 4500;
                taskData.baseResearchResult = 700;
                taskData.baseTaskStress = 6;
                break;

            case TaskGrade.A:
                taskData.requiredResearchPower = 50;
                taskData.baseMoneyReward = 5000;
                taskData.baseResearchResult = 1500;
                taskData.baseTaskStress = 7;
                break;

            case TaskGrade.S:
                taskData.requiredResearchPower = 60;
                taskData.baseMoneyReward = 5700;
                taskData.baseResearchResult = 3000;
                taskData.baseTaskStress = 8;
                break;

            case TaskGrade.SS:
                taskData.requiredResearchPower = 70;
                taskData.baseMoneyReward = 6500;
                taskData.baseResearchResult = 6000;
                taskData.baseTaskStress = 9;
                break;

            case TaskGrade.SSS:
                taskData.requiredResearchPower = 80;
                taskData.baseMoneyReward = 7500;
                taskData.baseResearchResult = 12000;
                taskData.baseTaskStress = 10;
                break;

            case TaskGrade.National:
                taskData.requiredResearchPower = 90;
                taskData.baseMoneyReward = 9000;
                taskData.baseResearchResult = 25000;
                taskData.baseTaskStress = 12;
                break;

            case TaskGrade.World:
                taskData.requiredResearchPower = 100;
                taskData.baseMoneyReward = 12000;
                taskData.baseResearchResult = 55000;
                taskData.baseTaskStress = 15;
                break;
        }

        return taskData;
    }
}