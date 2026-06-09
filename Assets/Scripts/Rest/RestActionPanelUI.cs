using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인력 스탯창의 '스트레스 감소시키기' 버튼을 눌렀을 때 열리는 휴식 행동 선택 창입니다.
///
/// 구성 방식:
/// - 큰 배경 PNG 1장
/// - 그 위에 카드 8개 투명 Button
/// - 각 Button은 StaffRestController에 휴식 행동 실행을 요청합니다.
/// </summary>
public class RestActionPanelUI : MonoBehaviour
{
    public static RestActionPanelUI Instance;

    [Header("패널")]
    public GameObject panelRoot;

    [Header("현재 스트레스")]
    public TextMeshProUGUI currentStressText;

    [Header("카드 투명 버튼 8개")]
    public Button waterButton;
    public Button shortRestButton;
    public Button catPlayButton;
    public Button napButton;
    public Button snackButton;
    public Button mealButton;
    public Button youtubeButton;
    public Button companyDinnerButton;

    [Header("닫기 버튼")]
    public Button closeButton;

    [Header("머리 위 휴식 아이콘")]
    public Sprite waterIcon;
    public Sprite shortRestIcon;
    public Sprite catPlayIcon;
    public Sprite napIcon;
    public Sprite snackIcon;
    public Sprite mealIcon;
    public Sprite youtubeIcon;
    public Sprite companyDinnerIcon;

    private StaffWorker selectedStaff;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BindButton(waterButton, 0);
        BindButton(shortRestButton, 1);
        BindButton(catPlayButton, 2);
        BindButton(napButton, 3);
        BindButton(snackButton, 4);
        BindButton(mealButton, 5);
        BindButton(youtubeButton, 6);
        BindButton(companyDinnerButton, 7);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        Close();
    }

    private void BindButton(Button button, int actionIndex)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnClickRestAction(actionIndex));
    }

    public void Open(StaffWorker staff)
    {
        if (staff == null)
        {
            Debug.LogWarning("RestActionPanelUI: 선택된 인력이 없습니다.");
            return;
        }

        selectedStaff = staff;

        RefreshStressText();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    public void Close()
    {
        selectedStaff = null;

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void OnClickRestAction(int actionIndex)
    {
        if (StaffRestController.Instance == null)
        {
            Debug.LogWarning("RestActionPanelUI: StaffRestController.Instance가 없습니다.");
            return;
        }

        Sprite icon = GetIconByActionIndex(actionIndex);
        StaffRestController.Instance.StartRestAction(selectedStaff, actionIndex, icon);
    }

    private Sprite GetIconByActionIndex(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0:
                return waterIcon;
            case 1:
                return shortRestIcon;
            case 2:
                return catPlayIcon;
            case 3:
                return napIcon;
            case 4:
                return snackIcon;
            case 5:
                return mealIcon;
            case 6:
                return youtubeIcon;
            case 7:
                return companyDinnerIcon;
            default:
                return null;
        }
    }

    private void RefreshStressText()
    {
        if (currentStressText == null || selectedStaff == null)
        {
            return;
        }

        int currentStress = GetCurrentStress(selectedStaff);

        currentStressText.text = currentStress.ToString();
    }

    private int GetCurrentStress(StaffWorker staff)
    {
        if (staff == null)
        {
            return 0;
        }

        if (staff.runtimeData != null)
        {
            return staff.runtimeData.currentStress;
        }

        return staff.currentStress;
    }
}

/// <summary>
/// 카드 하나에 대응되는 휴식 행동 데이터입니다.
/// </summary>
[Serializable]
public class RestActionData
{
    public string actionName;
    public float duration;
    public int stressDecrease;
    public long cost;
    public bool isInstant;
    public bool applyToAllStaff;
}

