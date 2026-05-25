using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 연구생 고용창 UI.
/// 
/// 연구생 고용창 PNG 위에 투명 버튼을 올려서 고용 기능을 연결한다.
/// </summary>
public class ResearcherHirePanelUI : MonoBehaviour
{
    [Header("패널")]
    public GameObject panelRoot;

    [Header("버튼")]
    public Button closeButton;
    public Button hireUndergraduateButton;
    public Button hireMasterButton;
    public Button hirePhDButton;

    private void Awake()
    {
        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        if (hireUndergraduateButton != null)
        {
            hireUndergraduateButton.onClick.AddListener(HireUndergraduate);
        }

        if (hireMasterButton != null)
        {
            hireMasterButton.onClick.AddListener(HireMaster);
        }

        if (hirePhDButton != null)
        {
            hirePhDButton.onClick.AddListener(HirePhD);
        }
    }

    public void Open()
    {
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
    }

    public void HireUndergraduate()
    {
        Hire(StaffType.Undergraduate);
    }

    public void HireMaster()
    {
        Hire(StaffType.Master);
    }

    public void HirePhD()
    {
        Hire(StaffType.PhD);
    }

    /// <summary>
    /// 연구생 고용 버튼을 눌렀을 때 실행되는 공통 처리 함수.
    /// </summary>
    private void Hire(StaffType staffType)
    {
        if (StaffHireManager.Instance == null)
        {
            Debug.LogError("StaffHireManager.Instance가 없습니다. Managers 오브젝트에 StaffHireManager를 추가했는지 확인하세요.");
            return;
        }

        bool hireSuccess = StaffHireManager.Instance.HireStaff(staffType);

        if (hireSuccess)
        {
            Close();
        }

        Debug.Log("연구생 고용 버튼 클릭: " + staffType);
    }
}