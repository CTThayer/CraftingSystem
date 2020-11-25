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
    private GameObject prevCamera;
    private PlayerCharacterController characterUsingApp;

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

        if (_uiManager == null)
            _uiManager = GetComponent<CraftingApparatusUIManager>();
        if (_uiManager == null)
            _inputController = GetComponent<CraftingViewInputController>();
        if (_camController == null)
            _camController = GetComponentInChildren<CraftingCameraController>();

        // Temp code for setting designreqs object position. This will go inside
        // the method that sets the design reqs when selected from the menu
        tempDesignReqsObject.transform.position = buildLocation.transform.position;
        selectedDesignReqs = tempDesignReqsObject.GetComponent<DesignRequirements>();
    }

    // Loads the requirements
    public override void LoadRequirements(GameObject reqsObject)
    {
        DesignRequirements designReqs = reqsObject.GetComponent<DesignRequirements>();
        if (designReqs != null)
        {
            if (DesignIsSupportedType(designReqs))
            {
                uiManager.partsPanel.LoadPartLayout(designReqs.partLayout.prefabPartLayoutUI);
                reqsObject.transform.position = buildLocation.transform.position;
                reqsObject.transform.rotation = buildLocation.transform.rotation;
            }
            else
            {
                Debug.LogError("ItemCraftingApparatus: ERROR: Supplied " +
                               "design requirements are NOT of a supported " +
                               "type for this crafting apparatus and" +
                               "therefore nothing was be loaded.");
            }
        }
        else
        {
            Debug.LogError("ItemCraftingApparatus: ERROR: Supplied GameObject" +
                           " does NOT have DesignRequirements and " +
                           "therefore nothing could be loaded.");
            return;
        }
    }

    // Crafts the current object
    public override void Craft()
    {
        string output = "";
        ItemPart[] parts = selectedDesignReqs.partLayout.GetPartsArray();
        if (parts != null)
        {
            if (ValidateNameAndDesc())
            {
                if (selectedDesignReqs.ValidateConfiguration(out output))
                {
                    bool success = factory.CreateItemFromParts(selectedDesignReqs,
                                                               parts,
                                                               itemName,
                                                               itemDescription,
                                                               out resultObject,
                                                               out output);
                    resultObject.transform.position = new Vector3(0f, 1f, 1f);
                    itemIsComplete = success;
                    uiManager.DisplayOutputMessage(output);
                    return;
                }
            }
            else
                output = "Error: Name and Description must be set before crafting.";
        }
        else
            output = "Error: Part configuration is incomplete or invalid.";

        itemIsComplete = false;
        uiManager.DisplayOutputMessage(output);
        return;
    }

    // For use by Interactable
    public override void Use(PlayerCharacterController PCC)
    {
        // Disable player camera
        prevCamera = PCC.playerCameraObj;
        prevCamera.SetActive(false);

        //Disable player input controller
        characterUsingApp = PCC;
        PCC.enabled = false;

        // Enable crafting camera.
        craftingCamera.SetActive(true);
        camController.enabled = true;
        inputController.enabled = true;

        // Activate the UI
        uiManager.ActivateUI();
    }

    // Exits the apparatus
    public override void Exit()
    {
        // Move any parts still in sockets back to inventory
        uiManager.ClearPartsPanel();

        // Destroy the instantiated design reqs game object
        Destroy(selectedDesignReqs.gameObject);

        // Disable crafting camera
        craftingCamera.SetActive(false);
        camController.enabled = false;
        inputController.enabled = false;

        // Enable previous camera
        prevCamera.SetActive(true);
        characterUsingApp.enabled = true;

        // Handle the item that was created or in progress.
        if (itemIsComplete)
        {
            resultObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            Destroy(resultObject);  // TODO: Is this necessary?
            resultObject = null;
        }

        uiManager.DeactivateUI();
    }

    /**************************** Private Methods *****************************/

    private bool ValidateNameAndDesc()
    { 
        return itemName.Length > 0
               && itemName.Length <= 80
               && itemDescription.Length > 0
               && itemDescription.Length <= 256;
    }

    private bool DesignIsSupportedType(DesignRequirements DR)
    {
        string designType = DR.itemType;
        string designSubType = DR.itemSubType;
        string concat = designType + designSubType;
        for (int i = 0; i < _supportedItemTypes.Length; i++)
        {
            if (_supportedItemTypes[i] == designType
                || _supportedItemTypes[i] == designSubType
                || _supportedItemTypes[i] == concat)
            {
                return true;
            }
        }
        return false;
    }

    /************************** END Private Methods ***************************/
}
