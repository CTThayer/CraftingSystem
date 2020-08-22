using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignRequirements : MonoBehaviour
{
    // Name for the general category/sub-category of item that this is
    [SerializeField] private string ItemArchetype;

    // Overall dimensions of the entire item
    [SerializeField] private XYZRange MinMaxItemDims;

    // Sockets (G.O. references) for all required components of this item design
    [SerializeField] private GameObject[] ComponentSockets;

    // Strings used to identify what type of components are valid in each socket
    [SerializeField] private string[] ComponentTypes;

    // Range that each component can be translated from default socket location
    [SerializeField] private XYZRange[] MinMaxCompTranslations;

    // MinMax "bounds" that each component can inhabit
    [SerializeField] private XYZRange[] MinMaxCompDimensions;

    // Max rotation on each axis (abs) from the default orientation of socket 
    // at a given component (0-1: 0 being no rotation, 1 being full rotation)
    [SerializeField] private Vector3[] MaxComponentRotation; 

    // The point(s) where an object is grabbed/picked-up/manipulated
    // NOTE: if item is to be held by a manipulator, all components should be
    // parented to that manipulator GameObject before being finalized.
    [SerializeField] private GameObject[] Manipulators;

    // Do all parts of the item have to touch another part of the item?
    [SerializeField] private bool AllSectionsMustTouch;

    // Does this item require a UI thumbnail?
    [SerializeField] private bool RequiresThumbnail;

    // Start is called before the first frame update
    void Start()
    {
        // Assertions about data in the design requirements
        Debug.Assert(   MinMaxItemDims.xMin > 0f
                     && MinMaxItemDims.yMin > 0f
                     && MinMaxItemDims.zMin > 0f);
        Debug.Assert(   MinMaxItemDims.xMax > 0f
                     && MinMaxItemDims.yMax > 0f
                     && MinMaxItemDims.zMax > 0f);

        Debug.Assert(   ComponentSockets.Length == ComponentTypes.Length
                     && ComponentSockets.Length == MinMaxCompTranslations.Length
                     && ComponentSockets.Length == MinMaxCompDimensions.Length
                     && ComponentSockets.Length == MaxComponentRotation.Length);

        for (int i = 0; i < ComponentTypes.Length; i++)
        {
            Debug.Assert(ComponentTypes[i] != null 
                         && ComponentTypes[i].Length > 0
                         && ComponentTypes[i] != " ");
        }

        // NOTE: remove this line if it is later determined an that item without 
        // some sort of manipulation point can/should exist.
        Debug.Assert(Manipulators != null && Manipulators.Length > 0);
    }

}
