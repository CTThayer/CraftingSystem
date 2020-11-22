﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingViewInputController : MonoBehaviour
{
    [SerializeField] private CraftingApparatus craftingApparatus;

    // Start is called before the first frame update
    void Start()
    {
        if (craftingApparatus == null)
        {
            craftingApparatus = GetComponent<CraftingApparatus>();
            if (craftingApparatus == null)
                Debug.LogError("ERROR: CraftingViewInputController: Missing " +
                               "reference to CraftingApparatus.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    /* HandleInput()
     * Helper method for reading/responding to user input. Called
     * from Update() each frame. Deals with mouse click selection,
     * and arrow key input for selecting the AddLocation.
     */
    private void HandleInput()
    {
        // Manipulate Camera
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"),
                                                 Input.GetAxis("Mouse Y"));
                craftingApparatus.camController.ProcessTumble(mouseDelta);
                return;     // Return to avoid multiple actions
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                craftingApparatus.camController.ProcessZoom(Input.mouseScrollDelta.y);
                return;     // Return to avoid multiple actions
            }
            if (Input.GetMouseButton(1))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"),
                                                 Input.GetAxis("Mouse Y"));
                craftingApparatus.camController.ProcessPan(mouseDelta);
                return;     // Return to avoid multiple actions
            }
        }

        // Handle Hotkey Presses
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            craftingApparatus.Exit();
        }
    }

}