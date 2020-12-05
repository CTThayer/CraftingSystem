using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownGameObjectSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] Objects;
    [SerializeField] private Dropdown DropdownMenu;

    // TODO: Add a field for thumbnail reference

    public delegate void SelectionDelegate(GameObject go);
    public SelectionDelegate SelectActionDelegate;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(DropdownMenu != null);
        if (Objects != null)
            UpdateDropdownList();
        else
            Debug.Log("DropdownGameObjectSelector: Objects List was not initialized with default set of objects.");
    }

    public void SetDropdownObjects(GameObject[] objects)
    {
        Objects = objects;
        UpdateDropdownList();

        // NOTE: this ensures that the top object/option in the
        // dropdown is automatically selected when the dataset changes.
        // Comment this out if you do not want this behavior.
        //SelectActionDelegate(Objects[0]);
    }

    public void SetDropdownDelegate(SelectionDelegate del)
    {
        SelectActionDelegate = del;
        //Debug.Log("DropdownGameObjectSelector: SelectionDelegate was set.");
    }

    public void OnSelection(int index)
    {
        //Debug.Log("Dropdown passed index: " + index);

        if (index < Objects.Length && index >= 0)
        {
            SelectActionDelegate(Objects[index]);
            
            
            // TODO: Update thumbnail with selected GameObject's image
        }
        else
            Debug.Log("DropdownObjectSelector: ExecuteDelegate() - Index was outside the bounds of Objects[].");
    }

    public GameObject GetObjectFromDropdown(int index)
    {
        return Objects[index];
    }

    private void UpdateDropdownList()
    {
        DropdownMenu.options.Clear();
        List<string> ObjectNames = new List<string>();
        for (int i = 0; i < Objects.Length; i++)
        {
            ObjectNames.Add(Objects[i].name);
        }
        DropdownMenu.AddOptions(ObjectNames);
    }
}
