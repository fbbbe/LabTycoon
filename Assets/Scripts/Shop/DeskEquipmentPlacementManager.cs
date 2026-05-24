using UnityEngine;

/// <summary>
/// 책상 위 장비 구매 후 Workstation 선택 모드를 관리하는 매니저.
/// 
/// 흐름:
/// 1. 상점에서 책상 위 장비 구매 버튼 클릭
/// 2. 이 매니저가 선택 모드 진입
/// 3. 플레이어가 Workstation 클릭
/// 4. 돈 차감
/// 5. Workstation에 장비 장착
/// 6. 해당 Workstation에 개인 장비 효과 등록
/// </summary>
public class DeskEquipmentPlacementManager : MonoBehaviour
{
    public static DeskEquipmentPlacementManager Instance;

    [Header("현재 장착 대기 중인 장비")]
    public EquipmentData pendingDeskEquipment;

    [Header("책상 장비 선택 모드 여부")]
    public bool isSelectingWorkstation = false;

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
    /// 책상 위 장비 선택 모드를 시작한다.
    /// </summary>
    public void StartDeskEquipmentPlacement(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            Debug.LogError("DeskEquipmentPlacementManager: equipmentData가 없습니다.");
            return;
        }

        if (equipmentData.installType != EquipmentInstallType.DeskEquipment)
        {
            Debug.LogError("DeskEquipmentPlacementManager: 책상 위 장비가 아닙니다: " + equipmentData.equipmentName);
            return;
        }

        if (equipmentData.deskEquipmentData == null)
        {
            Debug.LogError("DeskEquipmentPlacementManager: deskEquipmentData가 없습니다: " + equipmentData.equipmentName);
            return;
        }

        pendingDeskEquipment = equipmentData;
        isSelectingWorkstation = true;

        Debug.Log("Workstation 선택 모드 시작: " + equipmentData.equipmentName);
    }

    /// <summary>
    /// Workstation을 클릭했을 때 호출한다.
    /// </summary>
    public void TryInstallToWorkstation(WorkstationObject workstation)
    {
        if (isSelectingWorkstation == false)
        {
            return;
        }

        if (pendingDeskEquipment == null)
        {
            Debug.LogError("장착 대기 중인 장비가 없습니다.");
            CancelPlacement();
            return;
        }

        if (workstation == null)
        {
            Debug.LogError("선택한 Workstation이 없습니다.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance.HasEnoughMoney(pendingDeskEquipment.price) == false)
        {
            Debug.Log("돈이 부족해서 장착할 수 없습니다: " + pendingDeskEquipment.equipmentName);
            CancelPlacement();
            return;
        }

        // 먼저 Workstation에 장비 장착이 가능한지 확인하고 실제 장착한다.
        // 장착에 실패하면 돈을 차감하면 안 된다.
        bool installSuccess = workstation.InstallDeskEquipment(pendingDeskEquipment.deskEquipmentData);

        if (installSuccess == false)
        {
            Debug.Log("책상 위 장비 장착 실패. 돈을 차감하지 않습니다: " + pendingDeskEquipment.equipmentName);
            return;
        }

        // 장착 성공 후에만 돈 차감.
        bool spendSuccess = ResourceManager.Instance.SpendMoney(pendingDeskEquipment.price);

        if (spendSuccess == false)
        {
            Debug.Log("돈 차감 실패. 장비 설치를 취소합니다: " + pendingDeskEquipment.equipmentName);
            workstation.RemoveDeskEquipment();
            CancelPlacement();
            return;
        }

        // 해당 Workstation에만 컴퓨터 장비 효과 등록.
        if (EquipmentEffectManager.Instance != null)
        {
            EquipmentEffectManager.Instance.RegisterWorkstationComputer(
                workstation,
                pendingDeskEquipment
            );
        }

        Debug.Log("책상 위 장비 장착 완료: " + pendingDeskEquipment.equipmentName);

        pendingDeskEquipment = null;
        isSelectingWorkstation = false;

        if (ShopPurchaseManager.Instance != null)
        {
            ShopPurchaseManager.Instance.CancelPurchase();
        }
    }

    /// <summary>
    /// 장착 취소.
    /// 돈 차감 없음.
    /// </summary>
    public void CancelPlacement()
    {
        if (pendingDeskEquipment != null)
        {
            Debug.Log("책상 위 장비 장착 취소: " + pendingDeskEquipment.equipmentName);
        }

        pendingDeskEquipment = null;
        isSelectingWorkstation = false;

        if (ShopPurchaseManager.Instance != null)
        {
            ShopPurchaseManager.Instance.CancelPurchase();
        }
    }
}