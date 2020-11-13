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

    // TODO: Add DesignRequirements / PartRequirements database reference(s) for retrieving requirements

    /*********************** END User Configured Fields ***********************/

    // For use by the Crafting Apparatus Manager
    [HideInInspector]
    public string itemName;         // Variable containing the name of the item
    [HideInInspector]
    public string itemDescription;  // Variable containing the description of the item

    private bool itemIsComplete = false;

    ItemFactory factory = new ItemFactory();                                    // TODO: Consider making ItemFactory a singleton

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Validate field values
        // TODO: Check references
        // TODO: Verify components (e.g. has an Interactable component, etc.)
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
            Debug.LogError("ERROR: ItemCraftingApparatus: Supplied " +
                           "requirements are not DesignRequirements and " +
                           "cannot be loaded.");
            return;
        }
    }

    // Crafts the current object
    public override void Craft()
    {

        itemIsComplete = true;
    }

    // For use by Interactable
    public override void Use()
    {

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
