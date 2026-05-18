using System.Collections;
using UnityEngine;

/// <summary>
/// 게임 시작 시 기본 상태를 세팅하는 스크립트.
/// 
/// 현재 게임 시작 조건:
/// 1. 학사생 1명
/// 2. 낡은 노트북 1개
/// 3. 소정의 돈
/// 4. 9평 연구실
/// 
/// 9평 연구실은 LabGridManager.cs가 이미 Tile.png 9개로 생성한다.
/// 이 스크립트는 그 위에 "초기 인력"과 "초기 장비"를 생성한다.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("초기 자원")]
    [Tooltip("게임 시작 시 지급할 돈입니다.")]
    public int startingMoney = 100000;

    [Header("초기 인력 프리팹")]
    [Tooltip("게임 시작 시 생성할 학사생 프리팹입니다. Undergraduate.prefab을 넣습니다.")]
    public GameObject undergraduatePrefab;

    [Header("초기 장비 프리팹")]
    [Tooltip("게임 시작 시 기본으로 배치할 낡은 노트북 프리팹입니다. OldLaptop.prefab을 넣습니다.")]
    public GameObject oldLaptopPrefab;

    [Header("인력 생성 위치")]
    [Tooltip("새 인력이 등장하는 문 앞 위치입니다.")]
    public Transform doorSpawnPoint;

    [Header("낡은 노트북 시작 타일 좌표")]
    [Tooltip("낡은 노트북을 배치할 타일의 X 좌표입니다.")]
    public int oldLaptopTileX = 1;

    [Tooltip("낡은 노트북을 배치할 타일의 Y 좌표입니다.")]
    public int oldLaptopTileY = 1;

    [Header("중복 초기화 방지")]
    [Tooltip("게임 시작 세팅이 이미 끝났는지 확인하는 값입니다.")]
    public bool initialized = false;

    private IEnumerator Start()
    {
        // 다른 Manager들의 Start()가 먼저 실행될 시간을 한 프레임 준다.
        // 이유:
        // LabGridManager.cs도 Start()에서 타일을 생성하므로,
        // GameInitializer가 너무 빨리 실행되면 아직 타일이 없을 수 있다.
        yield return null;

        InitializeGame();
    }

    /// <summary>
    /// 게임 시작 상태를 세팅한다.
    /// </summary>
    public void InitializeGame()
    {
        // 이미 초기화했다면 다시 실행하지 않는다.
        // 실수로 두 번 실행되면 학사생이나 노트북이 중복 생성될 수 있기 때문이다.
        if (initialized)
        {
            return;
        }

        initialized = true;

        SetInitialMoney();
        SpawnInitialStaff();
        SpawnInitialEquipment();
    }

    /// <summary>
    /// 초기 돈을 설정한다.
    /// </summary>
    private void SetInitialMoney()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("GameInitializer: ResourceManager.Instance가 없습니다.");
            return;
        }

        // ResourceManager의 돈 값을 시작 금액으로 설정한다.
        ResourceManager.Instance.money = startingMoney;
    }

    /// <summary>
    /// 게임 시작 시 학사생 1명을 문 앞에 생성한다.
    /// </summary>
    private void SpawnInitialStaff()
    {
        if (undergraduatePrefab == null)
        {
            Debug.LogError("GameInitializer: Undergraduate Prefab이 연결되지 않았습니다.");
            return;
        }

        if (doorSpawnPoint == null)
        {
            Debug.LogError("GameInitializer: Door Spawn Point가 연결되지 않았습니다.");
            return;
        }

        // 문 앞 위치에 학사생 프리팹을 생성한다.
        GameObject staffObject = Instantiate(
            undergraduatePrefab,
            doorSpawnPoint.position,
            Quaternion.identity
        );

        staffObject.name = "Undergraduate_Initial";
    }

    /// <summary>
    /// 게임 시작 시 낡은 노트북 1개를 기본 타일에 배치한다.
    /// </summary>
    private void SpawnInitialEquipment()
    {
        if (oldLaptopPrefab == null)
        {
            Debug.LogError("GameInitializer: Old Laptop Prefab이 연결되지 않았습니다.");
            return;
        }

        if (LabGridManager.Instance == null)
        {
            Debug.LogError("GameInitializer: LabGridManager.Instance가 없습니다.");
            return;
        }

        // 설정한 좌표의 타일을 가져온다.
        LabTile targetTile = LabGridManager.Instance.GetTile(oldLaptopTileX, oldLaptopTileY);

        if (targetTile == null)
        {
            Debug.LogError("GameInitializer: 낡은 노트북을 배치할 타일을 찾지 못했습니다.");
            return;
        }

        // 이미 점유된 타일이면 장비를 놓지 않는다.
        if (targetTile.CanPlaceObject() == false)
        {
            Debug.LogError("GameInitializer: 해당 타일은 이미 사용 중입니다.");
            return;
        }

        // 타일 중심 위치에 낡은 노트북 생성.
        GameObject laptopObject = Instantiate(
            oldLaptopPrefab,
            targetTile.GetCenterPosition(),
            Quaternion.identity
        );

        laptopObject.name = "OldLaptop_Initial";

        // PlaceableObject가 있으면 배치 완료 상태로 표시한다.
        PlaceableObject placeableObject = laptopObject.GetComponent<PlaceableObject>();

        if (placeableObject != null)
        {
            placeableObject.ConfirmPlacement(targetTile);
        }

        // 해당 타일을 사용 중으로 표시한다.
        // 이렇게 해야 나중에 이 타일 위에 다른 장비를 또 배치하지 못한다.
        LabGridManager.Instance.MarkTileOccupied(targetTile);
    }
}