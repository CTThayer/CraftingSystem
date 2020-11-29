using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Update this list to reflect final choices for EquipmentTypes
public enum EquipmentType
{
    Head,
    Chest,
    //Arms,
    //Gloves,
    //LeftHandEquip,
    //RightHandEquip,
    //Legs,
    //Feet,
    //Belt,
    //Necklace,
    //Ring,

    // For the purposes of matching tutorial... TODO: use above for final version
    Accessory1,
    Accessory2,
    Weapon1,
    Weapon2,
    Other

}

//public class Equipable : MonoBehaviour, IActionable
public class Equipable : Storable, IActionable
{
    [SerializeField] private EquipmentType _equipmentType;
    public EquipmentType equipmentType
    {
        get => _equipmentType;
        private set => _equipmentType = value;
    }

    // TODO: Add field for storing what animation(s) go with this item when it is equipped to the character.

    public void Initialize()
    {
        // TODO: Generalize the initializer so that it doesn't take parameters
    }

    public void Initialize(EquipmentType equipmentType, Sprite icon)
    {
        // TODO: Implement initializer
    }

    public void Equip()
    {
        // 1.a. Set this item's position and rotation to match the equip 
        //      location on the character.
        //   b. Parent the item to the corresponding bone in the skeleton.
        //   c. Update the skeleton to reflect the equipped item (i.e. close the
        //      character's hand around the handle of an item)
        // 2.c. Re-enable the disabled components of the item (i.e. collider,
        //      mesh, renderer, etc.)
        //   b. Disable (or leave disabled) components that we don't want to use
        //      while equipped, i.e. deactivate Interactable so other players 
        //      can't take actionson it, etc.
        // 3.   Update the character's animations to reflect the equipment. For
        //      example, if an item is now wielded in their right hand, update
        //      the main action animation to reflect the type of item.
        // 4.   Apply Buffs / Debuffs(as necessary)
    }

    public void Unequip(Character c)
    {
        // 1.a. Unparent the item to the corresponding bone in the skeleton.
        //   b. Update the skeleton to reflect the unequipped item (i.e. open
        //      the character's hand if it was previously closed around an item)
        // 2.a. If added to storage/inventory, disable the components of the 
        //      item (i.e. colliders, mesh, etc.) 
        //   b. If dropped to the world, move item slightly in front of the
        //      character and activate the rigidbody/make it non-kinematic.
        // 3.   Update the character's animations to reflect the unequipping.
        //      i,e. Revert to non-equipped animations.
        // 4.   Apply Buffs / Debuffs(as necessary)
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
