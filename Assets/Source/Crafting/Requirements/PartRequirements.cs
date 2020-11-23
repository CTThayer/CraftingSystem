using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfCollider
{
    Box,
    Capsule,
    Mesh,
    Sphere,
    Terrain,                // TODO: Is this actually necessary?
    Wheel,                  // TODO: Is this actually necessary?
    Compound,
    NONE
}

[System.Serializable]
public class PartRequirements : Requirements
{
    [SerializeField] private string[] _allowedPartTypes;
    public string[] allowedPartTypes { get => _allowedPartTypes; }

    [SerializeField] private string[] _allowedMaterials;
    public string[] allowedMaterials { get => _allowedMaterials; }

    [SerializeField] private TypeOfCollider _requiredCollider;
    public TypeOfCollider requiredCollider { get => _requiredCollider; }

    [SerializeField] private string[] _requiredComponents;                      // NOTE: Components that must be attached to this part, NOT the top level of the item hierarchy
    public string[] requiredComponents { get => _requiredComponents; }          // NOTE: Strings MUST match the exact name of a script in order to be added.

    [SerializeField] private string[] _componentsToDeactivate;                  // NOTE: Components that need to be deactivated on this part when an item is built using it.
    public string[] componentsToDeactivate { get => _componentsToDeactivate; }  // NOTE: Strings MUST match the exact name of a script in order to be added.

    [SerializeField] private bool _useConnectionPoints;
    public bool useConnectionPoints { get => _useConnectionPoints; }

    [SerializeField] private bool _lockToConnectionPoint;
    public bool lockToConnectionPoint { get => _lockToConnectionPoint; }

    [SerializeField] private XYZRange _partDimensionsRange;
    public XYZRange partDimensionsRange { get => _partDimensionsRange; }

    [SerializeField] private XYZRange _partTranslationRange;
    public XYZRange partTranslationRange { get => _partTranslationRange; }

    [SerializeField] private XYZRange _partRotationRange;
    public XYZRange partRotationRange { get => _partRotationRange; }

    void Start()
    {
        // lockToConnectionPoints cannot be true while useConnectionPoints is false.
        Debug.Assert(!(!_useConnectionPoints && _lockToConnectionPoint));

        Debug.Assert(_allowedPartTypes != null && _allowedPartTypes.Length > 0);
        Debug.Assert(_allowedMaterials != null && _allowedMaterials.Length > 0);
    }


    public PartRequirements(XYZRange dimRange,                                  // TODO: Should we remove this constructor or change it to an Initialize() function?
                            XYZRange posRange,
                            XYZRange rotRange,
                            string[] allowedTypes,
                            string[] allowedMats,
                            TypeOfCollider colliderType,
                            bool useCPs,
                            bool useSC 
                            )
    {
        // Set part area/transform constraints
        _partDimensionsRange = dimRange;
        _partTranslationRange = posRange;
        _partRotationRange = rotRange;

        // Set allowed part types, materials, and collider type
        _allowedPartTypes = allowedTypes;
        _allowedMaterials = allowedMats;
        _requiredCollider = colliderType;

        // Set connection type
        _useConnectionPoints = useCPs;
    }

    /*********************** Public Validation Methods ************************/

    public override bool ValidateConfiguration(out string output)               // TODO: Finish this stub method
    {
        output = "finished";
        return true;
    }

    public bool PartMeetsBaseRequirements(ItemPart part)
    {
        //bool meetsReqs = IsAllowedItemType(part)
        return  IsAllowedItemType(part)
                && HasAllowedMaterialType(part)
                && HasCorrectCollider(part)
                && FitsInDimensions(part)
                && PartHasScripts(part);

        //    if (_useConnectionPoints)
        //    {
        //        meetsReqs = meetsReqs && PartsAreConnected(part, otherPart);
        //    }
        //    return meetsReqs;
    }

    /********************* END Public Validation Methods **********************/



    /******************* Private Validation Helper Methods ********************/

    /* Is Allowed Item Type
     * Checks if the part type for the specified part matches one of the allowed 
     * part types listed in the part requirements.
     */
    private bool IsAllowedItemType(ItemPart part)
    {
        for (int i = 0; i < allowedPartTypes.Length; i++)
        {
            if (allowedPartTypes[i] == part.partType)
                return true;
        }
        return false;
    }

    /* Is Allowed Item Type (overload)
     * Checks if the specified part type matches one of the allowed part types 
     * in the allowedPartTypes array.
     */
    private bool IsAllowedItemType(string partType)
    {
        for (int i = 0; i < allowedPartTypes.Length; i++)
        {
            if (allowedPartTypes[i] == partType)
                return true;
        }
        return false;
    }

    /* Has Allowed Material Type
     * Checks if the crafting material applied to the part is one of the 
     * material types in the allowed materials array of the part requirements.
     */
    private bool HasAllowedMaterialType(ItemPart part)
    {
        if (part == null)
            return false;
        for (int i = 0; i < allowedMaterials.Length; i++)
        {
            if (allowedMaterials[i] == part.craftingMaterial.materialType)
                return true;
        }
        return false;
    }

    /* Has Correct Collider
     * Checks if the collider attached to the part matches the type specified in
     * the part requirements.
     */
    private bool HasCorrectCollider(ItemPart part)                              // TODO: TEST THIS!
    {
        bool isCorrectCollider = false;

        // Get colliders on this object and its children
        Collider c = part.GetComponent<Collider>();
        List<Collider> colliders = new List<Collider>();
        Collider[] thisObjColliders = part.gameObject.GetComponents<Collider>();
        Collider[] childObjColliders = part.gameObject.GetComponentsInChildren<Collider>();
        colliders.AddRange(thisObjColliders);
        for (int i = 0; i < childObjColliders.Length; i++)
        {
            for (int j = 0; j < thisObjColliders.Length; j++)
            {
                if (childObjColliders[i] != colliders[j])
                    colliders.Add(childObjColliders[i]);
            }
        }

        if (c == null && colliders.Count == 0)
        {
            if (requiredCollider == TypeOfCollider.NONE)
                return true;
            else
                return false;
        }

        if (requiredCollider != TypeOfCollider.Compound && colliders.Count > 1)
            return false;
        else if (requiredCollider == TypeOfCollider.Compound && colliders.Count <= 1)
            return false;
        else if (requiredCollider == TypeOfCollider.Compound && colliders.Count > 1)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i] is TerrainCollider || colliders[i] is WheelCollider)
                    return false;
            }
            return true;
        }
        else
        {
            switch (requiredCollider)
            {
                case TypeOfCollider.Box:
                    if (c is BoxCollider)
                        isCorrectCollider = true;
                    break;
                case TypeOfCollider.Capsule:
                    if (c is CapsuleCollider)
                        isCorrectCollider = true;
                    break;
                case TypeOfCollider.Mesh:
                    if (c is MeshCollider)
                        isCorrectCollider = true;
                    break;
                case TypeOfCollider.Sphere:
                    if (c is SphereCollider)
                        isCorrectCollider = true;
                    break;
                case TypeOfCollider.Terrain:
                    if (c is TerrainCollider)
                        isCorrectCollider = true;
                    break;
                case TypeOfCollider.Wheel:
                    if (c is WheelCollider)
                        isCorrectCollider = true;
                    break;
            }
            return isCorrectCollider;
        }
    }

    /* Fits In Dimensions
     * Checks if the bounds of the part fit within the specified min/max range
     * of the part requirements.
     */
    private bool FitsInDimensions(ItemPart part)
    {
        Bounds partBounds = part.GetComponent<Renderer>().bounds;
        return partDimensionsRange.ContainsPoint(partBounds.size);
    }

    /* Parts Has Scripts
     * Checks whether the part has the required scripts attached to it.
     */
    private bool PartHasScripts(ItemPart part)
    {
        for (int i = 0; i < _requiredComponents.Length; i++)
        {
            System.Type componentType = System.Type.GetType(_requiredComponents[i]);
            if (componentType != null)
            {
                if (part.GetComponent(componentType) == null)
                    return false;
            }
        }
        return true;
    }

    /* Validate Part Connections
     * Checks if the specified part has the expected number of connections and
     * where these connections are filled.
     */
    public bool ValidatePartConnections(ItemPart part, out string s)
    {
        //if (part.connectedParts.Length == numConnections)
        //{
            for (int i = 0; i < part.connectedParts.Length; i++)
            {
                if (part.connectedParts[i] == null)
                {
                   s = "Part " + part.name + " is missing a connected part " +
                        "at connection point " + i + ".";
                    return false;
                }

                ItemPart p = part.connectedParts[i].GetComponent<ItemPart>();
                if (p == null)
                {
                    s = "Part " + part.name + " does not have a valid Item " +
                        "Part at connection point " + i + ".";
                    return false;
                }
                else if (p.GetIndexOfConnection(part) < 0)
                {
                    s = "Part " + part.name + " is connected to " + p.name +
                        " but part " + p.name + " is not connected " +
                        "to " + part.name;
                    return false;
                }

            }
            s = "";
            return true;
        //}
        //else
        //{
        //    s = "Part " + part.name + "does not have the correct number " +
        //        "of connections for this design. Please use a different part.";
        //    return false;
        //}
    }

    /* Part Has Enough Connections
     * Checks if the specified part has the expected number of connection points
     */
    //public bool PartHasEnoughConnections(ItemPart part)
    //{
    //    return part.connectedParts.Length == numConnections;
    //}

    /* Parts Are Connected
     * Checks if two parts are connected via connection points in each part.
     */
    //private bool PartsAreConnected(ItemPart part, ItemPart otherPart)           // TODO: Is this unnecessary now?
    //{
    //    if (part.GetIndexOfConnection(otherPart) >= 0
    //        && otherPart.GetIndexOfConnection(otherPart) >= 0)
    //        return true;
    //    else
    //        return false;
    //}

    /* Part Surfaces Touch
     * Checks if the colliders of the two parts touch or overlap.
     */
    private bool PartSurfacesTouch(ItemPart part, ItemPart otherPart)           // TODO: Is this unnecessary now?
    {
        Collider c = part.GetComponent<Collider>();
        if (c is BoxCollider)
        {
            Collider[] overlaps = Physics.OverlapBox(c.bounds.center, c.bounds.extents);
            return IncludesCollider(overlaps, otherPart.GetComponent<Collider>());
        }
        if (c is CapsuleCollider)
        {
            CapsuleCollider cap = c as CapsuleCollider;
            Vector3 p1 = cap.center;
            Vector3 p2 = p1;
            p1.y += (cap.height / 2) - cap.radius;
            p2.y -= (cap.height / 2) - cap.radius;
            Collider[] overlaps = Physics.OverlapCapsule(p1, p2, cap.radius);
            return IncludesCollider(overlaps, otherPart.GetComponent<Collider>());
        }
        if (c is SphereCollider)
        {
            SphereCollider s = c as SphereCollider;
            Collider[] overlaps = Physics.OverlapSphere(s.center, s.radius);
            return IncludesCollider(overlaps, otherPart.GetComponent<Collider>());
        }
        if (c is MeshCollider)
        {
            // TODO: implement mesh intersection testing & use here.
        }
        return false;
    }

    /* Includes Collider
    * Simple helper method for checking if an array of colliders contains a
    * specific collider.
    */
    private bool IncludesCollider(Collider[] colliders, Collider target)
    {
        if (colliders != null && target != null)
        { 
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == target)
                    return true;
            }
        }
        return false;
    }

    /***************** END Private Validation Helper Methods ******************/

}
