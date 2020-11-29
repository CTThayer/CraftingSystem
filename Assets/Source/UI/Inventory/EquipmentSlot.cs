using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    public EquipmentType equipmentType;

    [SerializeField] private GameObject _equipLocation;
    public GameObject equipLocation
    {
        get => _equipLocation;
        private set
        {
            if (value != null)
                _equipLocation = value;
        }
    }

    /* EquipToBone()
     * Parents the newly equiped item to the bone associated with this slot.
     * NOTE: The storable is Reactivated because we know it needs to be since it
     * is now attached to a bone in the game world.
     */
    public void EquipToBone(Equipable equipableItem)
    {
        Storable s = equipableItem as Storable;
        s.ReactivateInWorld(equipLocation.transform, true);
        s.transform.parent = equipLocation.transform;
    }

    //private void EquipToBone(Storable storableObj)
    //{
    //    storableObj.ReactivateInWorld(equipLocation.transform, true);
    //    storableObj.transform.parent = equipLocation.transform;
    //}

    /* UnequipFromBone()
     * Unparents the stored item from the bone associated with this slot and 
     * returns it.
     * NOTE: The storable is NOT reactivated or deactivated here because this
     * method is not aware of whether it is going back to storage or if it is 
     * being dropped into the world.
     */
    public Storable UnequipFromBone()
    {
        if (this.storedItem != null)
        {
            this.storedItem.transform.parent = null;
            return this.storedItem;
        }
        else
        {
            return null;
        }
    }

    public override bool CanReceiveItem(Storable item)
    {
        if (item == null)
            return true;
        Equipable equipableItem = item as Equipable;
        return equipableItem != null && equipableItem.equipmentType == equipmentType;
    }

    public override void AddToSlot(Storable storableObject)
    {
        if (storableObject != null && storedItem == null)
        {
            EquipToBone(storableObject as Equipable);
            //Debug.Log("Added " + storableObject.name + " to EquipmentSlot");
        }
        storedItem = storableObject;
    }

    public override Storable RemoveFromSlot()
    {
        Storable prevStoredItem = UnequipFromBone();
        if (prevStoredItem != null)
        {
            prevStoredItem.DeactivateInWorld();
            storedItem = null;
        }
        return prevStoredItem;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = equipmentType.ToString() + "Slot";
    }

}
