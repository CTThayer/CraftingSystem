using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentAssembler : MonoBehaviour
{
    private float PosTolerance = 0.00001f;
    private float NormTolerance = 0.005f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool ValidateSegmentConfiguration(ComponentSegment[] segments, float componentMaxLength)
    {
        if (segments == null || segments.Length == 0)
        {
            Debug.Log("ComponentAssembler: INVALID CONFIG - Supplied BladeSegment array contains no segments.");
            return false;
        }
        else if (!segments[segments.Length - 1].SegmentIsEndSegment())
        {
            Debug.Log("ComponentAssembler: INVALID CONFIG - Last blade segment is not an end segment.");
            return false;
        }
        else if (segments.Length == 1 && segments[0].SegmentIsEndSegment())
        {
            Debug.Log("ComponentAssembler: VALID CONFIG - Supplied BladeSegment array contains only 1 blade segment which is a valid end segment.");
            return true;
        }
        else
        {
            float totalLength = 0.0f;
            for (int i = 0; i < segments.Length - 2; i++)
            {
                if (segments[i].GetConnectionID() != segments[i + 1].GetConnectionID())
                    return false;
                //if (segments[i].GetTopEdgeVertices().Length != segments[i + 1].GetBottomEdgeVertices().Length)
                //    return false;
                totalLength += segments[i].GetSegmentLength();
            }
            totalLength += segments[segments.Length - 1].GetSegmentLength();
            if (totalLength <= componentMaxLength)
                return true;
            else
                return false;
        }
    }

    /* Combine Segment Geometry
     * 
     * Combines the meshes of a set of ComponentSegments. This function removes 
     * any resulting duplicate vertices where the segment edges meet. This merge
     * process also fixes any affected triangles and normals.
     * Assumes segment configuration has been validated previously. 
     * DOES NOT VALIDATE INPUTS.
     * 
     * @Param ComponentSegment[] segments - An array of ComponentSegments to merge. 
     * Returns a Mesh made of the combined vertices, triangles, and normals.
     */
    public Mesh CombineSegmentGeometry(ComponentSegment[] segments)
    {
        // Lists of resulting vertices and tris
        List<Vector3> FinalVerts = new List<Vector3>();
        List<int> FinalTris = new List<int>();
        List<Vector3> FinalNormals = new List<Vector3>();

        // Used to offset the triangle indices by the correct amount
        int VertexCount = 0;

        // Add all vertices, triangles, and normals from each segment
        // to the final list (while offsetting their values correctly)
        foreach (ComponentSegment segment in segments)
        {
            Vector3[] segVerts = segment.transform.GetComponent<MeshFilter>().mesh.vertices;
            foreach (Vector3 vert in segVerts)
            {
                FinalVerts.Add(segment.transform.TransformPoint(vert));
            }
            int[] segTris = segment.transform.GetComponent<MeshFilter>().mesh.triangles;
            foreach (int index in segTris)
            {
                FinalTris.Add(index + VertexCount);
            }
            FinalNormals.AddRange(segment.transform.GetComponent<MeshFilter>().mesh.normals);
            VertexCount = FinalVerts.Count;
        }

        Debug.Assert(FinalVerts.Count == FinalNormals.Count);

        Debug.Log("FinalVerts.Count (Before Welding) = " + FinalVerts.Count);

        // Find any vertices that share the same position and normal 
        // and eliminate the unnecessary copies.
        for (int i = 0; i < FinalVerts.Count; i++)
        {
            for (int j = i; j < FinalVerts.Count; j++)          // TODO: Optimize here if possible
            {
                if (FinalVerts[i] == FinalVerts[j] && FinalNormals[i] == FinalNormals[j])
                {
                    // Adjust triangles to use same vertex when vertices are equal and
                    // subtract 1 from any tri indexes greater than the one being removed.
                    for (int k = 0; k < FinalTris.Count; k++)   // TODO: Optimize here if possible
                    {
                        if (FinalTris[k] == j)
                            FinalTris[k] = i;
                        if (FinalTris[k] > j)
                            FinalTris[k]--;
                    }
                    // Remove extra vertex and normal
                    FinalVerts.Remove(FinalVerts[j]);
                    FinalNormals.Remove(FinalNormals[j]);
                }
            }
        }

        // TODO: Fix/Recalculate UVs

        Mesh result = new Mesh();
        result.vertices = FinalVerts.ToArray();
        result.triangles = FinalTris.ToArray();
        result.normals = FinalNormals.ToArray();

        // TODO: Assign correct UVs to mesh;

        return result;
    }

    /* Weld Segment Meshes
     * 
     * This function combines the all the meshes from an array of ComponentSegments. 
     * This removes any duplicate vertices/normals where the segment edges meet and
     * also fixes any affected triangles. Then it returns a single unified mesh.
     * ASSUMES segments have been validated previously (does NOT validate inputs).
     * 
     * @Param ComponentSegment[] segments - An array of ComponentSegments to merge. 
     * Returns a Mesh made of the combined vertices, triangles, and normals.
     */
    public Mesh WeldSegmentMeshes(ComponentSegment[] segments)
    {
        Debug.Log("ComponentAssembler: WeldSegmentMeshes() - Start");

        //// Initalize arrays to hold all vertices, normals, and triangles from all segments
        //int totalVerts = 0;
        //for (int i = 0; i < segments.Length; i++)
        //{
        //    totalVerts += segments[i].transform.GetComponent<MeshFilter>().mesh.vertices.Length;
        //}
        //Debug.Log("Total Original Verts: " + totalVerts);
        //Vector3[] AllVertices = new Vector3[totalVerts];
        //Vector3[] AllNormals = new Vector3[totalVerts];
        //int[] AllTriangles = new int[totalVerts * 3];

        // Lists of resulting vertices and tris
        List<Vector3> AllVertices = new List<Vector3>();
        List<Vector3> AllNormals = new List<Vector3>();
        List<int> AllTriangles = new List<int>();

        Debug.Log("Total Vertices from Segments: " + AllVertices.Count);
        Debug.Log("Total Normals from Segments: " + AllNormals.Count);
        Debug.Log("Total Triangles from Segments: " + AllTriangles.Count);

        //// Store all vertices, normals, and triangles from all segments in the arrays
        GetAllVertsNormsAndTris(segments, ref AllVertices, ref AllNormals, ref AllTriangles);

        // Initialize Lists to store final vertices and normals in and an array to map
        // old duplicate vertices to the final ones (for correcting triangles)
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        int[] vertexMap = new int[AllVertices.Count];

        // Call helper function to filter duplicates and create the map
        MapAndFilterDuplicates(AllVertices, AllNormals, ref newVertices, ref newNormals, ref vertexMap);

        Debug.Log("Result Number of Verts: " + newVertices.Count);
        Debug.Log("Result Number of Normals: " + newNormals.Count);

        // Fix triangles
        FixTriangles(ref AllTriangles, vertexMap);

        Debug.Log("Result Number of Triangles: " + AllTriangles.Count);

        // Create new mesh and assign the finalized vertices, normals & triangles.
        Mesh result = new Mesh();
        result.vertices = newVertices.ToArray();
        result.normals = newNormals.ToArray();
        result.triangles = AllTriangles.ToArray();

        return result;
    }

    ///* Get All Verts Norms And Tris
    // * Private helper method for combining all the vertices, normals, and 
    // * triangles from each of the ComponentSegments in the segments array.
    // * No return type; vertices[], normals[], and triangles[] passed as ref
    // * so they can be modified in the function.
    // * ASSUMES vertices[], normals[], and triangles[] are initialized to 
    // * the correct size before being passed.
    // */
    //private void GetAllVertsNormsAndTris(ComponentSegment[] segments,
    //                                     ref Vector3[] vertices,
    //                                     ref Vector3[] normals,
    //                                     ref int[] triangles)
    //{
    //    // Used to offset the triangle indices by the correct amount
    //    int VertexCount = 0;

    //    // Add all vertices, triangles, and normals from each segment
    //    // to the final list (while offsetting their values correctly)
    //    for(int s = 0; s < segments.Length; s++)
    //    {
    //        Vector3[] segmentVerts = segments[s].transform.GetComponent<MeshFilter>().mesh.vertices;
    //        Vector3[] segmentNormals = segments[s].transform.GetComponent<MeshFilter>().mesh.normals;
    //        int[] segmentTriangles = segments[s].transform.GetComponent<MeshFilter>().mesh.triangles;
    //        int v = 0;
    //        for (; v < segmentVerts.Length; v++)
    //        {
    //            vertices[v + VertexCount] = segments[s].transform.TransformPoint(segmentVerts[v]);
    //            normals[v + VertexCount] = segmentNormals[v];
    //            triangles[v + VertexCount] = segmentTriangles[v] + VertexCount;
    //            triangles[v + 1 + VertexCount] = segmentTriangles[v] + 1 + VertexCount;
    //            triangles[v + 2 + VertexCount] = segmentTriangles[v] + 2 + VertexCount;
    //        }
    //        VertexCount += segmentVerts.Length;
    //    }

    //    Debug.Log("ComponentAssembler: GetAllVertsNormsAndTris(): VertexCount = " + VertexCount);
    //}

    /* Get All Verts Norms And Tris
     * Private helper method for combining all the vertices, normals, and 
     * triangles from each of the ComponentSegments in the segments array.
     * No return type; vertices[], normals[], and triangles[] passed as ref
     * so they can be modified in the function.
     * ASSUMES vertices[], normals[], and triangles[] are initialized to 
     * the correct size before being passed.
     */
    private void GetAllVertsNormsAndTris(ComponentSegment[] segments,
                                         ref List<Vector3> vertices,
                                         ref List<Vector3> normals,
                                         ref List<int> triangles)
    {
        // Used to offset the triangle indices by the correct amount
        int VertexCount = 0;

        // Add all vertices, triangles, and normals from each segment
        // to the final list (while offsetting their values correctly)
        foreach (ComponentSegment segment in segments)
        {
            Vector3[] segVerts = segment.transform.GetComponent<MeshFilter>().mesh.vertices;
            foreach (Vector3 vert in segVerts)
            {
                vertices.Add(segment.transform.TransformPoint(vert));
            }
            normals.AddRange(segment.transform.GetComponent<MeshFilter>().mesh.normals);
            int[] segTris = segment.transform.GetComponent<MeshFilter>().mesh.triangles;
            foreach (int index in segTris)
            {
                triangles.Add(index + VertexCount);
            }
            VertexCount = vertices.Count;
        }
    }

    /* Map And Filter Duplicates
     * Based directly off of Bunny83's answer regarding welding vertices
     * from the Unity forum thread:
     * https://answers.unity.com/questions/1382854/welding-vertices-at-runtime.html
     * Private helper method for eliminating duplicate vertices and mapping
     * the originals to the final versions so that the triangles can be fixed.
     * No return type; newVertices, newNormals, and map passed as ref so they
     * modified by the function.
     * ASSUMES map was initialized to the correct size before being passed.
     */
    //private void MapAndFilterDuplicates(Vector3[] originalVertices,
    //                                    Vector3[] originalNormals,
    //                                    ref List<Vector3> newVertices,
    //                                    ref List<Vector3> newNormals,
    //                                    ref int[] map)
    private void MapAndFilterDuplicates(List<Vector3> originalVertices,
                                        List<Vector3> originalNormals,
                                        ref List<Vector3> newVertices,
                                        ref List<Vector3> newNormals,
                                        ref int[] map)
    {
        bool isDuplicate;
        for (int i = 0; i < originalVertices.Count; i++)
        {
            isDuplicate = false;
            for (int j = 0; j < newVertices.Count; j++)
            {
                //if (originalVertices[i] == newVertices[j] && originalNormals[i] == newNormals[j])
                if((originalVertices[i] - newVertices[j]).sqrMagnitude < PosTolerance
                    && Vector3.Angle(originalNormals[i], newNormals[j]) < NormTolerance)
                {
                    isDuplicate = true;
                    map[i] = j;
                    break;
                }
            }
            if(!isDuplicate)
            {
                map[i] = newVertices.Count;
                newVertices.Add(originalVertices[i]);
                newNormals.Add(originalNormals[i]);
            }
        }
    }

    /* Fix Triangles
    * Based directly off of Bunny83's answer regarding welding vertices
    * from the Unity forum thread:
    * https://answers.unity.com/questions/1382854/welding-vertices-at-runtime.html
    * Private helper method for correcting the triangle indices after
    * duplicates have been removed and a map of original dupes to final 
    * verts has been generated.
    * No return type; triangles is passed as ref so it can be modified 
    * by the function.
    * ASSUMES the map was generated previously and is correct.
    */
    private void FixTriangles(ref List<int> triangles, int[] map)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i] = map[triangles[i]];
        }
    }

    /* Get UV Bounds
     * 
     * Calculates the bounding box values of the supplied UV set.
     * @Param Vector2[] uvs - An array of UV values (Vector2). 
     * Returns a float[] in the format MinU, MinV, MaxU, MaxV. 
     * Returns null if an empty set of UVs is passed in.
     */
    private float[] GetUVBounds(Vector2[] uvs)
    {
        if (uvs != null && uvs.Length > 0)
        {
            // UV bounds in format MinU, MinV, MaxU, MaxV 
            float[] Bounds = new float[4] { uvs[0].x, uvs[0].y, uvs[0].x, uvs[0].y };

            for (int i = 1; i < uvs.Length; i++)
            {
                Bounds[0] = uvs[i].x < Bounds[0] ? uvs[i].x : Bounds[0];
                Bounds[1] = uvs[i].y < Bounds[1] ? uvs[i].y : Bounds[1];
                Bounds[2] = uvs[i].x > Bounds[2] ? uvs[i].x : Bounds[2];
                Bounds[3] = uvs[i].y > Bounds[3] ? uvs[i].y : Bounds[3];
            }
            return Bounds;
        }
        else
        {
            Debug.Log("GetUVBounds: Supplied UV array did not contain UVs");
            return null;
        }
    }


    //private Vector2[] CubeMapUVs(Mesh mesh)
    //{
    //    List<Vector2> newUVs = new List<Vector2>();

    //    float[] bbsize = new float[3] { mesh.bounds.size.x, mesh.bounds.size.y, mesh.bounds.size.z };
    //    float boundsMax = Mathf.Max(bbsize);

    //    Vector3 objCenter = GetObjectCenter(mesh.vertices);

    //    bool facesX = false, facesY = false, facesZ = false, negDir = false;
    //    for(int i = 0; i < mesh.triangles.Length; i += 3)
    //    {
    //        Vector3 faceNormal = (mesh.normals[mesh.triangles[i]] + mesh.normals[mesh.triangles[i + 1]] + mesh.normals[mesh.triangles[i + 2]]) / 3.0f;

    //        if (Mathf.Abs(faceNormal.x) >= Mathf.Abs(faceNormal.y) && Mathf.Abs(faceNormal.x) >= Mathf.Abs(faceNormal.z))
    //        {
    //            facesX = true;
    //            negDir = faceNormal.x > 0.0f ? false : true;
    //        }
    //        else if (Mathf.Abs(faceNormal.y) >= Mathf.Abs(faceNormal.x) && Mathf.Abs(faceNormal.y) >= Mathf.Abs(faceNormal.z))
    //        {
    //            facesY = true;
    //            negDir = faceNormal.y > 0.0f ? false : true;
    //        }
    //        else
    //        {
    //            facesZ = true;
    //            negDir = faceNormal.z > 0.0f ? false : true;
    //        }

    //        if(facesX)
    //        {

    //        }
    //        else if (facesY)
    //        {

    //        }
    //        else
    //        {

    //        }
    //    }
    //}

    private Vector3 GetObjectCenter(Vector3[] vertices)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            result += vertices[i];
        }
        result.x /= (vertices.Length - 1);
        result.y /= (vertices.Length - 1);
        result.z /= (vertices.Length - 1);
        return result;
    }


    //public Mesh ApplyUniformTaper(float percent, Vector3 start, Vector3 end, Mesh mesh)
    //{

    //}

}
