using UnityEngine;

/// <summary>
/// Workstation 위에 장착된 책상 장비를 관리하는 스크립트.
/// 
/// 이 오브젝트는 바닥 타일 위에 직접 배치되는 물건이 아니다.
/// Workstation의 DeskEquipment 자식 오브젝트에 붙어서
/// 현재 장착된 장비 이미지를 보여주는 역할을 한다.
/// </summary>
public class DeskEquipmentObject : MonoBehaviour
{
    [Header("현재 장착된 장비 데이터")]
    [Tooltip("현재 책상 위에 설치된 장비 데이터입니다.")]
    public DeskEquipmentData currentData;

    [Header("SpriteRenderer")]
    [Tooltip("책상 위 장비 이미지를 표시하는 SpriteRenderer입니다.")]
    public SpriteRenderer spriteRenderer;

    [Header("소속 Workstation")]
    [Tooltip("이 장비가 설치된 Workstation입니다.")]
    public WorkstationObject ownerWorkstation;

    private void Awake()
    {
        AutoFindRendererIfNeeded();
    }

    private void OnValidate()
    {
        AutoFindRendererIfNeeded();
    }

    /// <summary>
    /// SpriteRenderer가 연결되지 않았으면 자동으로 찾는다.
    /// </summary>
    private void AutoFindRendererIfNeeded()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// 책상 장비를 설치한다.
    /// 
    /// 이 함수는 WorkstationObject에서 호출한다.
    /// 설치되는 순간 장비 데이터가 저장되고,
    /// Workstation의 현재 방향에 맞는 Sprite가 적용된다.
    /// </summary>
    public void InstallEquipment(DeskEquipmentData data, WorkstationObject workstation)
    {
        currentData = data;
        ownerWorkstation = workstation;

        if (currentData == null)
        {
            Debug.LogError("DeskEquipmentObject: 설치할 장비 데이터가 없습니다.");
            return;
        }

        if (ownerWorkstation == null)
        {
            Debug.LogError("DeskEquipmentObject: 소속 Workstation이 없습니다.");
            return;
        }

        ApplyDirection(ownerWorkstation.currentDirection);
    }

    /// <summary>
    /// Workstation 방향에 맞춰 장비 이미지를 바꾼다.
    /// 
    /// 예:
    /// Workstation RD → OldLaptop_RD
    /// Workstation RU → OldLaptop_RU
    /// </summary>
    public void ApplyDirection(PlacementDirection direction)
    {
        if (currentData == null)
        {
            return;
        }

        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = currentData.GetSprite(direction);
    }

    /// <summary>
    /// 장비를 제거한다.
    /// 
    /// 나중에 장비 교체/판매 기능에서 사용할 수 있다.
    /// </summary>
    public void ClearEquipment()
    {
        currentData = null;
        ownerWorkstation = null;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }
}