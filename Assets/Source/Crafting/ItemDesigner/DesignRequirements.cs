using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignRequirements : MonoBehaviour
{
    // Name for the general category/sub-category of item that this is
    [SerializeField] private string _itemArchetype;
    public string itemArchetype { get; }

    // Overall dimensions of the entire item
    [SerializeField] private XYZRange _itemDimensionsRange;
    public XYZRange itemDimensionsRange { get; }

    // Sockets (G.O. references) for all required components of this item design
    [SerializeField] private GameObject[] _partSockets;
    public GameObject[] partSockets { get; }

    [SerializeField] private PartRequirements[] _partReqs;
    public PartRequirements[] partReqs { get; }

    //// Strings used to identify what type of components are valid in each socket
    //[SerializeField] private string[] _partTypes;
    //public string[] partTypes { get; }

    //// TODO: Do we need to store allowed materials per part?

    //// MinMax "bounds" that each component can inhabit
    //[SerializeField] private XYZRange[] _partDimensionRanges;
    //public XYZRange[] partDimensionRanges { get; }

    // Range that each component can be translated from default socket location
    //[SerializeField] private XYZRange[] _partTranslationRanges;
    //public XYZRange[] partTranslationRanges { get; }

    //// Min/Max rotation on each axis (abs) from the default orientation of
    //// socket at a given component. Should be at most -360 to 360.
    //[SerializeField] private XYZRange[] _partRotationRanges;
    //public XYZRange[] partRotationRanges { get; }

    // Required components (scripts) that need to be attached to the top-level
    // of the resulting item's hierarchy.
    [SerializeField] private string[] _requiredScripts;
    public string[] requiredScripts { get; }


    // The point(s) where an object is grabbed/picked-up/manipulated
    // NOTE: if item is to be held by a manipulator, all components should be
    // parented to that manipulator GameObject before being finalized.
    [SerializeField] private GameObject[] _manipulators;
    public GameObject[] manipulators { get; }

    // Range that each component can be translated from default socket location
    [SerializeField] private XYZRange[] _manipulatorPosRanges;
    public XYZRange[] manipulatorPosRanges { get; }

    // Do all parts of the item have to touch another part of the item?
    [SerializeField] private bool _allSectionsMustTouch;
    public bool allSectionsMustTouch { get;}

    // Does this item require a UI thumbnail?
    [SerializeField] private bool _requiresThumbnail;
    public bool requiresThumbnail { get; }

    // Start is called before the first frame update
    void Start()
    {
        // Assertions about data in the design requirements
        Debug.Assert(   _itemDimensionsRange.xMin > 0f
                     && _itemDimensionsRange.yMin > 0f
                     && _itemDimensionsRange.zMin > 0f);

        Debug.Assert(_partSockets.Length == _partReqs.Length);
                     //&& _partSockets.Length == _partTranslationRanges.Length
                     //&& _partSockets.Length == _partRotationRanges.Length);

        // NOTE: remove these lines if it is later determined an that item 
        // without some sort of manipulation point can/should exist.
        Debug.Assert(_manipulators != null && _manipulators.Length > 0);
        Debug.Assert(_manipulatorPosRanges.Length == _manipulators.Length);
    }

    public bool ValidatePartConfig(ItemPart[] parts, out string result)
    {
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

        Bounds overallBounds = new Bounds();

        // Check part configuration                                             TODO: IMMPLEMENT CONFIG CHECKS
        for (int i = 0; i < parts.Length; i++)
        {
            // Check part position
            //Vector3 partPosOffset = parts[i].transform.position - partSockets[i].transform.position;
            //float dist = partPosOffset.magnitude;

            // Check part orientation


            // Check part requirements
            if (partReqs[i].PartMeetsBaseRequirements(parts[i]))
            {

            }

            // Add ItemPart's bounds to total bounds
            overallBounds.Encapsulate(parts[i].gameObject.GetComponent<Renderer>().bounds);
        }

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

        result = "";
        return false;
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
