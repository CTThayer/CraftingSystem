using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PartLayout : MonoBehaviour
{
    public Vector3 buildLocation;                                               // TODO: Can this be eliminated? If the PartLayout script is always 
                                                                                // attached to an empty G.O. then that will always be positioned at the
                                                                                // build location anyway so we could just use the transform of this object.
    [SerializeField] private ItemPart[] _defaultParts;
    public ItemPart[] defaultParts { get => _defaultParts; }                    // TODO: This public field is unused. Is it needed for anything?

    [SerializeField] private GameObject _prefabPartLayoutUI;                    // TODO: Can this be replaced by dynamically creating/loading PartSlots 
    public GameObject prefabPartLayoutUI { get => _prefabPartLayoutUI; }        // in the PartPanel and configuring them based on the part reqs?

    private ItemPart[] parts;

    // Start is called before the first frame update
    void Start()
    {
        // Initial asserts to verify correct setup of instance data
        Debug.Assert(_defaultParts != null && _defaultParts.Length > 0);
        Debug.Assert(_prefabPartLayoutUI != null);

        // Initialize parts array
        parts = new ItemPart[_defaultParts.Length];
        Array.Copy(_defaultParts, parts, _defaultParts.Length);
        for (int i = 0; i < _defaultParts.Length; i++)
        {
            // Assert that a default part exists at every index in the array
            Debug.Assert(_defaultParts[i] != null);
            
            // Validate connections for each default part
            ItemPart[] connectedParts = _defaultParts[i].connectedParts;
            for (int j = 0; j < connectedParts.Length; j++)
            {
                Debug.Assert(connectedParts[j] != null);
            }
        }
    }

    /* Add
     * Adds the specified part at the specified index in the parts array.
     * Returns true if the part could be added at the specified index and false
     * if it didn't meet the requirements or if the index was out of bounds.
     */
    public bool Add(int index, ItemPart newPart)
    {
        //buildLocation = transform.position;

        if (index >= 0 && index < parts.Length)
        {
            if (newPart == null                                                 // If part to add is null, exit
                || parts[index] != _defaultParts[index]                         // Or if there is a part in this spot, exit to remove it first
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

    /* Remove
     * Removes the part that is in the specified index (if there is one) and 
     * outputs that part to the out parameter "output". This method returns true
     * if there was a part to remove/it was removed and returns false if there
     * was no part to remove or if the specified index was out of bounds.
     */
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

    /* Set Part To Activation Location
     * Adds a new part to the part layout in place of the default part/current
     * part in the specified index.
     * Assumes that the "base part" is always in the 0 index of the default
     * parts array. If another index should be used, a basePartIndex variable
     * should be added and checked in place of 0 here.
     */
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

    /* Get CP Rotation Change
     * Calculates and returns a quaternion representing the difference between 
     * the orientation of an old object and a new object. This is a utility 
     * method used for updating parts when a part is added to or removed from 
     * the layout.
     */
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

    /* Transfer Connections
     * Copies all of the connections from one part into another part.
     */
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

    /* Remove All Connections From
     * Clears all of the connections of a specified part by setting them to null.
     */
    private void RemoveAllConnectionsFrom(ItemPart part)
    {
        int index = part.connectedParts.Length - 1;
        while (index >= 0)
        {
            part.SetConnectedPart(index, null);
            index--;
        }
    }

    /* Verify Configuration
     * Returns true if there is a non-default part in every slot. If there are
     * any default parts still in the parts array, then this method returns null.
     */
    public bool VerifyConfiguration()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == _defaultParts[i])
                return false;
        }
        return true;
    }

    /* Get Parts Array
     * Outputs the current parts[] array to the out parameter "partArray" if 
     * there is a non-default part in every slot and returns true. If there is 
     * not a valid part in each slot, then it outputs null and returns false.
     */
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

    /* Get Parts Array
     * Returns the current parts[] array if there is a non-default part in every
     * slot. If there is not, then this method returns null.
     */
    public ItemPart[] GetPartsArray()
    {
        if (VerifyConfiguration())
            return parts;
        else
            return null;
    }

    /* Get Part Requirements
     * Returns an array of the part requirements for each of the parts in this
     * part layout. The part requirements should always be attached to the 
     * default parts for this layout since these are what is returned.
     */
    public PartRequirements[] GetPartRequirements()
    {
        PartRequirements[] partReqs = new PartRequirements[_defaultParts.Length];
        for (int i = 0; i < _defaultParts.Length; i++)
        {
            partReqs[i] = _defaultParts[i].GetComponent<PartRequirements>();
        }
        return partReqs;
    }

}
