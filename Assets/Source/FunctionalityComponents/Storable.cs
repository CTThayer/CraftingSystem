﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storable : MonoBehaviour, IActionable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public string StoreItemInInventory(GameObject player)
    {
        // Deactivate the Item
        // Add to Inventory
        // Open Inventory???
        return "";
    }

    /********************* IActionable Interface Members **********************/
    /* IActionable is used by Interactable to get all actions that can be taken
     * on any given object. These methods MUST be implemented for interaction 
     * to work correctly.                                                     */

    // Returns all the ActionDelegate methods associated with this component
    public ActionDelegate[] GetActions()
    {
        ActionDelegate[] actions = new ActionDelegate[1];
        actions[0] = StoreItemInInventory;
        return actions;
    }

    // Returns all the UI display names for the ActionDelegate methods
    public string[] GetActionNames()
    {
        string[] actionNames = new string[1];
        actionNames[0] = "Add to Inventory";
        return actionNames;
    }
}
