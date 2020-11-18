using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSocket : MonoBehaviour
{
    static Color socketSelectionColor = new Color(1.0f, 1.0f, 0.3f, 0.4f);
    private Color originalColor;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 originalXformUp;

    private Vector3 initialPartPos;
    private Quaternion initialPartRotation;

    private GameObject partInSocket;

    [SerializeField] private GameObject parentSocket;
    [SerializeField] private GameObject[] childSockets;
    //[SerializeField] private PartSocket parentSocket;
    //[SerializeField] private PartSocket[] childSockets;
    [SerializeField] private PartRequirements partReqs;
    [SerializeField] private bool moveChildrenOnAdd;
    [SerializeField] private bool useCenterForAddPos;
    [SerializeField] private bool isEditorConfigured;                           // TODO: Is this unnecessary?

    void Start()
    {
        originalColor = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;

        //if (isEditorConfigured)
        //{
        //    if (partReqs.partDimensionsRange.ContainsPoint(this.transform.localScale))
        //    {
                originalScale = this.transform.localScale;
                originalXformUp = this.transform.up;
                originalPosition = this.transform.position;
        //    }
        //}
    }

    /* Get Part In Socket
     * Simply returns a reference to the ItemPart attached to the GameObject 
     * that is currently in the socket (or null if it is empty). This method 
     * does NOT remove the part from the socket.
     */
    public ItemPart GetPartInSocket()
    {
        return partInSocket.GetComponent<ItemPart>();
    }

    /* Set Dependent Objects
     * Sets all the other GameObjects that will be affected when this socket's
     * contents are changed.
     */
    public void SetDependentObjects(GameObject[] dependents)
    {
        childSockets = dependents;
    }

    /* Add Part To Socket
     * Adds the supplied part to the socket. If there is already a part in the
     * socket, the part is removed from it.
     */
    public bool AddPartToSocket(ItemPart part, out Vector3 AddLocation)
    {
        // If there is already a part in this socket, skip adding and exit b/c 
        // it should have been removed before trying to add the new part.
        if (partInSocket == null || part != null)
        {
            if (partReqs.PartMeetsBaseRequirements(part))
            {
                if (partReqs.useConnectionPoints)
                {
                    return AddPartUsingConnectionPoints(part, out AddLocation);
                }
                else
                {
                    return AddPartWithoutConnectionPoints(part, out AddLocation);
                }
            }
        }
        AddLocation = Vector3.negativeInfinity;
        return false;
    }

    private bool AddPartUsingConnectionPoints(ItemPart part, out Vector3 AddLocation)
    {
        // Part needs exactly one more connection point than the number of child
        // sockets (the extra is for the parent)                                // TODO: Is this necessary and how do we handle PartSockets with parent but no children???
        if (part.GetNumberOfConnectionPoints() - 1 != childSockets.Length)
        {
            AddLocation = Vector3.negativeInfinity;
            return false;
        }

        // Attempt to establish connection w/parent
        PartSocket parentPartSocket = parentSocket.GetComponent<PartSocket>();
        if (parentPartSocket != null)
        {
            ItemPart parentPart = parentPartSocket.partInSocket.GetComponent<ItemPart>();
            if (parentPart == null)
            {
                return AddPartWithoutConnectionPoints(part, out AddLocation);   // TODO: If we do this, then adding w/connection points also needs to set downstream children connections that were unset previously
            }

            // Configure part in the socket
            partInSocket = part.gameObject;
            initialPartPos = part.transform.position;
            initialPartRotation = part.transform.rotation;

            ItemPart thisPart = partInSocket.GetComponent<ItemPart>();
            int index = parentPartSocket.GetIndexOfChild(this);
            parentPart.SetConnectedPart(index, thisPart);
            thisPart.SetConnectedPart(index, parentPart);
            GameObject parentConnectionPoint = parentPart.GetConnectionPoint(index);         // Note: For this to work correctly, the indices of part socket child sockets must match the indices of connection points in the ItemPart.

            AddLocation = parentConnectionPoint.transform.position;
            //partInSocket.transform.position = parentConnectionPoint.transform.position;
            partInSocket.transform.rotation = parentConnectionPoint.transform.rotation;

            // TODO: Handle children - Is the following code correct for this?
            if (moveChildrenOnAdd)
            {
                MoveChildrenToConnectionPoints(part.connectionPoints);
            }

            // Update socket position and size to match bounds of added part
            Vector3 partSize = partInSocket.GetComponent<Renderer>().bounds.size;
            Vector3 sizeDelta = partSize - this.transform.localScale;
            this.transform.localScale = partSize;

            return true;
        }

        AddLocation = Vector3.negativeInfinity;
        return false;
    }

    //private void AddPartWithoutConnectionPoints(ItemPart part)
    private bool AddPartWithoutConnectionPoints(ItemPart part, out Vector3 AddLocation)
    {
        // Configure part in the socket
        partInSocket = part.gameObject;
        initialPartPos = part.transform.position;
        initialPartRotation = part.transform.rotation;

        // Update socket position and size to match bounds of added part
        Vector3 partSize = partInSocket.GetComponent<Renderer>().bounds.size;
        Vector3 sizeDelta = partSize - this.transform.localScale;
        this.transform.localScale = partSize;

        // TODO: Handle children - Is the following code correct for this?
        if (moveChildrenOnAdd)
        {
            MoveChildrenToConnectionPoints(part.connectionPoints);
        }

        if (useCenterForAddPos)
        {
            AddLocation = transform.position + (transform.localScale.y * 0.5f) * transform.up;
        }
        else
            AddLocation = this.transform.position;
        return true;
    }

    public void RemovePartFromSocket()
    {
        if (partInSocket != null)
        {
            // Calculate part socket reset deltas & apply them
            Vector3 deltaPos = Vector3.zero;
            Quaternion deltaRot = Quaternion.identity;
            Vector3 deltaSca = Vector3.one;
            if (transform.position != originalPosition)
                deltaPos = originalPosition - this.transform.position;
            if (transform.up != originalXformUp)
                deltaRot = Quaternion.FromToRotation(transform.up, originalXformUp);
            if (transform.localScale != originalScale)
                deltaSca = originalScale - this.transform.localScale;
            this.transform.position = originalPosition;
            this.transform.localScale = originalScale;
            this.transform.rotation = deltaRot;
            UpdateDependentSockets(deltaPos, deltaRot, deltaSca);

            // Remove connections in parts if connectedObjects were set
            if (partReqs.useConnectionPoints)
            {
                ItemPart part = partInSocket.GetComponent<ItemPart>();
                ItemPart[] connections = part.connectedParts;
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i] != null)
                    {
                        int index = connections[i].GetIndexOfConnection(part);
                        connections[i].SetConnectedPart(index, null);
                        part.SetConnectedPart(i, null);
                    }
                }
            }

            // Reset the part in the socket
            partInSocket.transform.position = initialPartPos;
            partInSocket.transform.rotation = initialPartRotation;
            partInSocket = null;
        }
    }


    public void UpdateDependentSockets(Vector3 parentPosDelta, 
                                       Quaternion parentRotDelta,
                                       Vector3 parentScaDelta)
    {
        for (int i = 0; i < childSockets.Length; i++)
        {
            PartSocket dSocket = childSockets[i].GetComponent<PartSocket>();
            if (dSocket != null)
            {
                dSocket.OnParentPositionChanged(parentPosDelta);
                dSocket.OnParentRotationChanged(parentRotDelta);
                dSocket.OnParentScaleChanged(parentScaDelta);

                dSocket.UpdateDependentSockets(parentPosDelta, 
                                               parentRotDelta, 
                                               parentScaDelta);
            }
        }
    }

    private void OnParentPositionChanged(Vector3 parentPositionDelta)
    {
        transform.position += parentPositionDelta;
        if (partInSocket != null)
            partInSocket.transform.position += parentPositionDelta;
    }

    private void OnParentScaleChanged(Vector3 parentScaleDelta)                  // TODO: is this correct?
    {
        Vector3 delta = originalPosition;
        delta.x *= parentScaleDelta.x;
        delta.y *= parentScaleDelta.y;
        delta.z *= parentScaleDelta.z;
        transform.position += delta;

        if (partInSocket != null)
            partInSocket.transform.position += delta;
    }

    private void OnParentRotationChanged(Quaternion parentRotationDelta)
    {
        this.transform.rotation = this.transform.rotation * parentRotationDelta;    // TODO: Is this correct?

        if (partInSocket != null)
            partInSocket.transform.rotation = partInSocket.transform.rotation * parentRotationDelta;
    }

    private void MoveChildrenToConnectionPoints(GameObject[] connectionPoints)
    {
        if (childSockets.Length != connectionPoints.Length)
            return;
        for (int i = 0; i < childSockets.Length; i++)
        {
            Vector3 posDelta = childSockets[i].transform.position - connectionPoints[i].transform.position;
            Quaternion rotDelta = Quaternion.FromToRotation(childSockets[i].transform.up, connectionPoints[i].transform.up);
            childSockets[i].transform.position = connectionPoints[i].transform.position;
            childSockets[i].transform.rotation = connectionPoints[i].transform.rotation;
            PartSocket p = childSockets[i].GetComponent<PartSocket>();
            if (p != null)
            {
                p.UpdateDependentSockets(posDelta, rotDelta, Vector3.zero);     // NOTE: if rescaling is needed, change vector3.zero to the appropriate scale delta.
            }
        }
    }

    private void ResetChildSockets()
    {
        for (int i = 0; i < childSockets.Length; i++)
        {
            Vector3 posDelta = childSockets[i].transform.position - originalPosition;

            Quaternion rotDelta = Quaternion.identity;
            if (childSockets[i].transform.up != originalXformUp)
            {
                rotDelta = Quaternion.FromToRotation(childSockets[i].transform.up, originalXformUp);
            }

            Vector3 scaDelta = Vector3.zero;
            if (childSockets[i].transform.localScale != originalScale)
            {
                scaDelta = childSockets[i].transform.localScale - originalScale;
            }

            childSockets[i].transform.position = originalPosition;
            childSockets[i].transform.up = originalXformUp;
            childSockets[i].transform.localScale = originalScale;

            PartSocket dPartSocket = childSockets[i].GetComponent<PartSocket>();
            if (dPartSocket != null)
                dPartSocket.UpdateDependentSockets(posDelta, rotDelta, scaDelta);
        }
    }

    public int GetIndexOfChild(PartSocket child)
    {
        if (child == null)
            return -1;
        for (int i = 0; i < childSockets.Length; i++)
        {
            if (childSockets[i] == child)
                return i;
        }
        return -1;
    }

    /************************* Selection Highlighting *************************/
    public void OnSelect()
    {
        this.gameObject.GetComponent<Material>().color = socketSelectionColor;
    }

    public void OnDeselect()
    {
        this.gameObject.GetComponent<Material>().color = originalColor;
    }
    /*********************** END Selection Highlighting ***********************/


    /************************** Runtime Initializers **************************/
    public void InitializePartSocket(Vector3 position,
                                     Quaternion rotation,
                                     Vector3 scale)
    {
        this.transform.position = position;
        this.transform.localRotation = rotation;
        this.transform.localScale = scale;
        originalPosition = position;
        originalXformUp = this.transform.up;
        originalScale = scale;
    }

    public void InitializePartSocket(Vector3 position,
                                     Quaternion rotation,
                                     Vector3 scale,
                                     GameObject parentObj,
                                     GameObject[] dependents)
    {
        this.transform.position = position;
        this.transform.localScale = scale;
        this.transform.localRotation = rotation;
        originalPosition = position;
        originalXformUp = this.transform.up;
        originalScale = scale;
        parentSocket = parentObj;
        childSockets = dependents;
    }
    /************************ END Runtime Initializers ************************/

}
