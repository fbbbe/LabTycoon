using UnityEngine;

/// <summary>
/// 상점에 표시되는 아이템 하나의 데이터.
/// 
/// 왼쪽 목록 버튼 자체는 PNG로 만들어져 있으므로,
/// 왼쪽 목록에는 텍스트를 따로 넣지 않는다.
/// 
/// 대신 오른쪽 상세 정보 영역은 이 데이터로 텍스트를 갱신한다.
/// </summary>
[System.Serializable]
public class ShopItemData
{
    [Header("기본 정보")]
    [Tooltip("장비 이름입니다. 예: 낡은 노트북")]
    public string itemName;

    [Tooltip("이 장비가 속한 상점 카테고리입니다.")]
    public ShopCategory category;

    [Tooltip("구매 처리 방식입니다. 바닥 배치인지, 책상 위 장비인지 구분합니다.")]
    public ShopPurchaseType purchaseType;

    [Header("상세 정보")]
    [Tooltip("오른쪽 상세 영역에 표시할 장비 이미지입니다.")]
    public Sprite detailImage;

    [Tooltip("장비 설명입니다.")]
    [TextArea]
    public string description;

    [Tooltip("돈 보상 증가율입니다. 예: 0.02는 +2%입니다.")]
    public float moneyBonusRate;

    [Tooltip("해금 레벨입니다.")]
    public int unlockLevel;

    [Tooltip("가격입니다.")]
    public int price;

    [Tooltip("차지 평수입니다.")]
    public int spaceCost;

    [Header("구매 연결 데이터")]
    [Tooltip("TilePlaceable일 때 배치할 프리팹입니다. 예: Workstation.prefab")]
    public GameObject placeablePrefab;

    [Tooltip("DeskEquipment일 때 설치할 책상 위 장비 데이터입니다.")]
    public DeskEquipmentData deskEquipmentData;

    /// <summary>
    /// 가격을 UI에 표시하기 좋은 문자열로 반환한다.
    /// </summary>
    public string GetPriceText()
    {
        return price.ToString("N0") + "원";
    }

    /// <summary>
    /// 돈 보상 증가율을 UI에 표시하기 좋은 문자열로 반환한다.
    /// </summary>
    public string GetMoneyBonusText()
    {
        int percent = Mathf.RoundToInt(moneyBonusRate * 100f);
        return "+" + percent + "%";
    }
}