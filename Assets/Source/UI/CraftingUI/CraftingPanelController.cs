using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPanelController : MonoBehaviour
{
    [SerializeField] private Storage storage;
    [SerializeField] private PartPanel partPanel;
    [SerializeField] private PartSlotTooltip partSlotTooltip;
    [SerializeField] private Image draggableSlot;

    private ItemSlot draggedSlot;

    private void OnValidate()
    {
        if (partSlotTooltip == null)
            partSlotTooltip = FindObjectOfType<PartSlotTooltip>();
        partSlotTooltip.HideTooltip();
    }

    private void Awake()
    {
        // Set Up Events
        // Right Click to Equip/Unequip
        storage.OnRightClickEvent += AddPart;
        partPanel.OnRightClickEvent += RemovePart;
        // Pointer Hover Tooltip
        storage.OnPointerEnterEvent += ShowTooltip;
        storage.OnPointerExitEvent += HideTooltip;
        partPanel.OnPointerEnterEvent += ShowTooltip;
        partPanel.OnPointerEnterEvent += HideTooltip;
        // Drag Event Handlers
        storage.OnBeginDragEvent += BeginDrag;
        storage.OnEndDragEvent += EndDrag;
        storage.OnDragEvent += Drag;
        storage.OnDropEvent += Drop;
        partPanel.OnBeginDragEvent += BeginDrag;
        partPanel.OnEndDragEvent += EndDrag;
        partPanel.OnDragEvent += Drag;
        partPanel.OnDropEvent += Drop;

    }

    private void AddPart(ItemSlot itemSlot)
    {
        ItemPart itemPart = itemSlot.storedItem.GetComponent<ItemPart>();
        if (itemPart != null)
        {
            AddPartToPanel(itemPart, itemSlot.storedItem);
        }
    }

    private bool AddPartToPanel(ItemPart itemPart, Storable storable)
    {
        if (storage.RemoveItem(storable))
        {
            //Storable previousItem;
            //if (partPanel.AddPart(itemPart, out previousItem))
            //{
            //    if (previousItem != null)
            //        inventory.AddItem(previousItem);
            //}
            //else
            //{
            //    inventory.AddItem(equipableItem);
            //}
            return true;
        }
        return false;
    }

    private void RemovePart(ItemSlot itemSlot)
    {
        ItemPart itemPart = itemSlot.storedItem.GetComponent<ItemPart>();
        if (itemPart != null)
        {
            RemovePartFromPanel(itemPart);
        }
    }

    private bool RemovePartFromPanel(ItemPart itemPart)
    {

        return false;
    }

    private void ShowTooltip(ItemSlot itemSlot)
    {
        PartSlot partSlot = itemSlot as PartSlot;
        if (partSlot != null)
        {
            partSlotTooltip.ShowTooltip(partSlot);
        }
    }

    private void HideTooltip(ItemSlot itemSlot)
    {
        partSlotTooltip.HideTooltip();
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

    private void Drop(ItemSlot dropSlot)
    {
        if (draggedSlot == null)
            return;

        bool canDrop = dropSlot.CanReceiveItem(draggedSlot.storedItem);
        bool canSwap = draggedSlot.CanReceiveItem(dropSlot.storedItem);

        //if (canDrop && canSwap)
        //{
        //    ItemPart dragItemPart = draggedSlot.storedItem.GetComponent<ItemPart>();
        //    ItemPart dropItemPart = dropSlot.storedItem.GetComponent<ItemPart>();
        //    if (draggedSlot is PartSlot)
        //    {
        //        if (dragItemPart != null)
        //            dragItemPart.Unequip(this);
        //        if (dropItemPart != null)
        //            dragItemPart.Equip(this);
        //    }
        //    if (dropSlot is PartSlot)
        //    {
        //        if (dragItemPart != null)
        //            dragItemPart.Equip(this);
        //        if (dropItemPart != null)
        //            dragItemEQ.Unequip(this);
        //    }
        //    Storable draggedItem = draggedSlot.storedItem;
        //    draggedSlot.storedItem = dropSlot.storedItem;
        //    dropSlot.storedItem = draggedItem;
        //}
    }

}
