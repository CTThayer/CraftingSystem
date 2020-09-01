using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable : MonoBehaviour, IActionable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public string EquipItemOnPlayer(GameObject player)
    {
        // Check for open equipment slots (of this type) on the player
        // If there are any, equip this item
        // If not return error
        return "No Open Equipment Slots.";
    }

    /********************* IActionable Interface Members **********************/
    /* IActionable is used by Interactable to get all actions that can be taken
     * on any given object. These methods MUST be implemented for interaction 
     * to work correctly.                                                     */

    // Returns all the ActionDelegate methods associated with this component
    public ActionDelegate[] GetActions()
    {
        ActionDelegate[] actions = new ActionDelegate[1];
        actions[0] = EquipItemOnPlayer;
        return actions;
    }

    // Returns all the UI display names for the ActionDelegate methods
    public string[] GetActionNames()
    {
        string[] actionNames = new string[1];
        actionNames[0] = "Equip";
        return actionNames;
    }

}
