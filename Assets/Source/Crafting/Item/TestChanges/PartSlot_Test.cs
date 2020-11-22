using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSlot_Test : ItemSlot
{

    [SerializeField] private string[] _allowedPartTypes;
    public string[] allowedPartTypes { get; }

    [SerializeField] private PartLayout _partLayout;
    public PartLayout partLayout
    {
        get => _partLayout;
        set
        {
            if (value != null)
                _partLayout = value;
            else
                Debug.LogError("ERROR: PartSlot: partSocket cannot be null!");
        }
    }

    [SerializeField] private int _indexInLayout;
    public int indexInLayout { get => _indexInLayout; }

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
            bool success = partLayout.Add(indexInLayout, part);
            if (success)
                storedItem = storableObject;
        }
    }

    public override Storable RemoveFromSlot()
    {
        ItemPart part;
        bool success = partLayout.Remove(indexInLayout, out part);
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
