using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimer;

    [SerializeField] private Button btnMoves;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnMoves.onClick.AddListener(OnClickMoves);
        btnTimer.onClick.AddListener(OnClickTimer);

        CreateAutoButtons();
    }

    private void CreateAutoButtons()
    {
        GameObject btnWinGo = new GameObject("BtnAutoWin");
        btnWinGo.transform.SetParent(this.transform);
        btnWinGo.transform.localPosition = new Vector3(0, -100, 0); 
        Button btnWin = btnWinGo.AddComponent<Button>();
        btnWinGo.AddComponent<Image>().color = Color.gray;
        
        GameObject txtWinGo = new GameObject("Text");
        txtWinGo.transform.SetParent(btnWinGo.transform);
        txtWinGo.transform.localPosition = Vector3.zero;
        Text txtWin = txtWinGo.AddComponent<Text>();
        txtWin.text = "Auto Win";
        txtWin.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txtWin.alignment = TextAnchor.MiddleCenter;
        txtWin.color = Color.black;
        
        btnWin.onClick.AddListener(() => m_mngr.LoadLevelAuto(true));
        btnWinGo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
        txtWinGo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);

        GameObject btnLoseGo = new GameObject("BtnAutoLose");
        btnLoseGo.transform.SetParent(this.transform);
        btnLoseGo.transform.localPosition = new Vector3(0, -160, 0);
        Button btnLose = btnLoseGo.AddComponent<Button>();
        btnLoseGo.AddComponent<Image>().color = Color.gray;
        
        GameObject txtLoseGo = new GameObject("Text");
        txtLoseGo.transform.SetParent(btnLoseGo.transform);
        txtLoseGo.transform.localPosition = Vector3.zero;
        Text txtLose = txtLoseGo.AddComponent<Text>();
        txtLose.text = "Auto Lose";
        txtLose.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txtLose.alignment = TextAnchor.MiddleCenter;
        txtLose.color = Color.black;
        
        btnLose.onClick.AddListener(() => m_mngr.LoadLevelAuto(false));
        btnLoseGo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
        txtLoseGo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
    }

    private void OnDestroy()
    {
        if (btnMoves) btnMoves.onClick.RemoveAllListeners();
        if (btnTimer) btnTimer.onClick.RemoveAllListeners();
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
