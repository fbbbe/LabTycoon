using UnityEngine;

/// <summary>
/// 화면에 상점/고용/인력 스탯 같은 패널이 열려 있을 때
/// 월드 클릭, Workstation 클릭, 과제 버튼 클릭을 막기 위한 전역 차단기.
///
/// 사용법:
/// - Canvas 아래에 항상 켜져 있는 GameObject 하나를 만든다.
/// - 이름 예: UIBlocker
/// - 이 스크립트를 붙인다.
/// - 열렸을 때 입력을 막아야 하는 패널들을 blockingPanels 배열에 연결한다.
/// </summary>
public class UIBlocker : MonoBehaviour
{
    public static UIBlocker Instance;

    [Header("입력을 막을 패널들")]
    [Tooltip("장비샵, 연구생 고용창, 인력 스탯창 등 열려 있을 때 월드 클릭을 막아야 하는 패널 Root를 연결합니다.")]
    public GameObject[] blockingPanels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool IsBlockingWorldInput()
    {
        if (blockingPanels == null)
        {
            return false;
        }

        for (int i = 0; i < blockingPanels.Length; i++)
        {
            if (blockingPanels[i] != null && blockingPanels[i].activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }
}
