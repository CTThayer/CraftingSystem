using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PartLayout : MonoBehaviour
{
    public Vector3 buildLocation;

    [SerializeField] private ItemPart[] _defaultParts;
    public ItemPart[] defaultParts { get => _defaultParts; }

    [SerializeField] private int _basePartIndex;
    public int basePartIndex { get => _basePartIndex; }

    private Vector3[] defaultPositions;                                         // TODO: Do we actually need to cache these?
    private Quaternion[] defaultRotations;
    private ItemPart[] parts;

    // Start is called before the first frame update
    void Start()
    {
        buildLocation = transform.position;

        Debug.Assert(_defaultParts != null && _defaultParts.Length > 0);
        for (int i = 0; i < _defaultParts.Length; i++)
        {
            Debug.Assert(_defaultParts[i] != null);
        }
        Debug.Assert(_basePartIndex >= 0 && _basePartIndex < _defaultParts.Length);

        defaultPositions = new Vector3[_defaultParts.Length];
        defaultRotations = new Quaternion[_defaultParts.Length];
        parts = new ItemPart[_defaultParts.Length];
        Array.Copy(_defaultParts, parts, _defaultParts.Length);
        for (int i = 0; i < _defaultParts.Length; i++)
        {
            defaultPositions[i] = _defaultParts[i].transform.position;
            defaultRotations[i] = _defaultParts[i].transform.rotation;
        }
    }

    //public bool AddPartAt(int index, ItemPart part)
    //{
    //    if (parts[index] != _defaultParts[index])
    //    {
    //        if (part == null)
    //            return false;
    //        else if (part.partType != _defaultParts[index].partType)
    //            return false;
    //    }
    //    // Reactivate part and deactivate defaultPart
    //    Storable partStorable = part.GetComponent<Storable>();
    //    if (partStorable == null)
    //        return false;
    //    //Vector3 location;
    //    Transform location;
    //    if (index > 0)
    //    {
    //        ItemPart parentPart = parts[index].GetConnectedPart(0);
    //        int indexInParent = parentPart.GetIndexOfConnection(parts[index]);
    //        GameObject cpInParent = parentPart.GetConnectionPoint(indexInParent);
    //        location = cpInParent.transform; //.position;
    //    }
    //    else
    //        location = _defaultParts[index].transform; //.position;
    //    partStorable.ReactivateInWorld(location, true);

    //    // Add part to parts[] and set part connections
    //    parts[index] = part;
    //    TransferConnections(_defaultParts[index], parts[index]);

    //    // Deactivate corresponding default part
    //    _defaultParts[index].gameObject.SetActive(false);

    //    //Update positioning of parts to reflect new part addition
    //    //UpdateFromBasePart();

    //    return true;
    //}

    //public bool RemovePartAt(int index, out ItemPart output)
    //{
    //    if (index >= 0 && index < parts.Length)
    //    {
    //        if (parts[index] != _defaultParts[index])
    //        {
    //            output = parts[index];
    //            _defaultParts[index].gameObject.SetActive(true);
    //            TransferConnections(parts[index], _defaultParts[index]);
    //            RemoveAllConnectionsFrom(parts[index]);
    //            parts[index] = _defaultParts[index];
    //            UpdateFromBasePart();
    //            return true;
    //        }
    //    }
    //    output = null;
    //    return false;
    //}

    //private void UpdateFromBasePart()
    //{
    //    Matrix4x4 baseMatrix = Matrix4x4.TRS(Vector3.zero, parts[_basePartIndex].transform.rotation, Vector3.one);

    //    parts[_basePartIndex].transform.position = baseMatrix.MultiplyPoint(buildLocation);
    //    UpdateDependents(parts[_basePartIndex], baseMatrix);
    //}

    //private void UpdateDependents(ItemPart part, Matrix4x4 baseMatrix)
    //{
    //    ItemPart[] connections = part.connectedParts;
    //    GameObject[] CPs = part.connectionPoints;
    //    for (int i = 0; i < connections.Length; i++)
    //    {
    //        connections[i].transform.rotation = CPs[i].transform.localRotation;

    //        Matrix4x4 xMat = GetTRSMatrix(baseMatrix,
    //                                      CPs[i].transform.localPosition,
    //                                      CPs[i].transform.localRotation);

    //        connections[i].transform.position = xMat.MultiplyPoint(buildLocation);

    //        ItemPart[] subConnections = connections[i].connectedParts;
    //        if (subConnections != null && subConnections.Length > 0)
    //        {
    //            UpdateDependents(connections[i], xMat);
    //        }
    //    }
    //}

    //private Matrix4x4 GetTRSMatrix(Matrix4x4 parentXform, Vector3 pivot, Quaternion localRotation)
    //{
    //    Matrix4x4 p = Matrix4x4.TRS(pivot, Quaternion.identity, Vector3.one);
    //    Matrix4x4 invp = Matrix4x4.TRS(-pivot, Quaternion.identity, Vector3.one);
    //    Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, localRotation, Vector3.one);
    //    return parentXform * p * trs * invp;
    //}

    public bool VerifyConfiguration()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == _defaultParts[i])
                return false;
        }
        return true;
    }

    public bool RetrievePartsForItemAssembly(out ItemPart[] partArray)
    {
        if (VerifyConfiguration())
        {
            partArray = parts;
            return true;
        }
        else
        {
            partArray = null;
            return false;
        }
    }


    //private void OffsetPartsBy(Vector3 Offset)
    //{
    //    Vector3 offset = GetRecenterBaseOffset();
    //    foreach (ItemPart part in parts)
    //    {
    //        part.transform.position += offset;
    //    }
    //}

    //private Vector3 GetRecenterBaseOffset()
    //{
    //    Vector3 basePos = parts[basePartIndex].transform.position;
    //    Bounds baseBounds = parts[basePartIndex].GetComponent<Renderer>().bounds;
    //    basePos.y -= baseBounds.extents.y;
    //    Vector3 offset = buildLocation - basePos;
    //    return offset;
    //}

    //private bool BasePartIsAboveBuildLoc()
    //{
    //    Vector3 pos = parts[basePartIndex].transform.position;
    //    Bounds baseBounds = parts[basePartIndex].GetComponent<Renderer>().bounds;
    //    pos.y -= baseBounds.extents.y;
    //    return pos.y > buildLocation.y;
    //}

    public bool Add(int index, ItemPart newPart)
    {
        if (index >= 0 && index < parts.Length)
        {
            if (parts[index] != _defaultParts[index]                            // If there is a part in this spot, exit and remove it first
                || newPart == null                                              // Or if part to add is null, exit
                || newPart.partType != _defaultParts[index].partType)           // Or if newPart doesn't match partType, exit
            {
                    return false;
            }
            // Get ref to newPart's storable
            Storable partStorable = newPart.GetComponent<Storable>();
            if (partStorable == null)
                return false;
            // Calculate the correct position for the new part & reactivate
            Vector3 position;
            Quaternion rotation;
            bool b = GetActivationLocation(newPart, index, out position, out rotation);
            if (b)
                partStorable.ReactivateInWorld(position, rotation, true);
            else
                Debug.LogError("PartLayout: Add(): Error getting add location!");

            // Store oldPart ref, transfer connections, and add newPart to parts[]
            ItemPart oldPart = parts[index];
            TransferConnections(oldPart, newPart);
            parts[index] = newPart;

            // Update all of the connected parts based on differences between
            // the oldPart and the newPart
            UpdateDependents2(oldPart, newPart, index);

            // Deactivate corresponding default part
            _defaultParts[index].gameObject.SetActive(false);

            return true;
        }
        return false;
    }

    public bool Remove(int index, out ItemPart output)
    {
        if (index >= 0 && index < parts.Length)
        {
            if (parts[index] != _defaultParts[index])
            {
                ItemPart partToRemove = parts[index];
                _defaultParts[index].gameObject.SetActive(true);

                TransferConnections(parts[index], _defaultParts[index]);

                parts[index] = _defaultParts[index];

                Vector3 position;
                Quaternion rotation;
                bool b = GetActivationLocation(parts[index], index, out position, out rotation);
                if (b)
                {
                    parts[index].transform.position = position;
                    parts[index].transform.rotation = rotation;
                }
                else
                    Debug.LogError("PartLayout: Add(): Error getting add location!");

                UpdateDependents2(partToRemove, parts[index], index);

                RemoveAllConnectionsFrom(partToRemove);

                output = partToRemove;
                return true;
            }
        }
        output = null;
        return false;
    }

    private bool GetActivationLocation(ItemPart partToAdd,
                                       int index,
                                       out Vector3 position,
                                       out Quaternion rotation)
    {
        if (index < 0 || partToAdd == null)
        {
            position = Vector3.negativeInfinity;
            rotation = Quaternion.identity;
            return false;
        }
        else if (index > 0)
        {
            ItemPart parentPart = parts[index].GetConnectedPart(0);
            int indexInParent = parentPart.GetIndexOfConnection(parts[index]);
            GameObject cpInParent = parentPart.GetConnectionPoint(indexInParent);
            position = cpInParent.transform.position;
            rotation = cpInParent.transform.rotation;
            Vector3 posOffset = partToAdd.transform.position - partToAdd.GetConnectionPoint(0).transform.position;
            position += posOffset;
            return true;
        }
        else
        {
            position = buildLocation;
            Vector3 size = partToAdd.GetComponent<Renderer>().bounds.extents;
            Vector3 posOffset = Vector3.up * size.y;
            position += posOffset;
            rotation = _defaultParts[index].transform.rotation;
            return true;
        }
    }

    private Vector3 GetCPTranslation(GameObject oldCP, GameObject newCP)
    {
        return newCP.transform.position - oldCP.transform.position;
    }

    private Quaternion GetCPRotationChange(GameObject oldCP, GameObject newCP)
    {
        Quaternion prevRot = oldCP.transform.rotation;
        Quaternion newRot = newCP.transform.rotation;
        if (prevRot == newRot)
            return Quaternion.identity;
        else
        {
            Quaternion change = Quaternion.Inverse(prevRot) * newRot;
            return change;
        }
    }

    private void UpdateDependents2(ItemPart oldPart, ItemPart newPart, int index)
    {
        GameObject[] oldCPs = oldPart.connectionPoints;
        GameObject[] newCPs = newPart.connectionPoints;
        int start = index > 0 ? 1 : 0;
        // Updated downstream dependents (connection indices > 0)
        for (int i = start; i < newCPs.Length; i++)
        {
            Vector3 cpTranslation = GetCPTranslation(oldCPs[i], newCPs[i]);
            Quaternion cpRotation = GetCPRotationChange(oldCPs[i], newCPs[i]);
            ItemPart dependent = oldPart.GetConnectedPart(i);
            GameObject cp = newCPs[i];
            PropagateToDependents(dependent, cp, cpTranslation, cpRotation);
        }
    }

    private void PropagateToDependents(ItemPart dependentPart,
                                       GameObject parentCP,
                                       Vector3 translation,
                                       Quaternion addRotation)
    {
        if (dependentPart != null && parentCP != null)
        {
            dependentPart.gameObject.transform.position += translation;
            if (dependentPart.GetConnectionPoint(0).transform.position != parentCP.transform.position)
                Debug.LogError("PartLayout: PropagateToDependents(): Connection Points are misaligned.");
            Quaternion currentRotation = dependentPart.transform.rotation;
            Quaternion newRotation = currentRotation * addRotation;
            dependentPart.transform.rotation = newRotation;

            ItemPart[] childParts = dependentPart.connectedParts;
            if (childParts != null && childParts.Length > 0)
            {
                GameObject[] parentCPs = dependentPart.connectionPoints;
                for (int i = 1; i < childParts.Length; i++)
                {
                    GameObject childCP = childParts[i].GetConnectionPoint(0);
                    Vector3 cpTranslation = GetCPTranslation(childCP, parentCPs[i]);
                    PropagateToDependents(childParts[i], parentCPs[i], cpTranslation, addRotation);
                }
            }
            else
            {
                Debug.Log("Reached End");
                return;
            }
        }
    }

    private void TransferConnections(ItemPart fromPart, ItemPart toPart)
    {
        ItemPart[] oldConnections = fromPart.connectedParts;

        toPart.SetConnectedPart(0, oldConnections[0]);
        int indexInParent = oldConnections[0].GetIndexOfConnection(fromPart);
        oldConnections[0].SetConnectedPart(indexInParent, toPart);

        for (int i = 1; i < oldConnections.Length; i++)
        {
            toPart.SetConnectedPart(i, oldConnections[i]);
            oldConnections[i].SetConnectedPart(0, toPart);
        }
    }

    private void RemoveAllConnectionsFrom(ItemPart part)
    {
        int index = part.connectedParts.Length - 1;
        while (index >= 0)
        {
            part.SetConnectedPart(index, null);
            index--;
        }
    }

}
