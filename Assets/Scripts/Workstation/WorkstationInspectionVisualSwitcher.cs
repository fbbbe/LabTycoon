using UnityEngine;

/// <summary>
/// 검사 연출 중 Workstation의 착석 이미지를 빈 의자 이미지로 바꿔주는 스크립트.
/// 
/// 현재 구조:
/// - 평소에는 ChairRenderer에 "의자+인력 합성 이미지"가 들어감
/// - 검사 중에는 ChairRenderer를 "빈 의자 이미지"로 교체
/// - 검사 끝나면 원래 합성 이미지로 복구
/// 
/// 장점:
/// - 실제 StaffWorker 오브젝트는 건드리지 않음
/// - 의자는 사라지지 않음
/// - 검사 연출 중 원래 자리에 인력만 사라진 것처럼 보임
/// </summary>
public class WorkstationInspectionVisualSwitcher : MonoBehaviour
{
    [Header("교체 대상 Renderer")]
    [Tooltip("평소에 의자+인력 합성 이미지가 표시되는 SpriteRenderer입니다.")]
    public SpriteRenderer chairRenderer;

    [Header("빈 의자 4방향 Sprite")]
    public Sprite chairOnlyRightDownSprite;
    public Sprite chairOnlyRightUpSprite;
    public Sprite chairOnlyLeftDownSprite;
    public Sprite chairOnlyLeftUpSprite;

    private Sprite cachedSeatedSprite;
    private bool isSwitched = false;

    /// <summary>
    /// 검사 연출 시작 시 호출.
    /// 현재 착석 이미지를 저장하고, 방향에 맞는 빈 의자로 교체한다.
    /// </summary>
    public void ShowChairOnlyForInspection()
    {
        if (chairRenderer == null)
        {
            Debug.LogWarning("WorkstationInspectionVisualSwitcher: chairRenderer가 연결되지 않았습니다.");
            return;
        }

        if (chairRenderer.sprite == null)
        {
            Debug.LogWarning("WorkstationInspectionVisualSwitcher: 현재 chairRenderer에 Sprite가 없습니다.");
            return;
        }

        if (isSwitched)
        {
            return;
        }

        cachedSeatedSprite = chairRenderer.sprite;

        Sprite chairOnlySprite = GetChairOnlySpriteByCurrentSpriteName(cachedSeatedSprite.name);

        if (chairOnlySprite == null)
        {
            Debug.LogWarning("방향에 맞는 빈 의자 Sprite를 찾지 못했습니다. 현재 Sprite 이름: " + cachedSeatedSprite.name);
            return;
        }

        chairRenderer.sprite = chairOnlySprite;
        isSwitched = true;
    }

    /// <summary>
    /// 검사 연출 종료 시 호출.
    /// 검사 전 저장해둔 의자+인력 합성 이미지로 복구한다.
    /// </summary>
    public void RestoreSeatedVisualAfterInspection()
    {
        if (chairRenderer == null)
        {
            return;
        }

        if (cachedSeatedSprite == null)
        {
            return;
        }

        chairRenderer.sprite = cachedSeatedSprite;
        cachedSeatedSprite = null;
        isSwitched = false;
    }

    /// <summary>
    /// 현재 착석 이미지 이름을 보고 방향을 추정한다.
    /// 
    /// 네가 쓰는 Sprite 이름 끝에 RD, RU, LD, LU가 들어가 있으면 자동으로 맞춰진다.
    /// 예:
    /// UndergraduateChair_RD
    /// Chair_Set_Master_RU
    /// SeatedPhD_LD
    /// </summary>
    private Sprite GetChairOnlySpriteByCurrentSpriteName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            return null;
        }

        if (spriteName.Contains("_RD") || spriteName.EndsWith("RD"))
        {
            return chairOnlyRightDownSprite;
        }

        if (spriteName.Contains("_RU") || spriteName.EndsWith("RU"))
        {
            return chairOnlyRightUpSprite;
        }

        if (spriteName.Contains("_LD") || spriteName.EndsWith("LD"))
        {
            return chairOnlyLeftDownSprite;
        }

        if (spriteName.Contains("_LU") || spriteName.EndsWith("LU"))
        {
            return chairOnlyLeftUpSprite;
        }

        return null;
    }
}