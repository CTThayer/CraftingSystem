using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PartDesignSelectorMenu : MonoBehaviour
{
    [SerializeField] private PartCraftingApparatus _craftingApparatus;
    //public PartCraftingApparatus craftingApparatus
    //{
    //    get => _craftingApparatus;
    //    //set
    //    //{
    //    //    if (value != null)
    //    //    {
    //    //        supportedTypes = value.supportedPartTypes;
    //    //        List<string> options = new List<string>(supportedTypes);
    //    //        SetPartTypeDropdownOptions(options);
    //    //    }
    //    //}
    //}

    [SerializeField] private string[] supportedTypes;
    [SerializeField] private Dropdown _partTypeDropdown;
    [SerializeField] private Dropdown _partSubtypeDropdown;
    [SerializeField] private Dropdown _partNamesDropdown;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;

    private bool _isInitialized = false;
    public bool isInitialized { get => _isInitialized; }

    //private string selectedType;
    //private string selectedSubtype;
    //private string selectedPartName;

    public string selectedType;
    public string selectedSubtype;
    public string selectedPartName;

    // Start is called before the first frame update
    void Start()
    {
        if (!isInitialized)
            Initialize();
    }

    public void Initialize(PartCraftingApparatus apparatus)
    {
        if (_craftingApparatus != null)
        {
            _craftingApparatus = apparatus;
            Initialize();
        }
    }

    public void Initialize()
    {
        Debug.Assert(_partTypeDropdown != null);
        Debug.Assert(_partSubtypeDropdown != null);
        Debug.Assert(_partNamesDropdown != null);
        Debug.Assert(_confirmButton != null);
        Debug.Assert(_backButton != null);

        supportedTypes = _craftingApparatus.supportedPartTypes;
        List<string> types = new List<string>(supportedTypes);
        SetPartTypeDropdownOptions(types);
        selectedType = types[0];

        List<string> subtypes = GetSubTypesForPartType(types[0]);
        SetSubTypeDropdownOptions(subtypes);
        selectedSubtype = subtypes[0];

        List<string> names = GetNames(types[0], subtypes[0]);
        SetPartNamesDropdownOptions(names);
        selectedPartName = names[0];

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
        _partTypeDropdown.ClearOptions();
        _partTypeDropdown.AddOptions(partTypes);
    }

    public void SetSubTypeDropdownOptions(List<string> subtypes)
    {
        _partSubtypeDropdown.ClearOptions();
        _partSubtypeDropdown.AddOptions(subtypes);
    }

    public void SetPartNamesDropdownOptions(List<string> names)
    {
        _partNamesDropdown.ClearOptions();
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

    public void OnPartTypeSelectionChange(int index)
    {
        selectedType = _partTypeDropdown.options[index].text;
        _partSubtypeDropdown.ClearOptions();
        SetSubTypeDropdownOptions(GetSubTypesForPartType(selectedType));

        selectedSubtype = _partSubtypeDropdown.options[0].text;
        _partNamesDropdown.ClearOptions();
        SetPartNamesDropdownOptions(GetNames(selectedType, selectedSubtype));

        selectedPartName = _partNamesDropdown.options[0].text;
    }

    public void OnPartSubtypeSelectionChange(int index)
    {
        selectedSubtype = _partSubtypeDropdown.options[index].text;
        _partNamesDropdown.ClearOptions();
        SetPartNamesDropdownOptions(GetNames(selectedType, selectedSubtype));

        selectedPartName = _partNamesDropdown.options[0].text;
    }

    public void OnPartNameSelectionChange(int index)
    {
        selectedPartName = _partNamesDropdown.options[index].text;
    }

    public void OnConfirm()
    {
        if (selectedType != null && selectedType != ""
            && selectedSubtype != null && selectedSubtype != ""
            && selectedPartName != null && selectedPartName != "")
        {
            _craftingApparatus.LoadDesign(selectedType,
                                      selectedSubtype,
                                      selectedPartName);
            _craftingApparatus.ActivatePartCreator();
            transform.parent.gameObject.SetActive(false);
        }
        // TODO: Handle case where options aren't set
    }

    public void OnBack()
    {
        _craftingApparatus.uiManager.OnRestart();
    }

}
