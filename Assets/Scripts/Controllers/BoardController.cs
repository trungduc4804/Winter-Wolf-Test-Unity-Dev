using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    public Board m_board;

    private GameManager m_gameManager;

    private Camera m_cam;

    private GameSettings m_gameSettings;

    private bool m_gameOver;

    public SlotTray m_slotTray;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        m_slotTray = this.gameObject.AddComponent<SlotTray>();
        m_slotTray.Setup(this.transform, 5); // 5 slots based on requirement

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (m_board.IsEmpty())
        {
            m_gameManager.GameOver(true);
            m_gameOver = true;
            return;
        }

        if (m_slotTray.IsFull)
        {
            m_gameManager.GameOver(false);
            m_gameOver = true;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null && !cell.IsEmpty)
                {
                    if (!m_slotTray.IsFull)
                    {
                        Item item = cell.Item;
                        cell.Free(); // Remove from board
                        item.SetSortingLayerHigher();
                        m_slotTray.AddItem(item);
                        OnMoveEvent();
                    }
                }
            }
        }
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private Coroutine m_autoPlayCoroutine;

    public void StartAutoPlay(bool isWin)
    {
        if (m_autoPlayCoroutine != null) StopCoroutine(m_autoPlayCoroutine);
        m_autoPlayCoroutine = StartCoroutine(AutoPlayCoroutine(isWin));
    }

    private IEnumerator AutoPlayCoroutine(bool isWin)
    {
        yield return new WaitForSeconds(1f);
        
        while (!m_gameOver && !IsBusy)
        {
            yield return new WaitForSeconds(0.5f);
            if (m_gameOver || IsBusy) continue;

            if (isWin)
            {
                Cell cellToClick = FindValidAutoWinMove();
                if (cellToClick != null) ClickCell(cellToClick);
            }
            else
            {
                Cell cellToClick = FindValidAutoLoseMove();
                if (cellToClick != null) ClickCell(cellToClick);
            }
        }
    }

    private void ClickCell(Cell cell)
    {
        if (cell != null && !cell.IsEmpty && !m_slotTray.IsFull)
        {
            Item item = cell.Item;
            cell.Free();
            item.SetSortingLayerHigher();
            m_slotTray.AddItem(item);
            OnMoveEvent();
        }
    }

    private Cell FindValidAutoWinMove()
    {
        Dictionary<NormalItem.eNormalType, List<Cell>> types = new Dictionary<NormalItem.eNormalType, List<Cell>>();
        for (int x = 0; x < m_board.boardSizeX; x++)
        {
            for (int y = 0; y < m_board.boardSizeY; y++)
            {
                Cell c = m_board.m_cells[x, y];
                if (!c.IsEmpty && c.Item is NormalItem ni)
                {
                    if (!types.ContainsKey(ni.ItemType)) types[ni.ItemType] = new List<Cell>();
                    types[ni.ItemType].Add(c);
                }
            }
        }

        foreach (var kvp in types)
        {
            if (kvp.Value.Count > 0)
            {
                int inTray = m_slotTray.GetCountOfType(kvp.Key);
                if (inTray > 0 && inTray < 3) return kvp.Value[0];
            }
        }

        foreach (var kvp in types)
        {
            if (kvp.Value.Count >= 3) return kvp.Value[0];
        }

        for (int x = 0; x < m_board.boardSizeX; x++)
            for (int y = 0; y < m_board.boardSizeY; y++)
                if (!m_board.m_cells[x, y].IsEmpty) return m_board.m_cells[x, y];
                
        return null;
    }

    private Cell FindValidAutoLoseMove()
    {
        Dictionary<NormalItem.eNormalType, List<Cell>> types = new Dictionary<NormalItem.eNormalType, List<Cell>>();
        for (int x = 0; x < m_board.boardSizeX; x++)
        {
            for (int y = 0; y < m_board.boardSizeY; y++)
            {
                Cell c = m_board.m_cells[x, y];
                if (!c.IsEmpty && c.Item is NormalItem ni)
                {
                    if (!types.ContainsKey(ni.ItemType)) types[ni.ItemType] = new List<Cell>();
                    types[ni.ItemType].Add(c);
                }
            }
        }

        foreach (var kvp in types)
        {
            if (kvp.Value.Count > 0)
            {
                int inTray = m_slotTray.GetCountOfType(kvp.Key);
                if (inTray < 2) return kvp.Value[0];
            }
        }

        for (int x = 0; x < m_board.boardSizeX; x++)
            for (int y = 0; y < m_board.boardSizeY; y++)
                if (!m_board.m_cells[x, y].IsEmpty) return m_board.m_cells[x, y];
                
        return null;
    }
}
