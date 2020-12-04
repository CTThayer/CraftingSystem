using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class PartSelectorMenu : MonoBehaviour
{

    [SerializeField] private PartCraftingApparatus _craftingApparatus;
    public PartCraftingApparatus craftingApparatus
    {
        get => _craftingApparatus;
        set
        {
            if (value != null)
            {
                supportedTypes = value.supportedPartTypes;
                List<string> options = new List<string>(supportedTypes);
                SetPartTypeDropdownOptions(options);
            }
        }
    }

    [SerializeField] private string[] supportedTypes;
    [SerializeField] private Dropdown _partTypeDropdown;
    [SerializeField] private Dropdown _partSubtypeDropdown;
    [SerializeField] private Dropdown _partNamesDropdown;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;

    private bool _isInitialized;
    public bool isInitialized { get => _isInitialized; }

    private string selectedType;
    private string selectedSubtype;
    private string selectedPartName;

    // Start is called before the first frame update
    void Start()
    {
        if (!isInitialized)
            Initialize();
    }

    public void Initialize(PartCraftingApparatus apparatus)
    {
        if (craftingApparatus != null)
        {
            craftingApparatus = apparatus;
            Initialize();
        }
    }

    public void Initialize()
    {
        Debug.Assert(_partTypeDropdown != null);
        if (supportedTypes != null)
        {
            List<string> designList = new List<string>(supportedTypes);
            _partTypeDropdown.AddOptions(designList);
        }
        else
        {
            Debug.Log("DesignSelectorDropdown: DesignTypes was not " +
                      "initialized with default set of values.");
        }

        Debug.Assert(_partSubtypeDropdown != null);
        Debug.Assert(_partNamesDropdown != null);
        Debug.Assert(_confirmButton != null);
        Debug.Assert(_backButton != null);

        _confirmButton.onClick.AddListener(OnConfirm);
        _backButton.onClick.AddListener(OnBack);

        _isInitialized = true;
    }

    public List<string> GetSubTypesForPartType(string partType)
    {
        return _craftingApparatus.partDesignDB.GetSubtypesByType(partType);
    }

    public List<string> GetNames(string partType, string partSubtype)
    {
        return _craftingApparatus.partDesignDB.GetNamesByTypeAndSubtype(partType, partSubtype);
    }

    public void SetPartTypeDropdownOptions(List<string> partTypes)
    {
        _partSubtypeDropdown.AddOptions(partTypes);
    }

    public void SetSubTypeDropdownOptions(List<string> subtypes)
    {
        _partSubtypeDropdown.AddOptions(subtypes);
    }

    public void SetPartNamesDropdownOptions(List<string> names)
    {
        _partNamesDropdown.AddOptions(names);
    }

    public void SetConfirmButtonDelegate(UnityAction confirm)
    {
        _confirmButton.onClick.AddListener(confirm);
    }

    public void SetBackButtonDelegate(UnityAction back)
    {
        _confirmButton.onClick.AddListener(back);
    }

    public void OnPartTypeSelection(int index)
    {
        selectedType = _partTypeDropdown.options[index].text;
        _partSubtypeDropdown.ClearOptions();
        SetSubTypeDropdownOptions(GetSubTypesForPartType(selectedType));
        _partNamesDropdown.ClearOptions();
    }

    public void OnPartSubtypeSelection(int index)
    {
        selectedSubtype = _partSubtypeDropdown.options[index].text;
        _partNamesDropdown.ClearOptions();
        SetPartNamesDropdownOptions(GetNames(selectedType, selectedSubtype));
    }

    public void OnPartSelection(int index)
    {
        selectedPartName = _partNamesDropdown.options[index].text;
    }

    public void OnConfirm()
    {
        //GameObject design = _craftingApparatus.partDesignDB.GetPartDesign(selectedType, selectedSubtype, selectedPartName);
        //_craftingApparatus.SetPartDesign(design);

        _craftingApparatus.LoadDesign(selectedType,
                                      selectedSubtype,
                                      selectedPartName);
        _craftingApparatus.ActivatePartCreator();
        this.gameObject.SetActive(false);
    }

    public void OnBack()
    {
        _craftingApparatus.uiManager.OnRestart();
    }

}
