﻿using System.Collections;
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

    [SerializeField] private GameObject _prefabPartLayoutUI;
    public GameObject prefabPartLayoutUI { get => _prefabPartLayoutUI; }

    private ItemPart[] parts;
    private PartRequirements[] partReqs;

    private Vector3[] defaultPositions;                                         // TODO: Do we actually need to cache these?
    private Quaternion[] defaultRotations;                                      // TODO: Do we actually need to cache these?


    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_defaultParts != null && _defaultParts.Length > 0);
        for (int i = 0; i < _defaultParts.Length; i++)
        {
            Debug.Assert(_defaultParts[i] != null);
        }

        Debug.Assert(_basePartIndex >= 0 && _basePartIndex < _defaultParts.Length);

        Debug.Assert(_prefabPartLayoutUI != null);

        partReqs = new PartRequirements[_defaultParts.Length];
        defaultPositions = new Vector3[_defaultParts.Length];
        defaultRotations = new Quaternion[_defaultParts.Length];

        parts = new ItemPart[_defaultParts.Length];
        Array.Copy(_defaultParts, parts, _defaultParts.Length);
        for (int i = 0; i < _defaultParts.Length; i++)
        {
            partReqs[i] = _defaultParts[i].GetComponent<PartRequirements>();
            defaultPositions[i] = _defaultParts[i].transform.position;
            defaultRotations[i] = _defaultParts[i].transform.rotation;

            ItemPart[] connectedParts = _defaultParts[i].connectedParts;
            for (int j = 0; j < connectedParts.Length; j++)
            {
                Debug.Assert(connectedParts[j] != null);
                // TODO: Add additional checks here (i.e. connections exist in
                // the array of default parts)
            }
        }
    }

    public bool Add(int index, ItemPart newPart)
    {
        //buildLocation = transform.position;

        if (index >= 0 && index < parts.Length)
        {
            if (parts[index] != _defaultParts[index]                            // If there is a part in this spot, exit and remove it first
                || newPart == null                                              // Or if part to add is null, exit
                || newPart.partType != _defaultParts[index].partType)           // Or if newPart doesn't match partType, exit
            {
                    return false;
            }
            // Get ref to newPart's storable & reactivate it
            Storable partStorable = newPart.GetComponent<Storable>();
            if (partStorable == null)
                return false;
            partStorable.ReactivateInWorld(partStorable.transform, true);

            // Store oldPart ref, transfer connections, and add newPart to parts[]
            ItemPart oldPart = parts[index];
            TransferConnections(oldPart, newPart);
            parts[index] = newPart;

            // Set the new part to the correct position/rotation
            SetPartToActivationLocation(newPart, index);

            // Update all of the connected parts downstream from this one.
            UpdateDependents(parts[index], index);

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
                // Store ref to part being removed, reactivate its replacement,
                // transfer part's connections, and set parts[] ref to default
                ItemPart partToRemove = parts[index];
                _defaultParts[index].gameObject.SetActive(true);
                TransferConnections(parts[index], _defaultParts[index]);
                parts[index] = _defaultParts[index];

                // Set default part to the correct location based on current parts
                bool success = SetPartToActivationLocation(parts[index], index);
                if (!success)
                {
                    Debug.LogError("PartLayout: Remove(): Activation of default"
                                   + " part failed. Part was null or index " +
                                   "was less than 0.");
                }

                // Update all downstream parts based on switch to default part
                UpdateDependents(parts[index], index);

                // Remove all connection refs from part that is being removed
                RemoveAllConnectionsFrom(partToRemove);

                // return previous part/true
                output = partToRemove;
                return true;
            }
        }
        output = null;
        return false;
    }

    private bool SetPartToActivationLocation(ItemPart partToAdd, int index)
    {
        if (index < 0 || partToAdd == null)
        {
            return false;
        }
        else if (index > 0)
        {
            ItemPart parentPart = parts[index].GetConnectedPart(0);
            UpdateDependentPartTransform(parentPart, partToAdd);
            return true;
        }
        else
        {
            // TODO: Is this the way to handle base parts moving forward?
            Vector3 position = buildLocation;
            Vector3 size = partToAdd.GetComponent<Renderer>().bounds.extents;
            Vector3 posOffset = Vector3.up * size.y;
            partToAdd.transform.position = position + posOffset;
            partToAdd.transform.rotation = _defaultParts[index].transform.rotation;
            return true;
        }
    }

    /* Update Dependents
     * Updates the positions/rotations of all dependent parts for a specified 
     * starting parent part. This function runs recursively for all parts 
     * that are dependents of the first part passed to the fuction. This 
     * function is used to update all of the parts in a part layout whenever a
     * part is added to or removed from the layout.
     * @Param parentPart - reference to part object that has dependent parts to 
     * update. 
     * @Param index - starting index of the dependents to begin updating from.
     *      TODO: Consider changing index to a simple boolean since it really 
     *      just boils down to whether to start at 0 or 1.
     */
    private void UpdateDependents(ItemPart parentPart, int index)
    {
        ItemPart[] dependentParts = parentPart.connectedParts;
        if (dependentParts != null && dependentParts.Length > 0)
        {
            int start = index > 0 ? 1 : 0;
            for (int i = start; i < dependentParts.Length; i++)
            {
                UpdateDependentPartTransform(parentPart, dependentParts[i]);
                UpdateDependents(dependentParts[i], 1);
            }
        }
        else
            return;
    }

    /* Update Dependent Part Transform
     * Updates the position and rotation of a dependent part based on the 
     * provided parent part.
     * @Param parent - the parent part of the part being updated.
     * @Param dependent - the part that needs to be updated.
     * @Return boolean represented success/failure of updating the dependent.
     */
    private bool UpdateDependentPartTransform(ItemPart parent, 
                                              ItemPart dependent)
    {
        if (parent != null 
            && dependent != null 
            && parent != dependent)
        {
            ItemPart parentPart = dependent.GetConnectedPart(0);
            if (parentPart != parent)
            {
                Debug.LogError("PartLayout: GetTranslation(): Connections " +
                               "were not transferred correctly, parents do " +
                               "not match expected results.");
            }
            int indexInParent = parentPart.GetIndexOfConnection(dependent);

            // Get connection points
            GameObject parentCP = parentPart.GetConnectionPoint(indexInParent);
            GameObject childCP = dependent.GetConnectionPoint(0);

            // Calculate rotation for part
            Quaternion rotateByAmount = GetCPRotationChange(childCP, parentCP);
            dependent.transform.rotation *= rotateByAmount;

            // Calculate position for part
            dependent.transform.position = parentCP.transform.position;
            Vector3 posOffset = dependent.transform.position - 
                         dependent.GetConnectionPoint(0).transform.position;
            dependent.transform.position += posOffset;

            return true;
        }
        else
            return false;
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

    public bool VerifyConfiguration()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == _defaultParts[i])
                return false;
        }
        return true;
    }

    public bool GetPartsArray(out ItemPart[] partArray)
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

    public ItemPart[] GetPartsArray()
    {
        if (VerifyConfiguration())
            return parts;
        else
            return null;
    }

    public PartRequirements[] GetPartRequirements()
    {
        return partReqs;
    }

}
