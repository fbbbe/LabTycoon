using UnityEngine;

public class BuildingEditManager : MonoBehaviour
{
    public static BuildingEditManager Instance;

    public bool IsEditMode { get; private set; }

    [Header("건물 꾸미기 시 숨길 UI")]
    public GameObject moneyHUD;
    public GameObject researchResultHUD;

    public GameObject buildButton;
    public GameObject staffButton;
    public GameObject shopButton;

    public GameObject labInfoHUD;
    public GameObject labButton;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!IsEditMode)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndEditMode();
        }
    }

    public void StartEditMode()
    {
        IsEditMode = true;

        if (moneyHUD != null)
            moneyHUD.SetActive(false);

        if (researchResultHUD != null)
            researchResultHUD.SetActive(false);

        if (buildButton != null)
            buildButton.SetActive(false);

        if (staffButton != null)
            staffButton.SetActive(false);

        if (shopButton != null)
            shopButton.SetActive(false);

        if (labInfoHUD != null)
            labInfoHUD.SetActive(false);
        
        if (labButton != null)
            labButton.SetActive(false);

        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.ShowPlacementGrid();
        }

        Debug.Log("건물 꾸미기 모드 시작");
    }

    public void EndEditMode()
    {
        IsEditMode = false;

        if (moneyHUD != null)
            moneyHUD.SetActive(true);

        if (researchResultHUD != null)
            researchResultHUD.SetActive(true);

        if (buildButton != null)
            buildButton.SetActive(true);

        if (staffButton != null)
            staffButton.SetActive(true);

        if (shopButton != null)
            shopButton.SetActive(true);

        if (labInfoHUD != null)
            labInfoHUD.SetActive(true);

        if (labButton != null)
            labButton.SetActive(true);

        if (LabGridManager.Instance != null)
        {
            LabGridManager.Instance.HidePlacementGrid();
        }

        Debug.Log("건물 꾸미기 모드 종료");
    }
}