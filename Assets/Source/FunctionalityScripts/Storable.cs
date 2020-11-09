﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storable : MonoBehaviour, IActionable
{
    public Sprite icon;

    // Reference to the item that this storable instance is attached to
    [SerializeField] private GameObject _item;
    public GameObject item { get; private set; }

    // Boolean tracking whether this item is currently stored.
    [SerializeField] private bool _isStored;
    public bool isStored { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_item != null);
    }

    public void AddToStorage(Inventory storage)                                 // TODO: Rename Inventory class to Storage & generalize it.
    {
        if (isStored) // If item is already in storage, don't try to deactivate.
            return;
        
        // Reenable colliders
        Collider[] colliders = gameObject.GetComponents<Collider>();
        Collider[] childColliders = gameObject.GetComponentsInChildren<Collider>();
        int i = 0;
        for (; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
        for (i = 0; i < childColliders.Length; i++)
        {
            childColliders[i].enabled = false;
        }

        // Set rigidbody to Kinematic to avoid simulating it while hidden.
        Rigidbody r = gameObject.GetComponent<Rigidbody>();
        if (r != null)
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

        // Reenable the MeshRenderer
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        isStored = true;
    }

    public void ReactivateInWorld(Transform transform, bool isKinematicRigidbody)
    {
        if (!isStored)  // If item isn't in storage, don't try to reactivate
            return;
        
        // Update object
        gameObject.transform.position = transform.position;
        gameObject.transform.rotation = transform.rotation;
        // Do NOT set scale to match!

        // Reenable colliders
        Collider[] colliders = gameObject.GetComponents<Collider>();
        Collider[] childColliders = gameObject.GetComponentsInChildren<Collider>();
        int i = 0;
        for (; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        for (i = 0; i < childColliders.Length; i++)
        {
            childColliders[i].enabled = true;
        }

        // Set rigidbody to Kinematic or non-kinematic based on isKinematicRigidbody
        Rigidbody r = gameObject.GetComponent<Rigidbody>();
        if (r != null)
            r.isKinematic = isKinematicRigidbody;

        // Reenable the MeshRenderer
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        isStored = false;
    }

    /********************* IActionable Interface Members **********************/
    /* IActionable is used by Interactable to get all actions that can be taken
     * on any given object. These methods MUST be implemented for interaction 
     * to work correctly.                                                     */

    // Returns all the ActionDelegate methods associated with this component
    public ActionDelegate[] GetActions()
    {
        ActionDelegate[] actions = new ActionDelegate[1];
//        actions[0] = StoreItemInInventory;
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