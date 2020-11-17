using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class ItemPart : Item
public class ItemPart : MonoBehaviour
{
    [SerializeField] private string _partType;                                  // TODO: Change to Enum??
    public string partType
    {
        get => _partType;
        // TODO: implement full validation of set value against known types OR change to Enum
        set
        {
            if (value != null)
                _partType = value;
        }
    }

    public PhysicalStats physicalStats;

    [SerializeField] private float _maxDurability;
    public float maxDurability { get; private set; }

    [SerializeField] private float _currentDurability;
    public float currentDurability
    {
        get => _currentDurability;
        set
        {
            if (value >= 0 && value <= _maxDurability)
                _currentDurability = value;
        }
    }

    [SerializeField] private float _partQuality;                                // TODO: DO WE NEED THIS VALUE?
    public float partQuality
    {
        get => _partQuality;
        set => _partQuality = value > 0 ? value : _partQuality;
    }

    [SerializeField] private CraftingMaterial _craftingMaterial;                // TODO: DO WE NEED THIS REFERENCE?
    public CraftingMaterial craftingMaterial { get; private set; }              // TODO: Does the need to be settable also?


    private Color originalColor; // For selection/deselection purposes

    void Awake()
    {
        originalColor = this.GetComponent<MeshRenderer>().material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Verify that this GameObject has necessary components
        Debug.Assert(this.GetComponent<MeshFilter>() != null);
        Debug.Assert(this.GetComponent<MeshFilter>().mesh != null);
        Debug.Assert(this.GetComponent<MeshRenderer>() != null);
        Debug.Assert(this.GetComponent<Collider>() != null);

        // Validate data fields
        Debug.Assert(_partType != null && _partType.Length > 0);
        Debug.Assert(_maxDurability >= 0 && _maxDurability >= _currentDurability);
        Debug.Assert(_partQuality > 0);

        //_partVolume = CalculatePartVolume();
    }

    public void Initialize(string typeOfPart,
                           float durabilityMax,
                           float durabilityCurrent,
                           float quality,
                           CraftingMaterial mat)
    {
        maxDurability = durabilityMax;
        currentDurability = durabilityCurrent;
        partType = typeOfPart;
        partQuality = quality;
        if (mat != null)
            craftingMaterial = mat;
    }


    public void DeactivateScript(string scriptName)
    {
        Component script = GetComponent(scriptName);
        Behaviour behaviorScript  = script as Behaviour;
        if (behaviorScript != null)
            behaviorScript.enabled = false;
    }

    public void ActivateScript(string scriptName)
    {
        Component script = GetComponent(scriptName);
        Behaviour behaviorScript = script as Behaviour;
        if (behaviorScript != null)
            behaviorScript.enabled = true;
    }


                    // TODO: Are these next methods (OnSelect/OnDeselect) actually necessary??

    /* OnSelect()
     * Changes the color of the GameObject to indicate it is currently selected. 
     */
    public void OnSelect()
    {
        this.GetComponent<MeshRenderer>().material.color = new Color(0.902f, 0.863f, 0.560f, 0.500f);
    }

    /* OnDeselect()
     * Changes the color of the GameObject back to it's original color to 
     * indicate that it has been deselected.
     */
    public void OnDeselect()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originalColor;
    }
    
    // For parts that require the use of connection points/connected parts
    [SerializeField] private GameObject[] _connectionPoints;
    public GameObject[] connectionPoints
    {
        get => _connectionPoints;
        set
        {
            if (value != null && value.Length > 0)                              // TODO: Do we need more extensive checks here?
                _connectionPoints = value;
        }
    }
    public GameObject GetConnectionPoint(int index) { return _connectionPoints[index]; }
    public int GetNumberOfConnectionPoints() { return _connectionPoints.Length - 1; }

    [SerializeField] private ItemPart[] _connectedParts;
    public ItemPart[] connectedParts { get; }
    public ItemPart GetConnectedPart(int index) { return _connectedParts[index]; }
    public int GetNumberOfConnectedParts() { return _connectedParts.Length - 1; }
    public int GetIndexOfConnection(ItemPart part)
    {
        if (part != null)
        {
            for (int i = 0; i < _connectedParts.Length; i++)
            {
                if (_connectedParts[i] == part)
                    return i;
            }
        }
        return -1;
    }
}
