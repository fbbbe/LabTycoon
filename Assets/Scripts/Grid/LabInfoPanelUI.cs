using UnityEngine;
using UnityEngine.UI;

public class LabInfoPanelUI : MonoBehaviour
{
    public static LabInfoPanelUI Instance;

    [Header("연구실 정보 패널")]
    public GameObject panelRoot;

    [Header("버튼")]
    public Button expandButton;
    public Button closeButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (expandButton != null)
        {
            expandButton.onClick.RemoveAllListeners();
            expandButton.onClick.AddListener(OnClickExpandButton);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        Close();
    }

    public void Open()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    public void Close()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void OnClickExpandButton()
    {
        if (LabExpansionManager.Instance == null)
        {
            Debug.LogWarning("LabExpansionManager.Instance가 없습니다.");
            return;
        }

        bool success = LabExpansionManager.Instance.TryExpandLab();

        if (success)
        {
            Close();
        }
    }
}