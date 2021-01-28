using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StorageUI : MonoBehaviour, ISlotPanelIO
{
    [SerializeField] private Storage loadedStorage;
    [SerializeField] private ItemSlot[] storageSlots;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    void OnValidate()
    {
        storageSlots = GetComponentsInChildren<ItemSlot>();
        if (storageSlots == null || storageSlots.Length == 0)
        {
            Debug.LogError("StorageUI: No ItemSlot components were found " +
                            "on StorageUI gameObject.");
        }
        else
        {
            if (storageSlots != null)
                ClearPreviousState();
        }
    }

    private void Start()
    {
        for (int i = 0; i < storageSlots.Length; i++)
        {
            // Remove delegate if it was set previously to avoid double calling
            storageSlots[i].OnPointerEnterEvent -= OnPointerEnterEvent;
            storageSlots[i].OnPointerExitEvent -= OnPointerExitEvent;
            storageSlots[i].OnRightClickEvent -= OnRightClickEvent;
            storageSlots[i].OnBeginDragEvent -= OnBeginDragEvent;
            storageSlots[i].OnEndDragEvent -= OnEndDragEvent;
            storageSlots[i].OnDragEvent -= OnDragEvent;
            storageSlots[i].OnDropEvent -= OnDropEvent;

            // Add delegates
            storageSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            storageSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            storageSlots[i].OnRightClickEvent += OnRightClickEvent;
            storageSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            storageSlots[i].OnEndDragEvent += OnEndDragEvent;
            storageSlots[i].OnDragEvent += OnDragEvent;
            storageSlots[i].OnDropEvent += OnDropEvent;
        }
    }

    public bool SetDelegateActions(Action<ItemSlot>[] delegates)
    {
        if (delegates != null && delegates.Length == 7)
        {
            // Remove delegate if it was set previously to avoid double calling
            OnPointerEnterEvent -= delegates[0];
            OnPointerExitEvent -= delegates[1];
            OnRightClickEvent -= delegates[2];
            OnBeginDragEvent -= delegates[3];
            OnEndDragEvent -= delegates[4];
            OnDragEvent -= delegates[5];
            OnDropEvent -= delegates[6];

            // Add delegates
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

    public bool SetStorage(Storage storage)
    {
        if (storage != null)
        {
            loadedStorage = storage;
            ClearPreviousState();
            UpdateAddRemoveDelegates();
            //LoadStorageContents(storage.storedItems);
            LoadStorageContents();
            return true;
        }
        else
        {
            loadedStorage = storage;
            ClearPreviousState();
            return false;
        }
    }

    public void LoadStorageContents()
    {
        if (loadedStorage != null)
        {
            Storable[] items = loadedStorage.storedItems;
            int count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (count < storageSlots.Length)
                {
                    // Set delegates
                    storageSlots[i].OnAddSpecific = loadedStorage.AddItem;
                    storageSlots[i].OnRemoveSpecific = loadedStorage.RemoveItem;
                    // set slot index
                    storageSlots[i].index = i;
                    // add item
                    storageSlots[i].AddToSlot(items[i]);
                    count++;
                }
                else return;
            }
        }
    }

    private void ClearPreviousState()
    {
        for (int i = 0; i < storageSlots.Length; i++)
        {
            storageSlots[i].storedItem = null;

            // TODO: May need to deactivate the ItemSlot or the object here
            // to ensure that the number of active slots is never higher than
            // the number of slots in the storage.
        }
    }

    private void UpdateAddRemoveDelegates()
    {
        for (int i = 0; i < storageSlots.Length; i++)
        {
            storageSlots[i].OnAddSpecific = loadedStorage.AddItem;
            storageSlots[i].OnRemoveSpecific = loadedStorage.RemoveItem;
        }
    }

    public ItemSlot CanAdd(ItemSlot input)
    {
        if (loadedStorage.StorableFitsInStorage(input.storedItem))
        {
            int index;
            if (loadedStorage.GetEmptySlot(out index))
            {
                if (index < storageSlots.Length)
                    return storageSlots[index];
            }
        }
        return null;
    }
}
