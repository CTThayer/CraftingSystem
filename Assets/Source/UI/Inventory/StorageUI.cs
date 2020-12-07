using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StorageUI : MonoBehaviour, ISlotPanelIO
{
    [SerializeField] private Storage loadedStorage;
    [SerializeField] private ItemSlot[] storageSlots;

    //[SerializeField] private int maxHeight;
    //[SerializeField] private int maxWidth;

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
        //Debug.Assert(storageSlots.Length - 1 <= maxWidth * maxHeight);

        for (int i = 0; i < storageSlots.Length; i++)
        {
            storageSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            storageSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            storageSlots[i].OnRightClickEvent += OnRightClickEvent;
            storageSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            storageSlots[i].OnEndDragEvent += OnEndDragEvent;
            storageSlots[i].OnDragEvent += OnDragEvent;
            storageSlots[i].OnDropEvent += OnDropEvent;

            //if (loadedStorage != null)
            //{
            //    storageSlots[i].OnAddSpecific += loadedStorage.AddItem;
            //    storageSlots[i].OnRemoveSpecific += loadedStorage.RemoveItem;
            //}
        }

        //ClearPreviousState();
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

    public bool SetStorage(Storage storage)
    {
        if (storage != null)
        {
            loadedStorage = storage;
            ClearPreviousState();
            //UpdateAddRemoveDelegates();
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

    /* Load Storage Contents
     * Performs all of the setup for the storage slots
     * Truncates the displayed inventory if it is larger than the amound of 
     * slots in this StorageUI. This isn't ideal, but it should work for now.
     */
    //public void LoadStorageContents(Storable[,] storedItems)
    //{
    //    if (storedItems != null)
    //    {
    //        int rows = storedItems.GetLength(0);
    //        int columns = storedItems.GetLength(1);
    //        int count = 0;
    //        for (int x = 0; x < rows; x++)
    //        {
    //            for (int y = 0; y < columns; y++)
    //            {
    //                if (count < storageSlots.Length)
    //                {
    //                    int slotIndex = (x * rows) + y;
    //                    storageSlots[slotIndex].OnAddSpecific += loadedStorage.AddItem;
    //                    storageSlots[slotIndex].OnRemoveSpecific += loadedStorage.RemoveItem;
    //                    storageSlots[slotIndex].xIndex = x;
    //                    storageSlots[slotIndex].yIndex = y;
    //                    storageSlots[slotIndex].AddToSlot(storedItems[x, y]);
    //                    count++;
    //                }
    //                else return;
    //            }
    //        }
    //    }
    //}


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
            // to ensure that only a number of active slots is never higher than
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

    //public ItemSlot CanAdd(ItemSlot input)
    //{
    //    if (loadedStorage.StorableFitsInStorage(input.storedItem))
    //    {
    //        int x, y;
    //        if (loadedStorage.GetEmptySlot(out x, out y))
    //        {
    //            int index = (loadedStorage.rows * x) + y;
    //            if (index < storageSlots.Length)
    //                return storageSlots[index];
    //        }
    //    }
    //    return null;
    //}

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
