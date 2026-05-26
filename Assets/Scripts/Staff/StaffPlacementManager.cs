using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 인력 배치 상태를 관리한다.
/// 
/// 1차 목표:
/// - 고용 버튼 클릭 후 인력 프리뷰가 마우스를 따라다님
/// - 컴퓨터 장비가 설치된 빈 Workstation을 클릭하면 고용 확정
/// </summary>
public class StaffPlacementManager : MonoBehaviour
{
    public static StaffPlacementManager Instance;

    [Header("인력 프리뷰 설정")]
    public float previewAlpha = 0.55f;
    public int previewSortingOrder = 32000;
    public float previewZPosition = -5f;
    private bool isHirePlacementMode;
    private StaffHireData pendingHireData;
    private GameObject previewStaffObject;
    private StaffWorker previewStaffWorker;

    [Header("인력 자리 이동/교환 상태")]
    private bool isSwapPlacementMode;
    private StaffWorker movingStaff;
    private WorkstationObject originalWorkstation;
    private GameObject swapPreviewObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (isHirePlacementMode)
        {
            UpdatePreviewPosition();
            HandleInput();
            return;
        }

        if (isSwapPlacementMode)
        {
            UpdateSwapPreviewPosition();
            HandleSwapInput();
            return;
        }
    }

    /// <summary>
    /// 고용 가능한 Workstation이 하나라도 있는지 검사한다.
    /// </summary>
    public bool HasAvailableWorkstationForHire()
    {
        WorkstationObject[] workstations = FindObjectsByType<WorkstationObject>(FindObjectsSortMode.None);

        for (int i = 0; i < workstations.Length; i++)
        {
            if (workstations[i] != null && workstations[i].CanSeatNewStaff())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 연구생 고용 배치 모드를 시작한다.
    /// </summary>
    public void StartHirePlacementMode(StaffHireData hireData)
    {
        if (hireData == null)
        {
            return;
        }

        CancelPlacementMode(false);

        pendingHireData = hireData;
        isHirePlacementMode = true;

        CreatePreviewStaff(hireData);

        Debug.Log("인력 배치 상태 진입: " + hireData.staffName);
    }

    private void CreatePreviewStaff(StaffHireData hireData)
    {
        GameObject prefab = Resources.Load<GameObject>(hireData.prefabResourcePath);

        if (prefab == null)
        {
            Debug.LogError("인력 프리뷰 prefab을 찾지 못했습니다: " + hireData.prefabResourcePath);
            CancelPlacementMode();
            return;
        }

        previewStaffObject = Instantiate(prefab);
        previewStaffObject.name = "Preview_" + hireData.staffName;

        previewStaffWorker = previewStaffObject.GetComponent<StaffWorker>();

        DisablePreviewInteraction(previewStaffObject);
        SetPreviewAlpha(previewStaffObject, previewAlpha);
        SetPreviewRenderPriority(previewStaffObject);
    }

    private void UpdatePreviewPosition()
    {
        if (previewStaffObject == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = previewZPosition;

        previewStaffObject.transform.position = mouseWorldPosition;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacementMode();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceStaffOnWorkstation();
        }
    }

    private void TryPlaceStaffOnWorkstation()
    {
        WorkstationObject targetWorkstation = FindWorkstationUnderMouse();

        if (targetWorkstation == null)
        {
            Debug.Log("인력 배치 실패: 선택된 Workstation이 없습니다.");
            return;
        }

        if (targetWorkstation.CanSeatNewStaff() == false)
        {
            Debug.Log("인력 배치 실패: 컴퓨터 장비가 설치된 빈 Workstation이 아닙니다.");
            return;
        }

        if (StaffHireManager.Instance == null)
        {
            Debug.LogError("StaffHireManager.Instance가 없습니다.");
            return;
        }

        bool success = StaffHireManager.Instance.ConfirmHireToWorkstation(targetWorkstation);

        if (success)
        {
            CancelPlacementMode(false);
        }
    }

    private WorkstationObject FindWorkstationUnderMouse()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return null;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

        if (hit.collider == null)
        {
            return null;
        }

        WorkstationObject workstation = hit.collider.GetComponentInParent<WorkstationObject>();

        return workstation;
    }

    public void CancelPlacementMode(bool clearPendingHire = true)
    {
        if (previewStaffObject != null)
        {
            Destroy(previewStaffObject);
            previewStaffObject = null;
        }

        previewStaffWorker = null;
        pendingHireData = null;
        isHirePlacementMode = false;

        if (clearPendingHire && StaffHireManager.Instance != null)
        {
            StaffHireManager.Instance.CancelPendingHire();
        }

        Debug.Log("인력 배치 상태 종료");
    }

    private void DisablePreviewInteraction(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(true);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        MonoBehaviour[] behaviours = target.GetComponentsInChildren<MonoBehaviour>(true);

        for (int i = 0; i < behaviours.Length; i++)
        {
            behaviours[i].enabled = false;
        }
    }

    private void SetPreviewAlpha(GameObject target, float alpha)
    {
        if (target == null)
        {
            return;
        }

        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            Color color = renderers[i].color;
            color.a = alpha;
            renderers[i].color = color;
        }
    }

    /// <summary>
    /// 앉아 있는 인력을 꾹 클릭했을 때 호출한다.
    /// 해당 인력을 마우스로 들고 다니는 자리 이동/교환 모드로 들어간다.
    /// </summary>
    public void StartSwapPlacementMode(StaffWorker staff)
    {
        if (staff == null)
        {
            Debug.LogError("교환 모드 시작 실패: staff가 null입니다.");
            return;
        }

        if (staff.currentWorkstation == null)
        {
            Debug.Log("교환 모드 시작 실패: 현재 Workstation에 앉아 있지 않은 인력입니다.");
            return;
        }

        CancelPlacementMode();

        movingStaff = staff;
        originalWorkstation = staff.currentWorkstation;
        isSwapPlacementMode = true;

        CreateSwapPreview(staff);

        // 원래 자리에서는 인력이 빠진 것처럼 빈 의자로 보여준다.
        originalWorkstation.ClearSeatedStaffForSwap();

        Debug.Log("인력 교환 모드 시작: " + staff.staffName);
    }

    private void CreateSwapPreview(StaffWorker staff)
    {
        if (staff == null)
        {
            return;
        }

        swapPreviewObject = Instantiate(staff.gameObject);
        swapPreviewObject.name = "SwapPreview_" + staff.staffName;
        swapPreviewObject.SetActive(true);

        DisablePreviewInteraction(swapPreviewObject);
        SetPreviewAlpha(swapPreviewObject, previewAlpha);
        SetPreviewRenderPriority(swapPreviewObject);
    }

    private void UpdateSwapPreviewPosition()
    {
        if (swapPreviewObject == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = previewZPosition;

        swapPreviewObject.transform.position = mouseWorldPosition;
    }

    private void HandleSwapInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSwapPlacementMode(true);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceSwappingStaff();
        }
    }

    private void TryPlaceSwappingStaff()
    {
        WorkstationObject targetWorkstation = FindWorkstationUnderMouse();

        if (targetWorkstation == null)
        {
            Debug.Log("자리 이동 실패: 선택된 Workstation이 없습니다.");
            return;
        }

        if (targetWorkstation.HasDeskEquipment() == false)
        {
            Debug.Log("자리 이동 실패: 컴퓨터 장비가 설치된 Workstation이 아닙니다.");
            return;
        }

        if (movingStaff == null || originalWorkstation == null)
        {
            Debug.LogError("자리 이동 실패: 이동 중인 인력 정보가 없습니다.");
            CancelSwapPlacementMode(true);
            return;
        }

        // 원래 자리를 다시 클릭하면 원위치.
        if (targetWorkstation == originalWorkstation)
        {
            CancelSwapPlacementMode(true);
            return;
        }

        StaffWorker targetStaff = targetWorkstation.seatedStaff;

        // 대상 자리가 비어 있으면 단순 이동.
        if (targetStaff == null)
        {
            targetWorkstation.SetSeatedStaffForSwap(movingStaff);

            Debug.Log("인력 자리 이동 완료: " + movingStaff.staffName);

            EndSwapPlacementMode();
            return;
        }

        // 대상 자리에 다른 인력이 있으면 서로 자리 교환.
        originalWorkstation.SetSeatedStaffForSwap(targetStaff);
        targetWorkstation.SetSeatedStaffForSwap(movingStaff);

        Debug.Log("인력 자리 교환 완료: " + movingStaff.staffName + " ↔ " + targetStaff.staffName);

        EndSwapPlacementMode();
    }

    private void EndSwapPlacementMode()
    {
        if (swapPreviewObject != null)
        {
            Destroy(swapPreviewObject);
            swapPreviewObject = null;
        }

        movingStaff = null;
        originalWorkstation = null;
        isSwapPlacementMode = false;
    }

    private void CancelSwapPlacementMode(bool restoreOriginalSeat)
    {
        if (restoreOriginalSeat && movingStaff != null && originalWorkstation != null)
        {
            originalWorkstation.SetSeatedStaffForSwap(movingStaff);
        }

        if (swapPreviewObject != null)
        {
            Destroy(swapPreviewObject);
            swapPreviewObject = null;
        }

        movingStaff = null;
        originalWorkstation = null;
        isSwapPlacementMode = false;

        Debug.Log("인력 교환 모드 취소");
    }

    private void SetPreviewRenderPriority(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        // 프리뷰 루트에 SortingGroup을 강제로 붙인다.
        // 프리팹 내부에 SortingGroup이 있으면 자식 SpriteRenderer의 sortingOrder만 바꿔도
        // 전체 정렬이 안 바뀌는 경우가 있어서 루트 그룹을 직접 제어한다.
        SortingGroup rootSortingGroup = target.GetComponent<SortingGroup>();

        if (rootSortingGroup == null)
        {
            rootSortingGroup = target.AddComponent<SortingGroup>();
        }

        int highestSortingLayerID = GetHighestSortingLayerIDInScene(target);

        rootSortingGroup.sortingLayerID = highestSortingLayerID;
        rootSortingGroup.sortingOrder = previewSortingOrder;

        SortingGroup[] sortingGroups = target.GetComponentsInChildren<SortingGroup>(true);

        for (int i = 0; i < sortingGroups.Length; i++)
        {
            sortingGroups[i].sortingLayerID = highestSortingLayerID;
            sortingGroups[i].sortingOrder = previewSortingOrder;
        }

        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingLayerID = highestSortingLayerID;
            renderers[i].sortingOrder = previewSortingOrder;
        }

        Vector3 position = target.transform.position;
        position.z = previewZPosition;
        target.transform.position = position;
    }

    private int GetHighestSortingLayerIDInScene(GameObject excludedTarget)
    {
        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

        int highestLayerID = SortingLayer.NameToID("Default");
        int highestLayerValue = SortingLayer.GetLayerValueFromID(highestLayerID);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
            {
                continue;
            }

            if (excludedTarget != null && renderers[i].transform.IsChildOf(excludedTarget.transform))
            {
                continue;
            }

            int layerValue = SortingLayer.GetLayerValueFromID(renderers[i].sortingLayerID);

            if (layerValue > highestLayerValue)
            {
                highestLayerValue = layerValue;
                highestLayerID = renderers[i].sortingLayerID;
            }
        }

        return highestLayerID;
    }
}