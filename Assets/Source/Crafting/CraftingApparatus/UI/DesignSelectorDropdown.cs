using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesignSelectorDropdown : MonoBehaviour
{
    [SerializeField] private string[] designTypes;
    [SerializeField] private Dropdown DropdownMenu;

    // TODO: Add a field for thumbnail reference

    public delegate void DesignSelectionDelegate(string s);
    public DesignSelectionDelegate SelectActionDelegate;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(DropdownMenu != null);
        if (designTypes != null)
        {
            List<string> designList = new List<string>(designTypes);
            DropdownMenu.AddOptions(designList);
        }
        else
        {
            Debug.Log("DesignSelectorDropdown: DesignTypes was not " +
                      "initialized with default set of values.");
        }
    }

    public void SetDropdownObjects(string[] designs)
    {
        designTypes = designs;
        List<string> designList = new List<string>(designTypes);
        DropdownMenu.AddOptions(designList);
        SelectActionDelegate(designTypes[0]);   // NOTE: this ensures that the top object/option in the dropdown is automatically selected when the dataset changes. Comment this out if you do not want this behavior.
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
            Debug.Log("DropdownObjectSelector: ExecuteDelegate() - Index was outside the bounds of Objects[].");
    }
}
