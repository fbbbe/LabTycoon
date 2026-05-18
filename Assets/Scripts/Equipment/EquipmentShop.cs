using UnityEngine;

/// <summary>
/// 장비 상점 역할을 하는 스크립트.
/// 
/// 기존 방식:
/// - 버튼 클릭
/// - 돈 차감
/// - SpawnPoint에 자동 생성
/// 
/// 새 방식:
/// - 버튼 클릭
/// - 돈이 충분한지만 확인
/// - PlacementManager에게 배치 모드 시작 요청
/// - 실제 돈 차감은 배치 확정 시점에 진행
/// </summary>
public class EquipmentShop : MonoBehaviour
{
    [Header("장비 목록")]
    [Tooltip("상점에서 구매 가능한 장비 목록입니다.")]
    public EquipmentData[] equipmentList;

    /// <summary>
    /// 장비 구매 버튼에서 호출할 함수.
    /// 
    /// index는 equipmentList의 몇 번째 장비인지 의미한다.
    /// 예:
    /// 0 = 책상
    /// 1 = 커피머신
    /// 2 = 컴퓨터
    /// </summary>
    public void BuyEquipment(int index)
    {
        if (index < 0 || index >= equipmentList.Length)
        {
            Debug.LogError("EquipmentShop: 잘못된 장비 인덱스입니다.");
            return;
        }

        EquipmentData equipment = equipmentList[index];

        if (equipment.equipmentPrefab == null)
        {
            Debug.LogError(equipment.equipmentName + " 프리팹이 연결되지 않았습니다.");
            return;
        }

        if (PlacementManager.Instance == null)
        {
            Debug.LogError("PlacementManager.Instance가 없습니다. Managers 오브젝트에 PlacementManager.cs를 붙였는지 확인하세요.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 없습니다.");
            return;
        }

        // 돈이 부족하면 배치 모드에 들어가지 않는다.
        // 실제 차감은 PlacementManager에서 배치 확정 시점에 한다.
        if (ResourceManager.Instance.money < equipment.price)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        // 장비 자동 생성이 아니라 배치 모드로 진입한다.
        PlacementManager.Instance.StartPlacement(equipment.equipmentPrefab, equipment.price);

        Debug.Log(equipment.equipmentName + " 배치 모드 시작");
    }
}