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
    Terrain,
    Wheel
}

[System.Serializable]
public class PartRequirements : MonoBehaviour
{
    [SerializeField] private XYZRange _partDimensionsRange;
    public XYZRange partDimensionsRange { get; }

    [SerializeField] private string[] _allowedPartTypes;
    public string[] allowedPartTypes { get; }

    [SerializeField] private string[] _allowedMaterials;
    public string[] allowedMaterials { get; }

    [SerializeField] private TypeOfCollider _requiredCollider;
    public TypeOfCollider requiredCollider { get; }

    [SerializeField] private string[] _requiredScripts;
    public string[] requiredScripts { get; }

    [SerializeField] private bool _useConnectionPoints;
    public bool useConnectionPoints { get; }

    [SerializeField] private bool _useSurfaceContact;
    public bool useSurfaceContact { get; }

    public PartRequirements(XYZRange dimRange,
                            string[] allowedTypes,
                            string[] allowedMats,
                            TypeOfCollider colliderType,
                            bool useCPs,
                            bool useSC 
                            )
    {
        _partDimensionsRange = dimRange;
        _allowedPartTypes = allowedTypes;
        _allowedMaterials = allowedMats;
        _requiredCollider = colliderType;
        _useConnectionPoints = useCPs;
        _useSurfaceContact = useSC;
    }

    /*********************** Public Validation Methods ************************/

    public bool PartMeetsBaseRequirements(ItemPart part)
    {
        return (   IsAllowedItemType(part)
                && HasAllowedMaterialType(part)
                && HasCorrectCollider(part)
                && FitsInDimensions(part)
               );
    }

    public bool PartMeetsRequirements(ItemPart part, ItemPart otherPart)
    {
        bool meetsReqs = IsAllowedItemType(part) 
                         && HasAllowedMaterialType(part)
                         && HasCorrectCollider(part) 
                         && FitsInDimensions(part);

        if (_useConnectionPoints)
        {
            meetsReqs = meetsReqs && PartsAreConnected(part, otherPart);
        }
        if (_useSurfaceContact)
        {
            meetsReqs = meetsReqs && PartSurfacesTouch(part, otherPart);
        }
        return meetsReqs;
    }

    /********************* END Public Validation Methods **********************/



    /******************* Private Validation Helper Methods ********************/

    private bool IsAllowedItemType(ItemPart part)
    {
        for (int i = 0; i < allowedPartTypes.Length; i++)
        {
            if (allowedPartTypes[i] == part.partType)
                return true;
        }
        return false;
    }

    private bool HasAllowedMaterialType(ItemPart part)
    {
        for (int i = 0; i < allowedMaterials.Length; i++)
        {
            if (allowedMaterials[i] == part.craftingMaterial.materialType)
                return true;
        }
        return false;
    }

    private bool HasCorrectCollider(ItemPart part)
    {
        bool isCorrectCollider = false;
        Collider c = part.GetComponent<Collider>();
        if (c == null)
            return isCorrectCollider;
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

    private bool FitsInDimensions(ItemPart part)
    {
        Bounds partBounds = part.GetComponent<Renderer>().bounds;
        return partDimensionsRange.ContainsPoint(partBounds.size);
    }

    /* Parts Are Connected
     * Checks if two parts are connected via connection points in each part.
     */
    private bool PartsAreConnected(ItemPart part, ItemPart otherPart)
    {
        if (part.GetIndexOfConnection(otherPart) >= 0
            && otherPart.GetIndexOfConnection(otherPart) >= 0)
            return true;
        else
            return false;
    }

    /* Part Surfaces Touch
     * Checks if the colliders of the two parts touch or overlap.
     */
    private bool PartSurfacesTouch(ItemPart part, ItemPart otherPart)
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
