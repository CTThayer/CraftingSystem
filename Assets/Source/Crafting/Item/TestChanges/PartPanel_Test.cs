﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PartPanel_Test : MonoBehaviour, ISlotPanelIO
{
    [SerializeField] private GameObject partLayoutUIParent;
    [SerializeField] private GameObject partLayoutUI;                             // TODO: Make this public? or Property?
    [SerializeField] private PartSlot_Test[] _partSlots;
    public PartSlot_Test[] partSlots { get => _partSlots; }

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(partLayoutUIParent != null);
        Debug.Assert(partLayoutUIParent.GetComponent<RectTransform>() != null);

        ConfigurePartSlots();
    }

    // Called in the editor only (unless explicitly called elsewhere). This is
    // called when the script is loaded or a value is changed in the inspector.
    private void OnValidate()
    {
        if (partLayoutUI != null)
        {
            if (partLayoutUIParent != partLayoutUI)
                LoadPartLayout(partLayoutUI);
            else
                Debug.LogError("PartPanel: PartLayoutUI is set to the same" +
                               "object as partLayoutParent.");
        }
        else
        {
            Debug.LogError("PartPanel: PartLayoutUI is not set to an object.");
        }
    }

    public bool LoadPartLayout(GameObject partLayout)
    {
        if (partLayout != null)
        {
            partLayout.transform.parent = partLayoutUIParent.transform;
            PartSlot_Test[] layoutSlots = partLayout.GetComponentsInChildren<PartSlot_Test>();
            _partSlots = layoutSlots;
            ConfigurePartSlots();
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ConfigurePartSlots()
    {
        if (_partSlots != null && _partSlots.Length > 0)
        {
            for (int i = 0; i < _partSlots.Length; i++)
            {
                // Remove delegate if it was set previously to avoid double calling
                _partSlots[i].OnPointerEnterEvent -= OnPointerEnterEvent;
                _partSlots[i].OnPointerExitEvent -= OnPointerExitEvent;
                _partSlots[i].OnRightClickEvent -= OnRightClickEvent;
                _partSlots[i].OnBeginDragEvent -= OnBeginDragEvent;
                _partSlots[i].OnEndDragEvent -= OnEndDragEvent;
                _partSlots[i].OnDragEvent -= OnDragEvent;
                _partSlots[i].OnDropEvent -= OnDropEvent;

                // Add delegates
                _partSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
                _partSlots[i].OnPointerExitEvent += OnPointerExitEvent;
                _partSlots[i].OnRightClickEvent += OnRightClickEvent;
                _partSlots[i].OnBeginDragEvent += OnBeginDragEvent;
                _partSlots[i].OnEndDragEvent += OnEndDragEvent;
                _partSlots[i].OnDragEvent += OnDragEvent;
                _partSlots[i].OnDropEvent += OnDropEvent;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public ItemSlot CanAdd(ItemSlot input)
    {
        if (input != null && input.storedItem != null)
        {
            ItemPart itemPart = input.storedItem.GetComponent<ItemPart>();
            if (itemPart == null)
                return null;
            else
            {
                for (int i = 0; i < _partSlots.Length; i++)
                {
                    if (_partSlots[i].storedItem == null
                        && _partSlots[i].CanReceiveItem(input.storedItem))
                    {
                        return _partSlots[i];
                    }
                }
                return null;
            }
        }
        return null;
    }

    public bool AddPart(Storable storableItem, out Storable previousStorable)
    {
        for (int i = 0; i < _partSlots.Length; i++)
        {
            if (_partSlots[i].CanReceiveItem(storableItem))
            {
                previousStorable = _partSlots[i].RemoveFromSlot();
                _partSlots[i].AddToSlot(storableItem);
                return true;
            }
        }
        previousStorable = null;
        return false;
    }

    public bool RemovePart(Storable item)
    {
        for (int i = 0; i < _partSlots.Length; i++)
        {
            if (_partSlots[i].storedItem == item)
            {
                _partSlots[i].storedItem = null;
                return true;
            }
        }
        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < _partSlots.Length; i++)
        {
            if (_partSlots[i].storedItem == null)
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

}
