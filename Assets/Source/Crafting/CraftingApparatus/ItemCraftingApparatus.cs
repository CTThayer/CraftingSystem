using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCraftingApparatus : CraftingApparatus
{
    /************************* User Configured Fields *************************/

    /* Supported Item Types
     * Governs what types of ITEMS this crafting apparatus can make.
     * TODO: Consider making this use an enum instead of strings to simplify data validation.
     */
    [SerializeField] private string[] _supportedItemTypes;
    public string[] supportedItemTypes
    {
        get => _supportedItemTypes;
        private set
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == null || value[i].Length == 0)
                        return;
                }
                _supportedItemTypes = value;
            }
        }
    }

    // Temporary field for design requirements to be set manually for testing purposes. 
    // TODO: This should be changed/removed when menu UI for selecting design from database is added.
    [SerializeField] private DesignRequirements _selectedDesignReqs;
    public DesignRequirements selectedDesignReqs
    {
        get => _selectedDesignReqs;
        private set => _selectedDesignReqs = value;
    }
    [SerializeField] private GameObject tempDesignReqsObject;

    // TODO: Add DesignRequirements / PartRequirements database reference(s) for retrieving requirements

    /*********************** END User Configured Fields ***********************/

    // For use by the Crafting Apparatus UI Manager
    [HideInInspector]
    public string itemName;         // Variable containing the name of the item
    [HideInInspector]
    public string itemDescription;  // Variable containing the description of the item

    private bool itemIsComplete = false;

    [SerializeField] private ItemFactory factory; // = new ItemFactory();                                    // TODO: Consider making ItemFactory a singleton

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Validate field values
        // TODO: Check references
        // TODO: Verify components (e.g. has an Interactable component, etc.)

        itemName = "";
        itemDescription = "";

        if (factory == null)
            factory = GetComponent<ItemFactory>();
        Debug.Assert(factory != null);

        // Temp code for setting designreqs object position. This will go inside
        // the method that sets the design reqs when selected from the menu
        tempDesignReqsObject.transform.position = buildLocation.transform.position;
        selectedDesignReqs = tempDesignReqsObject.GetComponent<DesignRequirements>();
    }

    // TODO: Fill in stub methods

    // Loads the requirements
    public override void LoadRequirements(GameObject reqsObject)
    {
        DesignRequirements designReqs = reqsObject.GetComponent<DesignRequirements>();
        if (designReqs != null)
        {
            uiManager.partsPanel.LoadPartLayout(designReqs.designLayoutUIElements);
            reqsObject.transform.position = buildLocation.transform.position;
            reqsObject.transform.rotation = buildLocation.transform.rotation;
        }
        else
        {
            Debug.LogError("ERROR: ItemCraftingApparatus: Supplied GameObject" +
                           " does NOT have DesignRequirements and " +
                           "therefore nothing could be loaded.");
            return;
        }
    }

    // Crafts the current object
    public override void Craft()
    {
        string output;
        ItemPart[] parts = selectedDesignReqs.GetPartsFromSockets();
        bool success = factory.CreateItemFromParts(selectedDesignReqs,
                                                   parts, 
                                                   itemName, 
                                                   itemDescription, 
                                                   out resultObject, 
                                                   out output);
        itemIsComplete = success;
        uiManager.DisplayOutputMessage(output);
    }

    // For use by Interactable
    public override void Use()
    {
        // TODO: Disable player camera
        // TODO: Disable player input controller

        // TODO: Enable crafting camera
        camController.enabled = true;
        inputController.enabled = true;

        uiManager.ActivateUI();
    }

    // Exits the apparatus
    public override void Exit()
    {
        // Move any parts still in sockets back to inventory
        uiManager.ClearPartsPanel();

        // Destroy the instantiated design reqs game object
        Destroy(selectedDesignReqs.gameObject);
    }

    /**************************** Private Methods *****************************/


    /************************** END Private Methods ***************************/
}
