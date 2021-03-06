﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSlot : ItemSlot
{
    [SerializeField] private string[] _allowedPartTypes;
    public string[] allowedPartTypes { get; }

    [SerializeField] private PartSocket _partSocket;
    public PartSocket partSocket
    {
        get => _partSocket;
        set
        {
            if (value != null)
                _partSocket = value;
            else
                Debug.LogError("ERROR: PartSlot: partSocket cannot be null!");
        }
    }

    public override bool CanReceiveItem(Storable storableObject)
    {
        if (storableObject == null)
            return true;
        ItemPart part = storableObject.gameObject.GetComponent<ItemPart>();
        return part != null && PartIsAllowedType(part);
    }

    private bool PartIsAllowedType(ItemPart part)
    {
        for (int i = 0; i < _allowedPartTypes.Length; i++)
        {
            if (part.partType == _allowedPartTypes[i])
                return true;
        }
        return false;
    }

    public override void AddToSlot(Storable storableObject)
    {
        if (storableObject != null)
        {
            ItemPart part = storableObject.gameObject.GetComponent<ItemPart>();
            Vector3 addLoc;
            if (partSocket.AddPartToSocket(part, out addLoc))
                storableObject.ReactivateInWorld(partSocket.transform, true);
        }
        storedItem = storableObject;
    }

    public override Storable RemoveFromSlot()
    {
        partSocket.RemovePartFromSocket();
        Storable prevStoredItem = storedItem;
        if (prevStoredItem != null)
        {
            prevStoredItem.DeactivateInWorld();
            storedItem = null;
        }
        return prevStoredItem;
    }

    /**************************** EDITOR FUNCTIONS ****************************/
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    /************************** END EDITOR FUNCTIONS **************************/

}
