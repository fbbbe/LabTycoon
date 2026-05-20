using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 투명 영역이 큰 PNG 버튼의 클릭 판정을 보이는 부분에만 적용하는 스크립트.
/// 
/// 문제 상황:
/// - PNG 이미지에는 투명한 여백이 많음
/// - Unity UI Button은 기본적으로 RectTransform 전체를 클릭 영역으로 봄
/// - 그래서 투명 영역이 다른 버튼 위를 덮으면 클릭을 가로챔
/// 
/// 해결:
/// - Image.alphaHitTestMinimumThreshold 값을 설정해서
///   PNG의 알파값이 일정 이상인 부분만 클릭되게 만든다.
/// 
/// 예:
/// threshold = 0.1
/// → 알파값이 10% 이상인 픽셀만 클릭 가능
/// → 완전 투명한 부분은 클릭 무시
/// </summary>
[RequireComponent(typeof(Image))]
public class AlphaHitTestUI : MonoBehaviour
{
    [Header("알파 클릭 판정 기준")]
    [Tooltip("이 값보다 투명한 픽셀은 클릭되지 않습니다. 0.1 정도 추천.")]
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    private Image targetImage;

    private void Awake()
    {
        ApplyAlphaHitTest();
    }

    private void OnValidate()
    {
        ApplyAlphaHitTest();
    }

    /// <summary>
    /// Image 컴포넌트에 알파 히트 테스트 기준을 적용한다.
    /// </summary>
    private void ApplyAlphaHitTest()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage == null)
        {
            return;
        }

        // Raycast Target이 켜져 있어야 버튼 클릭 판정을 받을 수 있다.
        targetImage.raycastTarget = true;

        // 투명 영역 클릭 무시 기준.
        // 이 값보다 알파가 낮은 픽셀은 클릭 영역으로 보지 않는다.
        targetImage.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}