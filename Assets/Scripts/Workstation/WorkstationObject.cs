using UnityEngine;

/// <summary>
/// 책상+의자+책상 위 장비+앉은 인력을 하나로 관리하는 복합 오브젝트.
/// 
/// Workstation은 게임에서 "작업 가능한 자리"를 의미한다.
/// 
/// 포함 요소:
/// - 책상
/// - 의자 또는 인력이 앉은 의자 이미지
/// - 책상 위 장비 슬롯
/// - 착석한 인력 데이터
/// 
/// 중요 규칙:
/// 1. 책상과 의자는 항상 세트로 배치된다.
/// 2. 책상+의자는 1평을 차지한다.
/// 3. 노트북/컴퓨터는 이 Workstation의 EquipmentSlot에만 설치된다.
/// 4. 인력이 앉으면 빈 의자 Sprite가 "인력+의자 합성 Sprite"로 바뀐다.
/// 5. 그래도 게임 로직상 seatedStaff를 저장해서 어떤 인력이 앉았는지 추적한다.
/// 6. 인력이 앉아 있으면 수정 모드에서 이동/회전 금지다.
/// </summary>
public class WorkstationObject : MonoBehaviour
{
    [Header("현재 방향")]
    [Tooltip("현재 Workstation 방향입니다.")]
    public PlacementDirection currentDirection = PlacementDirection.RD;

    [Header("방향별 설정")]
    [Tooltip("RD/RU/LD/LU 방향별 책상, 의자, 위치, 정렬 정보를 저장합니다.")]
    public WorkstationDirectionSetting[] directionSettings;

    [Header("SpriteRenderer 연결")]
    [Tooltip("책상 이미지를 표시하는 SpriteRenderer입니다.")]
    public SpriteRenderer deskRenderer;

    [Tooltip("빈 의자 또는 인력이 앉은 의자 이미지를 표시하는 SpriteRenderer입니다.")]
    public SpriteRenderer chairRenderer;

    [Tooltip("책상 위 장비를 표시하는 SpriteRenderer입니다. 아직 장비 시스템 전이면 비워도 됩니다.")]
    public SpriteRenderer deskEquipmentRenderer;

    [Header("위치 기준점")]
    [Tooltip("노트북/컴퓨터가 올라갈 위치입니다.")]
    public Transform equipmentSlot;

    [Tooltip("인력 착석 기준 위치입니다.")]
    public Transform seatPoint;

    [Header("배치 상태")]
    [Tooltip("이 Workstation이 배치된 타일입니다.")]
    public LabTile placedTile;

    [Header("책상 위 장비 상태")]
    [Tooltip("책상 위에 장비가 설치되어 있는지 여부입니다.")]
    public bool hasDeskEquipment = false;

    [Tooltip("설치된 책상 위 장비 오브젝트입니다. 나중에 DeskEquipmentObject를 만들면 연결합니다.")]
    public GameObject installedDeskEquipmentObject;

    [Header("책상 위 장비")]
    [Tooltip("책상 위 장비를 관리하는 DeskEquipmentObject입니다.")]
    public DeskEquipmentObject deskEquipmentObject;

    [Header("착석 인력 상태")]
    [Tooltip("인력이 앉아 있는지 여부입니다.")]
    public bool hasStaff = false;

    [Tooltip("이 Workstation에 앉아 있는 인력의 실제 데이터입니다.")]
    public StaffWorker seatedStaff;

    [Tooltip("앉아 있는 인력 종류입니다.")]
    public StaffType seatedStaffType;

    [Header("타일 기준 정렬")]
    [Tooltip("현재 배치된 타일 기준 Sorting Order입니다. 배치된 타일에 따라 자동 계산됩니다.")]
    public int tileBaseSortingOrder = 0;

    private void Awake()
    {
        AutoFindReferencesIfNeeded();
        ApplyDirection(currentDirection);
    }

    private void Start()
    {
        ApplyDirection(currentDirection);
    }

    /// <summary>
    /// 에디터에서 값이 바뀔 때 자동 실행된다.
    /// 
    /// Sprite나 방향 설정을 바꿨을 때
    /// Play를 누르지 않아도 프리팹 화면에 바로 반영되게 하기 위해 사용한다.
    /// </summary>
    private void OnValidate()
    {
        AutoFindReferencesIfNeeded();
        ApplyDirection(currentDirection);
    }

    /// <summary>
    /// 연결이 비어 있으면 자식 오브젝트 이름을 기준으로 자동 연결한다.
    /// 
    /// 권장 프리팹 구조:
    /// Workstation
    /// ├── Desk
    /// ├── Chair
    /// ├── DeskEquipment
    /// ├── EquipmentSlot
    /// └── SeatPoint
    /// </summary>
    private void AutoFindReferencesIfNeeded()
    {
        if (deskRenderer == null)
        {
            Transform deskTransform = transform.Find("Desk");

            if (deskTransform != null)
            {
                deskRenderer = deskTransform.GetComponent<SpriteRenderer>();
            }
        }

        if (chairRenderer == null)
        {
            Transform chairTransform = transform.Find("Chair");

            if (chairTransform != null)
            {
                chairRenderer = chairTransform.GetComponent<SpriteRenderer>();
            }
        }

        if (deskEquipmentRenderer == null)
        {
            Transform equipmentTransform = transform.Find("DeskEquipment");

            if (equipmentTransform != null)
            {
                deskEquipmentRenderer = equipmentTransform.GetComponent<SpriteRenderer>();
            }
        }

        if (equipmentSlot == null)
        {
            Transform slotTransform = transform.Find("EquipmentSlot");

            if (slotTransform != null)
            {
                equipmentSlot = slotTransform;
            }
        }

        if (seatPoint == null)
        {
            Transform seatTransform = transform.Find("SeatPoint");

            if (seatTransform != null)
            {
                seatPoint = seatTransform;
            }
        }

        if (deskEquipmentObject == null)
        {
            Transform equipmentTransform = transform.Find("DeskEquipment");

            if (equipmentTransform != null)
            {
                deskEquipmentObject = equipmentTransform.GetComponent<DeskEquipmentObject>();
            }
        }
    }

    /// <summary>
    /// 현재 방향에 맞는 설정을 적용한다.
    /// 
    /// 적용 내용:
    /// - 책상 Sprite
    /// - 빈 의자 또는 앉은 인력 Sprite
    /// - 책상 위치
    /// - 의자 위치
    /// - 장비 슬롯 위치
    /// - 착석 위치
    /// - 정렬 순서
    /// - 책상 위 장비 방향 갱신
    /// </summary>
    public void ApplyDirection(PlacementDirection direction)
    {
        currentDirection = direction;

        WorkstationDirectionSetting setting = GetDirectionSetting(direction);

        if (setting == null)
        {
            return;
        }

        ApplyDesk(setting);
        ApplyChairOrSeatedStaff(setting);
        ApplyLocalPositions(setting);
        ApplySortingOrders(setting);
        ApplyDeskEquipment(setting);
    }

    /// <summary>
    /// 방향에 맞는 책상 Sprite를 적용한다.
    /// </summary>
    private void ApplyDesk(WorkstationDirectionSetting setting)
    {
        if (deskRenderer == null)
        {
            return;
        }

        deskRenderer.sprite = setting.deskSprite;
    }

    /// <summary>
    /// 인력이 없으면 빈 의자 Sprite를 보여주고,
    /// 인력이 앉아 있으면 해당 인력 종류에 맞는 앉은 의자 Sprite를 보여준다.
    /// </summary>
    private void ApplyChairOrSeatedStaff(WorkstationDirectionSetting setting)
    {
        if (chairRenderer == null)
        {
            return;
        }

        if (hasStaff)
        {
            chairRenderer.sprite = setting.GetSeatedChairSprite(seatedStaffType);
        }
        else
        {
            chairRenderer.sprite = setting.emptyChairSprite;
        }
    }

    /// <summary>
    /// 방향별 위치값을 자식 오브젝트에 적용한다.
    /// 
    /// 이걸 쓰면 RD/RU/LD/LU마다 책상과 의자 위치를 따로 저장할 수 있다.
    /// </summary>
    private void ApplyLocalPositions(WorkstationDirectionSetting setting)
    {
        if (deskRenderer != null)
        {
            deskRenderer.transform.localPosition = setting.deskLocalPosition;
        }

        if (chairRenderer != null)
        {
            chairRenderer.transform.localPosition = setting.chairLocalPosition;
        }

        if (equipmentSlot != null)
        {
            equipmentSlot.localPosition = setting.equipmentSlotLocalPosition;
        }

        if (seatPoint != null)
        {
            seatPoint.localPosition = setting.seatPointLocalPosition;
        }

        if (deskEquipmentRenderer != null)
        {
            deskEquipmentRenderer.transform.localPosition = setting.equipmentSlotLocalPosition;
        }
    }

    /// <summary>
    /// 방향별 앞뒤 순서를 적용한다.
    /// 
    /// 최종값 = 타일 기준 정렬값 + 방향별 내부 정렬값
    /// </summary>
    private void ApplySortingOrders(WorkstationDirectionSetting setting)
    {
        if (deskRenderer != null)
        {
            deskRenderer.sortingOrder = GetFinalSortingOrder(setting.deskSortingOrder);
        }

        if (chairRenderer != null)
        {
            chairRenderer.sortingOrder = GetFinalSortingOrder(setting.chairSortingOrder);
        }

        if (deskEquipmentObject != null && deskEquipmentObject.spriteRenderer != null)
        {
            deskEquipmentObject.spriteRenderer.sortingOrder =
                GetFinalSortingOrder(setting.deskEquipmentSortingOrder);
        }
    }

    /// <summary>
    /// 최종 Sorting Order를 계산한다.
    /// 
    /// tileBaseSortingOrder:
    /// - 이 Workstation이 놓인 타일 위치 기준 앞뒤 순서
    /// 
    /// localSortingOrder:
    /// - 같은 Workstation 안에서 책상/의자/장비 중 무엇이 앞인지 정하는 보정값
    /// </summary>
    private int GetFinalSortingOrder(int localSortingOrder)
    {
        int finalOrder = tileBaseSortingOrder + localSortingOrder;

        // Unity sortingOrder에 너무 큰 값을 넣으면 값이 꼬일 수 있으므로 안전 범위로 제한한다.
        return Mathf.Clamp(finalOrder, -30000, 30000);
    }

    /// <summary>
    /// 현재 방향을 다음 방향으로 회전한다.
    /// 
    /// 인력이 앉아 있으면 수정 모드에서 회전 금지 요구사항 때문에 회전하지 않는다.
    /// </summary>
    public void RotateToNextDirection()
    {
        if (CanModify() == false)
        {
            Debug.Log("인력이 앉아 있는 Workstation은 회전할 수 없습니다.");
            return;
        }

        PlacementDirection nextDirection = GetNextDirection(currentDirection);
        ApplyDirection(nextDirection);
    }

    /// <summary>
    /// 인력이 앉아 있으면 이동/회전 금지.
    /// </summary>
    public bool CanModify()
    {
        return hasStaff == false;
    }

    /// <summary>
    /// 인력을 Workstation에 앉힌다.
    /// 
    /// 화면상으로는 인력 오브젝트가 사라지고,
    /// Chair Sprite가 '인력+의자 합성 이미지'로 바뀐다.
    /// 
    /// 하지만 seatedStaff에 실제 인력 데이터를 저장해서
    /// 나중에 클릭 시 정보카드가 뜰 수 있게 한다.
    /// </summary>
    public void SeatStaff(StaffWorker staff, StaffType staffType)
    {
        if (staff == null)
        {
            Debug.LogError("SeatStaff: staff가 null입니다.");
            return;
        }

        if (hasStaff)
        {
            Debug.Log("이미 인력이 앉아 있습니다.");
            return;
        }

        seatedStaff = staff;
        seatedStaffType = staffType;
        hasStaff = true;

        // 서 있던 인력 오브젝트는 화면에서 숨긴다.
        // 데이터는 seatedStaff에 남아 있으므로 삭제하지 않는다.
        staff.gameObject.SetActive(false);

        ApplyDirection(currentDirection);
    }

    /// <summary>
    /// 인력을 자리에서 빼낸다.
    /// 
    /// 나중에 휴식/이동 시스템에서 사용할 수 있다.
    /// </summary>
    public void UnseatStaff()
    {
        if (seatedStaff != null)
        {
            seatedStaff.gameObject.SetActive(true);
            seatedStaff.transform.position = GetSeatPosition();
        }

        seatedStaff = null;
        hasStaff = false;

        ApplyDirection(currentDirection);
    }

    /// <summary>
    /// 특정 방향에 해당하는 설정값을 찾는다.
    /// </summary>
    private WorkstationDirectionSetting GetDirectionSetting(PlacementDirection direction)
    {
        if (directionSettings == null)
        {
            return null;
        }

        foreach (WorkstationDirectionSetting setting in directionSettings)
        {
            if (setting != null && setting.direction == direction)
            {
                return setting;
            }
        }

        return null;
    }

    /// <summary>
    /// 다음 방향 반환.
    /// </summary>
    private PlacementDirection GetNextDirection(PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.RD:
                return PlacementDirection.RU;

            case PlacementDirection.RU:
                return PlacementDirection.LU;

            case PlacementDirection.LU:
                return PlacementDirection.LD;

            case PlacementDirection.LD:
                return PlacementDirection.RD;

            default:
                return PlacementDirection.RD;
        }
    }

    /// <summary>
    /// 이 Workstation이 특정 타일에 배치 완료되었을 때 호출한다.
    /// 
    /// 이 함수는 단순히 placedTile만 저장하는 게 아니라,
    /// 해당 타일 좌표 기준으로 앞뒤 정렬 기준값도 계산한다.
    /// </summary>
    public void SetPlacedTile(LabTile tile)
    {
        placedTile = tile;

        if (LabGridManager.Instance != null && tile != null)
        {
            tileBaseSortingOrder = LabGridManager.Instance.GetSortingOrderByTile(tile);
        }

        // 타일 기준 정렬값이 바뀌었으므로 현재 방향을 다시 적용한다.
        // 이렇게 해야 책상/의자/노트북의 sortingOrder가 새 타일 기준으로 갱신된다.
        ApplyDirection(currentDirection);
    }

    public bool CanInstallDeskEquipment()
    {
        return hasDeskEquipment == false;
    }

    public void SetDeskEquipmentInstalled(bool installed)
    {
        hasDeskEquipment = installed;
    }

    public Vector3 GetEquipmentSlotPosition()
    {
        if (equipmentSlot == null)
        {
            return transform.position;
        }

        return equipmentSlot.position;
    }

    public Vector3 GetSeatPosition()
    {
        if (seatPoint == null)
        {
            return transform.position;
        }

        return seatPoint.position;
    }


    /// <summary>
    /// 책상 위 장비를 설치한다.
    /// 
    /// 노트북/컴퓨터는 타일에 직접 배치되지 않고,
    /// 반드시 이 함수를 통해 Workstation 위에 장착된다.
    /// </summary>
    public bool InstallDeskEquipment(DeskEquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            Debug.LogError("InstallDeskEquipment: equipmentData가 null입니다.");
            return false;
        }

        if (hasDeskEquipment)
        {
            Debug.Log("이미 책상 위에 장비가 설치되어 있습니다.");
            return false;
        }

        if (deskEquipmentObject == null)
        {
            Debug.LogError("InstallDeskEquipment: DeskEquipmentObject가 연결되지 않았습니다.");
            return false;
        }

        hasDeskEquipment = true;

        deskEquipmentObject.InstallEquipment(equipmentData, this);

        ApplyDirection(currentDirection);

        return true;
    }

    /// <summary>
    /// 책상 위 장비를 제거한다.
    /// </summary>
    public void RemoveDeskEquipment()
    {
        if (deskEquipmentObject != null)
        {
            deskEquipmentObject.ClearEquipment();
        }

        hasDeskEquipment = false;

        ApplyDirection(currentDirection);
    }

    /// <summary>
    /// 책상 위 장비가 설치되어 있는지 확인한다.
    /// </summary>
    public bool HasDeskEquipment()
    {
        return hasDeskEquipment;
    }

    /// <summary>
    /// 이 Workstation에 인력을 앉힐 수 있는지 확인한다.
    /// 
    /// 조건:
    /// 1. 아직 인력이 앉아 있지 않아야 한다.
    /// 2. 책상+의자 세트가 정상적으로 존재해야 한다.
    /// 
    /// 나중에 조건을 더 추가할 수 있다.
    /// 예:
    /// - 책상 위에 노트북/컴퓨터가 있어야 과제 가능
    /// - 청소 필요 상태면 착석 불가
    /// - 수정 모드 중이면 착석 불가
    /// </summary>
    public bool CanSeatStaff()
    {
        // 이미 인력이 앉아 있으면 새 인력을 앉힐 수 없다.
        if (hasStaff)
        {
            return false;
        }

        // 기본적으로 비어 있으면 착석 가능.
        return true;
    }

    /// <summary>
    /// 책상 위 장비가 있을 경우, Workstation 방향에 맞게 같이 갱신한다.
    /// 
    /// 주의:
    /// 여기서도 sortingOrder는 반드시 tileBaseSortingOrder를 포함해서 계산해야 한다.
    /// 단순히 setting.deskEquipmentSortingOrder만 넣으면 장비가 책상 뒤로 갈 수 있다.
    /// </summary>
    private void ApplyDeskEquipment(WorkstationDirectionSetting setting)
    {
        if (deskEquipmentObject == null)
        {
            return;
        }

        // 장비 위치를 현재 방향 설정의 EquipmentSlot 위치로 맞춘다.
        deskEquipmentObject.transform.localPosition = setting.equipmentSlotLocalPosition;

        if (deskEquipmentObject.spriteRenderer != null)
        {
            deskEquipmentObject.spriteRenderer.sortingOrder =
                GetFinalSortingOrder(setting.deskEquipmentSortingOrder);
        }

        // 장비 데이터가 설치되어 있다면 현재 Workstation 방향에 맞는 Sprite를 적용한다.
        deskEquipmentObject.ApplyDirection(currentDirection);
    }
}