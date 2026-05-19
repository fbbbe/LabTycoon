using UnityEngine;

/// <summary>
/// 장비/가구 상점 스크립트.
/// 
/// 버튼을 누르면 물건을 바로 생성하지 않고,
/// PlacementManager에게 "배치 모드 시작"을 요청한다.
/// 
/// 예:
/// 책상+의자 세트 구매 버튼 클릭
/// → PlacementManager.StartPlacement(Workstation.prefab, 가격)
/// → 타일 위에 미리보기 표시
/// → 좌클릭으로 배치 확정
/// </summary>
public class EquipmentShop : MonoBehaviour
{
    [Header("구매 가능한 물건 목록")]
    [Tooltip("상점에서 구매 가능한 장비/가구 목록입니다.")]
    public EquipmentData[] equipmentList;

    /// <summary>
    /// 버튼 OnClick에서 호출할 함수.
    /// 
    /// index는 equipmentList의 몇 번째 물건인지 의미한다.
    /// 예:
    /// 0 = 책상+의자 세트
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
            Debug.LogError("EquipmentShop: " + equipment.equipmentName + " 프리팹이 연결되지 않았습니다.");
            return;
        }

        if (PlacementManager.Instance == null)
        {
            Debug.LogError("EquipmentShop: PlacementManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("EquipmentShop: ResourceManager.Instance가 없습니다.");
            return;
        }

        if (ResourceManager.Instance.money < equipment.price)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        // 실제 돈 차감은 배치 확정 시점에 PlacementManager에서 한다.
        PlacementManager.Instance.StartPlacement(equipment.equipmentPrefab, equipment.price);

        Debug.Log(equipment.equipmentName + " 배치 모드 시작");
    }
}