using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeAssembler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    //public Mesh AssembleBladeFromSegments(BladeSegment[] segments)
    //{

    //}

    private bool ValidateSegmentConfiguration(BladeSegment[] segments, float bladeTypeMaxLength)
    {
        if (segments == null || segments.Length == 0)
        {
            Debug.Log("BladeAssembler: INVALID CONFIG - Supplied BladeSegment array contains no segments.");
            return false;
        }
        else if (!segments[segments.Length - 1].SegmentIsEndSegment())
        {
            Debug.Log("BladeAssembler: INVALID CONFIG - Last blade segment is not an end segment.");
            return false;
        }
        else if (segments.Length == 1 && segments[0].SegmentIsEndSegment())
        {
            Debug.Log("BladeAssembler: VALID CONFIG - Supplied BladeSegment array contains only 1 blade segment which is a valid end segment.");
            return true;
        }
        else
        {
            float totalLength = 0.0f;
            for (int i = 0; i < segments.Length - 2; i++)
            {
                if (segments[i].GetSegmentDesignFamily() != segments[i + 1].GetSegmentDesignFamily())
                    return false;
                if (segments[i].GetTopEdgeVertices().Length != segments[i + 1].GetBottomEdgeVertices().Length)
                    return false;
                totalLength += segments[i].GetSegmentLength();
            }
            totalLength += segments[segments.Length - 1].GetSegmentLength();
            if (totalLength <= bladeTypeMaxLength)
                return true;
            else
                return false;
        }
    }

    private int GetVertexCountOfResultObj(BladeSegment[] segments)
    {
        if (segments != null && segments.Length > 0)
        {
            int VertexCount = 0;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                VertexCount += segments[i].transform.gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
                VertexCount -= segments[i].GetTopEdgeVertices().Length;
            }
            return VertexCount;
        }
        else
        {
            Debug.Log("BladeAssembler: Failed to get new object vertex count because blade segment array was empty.");
            return -1;
        }
    }

    //private Mesh WeldSegments(BladeSegment segmentA, BladeSegment segmentB)
    //{
    //    List<int> FinalTris = new List<int>();
    //    List<Vector3> FinalVerts = new List<Vector3>();
    //    List<int> TrisToRebuild = new List<int>();

    //    Vector3[] VertsA = segmentA.transform.GetComponent<MeshFilter>().mesh.vertices;
    //    Vector3[] VertsB = segmentB.transform.GetComponent<MeshFilter>().mesh.vertices;
    //    int[] TrisA = segmentA.transform.GetComponent<MeshFilter>().mesh.triangles;
    //    int[] TrisB = segmentB.transform.GetComponent<MeshFilter>().mesh.triangles;

    //    int[] SegAEdgeVerts = segmentA.GetTopEdgeVertices();
    //    int[] SegBEdgeVerts = segmentB.GetBottomEdgeVertices();

    //    // Add all non-TopEdge verts from lower segment to FinalVerts list
    //    bool isFinalVert = true;
    //    for (int i = 0; i < VertsA.Length; i++)
    //    {
    //        for (int j = 0; j < SegAEdgeVerts.Length; j++)
    //        {
    //            if (VertsA[i] == VertsA[SegAEdgeVerts[j]])
    //            {
    //                isFinalVert = false;
    //                break;
    //            }
    //        }
    //        if(isFinalVert)
    //        {
    //            FinalVerts.Add(VertsA[i]);
    //        }
    //        isFinalVert = true;
    //    }

    //    // Add ALL verts from upper segment to 

    //    SortTris(TrisA, SegAEdgeVerts, ref TrisToRebuild, ref FinalTris);
    //    SortTris(TrisB, SegBEdgeVerts, ref TrisToRebuild, ref FinalTris);


    //}

    private void SortTris(int[] tris, int[] edgeVerts, ref List<int> trisToRebuild, ref List<int> finalTris)
    {
        bool addToFinalTris = true;
        for (int i = 0; i < tris.Length; i += 3)
        {
            for (int j = 0; j < edgeVerts.Length; j++)
            {
                if (   tris[i] == edgeVerts[j]
                    || tris[i + 1] == edgeVerts[j]
                    || tris[i + 2] == edgeVerts[j])
                {
                    trisToRebuild.Add(tris[i]);
                    trisToRebuild.Add(tris[i + 1]);
                    trisToRebuild.Add(tris[i + 2]);
                    addToFinalTris = false;
                    break;
                }
            }
            if (addToFinalTris)
            {
                finalTris.Add(tris[i]);
                finalTris.Add(tris[i + 1]);
                finalTris.Add(tris[i + 2]);
            }
            addToFinalTris = true;
        }
    }

}
