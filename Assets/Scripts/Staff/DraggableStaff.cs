using UnityEngine;

/// <summary>
/// 인력 캐릭터를 마우스로 드래그할 수 있게 하는 스크립트.
/// 
/// 사용 흐름:
/// 1. 학사생 오브젝트를 마우스로 클릭
/// 2. 드래그해서 Workstation 근처로 이동
/// 3. 마우스를 놓으면 가장 가까운 Workstation을 찾음
/// 4. Workstation이 비어 있으면 착석
/// 5. 착석 성공 시 서 있는 인력 오브젝트는 비활성화
/// 6. Workstation의 Chair 이미지가 인력이 앉은 이미지로 변경
/// 
/// 주의:
/// 이 스크립트가 작동하려면 인력 오브젝트에 Collider2D가 있어야 한다.
/// Collider2D는 마우스 클릭 판정을 받기 위한 충돌 영역이다.
/// </summary>
public class DraggableStaff : MonoBehaviour
{
    [Header("드래그 설정")]
    [Tooltip("Workstation에 앉힐 수 있는 최대 거리입니다. 값이 클수록 멀리 떨어져 있어도 앉습니다.")]
    public float seatDetectDistance = 1.2f;

    [Tooltip("드래그 중 캐릭터가 다른 오브젝트보다 앞에 보이도록 할 Sorting Order입니다.")]
    public int draggingSortingOrder = 50;

    [Tooltip("평소 캐릭터 Sorting Order입니다.")]
    public int normalSortingOrder = 10;

    private Camera mainCamera;
    private bool isDragging = false;

    private Vector3 originalPosition;
    private Vector3 mouseOffset;

    private StaffWorker staffWorker;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        mainCamera = Camera.main;

        staffWorker = GetComponent<StaffWorker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 마우스로 캐릭터를 클릭했을 때 호출된다.
    /// 
    /// OnMouseDown이 작동하려면 이 오브젝트에 Collider2D가 필요하다.
    /// 예: BoxCollider2D, CircleCollider2D
    /// </summary>
    private void OnMouseDown()
    {
        if (staffWorker == null)
        {
            return;
        }

        // 이미 Workstation에 앉아 있는 인력은 드래그하지 않는다.
        if (staffWorker.isSeated)
        {
            return;
        }

        isDragging = true;

        // 드래그 실패 시 원래 자리로 돌아가기 위해 현재 위치를 저장한다.
        originalPosition = transform.position;

        // 마우스와 캐릭터 중심 사이의 거리 차이를 저장한다.
        // 이걸 안 하면 클릭 순간 캐릭터 중심이 마우스 위치로 순간이동할 수 있다.
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        mouseOffset = transform.position - mouseWorldPosition;

        // 드래그 중에는 캐릭터가 앞에 보이도록 정렬 순서를 올린다.
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = draggingSortingOrder;
        }
    }

    /// <summary>
    /// 마우스를 누른 채 이동할 때 호출된다.
    /// 캐릭터 위치를 마우스 위치로 따라가게 만든다.
    /// </summary>
    private void OnMouseDrag()
    {
        if (isDragging == false)
        {
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        transform.position = mouseWorldPosition + mouseOffset;
    }

    /// <summary>
    /// 마우스를 놓았을 때 호출된다.
    /// 가장 가까운 Workstation을 찾아 착석을 시도한다.
    /// </summary>
    private void OnMouseUp()
    {
        if (isDragging == false)
        {
            return;
        }

        isDragging = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = normalSortingOrder;
        }

        TrySeatToNearestWorkstation();
    }

    /// <summary>
    /// 가장 가까운 Workstation을 찾아 인력을 앉힌다.
    /// 
    /// 성공:
    /// - Workstation.SeatStaff() 호출
    /// - 인력 오브젝트는 비활성화됨
    /// 
    /// 실패:
    /// - 원래 위치로 돌아감
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
            Debug.Log("이미 인력이 앉아 있는 자리입니다.");
            ReturnToOriginalPosition();
            return;
        }

        // Workstation에 인력 착석 요청.
        // 이 함수 내부에서 의자 이미지가 "인력 앉은 의자 이미지"로 바뀐다.
        nearestWorkstation.SeatStaff(staffWorker, staffWorker.staffType);

        // StaffWorker 쪽에도 현재 어떤 Workstation에 앉았는지 저장한다.
        staffWorker.SetSeated(nearestWorkstation);
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
    /// 착석 실패 시 원래 위치로 되돌린다.
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
    }

    /// <summary>
    /// 마우스 화면 좌표를 Unity 월드 좌표로 변환한다.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 카메라가 Z -10, 오브젝트가 Z 0에 있으므로
        // 카메라에서 Z 0 평면까지의 거리를 넣는다.
        mouseScreenPosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }
}