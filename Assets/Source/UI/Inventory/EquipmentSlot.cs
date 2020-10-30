using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    public EquipmentType equipmentType;
    
    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = equipmentType.ToString() + "Slot";
    }

    public override bool CanReceiveItem(Storable item)
    {
        if (item == null)
            return true;
        Equipable equipableItem = item as Equipable;
        return equipableItem != null && equipableItem.equipmentType == equipmentType;
    }
}
