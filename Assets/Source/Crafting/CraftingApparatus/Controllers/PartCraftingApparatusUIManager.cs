using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PartCraftingApparatusUIManager : MonoBehaviour
{
    // Reference back to the crafting apparatus that this is being done from
    [SerializeField] private PartCraftingApparatus _craftingApparatus;
    public PartCraftingApparatus craftingApparatus { get => _craftingApparatus; }

    // Fields for initial menu
    [SerializeField] private Canvas _initialMenusCanvas;
    [SerializeField] private InitialPartCraftingMenu _initialPartCraftingMenu;

    // Fields for designing a new part
    [SerializeField] private Canvas _partDesignUICanvas;
    [SerializeField] private PartDesignerUIController _partDesignerUIController;
    public PartDesignerUIController partDesignerUIController { get => _partDesignerUIController; }

    // Fields for finishing crafting a part
    [SerializeField] private Canvas _partCreatorUICanvas;
    [SerializeField] private PartCreatorUIController _partCreatorUIController;
    public PartCreatorUIController partCreatorUIController { get => _partCreatorUIController; }

    private bool _isInitialized;
    public bool isInitialized { get => _isInitialized; }

    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }

    public void Initialize()
    {
        Debug.Assert(_craftingApparatus != null);

        // Setup initial selection menus
        Debug.Assert(_initialMenusCanvas != null);
        Debug.Assert(_initialPartCraftingMenu != null);
        if (!_initialPartCraftingMenu.isInitialized)
            _initialPartCraftingMenu.Initialize();

        // Setup part designer UI
        Debug.Assert(_partDesignUICanvas != null);
        Debug.Assert(_partDesignerUIController != null);
        if (!_partDesignerUIController.isInitialized)
            _partDesignerUIController.Initialize();

        // Setup part creator UI
        Debug.Assert(_partCreatorUICanvas != null);
        Debug.Assert(_partCreatorUIController != null);
        if (!_partCreatorUIController.isInitialized)
            _partCreatorUIController.Initialize(this);
            //_partCreatorUIController.Initialize();
    }

    public void ActivateUI()
    {
        // Activate initial menu canvas & deactivate other crafting canvases
        _initialMenusCanvas.gameObject.SetActive(true);
        _partDesignUICanvas.gameObject.SetActive(false);
        _partCreatorUICanvas.gameObject.SetActive(false);

        // Ensure initial menu is active and enabled
        _initialPartCraftingMenu.enabled = true;
        _initialPartCraftingMenu.gameObject.SetActive(true);
    }

    public void DeactivateUI()
    {
        // Deactivate all associated canvases
        _initialMenusCanvas.gameObject.SetActive(false);
        _partDesignUICanvas.gameObject.SetActive(false);
        _partCreatorUICanvas.gameObject.SetActive(false);

        // Deactivate all associated UI scripts
        //_initialPartCraftingMenu.enabled = false;
        //_partDesignerUIController.enabled = false;
        //_partCreatorUIController.enabled = false;
    }

    /************************** Input Field Actions ***************************/
    //public void OnEndNameEdit(string input)
    //{
    //    if (input.Length < 80)
    //    {
    //        _craftingApparatus.partName = input;
    //        DisplayOutputMessage("");
    //    }
    //    else
    //    {
    //        DisplayOutputMessage("The entered item desccription is too long.");
    //    }
    //}

    //public void OnEndDescriptionEdit(string input)
    //{
    //    if (input.Length < 256)
    //    {
    //        _craftingApparatus.partDesc = input;
    //        DisplayOutputMessage("");
    //    }
    //    else
    //    {
    //        DisplayOutputMessage("The entered item desccription is too long.");
    //    }
    //}
    /************************ END Input Field Actions *************************/


    /******************************* Callbacks ********************************/
    //public void OnInitialChoiceNewDesign()
    //{
    //    initialMenu.SetActive(false);
    //    newDesignSelectorMenu.gameObject.SetActive(true);
    //}

    //public void OnInitialChoiceExistingDesign()
    //{
    //    initialMenu.SetActive(false);
    //    existingDesignSelectorMenu.gameObject.SetActive(true);
    //}

    //public void OnSelectionMenuBackButton()
    //{
    //    initialMenu.SetActive(true);
    //    newDesignSelectorMenu.gameObject.SetActive(false);
    //    existingDesignSelectorMenu.gameObject.SetActive(false);
    //}

    //public void OnNewDesignConfirm()
    //{
    //    // Close the current canvas and its menus
    //    newDesignSelectorMenu.gameObject.SetActive(false);
    //    existingDesignSelectorMenu.gameObject.SetActive(false);
    //    initialMenusCanvas.gameObject.SetActive(false);

    //    // Activate the part designer canvas
    //    _partDesignUICanvas.gameObject.SetActive(true);

    //    // Activate the PartDesigner and set part type
    //    craftingApparatus.ActivatePartDesigner();
    //    PartDesigner partDesigner = craftingApparatus.partDesignerObj.GetComponent<PartDesigner>();
    //    partDesigner.partType = selectedPartType;
    //    partDesigner.uiController.SetPartType(selectedPartType);

    //    // Switch to the crafting camera
    //    craftingApparatus.SwitchToCraftingCamera();
    //}

    //public void OnExistingDesignConfirm()
    //{
    //    // Close the current canvas and its menus
    //    newDesignSelectorMenu.gameObject.SetActive(false);
    //    existingDesignSelectorMenu.gameObject.SetActive(false);
    //    initialMenusCanvas.gameObject.SetActive(false);

    //    // Activate the part crafting canvas
    //    _partCraftingUICanvas.gameObject.SetActive(true);

    //    // Switch to the crafting camera
    //    craftingApparatus.SwitchToCraftingCamera();
    //}

    //public void OnPartTypeSelection(string partType)
    //{
    //    selectedPartType = partType;
    //}

    //public void OnCraftPartButtonClick()
    //{
    //    craftingApparatus.Craft();
    //}

    /* On Select Part Designer
     * This method is used to launch the PartDesigner UI and deactivate any 
     * other crafting UI that is active. It is mainly meant to be used by the
     * initial menu button when the player selects the create new design option.
     */
    public void OnSelectPartDesigner()
    {
        // Switch cameras
        _craftingApparatus.SwitchToCraftingCamera();

        // Deactivate the initial menu and its canvas
        _initialPartCraftingMenu.enabled = false;
        _initialPartCraftingMenu.gameObject.SetActive(false);
        _initialMenusCanvas.gameObject.SetActive(false);

        // Launch the part designer and activate its canvas
        _partDesignUICanvas.gameObject.SetActive(true);
        _craftingApparatus.ActivatePartDesigner();
        _partDesignerUIController.LaunchPartDesignerUI();
    }

    /* On Select Part Creator
     * This method is used to launch the PartCreator UI and deactivate any 
     * other crafting UI that is active. It is mainly meant to be used by the
     * initial menu button when the player selects the create new design option.
     */
    public void OnSelectPartCreator()
    {
        // Switch cameras
        _craftingApparatus.SwitchToCraftingCamera();

        // Deactivate the initial menu and its canvas
        _initialPartCraftingMenu.enabled = false;
        _initialPartCraftingMenu.gameObject.SetActive(false);
        _initialMenusCanvas.gameObject.SetActive(false);

        // Launch the part creator and activate it's canvas
        _partCreatorUICanvas.gameObject.SetActive(true);
        _partCreatorUIController.LaunchPartCreatorUI();

    }

    /* On Continue To Part Crafting
     * A function for the PartDesignerUI to call when the player has finished a
     * part design and selected the option to go on to the next step and create 
     * the part immediately (as opposed to simply save the design and create an
     * instance of it later.)
     */
    //TODO: Transfer the part design from the PartDesigner to the PartCreator in this method
    public void OnContinueToPartCrafting()
    {
        // Deactivate designer, activate creator
        _craftingApparatus.DeactivatePartDesigner();
        _craftingApparatus.ActivatePartCreator();

        // deactivate design canvas, activate crafting canvas
        _partDesignUICanvas.gameObject.SetActive(false);
        _partCreatorUICanvas.gameObject.SetActive(true);
    }

    /* On Restart
     * A public function meant to be called by either the part designer UI 
     * controller or the part creator UI controller if the player selects the 
     * back option immediately after entering either of those modes. This 
     * function simply closes that UI and reloads the initial menu.
     */
    public void OnRestart()
    {
        // Deactivate design & crafting canvases, reactivate initial menu canvas
        _partDesignUICanvas.gameObject.SetActive(false);
        _partCreatorUICanvas.gameObject.SetActive(false);
        _initialMenusCanvas.gameObject.SetActive(true);

        // Set initial menu active and sub-menus inactive
        _initialPartCraftingMenu.enabled = true;
        _initialPartCraftingMenu.gameObject.SetActive(true);

        // Switch back to original camera for this view
        craftingApparatus.SwitchToPlayerCamera();
    }

    /* On Exit
     * This method is for either of the other UI designers to use as a callback
     * to completely exit the part apparatus. This is meant to be used with the 
     * "X" button, the Escape key or any other situation where the player is
     * fully exiting the part apparatus UI.
     * It calls the Exit function in PartCraftingApparatus.cs to ensure that 
     * all of components of the Part Crafting Apparatus shut down correctly.
     */
    public void OnExit()
    {
        craftingApparatus.Exit();
    }

    /*************************** END Callbacks ********************************/

}
