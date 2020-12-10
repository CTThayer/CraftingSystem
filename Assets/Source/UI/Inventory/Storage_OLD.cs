using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Storage_OLD : MonoBehaviour, ISlotPanelIO
{
    [SerializeField] private GameObject uiSlotsParent;

    [SerializeField] private List<Storable> startingItems;
    private bool startIsConfigured = false;
    [SerializeField] private ItemSlot[] storageSlots;
    [SerializeField] private Vector2 slotLayout;

    [SerializeField] private Vector3 storageDimensions;
    [SerializeField] private float volumeCapacity;
    [SerializeField] private float volumeCurrentlyUsed;
    [SerializeField] private float massCapacity;
    [SerializeField] private float massCurrentlyUsed;

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
            storageSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            storageSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            storageSlots[i].OnRightClickEvent += OnRightClickEvent;
            storageSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            storageSlots[i].OnEndDragEvent += OnEndDragEvent;
            storageSlots[i].OnDragEvent += OnDragEvent;
            storageSlots[i].OnDropEvent += OnDropEvent;
        }

        // Ensures item references get set in a BUILD
        ClearPreviousState();
        SetStartingItems();
    }

    // Refreshes UI to match current list of starting storables
    private void SetStartingItems()
    {
        int i = 0;
        int lowerCount = startingItems.Count < storageSlots.Length ? startingItems.Count : storageSlots.Length;
        for (; i < lowerCount; i++)
        {
            AddItem(startingItems[i]);
        }
        for (; i < storageSlots.Length; i++)
        {
            storageSlots[i].storedItem = null;
        }
    }

    // Attempts to add the specified storable to the storage.
    public bool AddItem(Storable storableObject)
    {
        float m = storableObject.physicalStats.mass;
        float v = storableObject.physicalStats.volume;
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
                    storableObject.DeactivateInWorld();
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
                massCurrentlyUsed -= storableObject.physicalStats.mass;
                volumeCurrentlyUsed -= storableObject.physicalStats.volume;

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

    public bool SetDelegateActions(Action<ItemSlot>[] delegates)
    {
        if (delegates != null && delegates.Length == 7)
        {
            OnPointerEnterEvent += delegates[0];
            OnPointerExitEvent += delegates[1];
            OnRightClickEvent += delegates[2];
            OnBeginDragEvent += delegates[3];
            OnEndDragEvent += delegates[4];
            OnDragEvent += delegates[5];
            OnDropEvent += delegates[6];
            return true;
        }
        return false;
    }

    public ItemSlot CanAdd(ItemSlot input)
    {
        Storable storableObject = input.storedItem;
        float m = storableObject.physicalStats.mass;
        float v = storableObject.physicalStats.volume;
        if (massCurrentlyUsed + m < massCapacity
            && volumeCurrentlyUsed + v < volumeCapacity)
        {
            for (int i = 0; i < storageSlots.Length; i++)
            {
                if (storageSlots[i].storedItem == null)
                {
                    return storageSlots[i];
                }
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    // Ensures clean state before setting up "startingItems" in storage
    private void ClearPreviousState()
    {
        for (int i = 0; i < storageSlots.Length; i++)
        {
            if (storageSlots[i].storedItem != null)
            {
                Storable sObj = storageSlots[i].storedItem;
                Rigidbody r = sObj.GetComponent<Rigidbody>();
                sObj.ReactivateInWorld(sObj.transform, r.isKinematic);
            }
            storageSlots[i].storedItem = null;
        }
        massCurrentlyUsed = 0;
        volumeCurrentlyUsed = 0;
    }

    /**************************** EDITOR FUNCTIONS ****************************/
    // Called in the editor only (unless explicitly called elsewhere). This is
    // called when the script is loaded or a value is changed in the inspector.
    private void OnValidate()
    {
        ClearPreviousState();

        if (uiSlotsParent != null)
            storageSlots = uiSlotsParent.GetComponentsInChildren<ItemSlot>();

        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                startingItems[i].Initialize(true);
        }

        // This line causes a Null Reference Exception at line 68 (float m...)
        // This seems to be b/c the startingItems[i] not being initialized when 
        // on validate for Storage.cs runs in editor b/c it works fine in playmode
        SetStartingItems();    // Allows UI to refresh in the editor
    }
    /************************** END EDITOR FUNCTIONS **************************/
}
