using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignRequirements : MonoBehaviour
{
    // Name of the broad category this item falls into
    [SerializeField] private string _itemType;
    public string itemType { get; }

    // Name of the sub-category this item falls into
    [SerializeField] private string _itemSubType;
    public string itemSubType { get; }

    // Requirements for the overall dimensions of the entire item (min/max range)
    [SerializeField] private XYZRange _itemDimensionsRange;
    public XYZRange itemDimensionsRange { get; }

    // Sockets (G.O. references) for all required parts for this item design
    [SerializeField] private GameObject[] _partSockets;                         // TODO: Should this be changed to references to the PartSocket scripts?
    public GameObject[] partSockets { get; }

    // The requirements/constraints for each required part in this design.
    [SerializeField] private PartRequirements[] _partReqs;
    public PartRequirements[] partReqs { get; }

    // Required components (scripts) that need to be attached to the top-level
    // of the resulting item's hierarchy.
    [SerializeField] private string[] _requiredScripts;
    public string[] requiredScripts { get; }

    // The point(s) where an object is grabbed/picked-up/manipulated/touched
    [SerializeField] private GameObject[] _manipulators;
    public GameObject[] manipulators { get; }

                                                                                // TODO: Should manipulators be constrained to an ItemPart/the item itself?
                                                                                //       Also, should the primary manipulator be the resulting object's parent?

    // Range that each component can be translated from default socket location
    [SerializeField] private XYZRange[] _manipulatorPosRanges;
    public XYZRange[] manipulatorPosRanges { get; }

    // Do all parts of the item have to touch another part of the item?         // TODO: Is this necessary with connection points/surface contact??
    [SerializeField] private bool _allSectionsMustTouch;
    public bool allSectionsMustTouch { get;}

    // Does this design require ItemParts to use connection points for placement
    // and validation?
    [SerializeField] private bool _useConnectionPoints;
    public bool useConnectionPoints { get; }

    //// Does this design require ItemParts to use surface contact for placement
    //// and validation?
    //[SerializeField] private bool _useSurfaceContact;                         // TODO: Do we need this type of "freeform" part configuration?
    //public bool useSurfaceContact { get; }                                    //       If not, remove all instances and supporting code.

    // Does this item require a UI thumbnail?
    [SerializeField] private bool _requiresThumbnail;
    public bool requiresThumbnail { get; }

    // Does this item require a rigidbody? (most will)
    [SerializeField] private bool _requiresRigidbody;
    public bool requiresRigidbody { get; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(itemType != null && itemSubType != null);
        
        // Assertions about data in the design requirements
        Debug.Assert(   _itemDimensionsRange.xMin > 0f
                     && _itemDimensionsRange.yMin > 0f
                     && _itemDimensionsRange.zMin > 0f);

        Debug.Assert(_partSockets.Length == _partReqs.Length);

        // NOTE: remove these lines if it is later determined an that item 
        // without some sort of manipulation point can/should exist.
        Debug.Assert(_manipulators != null && _manipulators.Length > 0);
        Debug.Assert(_manipulatorPosRanges.Length == _manipulators.Length);
    }

    public bool ValidatePartConfig(ItemPart[] parts, out string result)
    {
        // Check for obvious errors
        if (parts == null || parts.Length == 0)
        {
            result = "ERROR: Invalid Configuration: No parts were provided.";
            return false;
        }
        if (parts.Length != partSockets.Length)
        {
            result = "ERROR: Invalid Configuration: Not all sockets of the " +
                     "item design have parts assigned to them.";
            return false;
        }

        // Check part configuration
        Bounds overallBounds = new Bounds();
        for (int i = 0; i < parts.Length; i++)
        {
            if (partReqs[i].PartMeetsBaseRequirements(parts[i]))
            {
                // If connection points are used, make sure there is the correct
                // number of them per the part reqs and make sure the are filled
                if (useConnectionPoints)
                {
                    bool r = partReqs[i].ValidatePartConnections(parts[i], out result);
                    // TODO: Add orientation checks? OR is this guaranteed during placement??
                    if (r == false)
                        return false;
                }
                // TODO: Add surface contact checks here if they are used.
            }
            // Add ItemPart's bounds to total bounds
            overallBounds.Encapsulate(parts[i].gameObject.GetComponent<Renderer>().bounds);
        }
        // Check if overall item bounds fit within size constraints
        if (!itemDimensionsRange.ContainsPoint(overallBounds.size))
        {
            result = "ERROR: Invalid Configuration: Current configuration " +
                     "does not fit within the required size constraints for " +
                     "an item of this type. Please reconfigure your layout.";
            return false;
        }

        // Check manipulator configuration
        for (int m = 0; m < manipulators.Length; m++)
        {
            // If manipulator is outside correspoding range, return false
            if (!manipulatorPosRanges[m].ContainsPoint(manipulators[m].transform.position))
            {
                // TODO: Add manipulator object error highlighting here
                result = "ERROR: Invalid Configuration: Manipulator outside " +
                         "acceptable position range.";
                return false;
            }
        }

        result = "SUCCESS: Part configuration is valid.";
        return true;
    }

    /************************* Private Helper Methods *************************/

    //private bool CheckPartPosition(ItemPart part)
    //{

    //}

    //private bool CheckPartOrientation(ItemPart part)
    //{

    //}


    /*********************** END Private Helper Methods ***********************/
}
