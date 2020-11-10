using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Storage : MonoBehaviour
{
    [SerializeField] private GameObject storageParent;

    [SerializeField] private List<Storable> startingItems;                      // TODO: Remove this & all refs
    [SerializeField] private ItemSlot[] storageSlots;
    [SerializeField] private Vector2 slotLayout;

    [SerializeField] private Vector3 storageDimensions;
    [SerializeField] private float volumeCapacity;
    [SerializeField] private float volumeCurrentlyUsed;
    [SerializeField] private float massCapacity;
    [SerializeField] private float massCurrentlyUsed;

    public event Action<ItemSlot> OnItemRightClickEvent;
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    //private void Awake()
    private void Start()
    {
        Debug.Assert(storageSlots.Length - 1 <= slotLayout.x * slotLayout.y);

        for (int i = 0; i < storageSlots.Length; i++)
        {
            storageSlots[i].OnRightClickEvent += OnItemRightClickEvent;
            storageSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            storageSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            storageSlots[i].OnRightClickEvent += OnRightClickEvent;
            storageSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            storageSlots[i].OnEndDragEvent += OnEndDragEvent;
            storageSlots[i].OnDragEvent += OnDragEvent;
            storageSlots[i].OnDropEvent += OnDropEvent;
        }

        SetStartingItems();    // Ensures item references get set in a BUILD
    }

    // Called in the editor only (unless explicitly called elsewhere). This is
    // called when the script is loaded or a value is changed in the inspector.
    private void OnValidate()
    {
        if (storageParent != null)
            storageSlots = storageParent.GetComponentsInChildren<ItemSlot>();

        SetStartingItems();    // Allows UI to refresh in the editor
    }

    // Refreshes UI to match current list of storables
    private void SetStartingItems()
    {
        int i = 0;
        for (; i < startingItems.Count && i < storageSlots.Length; i++)
        {
            float m = startingItems[i].objectPhysicalStats.mass;
            float v = startingItems[i].objectPhysicalStats.volume;
            if (massCurrentlyUsed + m < massCapacity
                && volumeCurrentlyUsed + v < volumeCapacity)
            {
                storageSlots[i].storedItem = startingItems[i];
            }
            else
            {
                storageSlots[i].storedItem = null;
            }
        }
        for (; i < storageSlots.Length; i++)
        {
            storageSlots[i].storedItem = null;
        }
    }

    public bool AddItem(Storable storableObject)
    {
        float m = storableObject.objectPhysicalStats.mass;
        float v = storableObject.objectPhysicalStats.volume;
        if (massCurrentlyUsed + m < massCapacity
            && volumeCurrentlyUsed + v < volumeCapacity)
        {
            for (int i = 0; i < storageSlots.Length; i++)
            {
                if (storageSlots[i].storedItem == null)
                {
                    storageSlots[i].storedItem = storableObject;
                    massCurrentlyUsed += m;
                    volumeCurrentlyUsed += v;
                    storableObject.DeactivateInWorld(this);
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItem(Storable storableObject)
    {
        for (int i = 0; i < storageSlots.Length; i++)
        {
            if (storageSlots[i].storedItem == storableObject)
            {
                storageSlots[i].storedItem = null;
                massCurrentlyUsed -= storableObject.objectPhysicalStats.mass;
                volumeCurrentlyUsed -= storableObject.objectPhysicalStats.volume;

                // TODO: Where should we reactivate the object in the world?

                return true;
            }
        }
        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < storageSlots.Length; i++)
        {
            if (storageSlots[i].storedItem == null)
            {
                return false;
            }
        }
        return true;
    }
}
