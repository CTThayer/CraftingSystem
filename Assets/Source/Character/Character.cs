using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    public Inventory inventory { get => _inventory; }

    [SerializeField] private EquipmentPanel _equipmentPanel;
    public EquipmentPanel equipmentPanel { get => _equipmentPanel; }

    [SerializeField] private ItemTooltip itemTooltip;
    [SerializeField] private Image draggableSlot;

    private ItemSlot draggedSlot;

    private void OnValidate()
    {
        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemTooltip>();
        itemTooltip.HideTooltip();
    }

    private void Awake()
    {
        // Setup Events
        // Right Click to Equip/Unequip
        _inventory.OnRightClickEvent += Equip;
        _equipmentPanel.OnRightClickEvent += Unequip;
        // Pointer Hover Tooltip
        _inventory.OnPointerEnterEvent += ShowTooltip;
        _inventory.OnPointerExitEvent += HideTooltip;
        _equipmentPanel.OnPointerEnterEvent += ShowTooltip;
        _equipmentPanel.OnPointerEnterEvent += HideTooltip;
        // Drag Event Handlers
        _inventory.OnBeginDragEvent += BeginDrag;
        _inventory.OnEndDragEvent += EndDrag;
        _inventory.OnDragEvent += Drag;
        //_inventory.OnDropEvent += Drop;
        _equipmentPanel.OnBeginDragEvent += BeginDrag;
        _equipmentPanel.OnEndDragEvent += EndDrag;
        _equipmentPanel.OnDragEvent += Drag;
        //_equipmentPanel.OnDropEvent += Drop;

    }
    
    private void Equip(ItemSlot itemSlot)
    {
        Equipable equipableItem = itemSlot.storedItem as Equipable;
        if (equipableItem != null)
        {
            Equip(equipableItem);
        }
    }

    private void Unequip(ItemSlot itemSlot)
    {
        Equipable equipableItem = itemSlot.storedItem as Equipable;
        if (equipableItem != null)
        {
            Unequip(equipableItem);
        }
    }

    private void ShowTooltip(ItemSlot itemSlot)
    {
        Storable storableItem = itemSlot.storedItem as Storable;
        if (storableItem != null)
        {
            itemTooltip.ShowTooltip(storableItem);
        }
    }

    private void HideTooltip(ItemSlot itemSlot)
    {
        itemTooltip.HideTooltip();
    }

    private void BeginDrag(ItemSlot itemSlot)
    {
        if (itemSlot != null)
        {
            draggedSlot = itemSlot;
            draggableSlot.sprite = draggedSlot.storedItem.icon;
            draggableSlot.transform.position = Input.mousePosition;
            draggableSlot.gameObject.SetActive(true);
            draggableSlot.enabled = true;
        }
    }

    private void EndDrag(ItemSlot itemSlot)
    {
        draggedSlot = null;
        draggableSlot.gameObject.SetActive(false);
        draggableSlot.enabled = false;
    }

    private void Drag(ItemSlot itemSlot)
    {
        if (draggableSlot.enabled)
            draggableSlot.transform.position = Input.mousePosition;
    }

    //private void Drop(ItemSlot dropSlot)
    //{
    //    if (draggedSlot == null)
    //        return;

    //    bool canDrop = dropSlot.CanReceiveItem(draggedSlot.storedItem);
    //    bool canSwap = draggedSlot.CanReceiveItem(dropSlot.storedItem);

    //    //if (dropSlot.CanReceiveItem(draggedSlot.storedItem)
    //    //    && draggedSlot.CanReceiveItem(dropSlot.storedItem))
    //    if (canDrop && canSwap)
    //    {
    //        Equipable dragItemEQ = draggedSlot.storedItem as Equipable;
    //        Equipable dropItemEQ = dropSlot.storedItem as Equipable;
    //        if (draggedSlot is EquipmentSlot)
    //        {
    //            if (dragItemEQ != null)
    //                dragItemEQ.Unequip(this);
    //            if (dropItemEQ != null)
    //                dragItemEQ.Equip(this);
    //        }
    //        if (dropSlot is EquipmentSlot)
    //        {
    //            if (dragItemEQ != null)
    //                dragItemEQ.Equip(this);
    //            if (dropItemEQ != null)
    //                dragItemEQ.Unequip(this);
    //        }
    //        Storable draggedItem = draggedSlot.storedItem;
    //        draggedSlot.storedItem = dropSlot.storedItem;
    //        dropSlot.storedItem = draggedItem;
    //    }
    //}

    private void Equip(Equipable equipableItem)
    {
        if (_inventory.RemoveItem(equipableItem))
        {
            Storable previousItem;
            if (_equipmentPanel.AddItem(equipableItem, out previousItem))
            {
                if (previousItem != null)
                    _inventory.AddItem(previousItem);
            }
            else
            {
                _inventory.AddItem(equipableItem);
            }
        }
    }

    private void Unequip(Equipable equipableItem)
    {
        Storable storableItem = equipableItem.gameObject.GetComponent<Storable>();       // TODO: this might not be necessary if Storable handles all UI 
        if (!_inventory.IsFull() && _equipmentPanel.RemoveItem(equipableItem))             // and Equipable is only used to physically add the object or if all refs are GOs
        {
            _inventory.AddItem(storableItem);
        }
    }

}
