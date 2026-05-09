using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SlotTray : MonoBehaviour
{
    public int MaxSlots = 5;
    private List<Item> m_items = new List<Item>();
    
    private Vector3[] m_slotPositions;

    public bool IsFull => m_items.Count >= MaxSlots;

    public void Setup(Transform root, int maxSlots = 5)
    {
        MaxSlots = maxSlots;
        m_slotPositions = new Vector3[MaxSlots];
        
        Camera cam = Camera.main;
        float screenBottomY = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        
        float startX = -(MaxSlots / 2f) + 0.5f;
        for (int i = 0; i < MaxSlots; i++)
        {
            m_slotPositions[i] = new Vector3(startX + i, screenBottomY + 1f, 0);
            
            GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
            if (prefabBG != null)
            {
                GameObject bg = Instantiate(prefabBG, m_slotPositions[i], Quaternion.identity, root);
            }
        }
    }

    public void AddItem(Item item)
    {
        if (IsFull) return;

        m_items.Add(item);
        
        SortItems();
        UpdateItemPositions();
        
        // Delay match checking slightly to let items move, but we can do it immediately logically
        CheckForMatches();
    }

    private void CheckForMatches()
    {
        for (int i = 0; i < m_items.Count - 2; i++)
        {
            if (m_items[i] is NormalItem item1 && 
                m_items[i+1] is NormalItem item2 && 
                m_items[i+2] is NormalItem item3)
            {
                if (item1.ItemType == item2.ItemType && item2.ItemType == item3.ItemType)
                {
                    RemoveMatch(i, item1, item2, item3);
                    return;
                }
            }
        }
    }

    private void RemoveMatch(int startIndex, Item item1, Item item2, Item item3)
    {
        m_items.RemoveAt(startIndex + 2);
        m_items.RemoveAt(startIndex + 1);
        m_items.RemoveAt(startIndex);

        item1.ExplodeView();
        item2.ExplodeView();
        item3.ExplodeView();

        UpdateItemPositions();
    }

    private void SortItems()
    {
        m_items.Sort((a, b) => 
        {
            if (a is NormalItem na && b is NormalItem nb)
            {
                return na.ItemType.CompareTo(nb.ItemType);
            }
            return 0;
        });
    }

    private void UpdateItemPositions()
    {
        for (int i = 0; i < m_items.Count; i++)
        {
            m_items[i].View.DOMove(m_slotPositions[i], 0.3f);
        }
    }

    public int GetCountOfType(NormalItem.eNormalType type)
    {
        int count = 0;
        foreach (var item in m_items)
        {
            if (item is NormalItem ni && ni.ItemType == type) count++;
        }
        return count;
    }
}
