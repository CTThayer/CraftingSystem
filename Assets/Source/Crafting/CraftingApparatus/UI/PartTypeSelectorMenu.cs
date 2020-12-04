using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PartTypeSelectorMenu : MonoBehaviour
{
    [SerializeField] private PartCraftingApparatusUIManager _uiManager;
    public PartCraftingApparatusUIManager uiManager
    {
        get => _uiManager;
        set
        {
            if (value != null)
            {
                designTypes = value.craftingApparatus.supportedPartTypes;
                SetDropdownObjects(designTypes);
                //SetDropdownDelegate(value.OnPartTypeSelection);
                //if (_backButton != null)
                //{
                //    UnityAction back = value.OnSelectionMenuBackButton;
                //    _backButton.onClick.AddListener(back);
                //}
            }
        }
    }

    [SerializeField] private string[] designTypes;
    [SerializeField] private Dropdown _dropdown;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;

    public delegate void DesignSelectionDelegate(string s);
    public DesignSelectionDelegate SelectActionDelegate;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_dropdown != null);
        if (designTypes != null)
        {
            List<string> designList = new List<string>(designTypes);
            _dropdown.AddOptions(designList);
        }
        else
        {
            Debug.Log("DesignSelectorDropdown: DesignTypes was not " +
                      "initialized with default set of values.");
        }

        Debug.Assert(_confirmButton != null);
        Debug.Assert(_backButton != null);
    }


    /**************************** Dropdown Controls ***************************/
    public void SetDropdownObjects(string[] designs)
    {
        designTypes = designs;
        List<string> designList = new List<string>(designTypes);
        _dropdown.AddOptions(designList);
        //SelectActionDelegate(designTypes[0]);   // NOTE: this ensures that the top object/option in the dropdown is automatically selected when the dataset changes. Comment this out if you do not want this behavior.
    }

    public void SetConfirmButtonDelegate(UnityAction confirm)
    {
        _confirmButton.onClick.AddListener(confirm);
    }

    public void SetBackButtonDelegate(UnityAction back)
    {
        _confirmButton.onClick.AddListener(back);
    }

    public void SetDropdownDelegate(DesignSelectionDelegate del)
    {
        SelectActionDelegate = del;
    }

    public void OnSelection(int index)
    {
        if (index < designTypes.Length && index >= 0)
        {
            SelectActionDelegate(designTypes[index]);
        }
        else
        {
            Debug.Log("DropdownObjectSelector: ExecuteDelegate() - Index was" +
                      " outside the bounds of Objects[].");
        }
    }
    /**************************** Dropdown Controls ***************************/
}
