using UnityEngine;

/// <summary>
/// 상점에서 선택한 장비의 구매 흐름을 관리하는 매니저.
/// 
/// 중요한 원칙:
/// - 구매 버튼 클릭 시 돈을 바로 차감하지 않는다.
/// - 구매 버튼 클릭 시 선택한 장비를 pendingEquipment로 저장한다.
/// - 실제 돈 차감과 효과 적용은 배치가 완료된 뒤에 한다.
/// </summary>
public class ShopPurchaseManager : MonoBehaviour
{
    public static ShopPurchaseManager Instance;

    [Header("현재 구매 대기 장비")]
    public EquipmentData pendingEquipment;

    [Header("구매 대기 상태")]
    public bool isWaitingForPlacement = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 상점 구매 버튼을 눌렀을 때 호출된다.
    /// 여기서는 돈을 차감하지 않고, 배치 준비만 한다.
    /// </summary>
    public void StartPurchase(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            Debug.LogWarning("구매할 장비 데이터가 없습니다.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance.HasEnoughMoney(equipmentData.price) == false)
        {
            Debug.Log("돈이 부족해서 구매할 수 없습니다: " + equipmentData.equipmentName);
            return;
        }

        pendingEquipment = equipmentData;
        isWaitingForPlacement = true;

        Debug.Log(
            "구매 대기 시작: " + equipmentData.equipmentName +
            " / 가격: " + equipmentData.price +
            " / 설치 타입: " + equipmentData.installType
        );

        // 여기서 바로 돈 차감 X
        // 여기서 바로 효과 적용 X

        StartPlacementModeByEquipmentType(equipmentData);
    }

    /// <summary>
    /// 장비 설치 타입에 따라 다음 배치 모드로 넘긴다.
    /// 아직 실제 배치 시스템 연결 전이므로 지금은 로그만 찍는다.
    /// 다음 단계에서 여기 안에 PlacementManager 연결 코드를 넣을 것이다.
    /// </summary>
    private void StartPlacementModeByEquipmentType(EquipmentData equipmentData)
    {
        if (equipmentData.installType == EquipmentInstallType.DeskEquipment)
        {
            if (DeskEquipmentPlacementManager.Instance == null)
            {
                Debug.LogError("DeskEquipmentPlacementManager.Instance가 없습니다.");
                return;
            }

            DeskEquipmentPlacementManager.Instance.StartDeskEquipmentPlacement(equipmentData);

            return;
        }

        if (equipmentData.installType == EquipmentInstallType.TilePlaceable)
        {
            if (TileEquipmentPlacementManager.Instance == null)
            {
                Debug.LogError("TileEquipmentPlacementManager.Instance가 없습니다.");
                return;
            }

            TileEquipmentPlacementManager.Instance.StartTileEquipmentPlacement(equipmentData);

            return;
        }

        Debug.LogWarning("알 수 없는 설치 타입입니다: " + equipmentData.installType);
    }

    /// <summary>
    /// 배치 완료 시 호출된다.
    /// 이 시점에서만 돈을 차감한다.
    /// </summary>
    public bool ConfirmPurchaseAfterPlacement()
    {
        if (pendingEquipment == null)
        {
            Debug.LogWarning("확정할 구매 대기 장비가 없습니다.");
            return false;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return false;
        }

        bool success = ResourceManager.Instance.SpendMoney(pendingEquipment.price);

        if (success == false)
        {
            Debug.Log("배치 확정 시점에 돈이 부족합니다: " + pendingEquipment.equipmentName);
            CancelPurchase();
            return false;
        }

        Debug.Log("구매 확정 및 돈 차감 완료: " + pendingEquipment.equipmentName);

        pendingEquipment = null;
        isWaitingForPlacement = false;

        return true;
    }

    /// <summary>
    /// 배치 취소 시 호출된다.
    /// 돈 차감 없이 구매 대기 상태만 해제한다.
    /// </summary>
    public void CancelPurchase()
    {
        if (pendingEquipment != null)
        {
            Debug.Log("구매 취소: " + pendingEquipment.equipmentName);
        }

        pendingEquipment = null;
        isWaitingForPlacement = false;
    }

    /// <summary>
    /// 현재 구매 대기 중인 장비 데이터를 반환한다.
    /// 배치 시스템에서 참조할 수 있다.
    /// </summary>
    public EquipmentData GetPendingEquipment()
    {
        return pendingEquipment;
    }
}