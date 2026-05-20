using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 인력 캐릭터를 마우스로 드래그할 수 있게 하는 스크립트.
/// 
/// 핵심 규칙:
/// 1. 드래그 중에는 인력이 무조건 화면 맨 앞으로 와야 한다.
/// 2. Sorting Layer는 건드리지 않고 Default를 유지한다.
/// 3. 단순 SpriteRenderer.sortingOrder만 바꾸지 않고,
///    SortingGroup.sortingOrder를 함께 바꿔서 오브젝트 전체를 앞으로 올린다.
/// 
/// 왜 SortingGroup을 쓰는가?
/// - SpriteRenderer 하나만 sortingOrder를 바꾸면,
///   자식 렌더러나 복합 오브젝트 구조에서 예상대로 안 보일 수 있다.
/// - SortingGroup은 해당 오브젝트 전체를 하나의 렌더링 묶음으로 만들어서
///   다른 오브젝트보다 앞/뒤에 놓을 수 있게 한다.
/// </summary>
public class DraggableStaff : MonoBehaviour
{
    [Header("착석 판정")]
    [Tooltip("Workstation에 앉힐 수 있는 최대 거리입니다. 값이 클수록 멀리 떨어져 있어도 착석됩니다.")]
    public float seatDetectDistance = 1.2f;

    [Header("정렬 설정")]
    [Tooltip("드래그 중에는 정렬 규칙을 무시하고 이 값으로 올립니다. Default Sorting Layer 안에서 매우 앞쪽 값입니다.")]
    public int draggingSortingOrder = 30000;

    [Tooltip("드래그 실패 후 복귀할 기본 Sorting Order입니다.")]
    public int normalSortingOrder = 10;

    private Camera mainCamera;

    private bool isDragging = false;

    // 드래그 실패 시 돌아갈 위치
    private Vector3 originalPosition;

    // 마우스 클릭 지점과 캐릭터 중심 사이의 거리 차이
    private Vector3 mouseOffset;

    private StaffWorker staffWorker;

    // 인력 오브젝트와 자식 오브젝트의 모든 SpriteRenderer
    private SpriteRenderer[] spriteRenderers;

    // 각 SpriteRenderer의 원래 Order in Layer 저장용
    private int[] originalSortingOrders;

    // 인력 전체를 하나의 정렬 묶음으로 다루기 위한 컴포넌트
    private SortingGroup sortingGroup;

    // 드래그 전 SortingGroup의 원래 Order 저장
    private int originalSortingGroupOrder;

    private void Awake()
    {
        mainCamera = Camera.main;

        staffWorker = GetComponent<StaffWorker>();

        // 이 오브젝트와 자식 오브젝트의 모든 SpriteRenderer를 가져온다.
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        originalSortingOrders = new int[spriteRenderers.Length];

        // SortingGroup이 없으면 자동으로 붙인다.
        // SortingGroup은 여러 SpriteRenderer를 하나의 정렬 단위로 묶어준다.
        sortingGroup = GetComponent<SortingGroup>();

        if (sortingGroup == null)
        {
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        }

        // 처음 기본값 저장
        if (sortingGroup != null)
        {
            normalSortingOrder = sortingGroup.sortingOrder;
        }
        else if (spriteRenderers.Length > 0 && spriteRenderers[0] != null)
        {
            normalSortingOrder = spriteRenderers[0].sortingOrder;
        }
    }

    /// <summary>
    /// 마우스로 인력을 클릭했을 때 호출된다.
    /// 
    /// OnMouseDown이 작동하려면 이 오브젝트에 BoxCollider2D 같은 Collider2D가 있어야 한다.
    /// </summary>
    private void OnMouseDown()
    {
        if (staffWorker == null)
        {
            Debug.LogError("DraggableStaff: StaffWorker가 없습니다.");
            return;
        }

        // 이미 Workstation에 앉아 있는 인력은 드래그하지 않는다.
        if (staffWorker.isSeated)
        {
            return;
        }

        isDragging = true;

        // 착석 실패 시 되돌아갈 위치 저장
        originalPosition = transform.position;

        // 마우스와 인력 중심 사이의 차이 저장
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        mouseOffset = transform.position - mouseWorldPosition;

        // 드래그 전 정렬값 저장
        SaveOriginalSortingOrders();

        // 드래그 시작 즉시 맨 앞으로 올림
        ForceDraggingSortingOrder();
    }

    /// <summary>
    /// 마우스를 누른 채 움직일 때 호출된다.
    /// </summary>
    private void OnMouseDrag()
    {
        if (isDragging == false)
        {
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        transform.position = mouseWorldPosition + mouseOffset;

        // 드래그 중에는 계속 최상단 유지
        ForceDraggingSortingOrder();
    }

    /// <summary>
    /// 다른 스크립트가 Update 이후 정렬값을 바꿔도,
    /// 프레임 마지막에 다시 최상단으로 고정한다.
    /// </summary>
    private void LateUpdate()
    {
        if (isDragging)
        {
            ForceDraggingSortingOrder();
        }
    }

    /// <summary>
    /// 마우스를 놓았을 때 호출된다.
    /// Workstation 착석을 시도한다.
    /// </summary>
    private void OnMouseUp()
    {
        if (isDragging == false)
        {
            return;
        }

        isDragging = false;

        TrySeatToNearestWorkstation();
    }

    /// <summary>
    /// 가장 가까운 Workstation을 찾아 착석을 시도한다.
    /// </summary>
    private void TrySeatToNearestWorkstation()
    {
        WorkstationObject nearestWorkstation = FindNearestWorkstation();

        if (nearestWorkstation == null)
        {
            ReturnToOriginalPosition();
            return;
        }

        float distance = Vector3.Distance(transform.position, nearestWorkstation.transform.position);

        if (distance > seatDetectDistance)
        {
            ReturnToOriginalPosition();
            return;
        }

        if (nearestWorkstation.CanSeatStaff() == false)
        {
            Debug.Log("이미 인력이 앉아 있거나 착석할 수 없는 자리입니다.");
            ReturnToOriginalPosition();
            return;
        }

        // 착석 성공 전 정렬값을 원래대로 복구한다.
        // 이후 오브젝트는 비활성화되지만,
        // 나중에 자리에서 일어날 때 이상한 정렬값으로 살아나는 것을 막기 위함이다.
        RestoreOriginalSortingOrders();

        // Workstation에 인력 착석 처리.
        // 이 함수 안에서:
        // - seatedStaff 저장
        // - hasStaff true 변경
        // - 빈 의자 Sprite를 인력 앉은 의자 Sprite로 변경
        // - 서 있는 인력 오브젝트 비활성화
        nearestWorkstation.SeatStaff(staffWorker, staffWorker.staffType);

        // StaffWorker 쪽에도 현재 앉은 Workstation 기록
        staffWorker.SetSeated(nearestWorkstation);
    }

    /// <summary>
    /// 착석 실패 시 원래 위치와 정렬 순서로 복귀한다.
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;

        RestoreOriginalSortingOrders();
    }

    /// <summary>
    /// 현재 씬에 있는 Workstation 중 가장 가까운 것을 찾는다.
    /// </summary>
    private WorkstationObject FindNearestWorkstation()
    {
        WorkstationObject[] workstations = FindObjectsByType<WorkstationObject>(FindObjectsSortMode.None);

        WorkstationObject nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (WorkstationObject workstation in workstations)
        {
            if (workstation == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, workstation.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = workstation;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 드래그 시작 전 인력의 모든 SpriteRenderer와 SortingGroup 정렬값을 저장한다.
    /// </summary>
    private void SaveOriginalSortingOrders()
    {
        if (sortingGroup != null)
        {
            originalSortingGroupOrder = sortingGroup.sortingOrder;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            originalSortingOrders[i] = spriteRenderers[i].sortingOrder;
        }
    }

    /// <summary>
    /// 드래그 중 인력을 최상단으로 올린다.
    /// 
    /// Sorting Layer는 그대로 Default를 사용한다.
    /// SortingGroup과 SpriteRenderer의 Order in Layer를 둘 다 올린다.
    /// </summary>
    private void ForceDraggingSortingOrder()
    {
        int safeOrder = Mathf.Clamp(draggingSortingOrder, -30000, 30000);

        // 핵심:
        // SortingGroup이 있으면 외부 오브젝트와의 정렬은 SortingGroup 기준으로 처리된다.
        // 그래서 SortingGroup의 sortingOrder를 올려야 진짜로 앞으로 온다.
        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = safeOrder;
        }

        // 자식 SpriteRenderer들도 같이 올려둔다.
        // SortingGroup 내부 정렬과 예외 상황을 모두 대비하기 위함이다.
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            spriteRenderers[i].sortingOrder = safeOrder;
        }
    }

    /// <summary>
    /// 착석 실패 또는 착석 성공 직전 드래그 전 정렬값으로 되돌린다.
    /// </summary>
    private void RestoreOriginalSortingOrders()
    {
        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = originalSortingGroupOrder;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            spriteRenderers[i].sortingOrder = originalSortingOrders[i];
        }
    }

    /// <summary>
    /// 마우스 화면 좌표를 Unity 월드 좌표로 변환한다.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2D 카메라는 보통 Z = -10,
        // 오브젝트들은 Z = 0에 있으므로 카메라에서 Z=0까지의 거리를 넣는다.
        mouseScreenPosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }
}