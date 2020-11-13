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

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

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
        if (storableObject != null)
            storableObject.DeactivateInWorld();
        storedItem = storableObject;
    }

    public virtual Storable RemoveFromSlot()
    {
        Storable prevItem = storedItem;
        storedItem = null;
        return prevItem;
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
