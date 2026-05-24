using System.Collections;
using UnityEngine;

/// <summary>
/// 교수님 검사 연출을 담당하는 매니저.
/// 
/// 인력 종류에 따라 검사받는 staff 프리팹을 다르게 생성한다.
/// </summary>
[DisallowMultipleComponent]
public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Instance;

    [Header("교수 프리팹")]
    public GameObject professorInspectionPrefab;

    [Header("검사용 Staff 프리팹")]
    public GameObject undergraduateInspectionPrefab;
    public GameObject masterInspectionPrefab;
    public GameObject phdInspectionPrefab;

    [Header("검사 위치")]
    public Transform professorSpawnPoint;
    public Transform staffInspectionPoint;

    [Header("검사 시간")]
    public float inspectionDuration = 2f;

    private GameObject currentProfessor;
    private GameObject currentStaffInspection;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartInspectionSequence(WorkstationTaskController taskController)
    {
        if (taskController == null)
        {
            Debug.LogError("InspectionManager: taskController가 없습니다.");
            return;
        }

        StartCoroutine(InspectionRoutine(taskController));
    }

    private IEnumerator InspectionRoutine(WorkstationTaskController taskController)
    {
        WorkstationObject workstation = taskController.workstation;

        if (workstation == null || workstation.seatedStaff == null)
        {
            Debug.LogError("InspectionManager: 검사할 인력 또는 Workstation이 없습니다.");
            yield break;
        }

        StaffWorker staff = workstation.seatedStaff;

        taskController.HideSeatedVisualForInspection();

        SpawnProfessor();
        SpawnInspectionStaff(staff);

        yield return new WaitForSeconds(inspectionDuration);

        ClearInspectionObjects();

        taskController.RestoreSeatedVisualAfterInspection();

        taskController.CompleteInspectionAfterSequence();
    }

    private void SpawnProfessor()
    {
        if (professorInspectionPrefab == null || professorSpawnPoint == null)
        {
            Debug.LogWarning("InspectionManager: 교수 프리팹 또는 교수 스폰 위치가 없습니다.");
            return;
        }

        currentProfessor = Instantiate(
            professorInspectionPrefab,
            professorSpawnPoint.position,
            Quaternion.identity
        );
    }

    private void SpawnInspectionStaff(StaffWorker staff)
    {
        if (staff == null)
        {
            return;
        }

        if (staffInspectionPoint == null)
        {
            Debug.LogWarning("InspectionManager: staff 검사 위치가 없습니다.");
            return;
        }

        GameObject prefab = GetInspectionStaffPrefab(staff.staffType);

        if (prefab == null)
        {
            Debug.LogError("InspectionManager: " + staff.staffType + "에 맞는 검사 staff 프리팹이 없습니다.");
            return;
        }

        currentStaffInspection = Instantiate(
            prefab,
            staffInspectionPoint.position,
            Quaternion.identity
        );
    }

    private GameObject GetInspectionStaffPrefab(StaffType staffType)
    {
        string typeName = staffType.ToString();

        if (typeName == "Undergraduate" || typeName == "Bachelor" || typeName == "Student" || typeName == "학사생")
        {
            return undergraduateInspectionPrefab;
        }

        if (typeName == "Master" || typeName == "Graduate" || typeName == "Masters" || typeName == "석사생")
        {
            return masterInspectionPrefab;
        }

        if (typeName == "PhD" || typeName == "Doctor" || typeName == "Doctoral" || typeName == "박사생")
        {
            return phdInspectionPrefab;
        }

        Debug.LogError("알 수 없는 StaffType입니다: " + typeName);
        return null;
    }

    private void ClearInspectionObjects()
    {
        if (currentProfessor != null)
        {
            Destroy(currentProfessor);
            currentProfessor = null;
        }

        if (currentStaffInspection != null)
        {
            Destroy(currentStaffInspection);
            currentStaffInspection = null;
        }
    }
}