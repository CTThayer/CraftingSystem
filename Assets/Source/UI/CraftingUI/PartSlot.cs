using System.Collections;
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

    protected override void OnValidate()
    {
        base.OnValidate();
        //gameObject.name = _allowedPartTypes[i] + "Slot";
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

    public void OnAddToSlot(Storable storableObject)
    {
        storableObject.ReactivateInWorld(partSocket.transform, false);
        ItemPart part = storableObject.gameObject.GetComponent<ItemPart>();
        partSocket.AddPartToSocket(part);
    }

    public void OnRemoveFromSlot(Storable storableObject, Inventory storage)
    {
        if (!storage.IsFull())
        {
            storage.AddItem(storableObject);
            storableObject.AddToStorage(storage);
        }
        else
        {
            storableObject.transform.position = Vector3.zero;                   // TODO: Change this to use a Drop Location variable from either the Character, CraftingApparatus, or Inventory class.
        }
    }

}
