using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BladeDesignFamily
{

}

public class BladeSegment : MonoBehaviour
{
    // Private Data Fields
    [SerializeField] private bool IsEndSegment;
    [SerializeField] private float SegmentLength;
    [SerializeField] private int SegmentID;
    [SerializeField] private BladeDesignFamily DesignFamily;
    [SerializeField] private int[] TopEdgeVerts;
    [SerializeField] private int[] BottomEdgeVerts;

    private float tolerance = 0.000001f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(SegmentLength > 0.0f);
        Debug.Assert(SegmentID != 0);           // TODO: Implement SegmentID validation (here or in manager class)

        // If top- and bottom-edge vertex arrays are not manually set, find edge vertices
        // based on assumed top and bottom in local space (y = 0 & y = SegmentLength)
        if (!IsEndSegment)
        {
            if (TopEdgeVerts == null || TopEdgeVerts.Length == 0)
            {
                TopEdgeVerts = FindVerticesWithYValue(SegmentLength);
                Debug.Assert(TopEdgeVerts != null && TopEdgeVerts.Length > 0);
            }
        }
        if (BottomEdgeVerts == null || BottomEdgeVerts.Length == 0)
        {
            BottomEdgeVerts = FindVerticesWithYValue(0.0f);
            Debug.Assert(BottomEdgeVerts != null && BottomEdgeVerts.Length > 0);
        }
    }

    // Getters & Setters
    public bool SegmentIsEndSegment() { return IsEndSegment; }
    public float GetSegmentLength() { return SegmentLength; }
    public int GetSegmentID() { return SegmentID; }
    public BladeDesignFamily GetSegmentDesignFamily() { return DesignFamily; }
    public int[] GetTopEdgeVertices() { return TopEdgeVerts; }
    public int[] GetBottomEdgeVertices() { return BottomEdgeVerts; }

    // Utility Methods
    //
    // FindTopEdgeVerts
    // Returns the indices of the vertices that make up the top edge of this segment's mesh
    // Assumes that the mesh's lowest verts/edges are at y = 0.0f and that the top edge is
    // located at y = SegmentLength (both in local space). 
    private int[] FindVerticesWithYValue(float y)
    {
        Vector3[] verts = this.transform.GetComponent<MeshFilter>().mesh.vertices;
        if (verts == null || verts.Length == 0)
        {
            return null;
        }
        List<int> TopEdgeVerts = new List<int>();
        for (int i = 0; i < verts.Length; i++)
        {
            if (verts[i].y >= y - tolerance && verts[i].y <= y + tolerance)
            {
                TopEdgeVerts.Add(i);
            }
        }
        return TopEdgeVerts.ToArray();
    }
    
}
