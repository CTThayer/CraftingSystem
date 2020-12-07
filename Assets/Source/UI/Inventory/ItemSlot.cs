using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, 
                        IPointerClickHandler, 
                        IPointerEnterHandler, 
                        IPointerExitHandler, 
                        IDragHandler, 
                        IBeginDragHandler, 
                        IEndDragHandler,
                        IDropHandler
{
    [SerializeField] private Image image;

    // Event callbacks for UI actions
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    // Delegate to add/remove from the array in the Storage class
    //public delegate bool AddSpecific(Storable s, int x, int y);
    public delegate bool AddSpecific(Storable s, int index);
    public AddSpecific OnAddSpecific;
    //public delegate Storable RemoveSpecific(int x, int y);
    public delegate Storable RemoveSpecific(int index);
    public RemoveSpecific OnRemoveSpecific;

    // Indices in Storage's array that this slot corresponds to
    public int index;
    //public int xIndex;
    //public int yIndex;

    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0);

    [SerializeField] private Storable _storedItem;
    public Storable storedItem
    {
        get { return _storedItem; }
        set
        {
            _storedItem = value;
            if (_storedItem == null)
            {
                image.color = disabledColor;
            }
            else
            {
                image.sprite = _storedItem.icon;
                image.color = normalColor;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterEvent != null)
            OnPointerEnterEvent(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitEvent != null)
            OnPointerExitEvent(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (OnRightClickEvent != null)
                OnRightClickEvent(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragEvent != null)
            OnBeginDragEvent(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragEvent != null)
            OnEndDragEvent(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropEvent != null)
            OnDropEvent(this);
    }

    public virtual bool CanReceiveItem(Storable item)
    {
        return true;
    }

    public virtual void AddToSlot(Storable storableObject)
    {
        //if (storableObject != null)
        //    storableObject.DeactivateInWorld();                                 // Do we actually do this here?
        //if (xIndex >= 0 && yIndex >= 0)
        //{
        //    OnAddSpecific(storableObject, xIndex, yIndex);
        //}
        if (index >= 0)
            OnAddSpecific(storableObject, index);
        storedItem = storableObject;
    }

    public virtual Storable RemoveFromSlot()
    {
        //Storable prevItem = storedItem;
        Storable s = null;
        //if (xIndex >= 0 && yIndex >= 0)
        //{
        //    s = OnRemoveSpecific(xIndex, yIndex);
        if (index >= 0)
        {
            s = OnRemoveSpecific(index);
            //if (prevItem != s)
            //{
            //    Debug.LogError("ItemSlot: RemoveFromSlot(): Contents of slot " +
            //        "do not match corresponding storage contents. Check if " +
            //        " Storage and StorageUI are configured correctly and " +
            //        "check if slot indices match intended Storage indices.");
            //}
        }
        storedItem = null;
        return s;
    }

    /**************************** EDITOR FUNCTIONS ****************************/
    // Make sure the image is set up correctly when validated
    protected virtual void OnValidate()
    {
        if (image == null)
            image = GetComponent<Image>();
    }
    /************************** END EDITOR FUNCTIONS **************************/
}
