using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentPanel equipmentPanel;

    private void Awake()
    {
        //inventory.OnItemRightClickEvent += EquipFromInventory;
        //equipmentPanel.OnItemRightClickEvent += UnequipFromEquipmentPanel;
    }

    // Intermediary methods to perform the necessary cast from a storable to an
    // equipable so that the Action<T> delegate can convert between storable and
    // equipable when shifting between inventory and equipment panel in order to
    // call the Equip(Equipable item) method here from other code. 
    private void EquipFromInventory(Storable item)
    {
        if (item is Equipable)
        {
            Equip((Equipable)item);
        }
    }

    private void UnequipFromEquipmentPanel(Storable item)
    {
        if (item is Equipable)
        {
            Unequip((Equipable)item);
        }
    }

    public void Equip(Equipable item)
    {
        //Storable storableItem = item.gameObject.GetComponent<Storable>();     // TODO: this might not be necessary if Storable handles all UI 
                                                                                // and Equipable is only used to physically add the object or if all refs are GOs
        //if (inventory.RemoveItem(storableItem))
        if (inventory.RemoveItem(item))
        {
            Storable previousItem;
            if (equipmentPanel.AddItem(item, out previousItem))
            {
                if (previousItem != null)
                    inventory.AddItem(previousItem);
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }

    public void Unequip(Equipable item)
    {
        Storable storableItem = item.gameObject.GetComponent<Storable>();       // TODO: this might not be necessary if Storable handles all UI 
        if (!inventory.IsFull() && equipmentPanel.RemoveItem(item))             // and Equipable is only used to physically add the object or if all refs are GOs
        {
            inventory.AddItem(storableItem);
        }
    }
}
