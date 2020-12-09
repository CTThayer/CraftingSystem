using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartCraftingApparatus : CraftingApparatus
{
    /* Supported Part Types
     * Governs what types of PARTS this crafting apparatus can make.
     * TODO: Consider making this use an enum instead of strings to simplify data validation.
     */
    [SerializeField] private string[] _supportedPartTypes;
    public string[] supportedPartTypes
    {
        get => _supportedPartTypes;
        private set
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == null || value[i].Length == 0)
                        return;
                }
                _supportedPartTypes = value;
            }
        }
    }

    [SerializeField] protected PartCraftingApparatusUIManager _uiManager;
    public PartCraftingApparatusUIManager uiManager { get => _uiManager; }

    [SerializeField] private GameObject _partDesignerObj;
    public GameObject partDesignerObj { get => _partDesignerObj; }

    [SerializeField] private GameObject _partCreatorObj;
    public GameObject partCreatorObj { get => _partCreatorObj; }

    [SerializeField] private PartDesignDatabase _partDesignDB;
    public PartDesignDatabase partDesignDB { get => _partDesignDB; }

    // Temporary field for design requirements to be set manually for testing purposes. 
    // TODO: This should be changed/removed when menu UI for selecting design from database is added.
    private PartRequirements _selectedPartReqs;                                 // TODO: Does PartCreator need this at all?
    public PartRequirements selectedPartReqs
    {
        get => _selectedPartReqs;
        private set => _selectedPartReqs = value;
    }
    [SerializeField] private GameObject tempPartReqsObject;

    [SerializeField] private bool _addToInventoryOnComplete;

    [HideInInspector] public string partName;
    [HideInInspector] public string partDesc;

    private bool _partIsComplete;

    // Loaded design data
    private PartRequirements reqs;
    private GameObject slotPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //base.Start();   // Do we actually want this?

        Debug.Assert(_partDesignDB != null);
    }

    // Loads the requirements
    public override void LoadRequirements(GameObject reqsObject)
    {
        PartRequirements partReqs = reqsObject.GetComponent<PartRequirements>();
        if (partReqs != null)
        {
            //uiManager.partsPanel.LoadPartLayout(partReqs);
            reqsObject.transform.position = buildLocation.transform.position;
            reqsObject.transform.rotation = buildLocation.transform.rotation;
        }
        else
        {
            Debug.LogError("ERROR: ItemCraftingApparatus: Supplied " +
                           "requirements are not PartRequirements and " +
                           "cannot be loaded.");
            return;
        }
    }

    // Crafts the current object
    public override void Craft()
    {
        if (resultObject != null)
        {
            PartCreator partCreator = partCreatorObj.GetComponent<PartCreator>();
            if (partCreator != null && partCreator.materialsAreSet)
            {
                bool success = partCreator.FinishPart(resultObject);
                if (success && _addToInventoryOnComplete)
                {
                    Storable resultStorable = resultObject.GetComponent<Storable>();
                    characterUsingApp.inventory.AddItem(resultStorable);
                    string output = "SUCCESS: " + resultObject.name + " was " +
                                "created and has been added to your inventory.";
                    uiManager.partCreatorUIController.DisplayOutput(output);
                    resultObject = null;
                    _partIsComplete = true;
                }
                else if (success)
                {
                    string output = "SUCCESS: " + resultObject.name + " was " +
                                    "created!";
                    uiManager.partCreatorUIController.DisplayOutput(output);
                    resultObject = null;
                    _partIsComplete = true;
                }
                else
                {
                    string output = "ERROR: The part could not created as " +
                                    "configured.";
                    uiManager.partCreatorUIController.DisplayOutput(output);
                }
            }
            else
            {
                string output = "ERROR: No crafting resources have been added.";
                uiManager.partCreatorUIController.DisplayOutput(output);
            }
        }
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
            // TODO: LOAD REQUIREMENTS??

            // Deactivate character input/movement/camera
            _characterUsingApp = pc;
            _characterUsingApp.DeactivateCharacterInput();
            _characterUsingApp.DeactivateCharacterHUD();

            // Enable crafting camera.
            //craftingCamera.SetActive(true);
            //camController.enabled = true;
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
        // If ther is an active part that has not been finished, destroy it
        if (resultObject != null && !_partIsComplete)
            Destroy(resultObject);

        uiManager.DeactivateUI();
        DeactivatePartDesigner();
        DeactivatePartCreator();
        characterUsingApp.ReactivateCharacterInput();
        characterUsingApp.ReactivateCharacterHUD();
        SwitchToPlayerCamera();
        _characterUsingApp = null;
    }


    public void ActivatePartDesigner()
    {
        _partDesignerObj.SetActive(true);
        //_partDesignerObj.GetComponent<PartDesigner>().enabled = true;
        //_partDesignerObj.GetComponent<PartDesignerUIController>().enabled = true;
        _partDesignerObj.GetComponent<PartDesignViewInputController>().enabled = true;
        //_partDesignerObj.GetComponent<ItemPartAssembler>().enabled = true;
    }

    public void DeactivatePartDesigner()
    {
        _partDesignerObj.SetActive(false);
        //_partDesignerObj.GetComponent<PartDesigner>().enabled = false;
        //_partDesignerObj.GetComponent<PartDesignerUIController>().enabled = false;
        _partDesignerObj.GetComponent<PartDesignViewInputController>().enabled = false;
        //_partDesignerObj.GetComponent<ItemPartAssembler>().enabled = false;
        _camController.ResetCameraTransform();
    }

    public void ActivatePartCreator()
    {
        _partCreatorObj.SetActive(true);
        _partCreatorObj.GetComponent<CraftingViewInputController>().enabled = true;
        _uiManager.partCreatorUIController.ActivateBackgroundUI();
        _uiManager.partCreatorUIController.LoadResourceSlots(slotPrefab, reqs);
    }

    public void DeactivatePartCreator()
    {
        _partCreatorObj.SetActive(false);
        _partCreatorObj.GetComponent<CraftingViewInputController>().enabled = false;
        _camController.ResetCameraTransform();
    }

    public void SwitchToCraftingCamera()
    {
        _characterUsingApp.DeactivateCharacterCamera();
        craftingCamera.SetActive(true);
        camController.enabled = true;
    }

    public void SwitchToPlayerCamera()
    {
        camController.enabled = false;
        craftingCamera.SetActive(false);
        _characterUsingApp.ReactivateCharacterCamera();
    }

    public void LoadDesign(GameObject design)
    {
        if (resultObject != null)
            Destroy(resultObject);                                              // TODO: Is this correct?

        resultObject = Instantiate(design);
        resultObject.transform.position = buildLocation.transform.position;
        resultObject.transform.rotation = buildLocation.transform.rotation;
        Vector3 size = resultObject.GetComponent<Renderer>().bounds.extents;
        Vector3 posOffset = Vector3.up * size.y;
        resultObject.transform.position += posOffset;
        _partIsComplete = false;
    }

    public bool LoadDesign(string type, string subtype, string name)
    {
        GameObject design = _partDesignDB.GetPartDesign(type, subtype, name, out reqs, out slotPrefab);
        if (design != null)
        {
            resultObject = Instantiate(design);
            resultObject.transform.position = buildLocation.transform.position;
            resultObject.transform.rotation = buildLocation.transform.rotation;
            Vector3 size = resultObject.GetComponent<Renderer>().bounds.extents;
            Vector3 posOffset = Vector3.up * size.y;
            resultObject.transform.position += posOffset;
            _partIsComplete = false;

            return true;
        }
        return false;
    }

    public GameObject GetLoadedDesign()
    {
        return resultObject;
    }
}
