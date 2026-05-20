using UnityEngine;

/// <summary>
/// 상점에서 판매하는 장비 하나의 데이터.
/// 
/// 왼쪽 장비 목록은 PNG 한 장으로 표시한다.
/// 따라서 왼쪽 목록 안의 텍스트는 코드로 만들지 않는다.
/// 
/// 하지만 오른쪽 상세 정보는 선택한 장비에 따라
/// 이 데이터의 값을 읽어서 Text와 Image를 갱신한다.
/// </summary>
[System.Serializable]
public class ShopItemData
{
    [Header("기본 정보")]
    [Tooltip("장비 이름입니다. 예: 낡은 노트북")]
    public string itemName;

    [Tooltip("장비 카테고리입니다.")]
    public ShopCategory category;

    [Tooltip("구매 처리 방식입니다.")]
    public ShopPurchaseType purchaseType;

    [Header("오른쪽 상세 정보")]
    [Tooltip("오른쪽 상세 영역에 표시할 장비 이미지입니다.")]
    public Sprite detailImage;

    [Tooltip("장비 설명입니다.")]
    [TextArea]
    public string description;

    [Tooltip("돈 보상 증가율입니다. 0.02는 +2%를 의미합니다.")]
    public float moneyBonusRate;

    [Tooltip("해금 레벨입니다.")]
    public int unlockLevel;

    [Tooltip("가격입니다.")]
    public int price;

    [Tooltip("차지 평수입니다.")]
    public int spaceCost;

    [Header("구매 연결 데이터")]
    [Tooltip("TilePlaceable일 때 배치할 프리팹입니다. 예: Workstation.prefab, CoffeeMachine.prefab")]
    public GameObject placeablePrefab;

    [Tooltip("DeskEquipment일 때 책상 위에 설치할 장비 데이터입니다.")]
    public DeskEquipmentData deskEquipmentData;

    /// <summary>
    /// 가격을 UI 표시용 문자열로 변환한다.
    /// </summary>
    public string GetPriceText()
    {
        return price.ToString("N0") + "원";
    }

    /// <summary>
    /// 구매 버튼에 표시할 문자열을 만든다.
    /// </summary>
    public string GetBuyButtonText()
    {
        return price.ToString("N0") + "원 구매";
    }

    /// <summary>
    /// 돈 보상 증가율을 UI 표시용 문자열로 변환한다.
    /// </summary>
    public string GetMoneyBonusText()
    {
        int percent = Mathf.RoundToInt(moneyBonusRate * 100f);
        return "+" + percent + "%";
    }
}