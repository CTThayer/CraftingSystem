using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PartLayout : MonoBehaviour
{
    public Vector3 buildLocation;

    [SerializeField] private ItemPart[] defaultParts { get; }
    [SerializeField] private int basePartIndex { get; }

    private Vector3[] defaultPositions;
    private Quaternion[] defaultRotations;
    private ItemPart[] parts;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(defaultParts != null && defaultParts.Length > 0);
        for (int i = 0; i < defaultParts.Length; i++)
        {
            Debug.Assert(defaultParts[i] != null);
        }
        Debug.Assert(basePartIndex > 0 && basePartIndex < defaultParts.Length);

        defaultPositions = new Vector3[defaultParts.Length];
        parts = new ItemPart[defaultParts.Length];
        Array.Copy(defaultParts, parts, defaultParts.Length);
        for (int i = 0; i < defaultParts.Length; i++)
        {
            defaultPositions[i] = defaultParts[i].transform.position;
            defaultRotations[i] = defaultParts[i].transform.rotation;
        }
    }

    public bool AddPartAt(int index, ItemPart part)
    {
        if (parts[index] != defaultParts[index])
        {
            if (part == null)
                return false;
            else if (part.partType != defaultParts[index].partType)
                return false;
        }

        //part.transform.position = defaultParts[index].transform.position;
        //part.transform.rotation = defaultParts[index].transform.rotation;

        parts[index] = part;
        TransferConnections(defaultParts[index], parts[index]);

        UpdateFromBasePart();

        return true;
    }

    public bool RemovePartAt(int index, out ItemPart output)
    {
        if (index > 0 && index < parts.Length)
        {
            ItemPart returnPart = parts[index];
            parts[index] = defaultParts[index];
            UpdateFromBasePart();
            RemoveAllConnectionsFrom(returnPart);
            output = returnPart;
            return true;
        }
        output = null;
        return false;
    }

    private void TransferConnections(ItemPart fromPart, ItemPart toPart)
    {
        ItemPart[] oldConnections = fromPart.connectedParts;
        for (int i = 0; i < oldConnections.Length; i++)
        {
            toPart.SetConnectedPart(i, oldConnections[i]);
        }
    }

    private void RemoveAllConnectionsFrom(ItemPart part)
    {
        int index = part.connectedParts.Length - 1;
        while (index >= 0)
        {
            part.SetConnectedPart(index, null);
        }
    }

    private void UpdateFromBasePart()
    {
        Matrix4x4 baseMatrix = Matrix4x4.TRS(Vector3.zero, parts[basePartIndex].transform.rotation, Vector3.one);

        parts[basePartIndex].transform.position = baseMatrix.MultiplyPoint(buildLocation);
        UpdateDependents(parts[basePartIndex], baseMatrix);
    }

    private void UpdateDependents(ItemPart part, Matrix4x4 baseMatrix)
    {
        ItemPart[] connections = part.connectedParts;
        GameObject[] CPs = part.connectionPoints;
        for (int i = 0; i < connections.Length; i++)
        {
            connections[i].transform.rotation = CPs[i].transform.localRotation;

            Matrix4x4 xMat = GetTRSMatrix(baseMatrix,
                                          CPs[i].transform.localPosition,
                                          CPs[i].transform.localRotation);

            connections[i].transform.position = xMat.MultiplyPoint(buildLocation);

            ItemPart[] subConnections = connections[i].connectedParts;
            if (subConnections != null && subConnections.Length > 0)
            {
                UpdateDependents(connections[i], xMat);
            }
        }
    }

    private Matrix4x4 GetTRSMatrix(Matrix4x4 parentXform, Vector3 pivot, Quaternion localRotation)
    {
        Matrix4x4 p = Matrix4x4.TRS(pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 invp = Matrix4x4.TRS(-pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, localRotation, Vector3.one);
        return parentXform * p * trs * invp;
    }

    public bool VerifyConfiguration()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == defaultParts[i])
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

}
