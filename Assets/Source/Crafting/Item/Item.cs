using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhysicalStats
{
    [SerializeField] private float _mass;
    public float mass
    {
        get => _mass;
        set => _mass = value > 0 ? value : _mass;
    }
    [SerializeField] private float _volume;
    public float volume
    {
        get => _volume;
        set => _volume = value > 0 ? value : _volume;
    }
}

public class Item : MonoBehaviour
{
    /********************* Properties Common To All Items *********************/

    // UniqueItemID (UIID)
    // For registering & querying item in the database. 
    [SerializeField] private string _uniqueItemID;
    public string uniqueItemID
    {
        get => _uniqueItemID;
        private set => _uniqueItemID = value;                                    // TODO: implement public setting/validation function
    }

    // Possibly unnecessary? Use GameObject.name instead? 
    [SerializeField] private string _itemName;
    public string itemName
    {
        get => _itemName;
        set => _itemName = (value != null) ? value : _itemName;
    }

    [SerializeField] private string _itemDescription;
    public string itemDescription
    {
        get => _itemDescription;
        set => _itemDescription = (value != null) ? value : _itemDescription;
    }

    //[SerializeField] private float _mass;
    //public float mass
    //{
    //    get => _mass;
    //    private set => _mass = (value > 0f) ? value : _mass;
    //}

    //[SerializeField] private float _volume;
    //public float volume
    //{
    //    get => _volume;
    //    private set => _volume = (value > 0f) ? value : _volume;
    //}

    //[SerializeField] private PhysicalStats _physicalStats;
    //public PhysicalStats physicalStats { get; private set; }

    public PhysicalStats physicalStats;

    [SerializeField] private float _baseValue;
    public float baseValue
    {
        get => _baseValue;
        set => _baseValue = (value >= 0f) ? value : _baseValue;
    }

    [SerializeField] private GameObject[] _manipulators;
    public GameObject[] manipulators
    {
        get => _manipulators;
        private set
        {
            if (value != null && value.Length > 0)
                _manipulators = value;
        }
    }
    
    // References to the ItemParts that make up this item.
    [SerializeField] private ItemPart[] _itemParts;                             // TODO: Do we need this?
    public ItemPart[] itemParts { get; private set; }                           // TODO: Should this still be a property?


    /************************** END Common Properties *************************/

    // Start is called before the first frame update
    void Start()
    {
        //// Ensure that the Item has a mesh and a collider
        //Debug.Assert(this.transform.GetComponent<MeshFilter>() != null);
        //Debug.Assert(this.transform.GetComponent<MeshFilter>().mesh != null);
        //Debug.Assert(this.transform.GetComponent<Collider>() != null);

        // Validate properties (in case they were set in engine or incorrectly)
        Debug.Assert(_uniqueItemID != null && _uniqueItemID.Length > 0);
        Debug.Assert(_itemName != null && _itemName.Length > 0);
        Debug.Assert(_itemDescription != null && _itemDescription.Length > 0);
        Debug.Assert(physicalStats.mass > 0f);
        Debug.Assert(physicalStats.volume > 0f);
        Debug.Assert(_baseValue > 0f);
        //Debug.Assert(itemParts != null && itemParts.Length > 0);
    }

    /* Initialize
     * Initializes the data values and references for this item using the 
     * supplied parameters.
     * NOTE: This does NOT perform any validation. All validation should be done
     * by the ItemFactory. Items should NEVER be initialized by non-factory code
     */
    public void Initialize(string itemID,
                           string name,
                           string itemDesc,
                           float totalMass,
                           float totalVolume,
                           float value,
                           GameObject[] manipulatorObjs,
                           ItemPart[] parts)
    {
        uniqueItemID = itemID;
        itemName = name;
        itemDescription = itemDesc;
        physicalStats.mass = totalMass;
        physicalStats.volume = totalVolume;
        baseValue = value;
        manipulators = manipulatorObjs;
        itemParts = parts;
    }

    /* Append To Description
     * Adds an additional string to the Item's description. This is meant to be
     * used for adding flavor text as an Item gets used or interacts with the
     * game world (e.g. "Lost in the caves of doom." etc.)
     * NOTE: No validation is done to the string.
     * @Param stringToAdd - the string to append to the description.
     */
    public void AppendToDescription(string stringToAdd)
    {
        _itemDescription += stringToAdd;
    }

}
