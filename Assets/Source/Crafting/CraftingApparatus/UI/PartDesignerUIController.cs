using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PartDesignerUIController : MonoBehaviour
{
    // Ref to PartDesigner & PartSegementDatabase
    [SerializeField] private PartDesigner partDesigner;
    [SerializeField] private PartSegmentDatabase segmentDatabase;

    // UI fields
    [SerializeField] private Canvas canvas;
    [SerializeField] private Text outputText;
    [SerializeField] private Text selectedPartTypeText;
    [SerializeField] private Dropdown designFamilyDropdown;
    [SerializeField] private Button finishDesignButton;

    // Menus, Dropdown Managers, and Prompts
    [SerializeField] private PartTypeSelectorMenu newDesignSelectorMenu;

    [SerializeField] private DropdownGameObjectSelector _segmentSelector;
    public DropdownGameObjectSelector segmentSelector { get => _segmentSelector; }

    [SerializeField] private GameObject continuePrompt;
    private ContinuePromptController continuePromptController;

    // Non-serialized data
    private string _partType;
    public string partType { get => _partType; }
    private bool _isInitialized = false;
    public bool isInitialized { get => _isInitialized; }

    /****************************** Unity Methods *****************************/
    /* Start is called before the first frame update */
    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }
    /**************************** END Unity Methods ***************************/


    /***************************** Public Methods *****************************/

    public void Initialize()
    {
        if (partDesigner == null)
            partDesigner = GetComponent<PartDesigner>();
        Debug.Assert(partDesigner != null);

        if (segmentDatabase == null)
            segmentDatabase = GetComponent<PartSegmentDatabase>();
        Debug.Assert(segmentDatabase != null);

        Debug.Assert(canvas != null);

        if (outputText == null)
            outputText = canvas.transform.Find("Output_Text").GetComponent<Text>();
        if (selectedPartTypeText == null)
            selectedPartTypeText = canvas.transform.Find("SelectedPartType_Text").GetComponent<Text>();
        if (designFamilyDropdown != null)
            designFamilyDropdown = canvas.transform.Find("PartDesignFamily_Dropdown").GetComponent<Dropdown>();
        if (_segmentSelector == null)
            _segmentSelector = canvas.transform.Find("SegmentSelector_Dropdown").GetComponent<DropdownGameObjectSelector>();
        Debug.Assert(outputText != null);
        Debug.Assert(selectedPartTypeText != null);
        Debug.Assert(newDesignSelectorMenu != null);
        Debug.Assert(finishDesignButton != null);
        Debug.Assert(designFamilyDropdown != null);
        Debug.Assert(_segmentSelector != null);
        Debug.Assert(continuePrompt != null);
        designFamilyDropdown.gameObject.SetActive(false);
        continuePrompt.SetActive(false);

        // Setup call the callbacks for the UI elements and menus
        SetupCallbacks();

        // Initialize dropdown options
        SetDesignFamilyDropdownOptions();

        _isInitialized = true;
    }

    public void ExitUI()
    {
        // TODO: do we need to deactivate any of the other UI elements?
        canvas.gameObject.SetActive(false);
    }

    public void LaunchPartDesignerUI()
    {
        if (!_isInitialized)
            Initialize();
        canvas.gameObject.SetActive(true);
        LaunchPartTypeSelector();
    }

    public void LaunchPartTypeSelector()
    {
        newDesignSelectorMenu.transform.parent.gameObject.SetActive(true);
        newDesignSelectorMenu.enabled = true;
    }

    public void LaunchContinuePrompt()
    {
        continuePrompt.SetActive(true);
    }

    public void SetDesignFamilyDropdownOptions()
    {
        List<string> familyNames = segmentDatabase.GetDesignFamilyNamesByPartType(_partType);
        designFamilyDropdown.AddOptions(familyNames);
        UpdateSegmentSelector(familyNames[0]);
    }

    public void SetOutputText(string output)
    {
        outputText.text = output;
    }

    public void SetPartType(string pType)
    {
        if (pType == null)
            return;
        _partType = pType;
        SetDesignFamilyDropdownOptions();
    }

    public void SetCanvas(Canvas newCanvas)
    {
        if (newCanvas != null)
        {
            canvas = newCanvas;
            Text t1 = canvas.transform.Find("Output_Text").GetComponent<Text>();
            Text t2 = canvas.transform.Find("SelectedPartType_Text").GetComponent<Text>();
            Dropdown d1 = canvas.transform.Find("PartDesignFamily_Dropdown").GetComponent<Dropdown>();
            DropdownGameObjectSelector d2 = canvas.transform.Find("SegmentSelector_Dropdown").GetComponent<DropdownGameObjectSelector>();
            if (t1 && t2 && d1 && d2)
            {
                outputText = t1;
                selectedPartTypeText = t2;
                designFamilyDropdown = d1;
                _segmentSelector = d2;
            }
        }
    }

    public void UpdateSegmentSelector(string designFamily)
    {
        GameObject[] segments = segmentDatabase.GetSegmentsByPartTypeAndFamily(_partType, designFamily);
        _segmentSelector.SetDropdownObjects(segments);
    }

    /*************************** END Public Methods ***************************/


    /************************ Functions for Callbacks *************************/

    public void OnContinue()
    {
        partDesigner.craftingApparatus.uiManager.OnContinueToPartCrafting();
    }

    public void OnDoNotContinue()
    {
        partDesigner.craftingApparatus.uiManager.OnExit();
    }

    public void OnDesignFamilySelection(int index)
    {
        UpdateSegmentSelector(designFamilyDropdown.options[index].text);
    }

    public void OnFinishButtonClicked()
    {
        if (partDesigner.AssemblePartDesign())
        {
            partDesigner.AddResultToPartDatabase();
            LaunchContinuePrompt();
        }
        else
        {
            outputText.text = "ERROR: Part could not be assembled. Please " +
                "check the configuration of your part segments to make sure " +
                "all segments are connected and there are base and end " +
                "segments in the correct locations.";
        }
    }

    public void OnLaunchMenuConfirm()
    {
        newDesignSelectorMenu.gameObject.SetActive(false);
    }

    public void OnLaunchMenuBackButton()
    {
        partDesigner.craftingApparatus.uiManager.OnRestart();
    }

    public void OnPartTypeSelection(string partType)
    {
        // TODO: Validate partType with DB?
        SetPartType(partType);
        partDesigner.partType = partType;
        selectedPartTypeText.text = partType;
    }

    /********************** END Functions for Callbacks ***********************/


    /*************************** Callback Config ******************************/
    public UnityAction confirmSelection;
    public UnityAction backButtonPressed;
    public UnityAction finishButtonPressed;

    public void SetupCallbacks()
    {
        SetupPartTypeSelector();
        SetupContinueMenu();
        SetupDropDownCallbacks();
        SetupFinishButtonCallbacks();
    }

    public void SetupPartTypeSelector()
    {
        if (newDesignSelectorMenu != null)
        {
            confirmSelection += OnLaunchMenuConfirm;
            backButtonPressed += OnLaunchMenuBackButton;
            newDesignSelectorMenu.SetConfirmButtonDelegate(confirmSelection);
            newDesignSelectorMenu.SetBackButtonDelegate(backButtonPressed);
            newDesignSelectorMenu.SetDropdownDelegate(OnPartTypeSelection);
        }
    }

    public void SetupContinueMenu()
    {
        continuePromptController = continuePrompt.GetComponentInChildren<ContinuePromptController>();
        Debug.Assert(continuePromptController != null);
        continuePromptController.SetNoButtonCallback(OnDoNotContinue);
        continuePromptController.SetYesButtonCallback(OnContinue);
    }

    public void SetupDropDownCallbacks()
    {
        designFamilyDropdown.onValueChanged.AddListener(
            delegate { OnDesignFamilySelection(designFamilyDropdown.value); } );
        _segmentSelector.SetDropdownDelegate(partDesigner.SetComponentToAdd);
    }

    public void SetupFinishButtonCallbacks()
    {
        finishButtonPressed += OnFinishButtonClicked;
        finishDesignButton.onClick.AddListener(finishButtonPressed);
    }

    /************************* END Callbacks Config ***************************/

}
