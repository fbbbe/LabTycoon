using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Image의 투명 영역을 클릭되지 않게 만드는 스크립트.
/// 
/// 사용 대상:
/// - UI Button
/// - UI Image 기반 아이콘
/// - 과제하기 / 검사받기 / 청소하기 버튼
/// 
/// 주의:
/// Source Image로 사용하는 PNG는 Import Settings에서 Read/Write가 켜져 있어야 한다.
/// </summary>
[RequireComponent(typeof(Image))]
public class AlphaHitTestUI : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("이 값보다 알파가 낮은 픽셀은 클릭되지 않습니다. 보통 0.1 정도 사용합니다.")]
    public float alphaThreshold = 0.1f;

    private Image targetImage;

    private void Awake()
    {
        targetImage = GetComponent<Image>();

        if (targetImage != null)
        {
            targetImage.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }

    private void OnValidate()
    {
        Image image = GetComponent<Image>();

        if (image != null)
        {
            image.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}