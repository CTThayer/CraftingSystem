using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Storable> startingItems;
    [SerializeField] private Transform itemParent;
    [SerializeField] private ItemSlot[] itemSlots;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            itemSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            itemSlots[i].OnRightClickEvent += OnRightClickEvent;
            itemSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            itemSlots[i].OnEndDragEvent += OnEndDragEvent;
            itemSlots[i].OnDragEvent += OnDragEvent;
            itemSlots[i].OnDropEvent += OnDropEvent;
        }

        SetStartingItems();    // Ensures item references get set in a BUILD
    }

    // Called in the editor only (unless explicitly called elsewhere). This is
    // called when the script is loaded or a value is changed in the inspector.
    private void OnValidate()
    {
        if (itemParent != null)
            itemSlots = itemParent.GetComponentsInChildren<ItemSlot>();

        SetStartingItems();    // Allows UI to refresh in the editor
    }

    // Refreshes UI to match current list of storables
    private void SetStartingItems()
    {
        int i = 0;
        for (; i < startingItems.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].storedItem = startingItems[i];
        }
        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].storedItem = null;
        }
    }

    public bool AddItem(Storable item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].storedItem == null)
            {
                itemSlots[i].storedItem = item;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Storable item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].storedItem == item)
            {
                itemSlots[i].storedItem = null;
                return true;
            }
        }
        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].storedItem == null)
            {
                return false;
            }
        }
        return true;
    }

}
