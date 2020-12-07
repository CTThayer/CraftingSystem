using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Update this list to reflect final choices for EquipmentTypes
public enum EquipmentType
{
    NOT_SET,
    Head,
    Chest,
    Arms,
    Legs,
    Feet,
    Belt,
    Gloves,
    HandEquip,
    Necklace,
    Ring,
    Accessory1,
    Accessory2,
    Other
}

//public class Equipable : MonoBehaviour, IActionable
public class Equipable : Storable, IActionable, IInitializer
{
    [SerializeField] private EquipmentType _equipmentType;
    public EquipmentType equipmentType
    {
        get => _equipmentType;
        private set => _equipmentType = value;
    }

    // TODO: Add field for storing what animation(s) go with this item when it is equipped to the character.

    public new void Initialize()
    {
        // Temp initialization sets EquipableType to HandEquip by default
        _equipmentType = EquipmentType.HandEquip;
        base.Initialize();
    }

    public void Initialize(EquipmentType equipmentType, Sprite icon)
    {
        // TODO: Implement initializer
    }

    /********************* IActionable Interface Members **********************/
    /* IActionable is used by Interactable to get all actions that can be taken
     * on any given object. These methods MUST be implemented for interaction 
     * to work correctly.                                                     */

    // Returns all the ActionDelegate methods associated with this component
    public override ActionDelegate[] GetActions()
    {
        List<ActionDelegate> actions = new List<ActionDelegate>(base.GetActions());
        actions.Add(Equip);
        return actions.ToArray();
    }

    // Returns all the UI display names for the ActionDelegate methods
    public override string[] GetActionNames()
    {
        List<string> actionNames = new List<string>(base.GetActionNames());
        actionNames.Add("Equip");
        return actionNames.ToArray();
    }

    /*********************** IActionable Event Members ************************/
    public string Equip(PlayerCharacter character)
    {
        string result = "";
        if (character != null)
        {
            Storable prevEquippedItem;
            if (character.equipmentPanel.AddItem(this, out prevEquippedItem))
            {
                if (prevEquippedItem != null)
                {
                    bool success = character.inventory.AddItem(prevEquippedItem);
                    if (!success)
                    {
                        Vector3 dropPos = GetWorldDropLocation(character);
                        prevEquippedItem.ReactivateInWorld(dropPos, this.transform.rotation, false);
                        result += "Dropped " + prevEquippedItem.name + ", ";
                    }
                }
                result += "Equipped " + this.name;
            }
            else
                result += "Cannot Equip " + this.name;
        }
        return result;
    }

    private Vector3 GetWorldDropLocation(PlayerCharacter character)
    {
        Vector3 dropPosition;
        Bounds objBounds = GetComponent<Renderer>().bounds;
        Vector3 castOrigin = character.interactionController.raycastOrigin.transform.position;
        Vector3 direction = character.playerCamObj.transform.forward;
        float length = 2.0f;
        RaycastHit hitInfo;
        if (Physics.BoxCast(castOrigin, objBounds.extents, direction, out hitInfo, this.transform.rotation, length))
        {
            Vector3 offset = Vector3.Project(objBounds.extents, -direction) + new Vector3(0.02f, 0.02f, 0.02f);
            dropPosition = hitInfo.point - offset;
        }
        else
        {
            dropPosition = castOrigin + (direction * length);
        }
        return dropPosition;
    }

}
