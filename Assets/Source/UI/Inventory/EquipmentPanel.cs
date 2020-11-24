using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentPanel : MonoBehaviour, ISlotPanelIO
{
    [SerializeField] private Transform EquipmentSlotsParent;
    [SerializeField] private EquipmentSlot[] equipmentSlots;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private void Start()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            equipmentSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            equipmentSlots[i].OnRightClickEvent += OnRightClickEvent;
            equipmentSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            equipmentSlots[i].OnEndDragEvent += OnEndDragEvent;
            equipmentSlots[i].OnDragEvent += OnDragEvent;
            equipmentSlots[i].OnDropEvent += OnDropEvent;

            // TODO: Set bone associated with each slot (if not set manually)
        }
    }

    private void OnValidate()
    {
        equipmentSlots = EquipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    public bool AddItem(Equipable item, out Storable previousItem)             // Might need to change the out to a generic gameObject 
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].equipmentType == item.equipmentType)
            {
                previousItem = equipmentSlots[i].storedItem;
                equipmentSlots[i].storedItem = item.gameObject.GetComponent<Storable>();
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    public bool RemoveItem(Equipable item)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].equipmentType == item.equipmentType)
            {
                equipmentSlots[i].storedItem = null;
                return true;
            }
        }
        return false;
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
        Equipable E = input.storedItem as Equipable;
        if (E != null)
        {
            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                if (equipmentSlots[i].equipmentType == E.equipmentType)
                {
                    return equipmentSlots[i];
                }
            }
            return null;
        }
        return null;
    }
}
