using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PartPanel : MonoBehaviour, ISlotPanelIO
{
    [SerializeField] private GameObject partLayoutParent;
    [SerializeField] private GameObject partLayout;                             // TODO: Make this public? or Property?
    [SerializeField] private PartSlot[] _partSlots;
    public PartSlot[] partSlots { get => _partSlots; }

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
        Debug.Assert(partLayoutParent != null);
        Debug.Assert(partLayoutParent.GetComponent<RectTransform>() != null);

        ConfigurePartSlots();
    }

    // Called in the editor only (unless explicitly called elsewhere). This is
    // called when the script is loaded or a value is changed in the inspector.
    private void OnValidate()
    {
        if (partLayout != null)
            LoadPartLayout(partLayout);
    }

    public bool LoadPartLayout(GameObject partLayout)
    {
        if (partLayout != null)
        {
            partLayout.transform.parent = partLayoutParent.transform;
            PartSlot[] layoutSlots = partLayout.GetComponentsInChildren<PartSlot>();
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
        if (input != null && input.storedItem != null)
        {
            ItemPart itemPart = input.storedItem.GetComponent<ItemPart>();
            if (itemPart == null)
                return null;
            else
            {
                for (int i = 0; i < _partSlots.Length; i++)
                {
                    if (_partSlots[i].CanReceiveItem(input.storedItem))
                    {
                        return _partSlots[i];
                    }
                }
                return null;
            }
        }
        return null;
    }
}
