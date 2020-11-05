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
    
    private GameObject partInSocket;
    //private bool hasPartInSocket;                                               // TODO: Is this unnecessary?

    [SerializeField] private GameObject parentSocket;
    [SerializeField] private GameObject[] childSockets;
    [SerializeField] private PartRequirements partReqs;
    [SerializeField] private bool moveChildrenOnAdd;
    [SerializeField] private bool isEditorConfigured;                           // TODO: Is this unnecessary?

    void Awake()
    {
        originalColor = this.gameObject.GetComponent<MeshRenderer>().material.color;

        if (isEditorConfigured)
        {
            if (partReqs.partDimensionsRange.ContainsPoint(this.transform.localScale))
            {
                originalScale = this.transform.localScale;
                originalXformUp = this.transform.up;
                originalPosition = this.transform.position;
            }
        }
    }

    //void Start()
    //{

    //}

    public void SetDependentObjects(GameObject[] dependents)
    {
        childSockets = dependents;
    }
    
    public void AddPartToSocket(ItemPart part)
    {
        // If there is already a part in this socket, remove it first.
        if (partInSocket != null)
        {
            RemovePartFromSocket();
        }
                                                                                // TODO: Run PartRequirements validation here?
        // Part needs to have the same number of connection points as the number
        // of dependents that this part socket has.
        if (part != null && part.GetNumberOfConnectionPoints() != childSockets.Length)
        { 
            // Set partInSocket
            partInSocket = part.gameObject;

            // Update position and size to match bounds of added part
            Vector3 partSize = partInSocket.GetComponent<Renderer>().bounds.size;
            Vector3 sizeDelta = partSize - this.transform.localScale;
            this.transform.localScale = partSize;

            // TODO: If PartRequirements require connection points, establish connections

            if (moveChildrenOnAdd)
            {
                // Adjust child sockets based on added part
                MoveChildrenToConnectionPoints(part.connectionPoints);
            }
        }

        // TODO: Handle case of null ItemPart
    }

    public void RemovePartFromSocket()
    {
        if (partInSocket != null)
        {
            // Set partInSocket
            partInSocket = null;

            //// Reset position and size to original
            Vector3 deltaPos = originalPosition - this.transform.position;
            this.transform.position = originalPosition;
            Vector3 deltaSca = originalScale - this.transform.localScale;
            this.transform.localScale = originalScale;

            // Adjust dependent sockets to account for resetting size/position
            ResetChildSockets();
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
