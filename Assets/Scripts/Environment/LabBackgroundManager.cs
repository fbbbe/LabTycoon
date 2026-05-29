using UnityEngine;

/// <summary>
/// 게임 화면 뒤쪽 배경을 관리하는 매니저입니다.
/// 
/// 역할:
/// 1. 배경 Sprite를 카메라 화면 크기에 맞게 자동 확대합니다.
/// 2. 연구실 레벨 구간에 따라 배경 Sprite를 교체합니다.
/// 
/// 배경은 타일/벽지가 아니라 카메라 뒤에 깔리는 큰 이미지입니다.
/// </summary>
public class LabBackgroundManager : MonoBehaviour
{
    public static LabBackgroundManager Instance;

    [Header("배경 SpriteRenderer")]
    [Tooltip("실제 배경 이미지를 표시하는 SpriteRenderer입니다.")]
    public SpriteRenderer backgroundRenderer;

    [Header("카메라")]
    [Tooltip("비워두면 Main Camera를 자동으로 찾습니다.")]
    public Camera targetCamera;

    [Header("레벨별 배경 Sprite")]
    [Tooltip("연구실 Lv.1~10 구간 배경입니다.")]
    public Sprite level1To10Background;

    [Tooltip("연구실 Lv.11~80 구간 배경입니다.")]
    public Sprite level11To80Background;

    [Tooltip("연구실 Lv.91~100 구간 배경입니다.")]
    public Sprite level91To100Background;

    [Header("배경 위치 설정")]
    [Tooltip("카메라보다 얼마나 뒤에 둘지 정합니다. 2D Orthographic에서는 Z값만 뒤로 보내면 됩니다.")]
    public float backgroundZ = 10f;

    [Tooltip("배경이 화면보다 살짝 더 크게 보이도록 추가 배율을 줍니다.")]
    public float extraScaleMultiplier = 1.05f;

    private int currentThemeIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Start()
    {
        RefreshBackgroundByLabLevel();
        FitBackgroundToCamera();
    }

    private void LateUpdate()
    {
        // 카메라 줌/이동이 있어도 배경이 항상 화면을 꽉 채우게 한다.
        FitBackgroundToCamera();
    }

    /// <summary>
    /// 현재 연구실 레벨에 맞는 배경으로 교체합니다.
    /// 타일/벽지 테마 구간과 같은 기준을 사용합니다.
    /// </summary>
    public void RefreshBackgroundByLabLevel()
    {
        int labLevel = GetCurrentLabLevel();
        int themeIndex = GetThemeIndexByLabLevel(labLevel);

        if (themeIndex == currentThemeIndex)
        {
            return;
        }

        currentThemeIndex = themeIndex;

        if (backgroundRenderer == null)
        {
            Debug.LogWarning("LabBackgroundManager: backgroundRenderer가 연결되지 않았습니다.");
            return;
        }

        backgroundRenderer.sprite = GetBackgroundSpriteByThemeIndex(themeIndex);

        FitBackgroundToCamera();

        Debug.Log("배경 테마 변경: 연구실 Lv." + labLevel + " / Theme " + themeIndex);
    }

    /// <summary>
    /// 카메라 화면 크기에 맞춰 배경 Sprite 크기를 자동 조정합니다.
    /// </summary>
    public void FitBackgroundToCamera()
    {
        if (targetCamera == null || backgroundRenderer == null || backgroundRenderer.sprite == null)
        {
            return;
        }

        if (targetCamera.orthographic == false)
        {
            Debug.LogWarning("LabBackgroundManager: 현재 코드는 Orthographic Camera 기준입니다.");
            return;
        }

        // 카메라 월드 기준 세로/가로 크기
        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        // Sprite 원본 월드 크기
        float spriteWidth = backgroundRenderer.sprite.bounds.size.x;
        float spriteHeight = backgroundRenderer.sprite.bounds.size.y;

        if (spriteWidth <= 0f || spriteHeight <= 0f)
        {
            return;
        }

        // 화면을 꽉 채우기 위해 가로/세로 중 더 큰 배율을 사용한다.
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;
        float finalScale = Mathf.Max(scaleX, scaleY) * extraScaleMultiplier;

        backgroundRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // 카메라 위치를 따라가게 해서 화면 전체 배경처럼 보이게 한다.
        Vector3 cameraPosition = targetCamera.transform.position;
        backgroundRenderer.transform.position = new Vector3(
            cameraPosition.x,
            cameraPosition.y,
            cameraPosition.z + backgroundZ
        );
    }

    private int GetThemeIndexByLabLevel(int labLevel)
    {
        if (labLevel <= 10)
        {
            return 0;
        }

        if (labLevel <= 80)
        {
            return 1;
        }

        return 2;
    }

    private Sprite GetBackgroundSpriteByThemeIndex(int themeIndex)
    {
        switch (themeIndex)
        {
            case 0:
                return level1To10Background;

            case 1:
                return level11To80Background;

            case 2:
                return level91To100Background;

            default:
                return level1To10Background;
        }
    }

    private int GetCurrentLabLevel()
    {
        if (ResourceManager.Instance == null)
        {
            return 1;
        }

        return ResourceManager.Instance.labLevel;
    }
}