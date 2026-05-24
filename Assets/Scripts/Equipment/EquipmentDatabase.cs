using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 장비 데이터를 코드로 관리하는 데이터베이스.
///
/// 중요한 설계 원칙:
/// 1. 이 클래스는 MonoBehaviour가 아니다.
/// 2. 따라서 Managers 오브젝트에 붙일 필요가 없다.
/// 3. Unity Inspector에 장비 가격/효과를 직접 입력하지 않는다.
/// 4. 상점의 투명 버튼은 장비 이름 문자열만 가진다.
/// 5. 장비 이름으로 여기서 EquipmentData를 찾아 반환한다.
///
/// 사용 예:
/// EquipmentData data = EquipmentDatabase.Instance.FindByName("낡은 노트북");
/// </summary>
public class EquipmentDatabase
{
    private static EquipmentDatabase instance;

    /// <summary>
    /// 전역 접근용 Instance.
    ///
    /// MonoBehaviour가 아니므로 씬에 오브젝트가 없어도 자동으로 생성된다.
    /// </summary>
    public static EquipmentDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EquipmentDatabase();
            }

            return instance;
        }
    }

    private List<EquipmentData> equipments;

    /// <summary>
    /// 생성자.
    /// Instance가 처음 호출될 때 한 번 실행된다.
    /// </summary>
    private EquipmentDatabase()
    {
        CreateEquipmentData();
    }

    /// <summary>
    /// 장비 데이터를 코드로 생성한다.
    ///
    /// Unity Inspector에 장비 가격/효과를 직접 입력하지 않는다.
    /// 장비의 모든 수치와 효과는 이 함수 흐름에서 생성된다.
    /// </summary>
    private void CreateEquipmentData()
    {
        equipments = new List<EquipmentData>();

        CreateComputerEquipments();
        CreateEnvironmentEquipments();
        CreateResearchEquipments();
        CreateCoffeeEquipments();
        CreateCleaningEquipments();
    }

    /// <summary>
    /// 컴퓨터 장비 데이터 생성.
    ///
    /// 규칙:
    /// - 낡은 노트북, 중고 컴퓨터, 기본 컴퓨터, 사무용 컴퓨터, 고사양 컴퓨터, 연구용 워크스테이션은 책상 위 장비다.
    /// - GPU 서버, AI 연구 서버는 타일 위에 직접 설치하는 장비다.
    /// </summary>
    private void CreateComputerEquipments()
    {
        EquipmentData oldLaptop = new EquipmentData(
            equipmentName: "낡은 노트북",
            category: EquipmentCategory.Computer,
            installType: EquipmentInstallType.DeskEquipment,
            listIndex: 0,
            unlockLevel: 1,
            price: 30000,
            spaceCost: 1,
            detailCardResourcePath: "Shop/Details/OldLaptop"
        );

        oldLaptop.deskEquipmentData = CreateDeskEquipmentData(
            equipmentName: "낡은 노트북",
            equipmentType: DeskEquipmentType.OldLaptop,
            price: 30000,
            spritePathPrefix: "DeskEquipmentSprites/OldLaptop",
            moneyBonusRate: 0.02f,
            researchBonusRate: 0f
        );

        oldLaptop.effects.Add(CreateEffect(EquipmentEffectScope.PersonalStaff, EquipmentEffectType.MoneyRewardBonus, 0.02f));
        equipments.Add(oldLaptop);

        EquipmentData usedComputer = new EquipmentData(
            equipmentName: "중고 컴퓨터",
            category: EquipmentCategory.Computer,
            installType: EquipmentInstallType.DeskEquipment,
            listIndex: 1,
            unlockLevel: 5,
            price: 100000,
            spaceCost: 1,
            detailCardResourcePath: "Shop/Details/UsedComputer"
        );

        usedComputer.deskEquipmentData = CreateDeskEquipmentData(
            equipmentName: "중고 컴퓨터",
            equipmentType: DeskEquipmentType.UsedComputer,
            price: 100000,
            spritePathPrefix: "DeskEquipmentSprites/UsedComputer",
            moneyBonusRate: 0.04f,
            researchBonusRate: 0f
        );

        usedComputer.effects.Add(CreateEffect(EquipmentEffectScope.PersonalStaff, EquipmentEffectType.MoneyRewardBonus, 0.04f));
        equipments.Add(usedComputer);

        AddComputerOnly(
            name: "기본 컴퓨터",
            type: DeskEquipmentType.BasicComputer,
            listIndex: 2,
            unlockLevel: 15,
            price: 400000,
            spaceCost: 1,
            detailPath: "Shop/Details/BasicComputer",
            spritePathPrefix: "DeskEquipmentSprites/BasicComputer",
            moneyBonus: 0.07f
        );

        AddComputerOnly(
            name: "사무용 컴퓨터",
            type: DeskEquipmentType.OfficeComputer,
            listIndex: 3,
            unlockLevel: 25,
            price: 1200000,
            spaceCost: 1,
            detailPath: "Shop/Details/OfficeComputer",
            spritePathPrefix: "DeskEquipmentSprites/OfficeComputer",
            moneyBonus: 0.10f
        );

        AddComputerOnly(
            name: "고사양 컴퓨터",
            type: DeskEquipmentType.HighEndComputer,
            listIndex: 4,
            unlockLevel: 40,
            price: 4000000,
            spaceCost: 1,
            detailPath: "Shop/Details/HighEndComputer",
            spritePathPrefix: "DeskEquipmentSprites/HighEndComputer",
            moneyBonus: 0.15f
        );

        AddComputerOnly(
            name: "연구용 워크스테이션",
            type: DeskEquipmentType.ResearchWorkstation,
            listIndex: 5,
            unlockLevel: 60,
            price: 12000000,
            spaceCost: 2,
            detailPath: "Shop/Details/ResearchWorkstation",
            spritePathPrefix: "DeskEquipmentSprites/ResearchWorkstation",
            moneyBonus: 0.22f
        );

        AddTileEquipment(
            name: "GPU 서버",
            category: EquipmentCategory.Computer,
            listIndex: 6,
            unlockLevel: 80,
            price: 40000000,
            spaceCost: 3,
            detailPath: "Shop/Details/GPUServer",
            spritePathPrefix: "TileEquipmentSprites/GPUServer",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.30f)
            }
        );

        AddTileEquipment(
            name: "AI 연구 서버",
            category: EquipmentCategory.Computer,
            listIndex: 7,
            unlockLevel: 95,
            price: 120000000,
            spaceCost: 4,
            detailPath: "Shop/Details/AIServer",
            spritePathPrefix: "TileEquipmentSprites/AIServer",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.40f)
            }
        );
    }

    /// <summary>
    /// 책상 위에 설치하는 컴퓨터 계열 장비를 추가한다.
    /// 컴퓨터 장비 효과는 해당 Workstation을 사용하는 인력에게만 적용된다.
    /// </summary>
    private void AddComputerOnly(
        string name,
        DeskEquipmentType type,
        int listIndex,
        int unlockLevel,
        int price,
        int spaceCost,
        string detailPath,
        string spritePathPrefix,
        float moneyBonus
    )
    {
        EquipmentData data = new EquipmentData(
            equipmentName: name,
            category: EquipmentCategory.Computer,
            installType: EquipmentInstallType.DeskEquipment,
            listIndex: listIndex,
            unlockLevel: unlockLevel,
            price: price,
            spaceCost: spaceCost,
            detailCardResourcePath: detailPath
        );

        data.deskEquipmentData = CreateDeskEquipmentData(
            equipmentName: name,
            equipmentType: type,
            price: price,
            spritePathPrefix: spritePathPrefix,
            moneyBonusRate: moneyBonus,
            researchBonusRate: 0f
        );

        data.effects.Add(CreateEffect(EquipmentEffectScope.PersonalStaff, EquipmentEffectType.MoneyRewardBonus, moneyBonus));
        equipments.Add(data);
    }

    /// <summary>
    /// 환경 장비 데이터 생성.
    /// 환경 장비는 과제 스트레스와 청소 스트레스 증가량을 전체적으로 감소시킨다.
    /// </summary>
    private void CreateEnvironmentEquipments()
    {
        EquipmentData deskChairSet = new EquipmentData(
            equipmentName: "책상 의자 세트",
            category: EquipmentCategory.Environment,
            installType: EquipmentInstallType.TilePlaceable,
            listIndex: 0,
            unlockLevel: 1,
            price: 50000,
            spaceCost: 1,
            detailCardResourcePath: "Shop/Details/DeskChairSet"
        );

        deskChairSet.placeablePrefabResourcePath = "Prefabs/Workstation/Workstation";
        equipments.Add(deskChairSet);

        AddTileEquipment(
            name: "선풍기",
            category: EquipmentCategory.Environment,
            listIndex: 1,
            unlockLevel: 3,
            price: 50000,
            spaceCost: 1,
            detailPath: "Shop/Details/Fan",
            spritePathPrefix: "TileEquipmentSprites/Fan",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.05f)
            }
        );

        AddTileEquipment(
            name: "전기장판",
            category: EquipmentCategory.Environment,
            listIndex: 2,
            unlockLevel: 5,
            price: 70000,
            spaceCost: 1,
            detailPath: "Shop/Details/ElectricBlanket",
            spritePathPrefix: "TileEquipmentSprites/ElectricBlanket",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.05f)
            }
        );

        AddTileEquipment(
            name: "기본 에어컨",
            category: EquipmentCategory.Environment,
            listIndex: 3,
            unlockLevel: 15,
            price: 700000,
            spaceCost: 1,
            detailPath: "Shop/Details/BasicAirConditioner",
            spritePathPrefix: "TileEquipmentSprites/BasicAirConditioner",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.10f)
            }
        );

        AddTileEquipment(
            name: "기본 난방기",
            category: EquipmentCategory.Environment,
            listIndex: 4,
            unlockLevel: 15,
            price: 700000,
            spaceCost: 1,
            detailPath: "Shop/Details/BasicHeater",
            spritePathPrefix: "TileEquipmentSprites/BasicHeater",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.10f)
            }
        );

        AddTileEquipment(
            name: "공기청정기",
            category: EquipmentCategory.Environment,
            listIndex: 5,
            unlockLevel: 25,
            price: 1500000,
            spaceCost: 1,
            detailPath: "Shop/Details/AirPurifier",
            spritePathPrefix: "TileEquipmentSprites/AirPurifier",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.08f)
            }
        );

        AddTileEquipment(
            name: "대형 에어컨",
            category: EquipmentCategory.Environment,
            listIndex: 6,
            unlockLevel: 45,
            price: 5000000,
            spaceCost: 2,
            detailPath: "Shop/Details/LargeAirConditioner",
            spritePathPrefix: "TileEquipmentSprites/LargeAirConditioner",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.18f)
            }
        );

        AddTileEquipment(
            name: "프리미엄 공조 시스템",
            category: EquipmentCategory.Environment,
            listIndex: 7,
            unlockLevel: 70,
            price: 20000000,
            spaceCost: 3,
            detailPath: "Shop/Details/PremiumAirSystem",
            spritePathPrefix: "TileEquipmentSprites/PremiumAirSystem",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.25f)
            }
        );

        AddTileEquipment(
            name: "스마트 연구실 환경 시스템",
            category: EquipmentCategory.Environment,
            listIndex: 8,
            unlockLevel: 90,
            price: 80000000,
            spaceCost: 4,
            detailPath: "Shop/Details/SmartLabEnvironmentSystem",
            spritePathPrefix: "TileEquipmentSprites/SmartLabEnvironmentSystem",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalStress, EquipmentEffectType.StressIncreaseReduction, 0.35f)
            }
        );
    }

    /// <summary>
    /// 연구 장비 데이터 생성.
    /// 연구 장비는 게임 시스템 전체의 연구성과 보상 계산에 적용된다.
    /// </summary>
    private void CreateResearchEquipments()
    {
        AddTileEquipment(
            name: "실험 키트",
            category: EquipmentCategory.Research,
            listIndex: 0,
            unlockLevel: 10,
            price: 150000,
            spaceCost: 1,
            detailPath: "Shop/Details/ExperimentKit",
            spritePathPrefix: "TileEquipmentSprites/ExperimentKit",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalResearchResult, EquipmentEffectType.ResearchResultBonus, 0.03f)
            }
        );

        AddTileEquipment(
            name: "분석 도구",
            category: EquipmentCategory.Research,
            listIndex: 1,
            unlockLevel: 20,
            price: 600000,
            spaceCost: 1,
            detailPath: "Shop/Details/AnalysisTool",
            spritePathPrefix: "TileEquipmentSprites/AnalysisTool",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalResearchResult, EquipmentEffectType.ResearchResultBonus, 0.06f)
            }
        );

        AddTileEquipment(
            name: "정밀 측정 장비",
            category: EquipmentCategory.Research,
            listIndex: 2,
            unlockLevel: 35,
            price: 2000000,
            spaceCost: 2,
            detailPath: "Shop/Details/PrecisionMeasurementDevice",
            spritePathPrefix: "TileEquipmentSprites/PrecisionMeasurementDevice",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalResearchResult, EquipmentEffectType.ResearchResultBonus, 0.10f)
            }
        );

        AddTileEquipment(
            name: "자동 실험 장비",
            category: EquipmentCategory.Research,
            listIndex: 3,
            unlockLevel: 55,
            price: 8000000,
            spaceCost: 2,
            detailPath: "Shop/Details/AutoExperimentDevice",
            spritePathPrefix: "TileEquipmentSprites/AutoExperimentDevice",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalResearchResult, EquipmentEffectType.ResearchResultBonus, 0.15f)
            }
        );

        AddTileEquipment(
            name: "첨단 분석 장비",
            category: EquipmentCategory.Research,
            listIndex: 4,
            unlockLevel: 75,
            price: 25000000,
            spaceCost: 3,
            detailPath: "Shop/Details/AdvancedAnalysisDevice",
            spritePathPrefix: "TileEquipmentSprites/AdvancedAnalysisDevice",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalResearchResult, EquipmentEffectType.ResearchResultBonus, 0.22f)
            }
        );

        AddTileEquipment(
            name: "차세대 연구 장비",
            category: EquipmentCategory.Research,
            listIndex: 5,
            unlockLevel: 92,
            price: 90000000,
            spaceCost: 4,
            detailPath: "Shop/Details/NextGenResearchDevice",
            spritePathPrefix: "TileEquipmentSprites/NextGenResearchDevice",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalResearchResult, EquipmentEffectType.ResearchResultBonus, 0.35f)
            }
        );
    }

    /// <summary>
    /// 커피 장비 데이터 생성.
    /// 커피 장비의 보상 효과는 전역 적용, 패널티는 전체 인력에게 적용된다.
    /// </summary>
    private void CreateCoffeeEquipments()
    {
        AddTileEquipment(
            name: "믹스커피 박스",
            category: EquipmentCategory.Coffee,
            listIndex: 0,
            unlockLevel: 1,
            price: 20000,
            spaceCost: 0,
            detailPath: "Shop/Details/MixCoffeeBox",
            spritePathPrefix: "TileEquipmentSprites/MixCoffeeBox",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.01f),
                CreateEffect(EquipmentEffectScope.AllStaffPenalty, EquipmentEffectType.ExtraStressIncrease, 1f)
            }
        );

        AddTileEquipment(
            name: "커피포트",
            category: EquipmentCategory.Coffee,
            listIndex: 1,
            unlockLevel: 8,
            price: 150000,
            spaceCost: 1,
            detailPath: "Shop/Details/CoffeePot",
            spritePathPrefix: "TileEquipmentSprites/CoffeePot",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.03f),
                CreateEffect(EquipmentEffectScope.AllStaffPenalty, EquipmentEffectType.ExtraStressIncrease, 2f)
            }
        );

        AddTileEquipment(
            name: "기본 커피머신",
            category: EquipmentCategory.Coffee,
            listIndex: 2,
            unlockLevel: 20,
            price: 800000,
            spaceCost: 1,
            detailPath: "Shop/Details/BasicCoffeeMachine",
            spritePathPrefix: "TileEquipmentSprites/BasicCoffeeMachine",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.05f),
                CreateEffect(EquipmentEffectScope.AllStaffPenalty, EquipmentEffectType.ExtraStressIncrease, 3f)
            }
        );

        AddTileEquipment(
            name: "고급 커피머신",
            category: EquipmentCategory.Coffee,
            listIndex: 3,
            unlockLevel: 45,
            price: 5000000,
            spaceCost: 1,
            detailPath: "Shop/Details/PremiumCoffeeMachine",
            spritePathPrefix: "TileEquipmentSprites/PremiumCoffeeMachine",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.08f),
                CreateEffect(EquipmentEffectScope.AllStaffPenalty, EquipmentEffectType.ExtraStressIncrease, 5f)
            }
        );

        AddTileEquipment(
            name: "연구실 카페테리아",
            category: EquipmentCategory.Coffee,
            listIndex: 4,
            unlockLevel: 75,
            price: 30000000,
            spaceCost: 3,
            detailPath: "Shop/Details/LabCafeteria",
            spritePathPrefix: "TileEquipmentSprites/LabCafeteria",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalMoneyReward, EquipmentEffectType.MoneyRewardBonus, 0.12f),
                CreateEffect(EquipmentEffectScope.AllStaffPenalty, EquipmentEffectType.ExtraStressIncrease, 10f)
            }
        );
    }

    /// <summary>
    /// 청소 장비 데이터 생성.
    /// 청소 장비 효과는 전체 청소 시스템에 적용된다.
    /// </summary>
    private void CreateCleaningEquipments()
    {
        AddTileEquipment(
            name: "청소 도구함",
            category: EquipmentCategory.Cleaning,
            listIndex: 0,
            unlockLevel: 5,
            price: 50000,
            spaceCost: 1,
            detailPath: "Shop/Details/CleaningBox",
            spritePathPrefix: "TileEquipmentSprites/CleaningBox",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalCleaning, EquipmentEffectType.CleaningTimeReduction, 1f)
            }
        );

        AddTileEquipment(
            name: "무선 청소기",
            category: EquipmentCategory.Cleaning,
            listIndex: 1,
            unlockLevel: 25,
            price: 800000,
            spaceCost: 1,
            detailPath: "Shop/Details/WirelessVacuum",
            spritePathPrefix: "TileEquipmentSprites/WirelessVacuum",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalCleaning, EquipmentEffectType.CleaningTimeReduction, 2f)
            }
        );

        AddTileEquipment(
            name: "청소 로봇",
            category: EquipmentCategory.Cleaning,
            listIndex: 2,
            unlockLevel: 55,
            price: 8000000,
            spaceCost: 1,
            detailPath: "Shop/Details/CleaningRobot",
            spritePathPrefix: "TileEquipmentSprites/CleaningRobot",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalCleaning, EquipmentEffectType.CleaningTimeReduction, 2f)
            }
        );

        AddTileEquipment(
            name: "자동 청소 시스템",
            category: EquipmentCategory.Cleaning,
            listIndex: 3,
            unlockLevel: 85,
            price: 50000000,
            spaceCost: 1,
            detailPath: "Shop/Details/AutoCleaningSystem",
            spritePathPrefix: "TileEquipmentSprites/AutoCleaningSystem",
            effects: new EquipmentEffectData[]
            {
                CreateEffect(EquipmentEffectScope.GlobalCleaning, EquipmentEffectType.CleaningTimeReduction, 2f)
            }
        );
    }

    /// <summary>
    /// 타일 위에 직접 배치하는 일반 장비를 추가한다.
    ///
    /// Workstation처럼 별도 prefab이 필요한 특수 장비가 아니라면,
    /// 장비마다 prefab을 따로 만들지 않고 공통 TilePlaceableEquipment prefab을 사용한다.
    /// 이 함수는 장비별 4방향 Sprite 데이터만 EquipmentData에 연결한다.
    /// </summary>
    private void AddTileEquipment(
        string name,
        EquipmentCategory category,
        int listIndex,
        int unlockLevel,
        int price,
        int spaceCost,
        string detailPath,
        string spritePathPrefix,
        EquipmentEffectData[] effects
    )
    {
        EquipmentData data = new EquipmentData(
            equipmentName: name,
            category: category,
            installType: EquipmentInstallType.TilePlaceable,
            listIndex: listIndex,
            unlockLevel: unlockLevel,
            price: price,
            spaceCost: spaceCost,
            detailCardResourcePath: detailPath
        );

        data.tilePlaceableData = CreateTilePlaceableEquipmentData(
            name,
            spritePathPrefix
        );

        if (effects != null)
        {
            data.effects.AddRange(effects);
        }

        equipments.Add(data);
    }

    /// <summary>
    /// 책상 위 장비 데이터를 만든다.
    /// 4방향 Sprite는 Resources 경로 규칙으로 불러온다.
    /// 예:
    /// DeskEquipmentSprites/OldLaptop_RD
    /// DeskEquipmentSprites/OldLaptop_RU
    /// </summary>
    private DeskEquipmentData CreateDeskEquipmentData(
        string equipmentName,
        DeskEquipmentType equipmentType,
        int price,
        string spritePathPrefix,
        float moneyBonusRate,
        float researchBonusRate
    )
    {
        DeskEquipmentData data = new DeskEquipmentData();

        data.equipmentName = equipmentName;
        data.equipmentType = equipmentType;
        data.price = price;

        data.rightDownSprite = Resources.Load<Sprite>(spritePathPrefix + "_RD");
        data.rightUpSprite = Resources.Load<Sprite>(spritePathPrefix + "_RU");
        data.leftDownSprite = Resources.Load<Sprite>(spritePathPrefix + "_LD");
        data.leftUpSprite = Resources.Load<Sprite>(spritePathPrefix + "_LU");

        data.moneyRewardBonusRate = moneyBonusRate;
        data.researchRewardBonusRate = researchBonusRate;

        return data;
    }

    /// <summary>
    /// 효과 데이터를 생성한다.
    /// EquipmentEffectData 생성자 변경에 따른 호출부 중복을 줄이기 위한 헬퍼 함수다.
    /// </summary>
    private EquipmentEffectData CreateEffect(EquipmentEffectScope scope, EquipmentEffectType effectType, float value)
    {
        return new EquipmentEffectData(scope, effectType, value);
    }

    /// <summary>
    /// 장비 이름으로 데이터를 찾는다.
    /// 버튼에 입력된 equipmentName과 정확히 일치해야 한다.
    /// </summary>
    public EquipmentData FindByName(string equipmentName)
    {
        if (string.IsNullOrWhiteSpace(equipmentName))
        {
            Debug.LogError("FindByName: equipmentName이 비어 있습니다.");
            return null;
        }

        for (int i = 0; i < equipments.Count; i++)
        {
            if (equipments[i] != null && equipments[i].equipmentName == equipmentName)
            {
                return equipments[i];
            }
        }

        Debug.LogError("장비 데이터를 찾지 못했습니다: " + equipmentName);
        return null;
    }

    /// <summary>
    /// 카테고리별 장비 목록을 listIndex 순서로 반환한다.
    /// </summary>
    public List<EquipmentData> GetByCategory(EquipmentCategory category)
    {
        List<EquipmentData> result = new List<EquipmentData>();

        for (int i = 0; i < equipments.Count; i++)
        {
            EquipmentData data = equipments[i];

            if (data != null && data.category == category)
            {
                result.Add(data);
            }
        }

        result.Sort((a, b) => a.listIndex.CompareTo(b.listIndex));

        return result;
    }

    private TilePlaceableEquipmentData CreateTilePlaceableEquipmentData(
        string equipmentName,
        string spritePathPrefix
    )
    {
        TilePlaceableEquipmentData data = new TilePlaceableEquipmentData();

        data.equipmentName = equipmentName;

        data.rightDownSprite = Resources.Load<Sprite>(spritePathPrefix + "_RD");
        data.rightUpSprite = Resources.Load<Sprite>(spritePathPrefix + "_RU");
        data.leftDownSprite = Resources.Load<Sprite>(spritePathPrefix + "_LD");
        data.leftUpSprite = Resources.Load<Sprite>(spritePathPrefix + "_LU");

        if (data.rightDownSprite == null)
        {
            Debug.LogWarning("RD Sprite를 찾지 못했습니다: " + spritePathPrefix + "_RD");
        }

        if (data.rightUpSprite == null)
        {
            Debug.LogWarning("RU Sprite를 찾지 못했습니다: " + spritePathPrefix + "_RU");
        }

        if (data.leftDownSprite == null)
        {
            Debug.LogWarning("LD Sprite를 찾지 못했습니다: " + spritePathPrefix + "_LD");
        }

        if (data.leftUpSprite == null)
        {
            Debug.LogWarning("LU Sprite를 찾지 못했습니다: " + spritePathPrefix + "_LU");
        }

        return data;
    }
}