using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimer;

    [SerializeField] private Button btnMoves;

    [SerializeField] private Button btnAutoWin;
    [SerializeField] private Button btnAutoLose;
    [SerializeField] private Button btnTimeChallenge;

    private UIMainManager m_mngr;

    private void Awake()
    {
        if (btnMoves) btnMoves.onClick.AddListener(OnClickMoves);
        if (btnTimer) btnTimer.onClick.AddListener(OnClickTimer);
        
        if (btnAutoWin) btnAutoWin.onClick.AddListener(() => m_mngr.LoadLevelAuto(true));
        if (btnAutoLose) btnAutoLose.onClick.AddListener(() => m_mngr.LoadLevelAuto(false));
        if (btnTimeChallenge) btnTimeChallenge.onClick.AddListener(() => m_mngr.LoadLevelTimeChallenge());
    }

    private void OnDestroy()
    {
        if (btnMoves) btnMoves.onClick.RemoveAllListeners();
        if (btnTimer) btnTimer.onClick.RemoveAllListeners();
        if (btnAutoWin) btnAutoWin.onClick.RemoveAllListeners();
        if (btnAutoLose) btnAutoLose.onClick.RemoveAllListeners();
        if (btnTimeChallenge) btnTimeChallenge.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickTimer()
    {
        m_mngr.LoadLevelTimer();
    }

    private void OnClickMoves()
    {
        m_mngr.LoadLevelMoves();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
