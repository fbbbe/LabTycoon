using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 하나의 전체 데이터.
/// 
/// 이 데이터는 유니티 Inspector에서 장비마다 직접 입력하지 않는다.
/// EquipmentDatabase.cs 내부에서 코드로 생성한다.
/// 
/// 상점 버튼은 장비 이름 문자열만 가지고 있고,
/// EquipmentDatabase가 그 이름에 맞는 EquipmentData를 찾아준다.
/// </summary>
public class EquipmentData
{
    public string equipmentName;
    public EquipmentCategory category;
    public EquipmentInstallType installType;

    public int listIndex;
    public int unlockLevel;
    public int price;
    public int spaceCost;

    // 오른쪽 상세 카드 PNG를 Resources에서 불러오기 위한 경로
    public string detailCardResourcePath;

    // 타일 위에 직접 배치되는 장비일 때 사용할 프리팹 경로
    public string placeablePrefabResourcePath;

    // 책상 위 장비일 때 사용할 DeskEquipmentData
    public DeskEquipmentData deskEquipmentData;

    // 장비 효과 목록
    public List<EquipmentEffectData> effects = new List<EquipmentEffectData>();

    public EquipmentData(
        string equipmentName,
        EquipmentCategory category,
        EquipmentInstallType installType,
        int listIndex,
        int unlockLevel,
        int price,
        int spaceCost,
        string detailCardResourcePath
    )
    {
        this.equipmentName = equipmentName;
        this.category = category;
        this.installType = installType;
        this.listIndex = listIndex;
        this.unlockLevel = unlockLevel;
        this.price = price;
        this.spaceCost = spaceCost;
        this.detailCardResourcePath = detailCardResourcePath;
    }

    /// <summary>
    /// 오른쪽 상세 카드 PNG를 Resources에서 불러온다.
    /// </summary>
    public Sprite LoadDetailCardSprite()
    {
        if (string.IsNullOrWhiteSpace(detailCardResourcePath))
        {
            Debug.LogError(equipmentName + "의 상세 카드 경로가 비어 있습니다.");
            return null;
        }

        Sprite sprite = Resources.Load<Sprite>(detailCardResourcePath);

        if (sprite == null)
        {
            Debug.LogError("상세 카드 PNG를 찾지 못했습니다: Resources/" + detailCardResourcePath);
        }

        return sprite;
    }

    /// <summary>
    /// 타일 위에 배치할 프리팹을 Resources에서 불러온다.
    /// </summary>
    public GameObject LoadPlaceablePrefab()
    {
        if (string.IsNullOrWhiteSpace(placeablePrefabResourcePath))
        {
            Debug.LogError(equipmentName + "의 배치 프리팹 경로가 비어 있습니다.");
            return null;
        }

        GameObject prefab = Resources.Load<GameObject>(placeablePrefabResourcePath);

        if (prefab == null)
        {
            Debug.LogError("배치 프리팹을 찾지 못했습니다: Resources/" + placeablePrefabResourcePath);
        }

        return prefab;
    }

    /// <summary>
    /// 특정 효과 타입의 총합을 반환한다.
    /// 같은 효과가 여러 개 있을 수 있으므로 합산한다.
    /// </summary>
    public float GetEffectValue(EquipmentEffectType type)
    {
        float total = 0f;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i] != null && effects[i].effectType == type)
            {
                total += effects[i].value;
            }
        }

        return total;
    }
}