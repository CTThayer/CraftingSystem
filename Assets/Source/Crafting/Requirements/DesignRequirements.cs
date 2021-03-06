﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignRequirements : Requirements
{
    // Name of the broad category this item falls into
    [SerializeField] private string _itemType;
    public string itemType { get => _itemType; }

    // Name of the sub-category this item falls into
    [SerializeField] private string _itemSubType;
    public string itemSubType { get => _itemSubType; }

    // Requirements for the overall dimensions of the entire item (min/max range)
    [SerializeField] private XYZRange _itemDimensionsRange;
    public XYZRange itemDimensionsRange { get => _itemDimensionsRange; }

    // The "blueprint" for the item with all the GameObjects and logic for 
    // adding parts to the design with supporting UI
    [SerializeField] private PartLayout _partLayout;
    public PartLayout partLayout { get => _partLayout; }

    // Required components (scripts) that need to be attached to the top-level
    // of the resulting item's hierarchy.
    [SerializeField] private string[] _requiredScripts;
    public string[] requiredScripts { get => _requiredScripts; }

    // The point(s) where an object is grabbed/picked-up/manipulated/touched
    [SerializeField] private GameObject[] _manipulators;
    public GameObject[] manipulators { get => _manipulators; }

    // Range that each component can be translated from default socket location
    [SerializeField] private XYZRange[] _manipulatorPosRanges;
    public XYZRange[] manipulatorPosRanges { get => _manipulatorPosRanges; }

    //// Do all parts of the item have to touch another part of the item?         // TODO: Is this necessary with connection points/surface contact??
    //[SerializeField] private bool _allSectionsMustTouch;
    //public bool allSectionsMustTouch { get => _allSectionsMustTouch; }

    // Does this design require ItemParts to use connection points for placement
    // and validation?
    [SerializeField] private bool _useConnectionPoints;
    public bool useConnectionPoints { get => _useConnectionPoints; }

    // Does this item require a UI thumbnail?
    [SerializeField] private bool _requiresThumbnail;
    public bool requiresThumbnail { get => _requiresThumbnail; }

    // Does this item require a rigidbody? (most do)
    [SerializeField] private bool _requiresRigidbody;
    public bool requiresRigidbody { get => _requiresRigidbody; }

    // Private Fields assigned in Start()
    private PartRequirements[] partReqs;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(itemType != null && itemSubType != null);
        
        // Assertions about data in the design requirements
        Debug.Assert(   _itemDimensionsRange.xMin > 0f
                     && _itemDimensionsRange.yMin > 0f
                     && _itemDimensionsRange.zMin > 0f);

        // NOTE: remove these lines if it is later determined an that item 
        // without some sort of manipulation point can/should exist.
        Debug.Assert(_manipulators != null && _manipulators.Length > 0);
        Debug.Assert(_manipulatorPosRanges.Length == _manipulators.Length);
    }

    /**************************** Override Methods ****************************/
    public override bool ValidateConfiguration(out string output)
    {
        ItemPart[] parts;
        bool fullArray = partLayout.GetPartsArray(out parts);
        if (fullArray)
            return ValidatePartConfig(parts, out output);
        else
        {
            output = "DesignRequirements: The current configuration is not " +
                     "valid because not all parts required by this design " +
                     "have been added to the workspace.";
            return false;
        }
    }
    /************************** END Override Methods **************************/


    /***************************** Public Methods *****************************/
    // TODO: Are the part checks necessary? Can this be eliminated if the checks
    // are run when the part is added?
    // TODO: Can this be private? - Currently used by factory
    public bool ValidatePartConfig(ItemPart[] parts, out string output)         
    {
        // Get PartRequirements from partLayout                                 
        partReqs = partLayout.GetPartRequirements();

        // Check for obvious errors
        if (parts == null || parts.Length == 0)
        {
            output = "ERROR: Invalid Configuration: No parts were provided.";
            return false;
        }
        if (parts.Length != partReqs.Length)
        {
            output = "ERROR: Invalid Configuration: Array dimension mismatch." +
                     "parts[] and partSockets[] are not the same size.";
            return false;
        }

        // Check part configuration & overall size
        Bounds partABounds = parts[0].gameObject.GetComponent<Renderer>().bounds;
        Bounds overallBounds = new Bounds(partABounds.center, partABounds.size);
        for (int i = 1; i < parts.Length; i++)
        {
            bool partCheck = CheckPartAgainstReqs(parts[i], i, out output);
            if (partCheck == false)
                return false;
            overallBounds.Encapsulate(parts[i].gameObject.GetComponent<Renderer>().bounds);
        }
        if (CheckOverallItemBounds(overallBounds, out output) == false)
            return false;

        //// Check manipulator configuration
        //for (int m = 0; m < manipulators.Length; m++)
        //{
        //    // If manipulator is outside correspoding range, return false
        //    if (!manipulatorPosRanges[m].ContainsPoint(manipulators[m].transform.position))
        //    {
        //        // TODO: Add manipulator object error highlighting here
        //        output = "ERROR: Invalid Configuration: Manipulator outside " +
        //                 "acceptable position range.";
        //        return false;
        //    }
        //}

        output = "SUCCESS: Part configuration is valid.";
        return true;
    }

    /*************************** END Public Methods ***************************/


    /************************* Private Helper Methods *************************/
    // TODO: Are the part checks necessary? Can this be eliminated if the checks
    // are run when the part is added?
    private bool CheckPartAgainstReqs(ItemPart part, 
                                      int index,
                                      out string output)
    {
        if (part == null)
        {
            // TODO: Improve this messaging.
            output = "ERROR: Invalid Configuration: Item is missing a part " +
                     "at " + index + ".";
            return false;
        }
        if (partReqs[index].PartMeetsBaseRequirements(part))
        {
            // If connection points are used, make sure there is the correct
            // number of them per the part reqs and make sure the are filled
            if (useConnectionPoints)
            {
                bool r = partReqs[index].ValidatePartConnections(part, out output);
                // TODO: Add orientation checks? OR is this guaranteed during placement??
                if (r == false)
                {
                    output = "ERROR: Invalid Configuration: Connection points "
                             + "are not connected properly for part " + index +
                             ": " + part.name;
                    return false;
                }
            }
            output = "";
            return true;
        }
        else
        {
            output = "ERROR: Invalid Configuration: Part " + index + ": " +
                     part.name + " doesn't meet the requirements for that " +
                     "part.";
            return false;
        }
    }

    private bool CheckOverallItemBounds(Bounds overallBounds, out string output)
    {
        // Check if overall item bounds fit within size constraints
        if (!itemDimensionsRange.ContainsPoint(overallBounds.size))
        {
            output = "ERROR: Invalid Configuration: Current configuration " +
                     "does not fit within the required size constraints for " +
                     "an item of this type. Please reconfigure your layout.";
            return false;
        }
        output = "";
        return true;
    }

    private bool CheckOverallItemBounds(Vector3 overallBoundsSize, out string output)
    {
        // Check if overall item bounds fit within size constraints
        if (!itemDimensionsRange.ContainsPoint(overallBoundsSize))
        {
            output = "ERROR: Invalid Configuration: Current configuration " +
                     "does not fit within the required size constraints for " +
                     "an item of this type. Please reconfigure your layout.";
            return false;
        }
        output = "";
        return true;
    }

    /*********************** END Private Helper Methods ***********************/

}
