using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownCollectionSelector : MonoBehaviour
{
    [SerializeField] List<string> CollectionNames;
    [SerializeField] GameObject[][] GameObjectCollection;
    [SerializeField] private Dropdown ThisDropdown;
    [SerializeField] private DropdownGameObjectSelector DependentDropdown;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(CollectionNames != null);
        Debug.Assert(GameObjectCollection != null);
        Debug.Assert(CollectionNames.Count == GameObjectCollection.Length);
        Debug.Assert(ThisDropdown != null);
        Debug.Assert(DependentDropdown != null);

        UpdateDropdownOptions();
    }

    public void OnDropdownSelection(int index)
    {
        DependentDropdown.SetDropdownObjects(GameObjectCollection[index]);
    }

    private void UpdateDropdownOptions()
    {
        ThisDropdown.AddOptions(CollectionNames);
    }
}
