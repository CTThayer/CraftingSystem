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

    [SerializeField] protected ItemCraftingApparatusUIManager _uiManager;
    public ItemCraftingApparatusUIManager uiManager { get => _uiManager; }

    //[SerializeField] protected CraftingViewInputController _inputController;
    //public CraftingViewInputController inputController { get => _inputController; }

    //[SerializeField] protected CraftingCameraController _camController;
    //public CraftingCameraController camController { get => _camController; }

    // Temporary field for design requirements to be set manually for testing purposes. 
    // TODO: This should be changed/removed when menu UI for selecting design from database is added.
    // TODO: Add DesignRequirements / PartRequirements database reference(s) for retrieving requirements
    private DesignRequirements _selectedDesignReqs;
    public DesignRequirements selectedDesignReqs
    {
        get => _selectedDesignReqs;
        private set => _selectedDesignReqs = value;
    }
    [SerializeField] private GameObject tempDesignReqsObject;

    [SerializeField] private bool autoAddToInventory;

    [SerializeField] private ItemFactory factory; // = new ItemFactory();       // TODO: Consider making ItemFactory a singleton

    /*********************** END User Configured Fields ***********************/

    private bool itemIsComplete = false;


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
            _uiManager = GetComponent<ItemCraftingApparatusUIManager>();
        if (_uiManager == null)
            _inputController = GetComponent<CraftingViewInputController>();
        if (_camController == null)
            _camController = GetComponentInChildren<CraftingCameraController>();

        // Temp code for setting designReqs object position. This will go inside
        // the method that sets the design reqs when selected from the menu
        LoadRequirements(tempDesignReqsObject);
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
                tempDesignReqsObject.GetComponent<PartLayout>().buildLocation = buildLocation.position;
                selectedDesignReqs = designReqs;
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
                    itemIsComplete = success;
                    uiManager.DisplayOutputMessage(output);
                    uiManager.DestroyUsedPartIcons();

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
    public override string Use(PlayerCharacter pc)
    {
        if (pc == null)
        {
            Debug.LogError("ItemCraftingApparatus: Use(): PlayerCharacter ref was null");
            return "PlayerCharacter was null";
        }
        if (_characterUsingApp == null)
        {
            //if (!selectedDesignReqs.gameObject.activeSelf)
                selectedDesignReqs.gameObject.SetActive(true);
            LoadRequirements(selectedDesignReqs.gameObject);

            // Deactivate character input/movement/camera
            _characterUsingApp = pc;
            _characterUsingApp.DeactivateCharacterInput();
            _characterUsingApp.DeactivateCharacterCamera();
            _characterUsingApp.DeactivateCharacterHUD();
            _characterUsingApp.DeactivateEquipmentMenu();

            // Enable crafting camera.
            craftingCamera.SetActive(true);
            camController.enabled = true;
            inputController.enabled = true;

            // Activate the UI
            uiManager.ActivateUI();
            return "";
        }
        else
        {
            return "Another player is using this apparatus.";
        }
    }

    // Exits the apparatus
    public override void Exit()
    {
        if (_characterUsingApp == null)
        {
            Debug.LogError("ItemCraftingApparatus: Exit(): PlayerCharacter ref was null while trying to exit. How did this happen?");
            return;
        }

        // Move any parts still in sockets back to inventory
        uiManager.RemovePartsFromPartsPanel();

        // Destroy the instantiated design reqs game object
        //Destroy(selectedDesignReqs.gameObject);
        selectedDesignReqs.gameObject.SetActive(false);

        // Disable crafting camera
        craftingCamera.SetActive(false);
        camController.enabled = false;
        inputController.enabled = false;

        // Handle the item that was created or in progress.
        if (itemIsComplete)
        {
            //OnItemCompletion();
        }
        else
        {
            Destroy(resultObject);  // TODO: Is this necessary?
            resultObject = null;
        }

        uiManager.DeactivateUI();

        // Reenable character input and character camera
        _characterUsingApp.ReactivateCharacterCamera();
        _characterUsingApp.ReactivateCharacterHUD();
        _characterUsingApp.ReactivateCharacterInput();
        _characterUsingApp.ReactivateEquipmentMenu();
        _characterUsingApp = null;
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
        string concat = designType + " " + designSubType;
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

    private Vector3 GetItemBaseOffset()
    {
        Vector3 offset = Vector3.zero;
        Vector3 currentPos = resultObject.transform.position;
        Bounds objBounds = resultObject.GetComponent<Renderer>().bounds;
        Ray ray = new Ray(currentPos, Vector3.down);
        float offsetDist;
        if (objBounds.IntersectRay(ray, out offsetDist))
        {
            offset = (Vector3.down * offsetDist);
        }
        return offset;
    }

    private void OnItemCompletion()
    {
        if (autoAddToInventory)
        {
            Storable resultStorable = resultObject.GetComponent<Storable>();
            bool b = _characterUsingApp.inventory.AddItem(resultStorable);
            if (b)
                return;
        }
        ActivateItemInWorld();
    }

    private void ActivateItemInWorld()
    {
        // Get and configure rigidbody
        Rigidbody rigidbody = resultObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = resultObject.GetComponentInChildren<Rigidbody>();
        if (rigidbody != null)
            rigidbody.isKinematic = false;

        // Set position in world
        if (spawnLocation == buildLocation)
            return;
        else
        {
            Vector3 loc = spawnLocation.position + GetItemBaseOffset();
            resultObject.transform.position = loc;
        }
    }

    /************************** END Private Methods ***************************/
}
