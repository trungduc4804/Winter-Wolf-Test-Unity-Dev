using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelGame : MonoBehaviour, IMenu
{
    public Text LevelConditionView;

    [SerializeField] private GameObject levelConditionPanel;

    [SerializeField] private Button btnPause;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnPause.onClick.AddListener(OnClickPause);
    }

    private void OnClickPause()
    {
        m_mngr.ShowPauseMenu();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    public void ShowConditionPanel(bool show)
    {
        if (levelConditionPanel != null)
            levelConditionPanel.SetActive(show);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        // Always hide the timer panel by default; only Time Challenge shows it
        ShowConditionPanel(false);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        ShowConditionPanel(false);
        if (LevelConditionView != null)
        {
            LevelConditionView.gameObject.SetActive(false);
            LevelConditionView.text = "";
        }
    }
}

