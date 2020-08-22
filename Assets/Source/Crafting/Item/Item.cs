using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    /********************* Properties Common To All Items *********************/
    private float _weight;
    public float weight
    {
        get => _weight;
        set => _weight = (value > 0f) ? value : _weight;
    }

    private float _baseValue;
    public float baseValue
    {
        get => _baseValue;
        set => _baseValue = (value >= 0f) ? value : _baseValue;
    }

    // Possibly unnecessary?
    private string _itemName;
    public string itemName
    {
        get => _itemName;
        set => _itemName = (value != null) ? value : _itemName;
    }

    // UniqueItemID (UIID)
    // For registering & querying item in the database. 
    // TODO: implement validation/assigment
    private string UniqueItemID;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure that the Item has a mesh and a collider
        Debug.Assert(this.transform.GetComponent<MeshFilter>() != null);
        Debug.Assert(this.transform.GetComponent<MeshFilter>().mesh != null);
        Debug.Assert(this.transform.GetComponent<Collider>() != null);
    }

}
