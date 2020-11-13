using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TwoPanelController : MonoBehaviour
{
    public GameObject panelA;
    public GameObject panelB;
    private ISlotPanelIO panelAScript;
    private ISlotPanelIO panelBScript;

    // TODO: Create a generic SlotTooltip and make ItemToolTip, PartSlotTooltip, etc. inherit from it.
    [SerializeField] private ItemTooltip itemTooltip;
    [SerializeField] private Image draggableSlot;

    private ItemSlot draggedSlot;

    SlotContentSwapper swapper = new SlotContentSwapper();

    private void OnValidate()
    {
        panelAScript = panelA.GetComponent<ISlotPanelIO>();
        panelBScript = panelB.GetComponent<ISlotPanelIO>();
    }

    void Awake()
    {
        Debug.Assert(draggableSlot != null);
        Debug.Assert(panelAScript != null);
        Debug.Assert(panelBScript != null);
        ConfigDelegates();
    }

    public void ConfigDelegates()
    {
        Action<ItemSlot>[] delegatesA = new Action<ItemSlot>[7]
        {
            ShowTooltip,
            HideTooltip,
            PanelARightClick,
            BeginDrag,
            EndDrag,
            Drag,
            Drop
        };
        Action<ItemSlot>[] delegatesB = new Action<ItemSlot>[7]
        {
            ShowTooltip,
            HideTooltip,
            PanelBRightClick,
            BeginDrag,
            EndDrag,
            Drag,
            Drop
        };
        panelAScript.SetDelegateActions(delegatesA);
        panelBScript.SetDelegateActions(delegatesB);
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

    private void Drop(ItemSlot dropSlot)
    {
        if (draggedSlot == null)
            return;

        bool canDrop = dropSlot.CanReceiveItem(draggedSlot.storedItem);
        bool canSwap = draggedSlot.CanReceiveItem(dropSlot.storedItem);

        if (canDrop && canSwap)
        {
            swapper.Swap(dropSlot, draggedSlot);
        }
    }

    private void PanelARightClick(ItemSlot itemSlotA)
    {
        ItemSlot itemSlotB = panelBScript.CanAdd(itemSlotA);
        if (itemSlotB != null)
        {
            swapper.Swap(itemSlotA, itemSlotB);
        }
    }

    private void PanelBRightClick(ItemSlot itemSlotB)
    {
        ItemSlot itemSlotA = panelAScript.CanAdd(itemSlotB);
        if (itemSlotA != null)
        {
            swapper.Swap(itemSlotA, itemSlotB);
        }
    }

    public void MoveItemBetweenPanels(ItemSlot slot, bool fromAtoB)
    {
        if (fromAtoB)
            PanelARightClick(slot);
        else
            PanelBRightClick(slot);
    }
}
