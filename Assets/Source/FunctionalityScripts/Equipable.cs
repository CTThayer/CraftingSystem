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

    public void Equip(Character c)
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
    public ActionDelegate[] GetActions()
    {
        ActionDelegate[] actions = new ActionDelegate[1];
        actions[0] = AddToInventory;                                                   // TODO: change format of Action Delegates to reflect character parameter OR make generic (if possible)
        actions[1] = Equip;
        return actions;
    }

    // Returns all the UI display names for the ActionDelegate methods
    public string[] GetActionNames()
    {
        string[] actionNames = new string[1];
        actionNames[0] = "Add to Inventory";
        actionNames[1] = "Equip";
        return actionNames;
    }

    /*********************** IActionable Event Members ************************/
    public override string AddToInventory(GameObject caller)
    {
        string result = "";
        Character character = caller.GetComponent<Character>();                 // TODO: Consider just passing character instead of GameObject as the "caller"
        if (character != null)
        {
            character.inventory.AddItem(this);
        }
        return result;
    }

    public string Equip(GameObject caller)                                      // TODO: Consider just passing character instead of GameObject as the "caller"
    {
        string result = "";
        Character character = caller.GetComponent<Character>();
        if (character != null)
        {
            Storable prevEquippedItem;
            character.equipmentPanel.AddItem(this, out prevEquippedItem);
            if (prevEquippedItem != null)
            {
                bool success = character.inventory.AddItem(prevEquippedItem);
                if (!success)
                {
                    // TODO: change transform to position & rotation because 
                    // transforms can't be constructed at runtime without a 
                    // GameObject and setting objects to the exact same pos/rot
                    // as the one being picked up likely has a lot of problems.
                    prevEquippedItem.ReactivateInWorld(this.transform, true);
                }
            }
        }
        return result;
    }

}
