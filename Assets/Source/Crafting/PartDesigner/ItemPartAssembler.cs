using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPartAssembler : MonoBehaviour
{
    private float PosTolerance = 0.00001f;
    private float NormTolerance = 0.005f;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(PosTolerance > 0f && NormTolerance > 0f);
    }

    /* Validate Segment Configuration
     * 
     * Verifies that the current segment configuration is valid and that
     * the segments can be welded without issue.
     * 
     * @Param ComponentSegment[] segments - Array of ComponentSegments to weld. 
     * Returns bool for whether the configuration is valid.
     */
    public bool ValidateSegmentConfiguration(ItemPartSegment[] segments, 
                                             float componentMaxLength)
    {
        if (segments == null || segments.Length == 0)
        {
            Debug.Log("ComponentAssembler: INVALID CONFIG " +
                      "- Supplied segment array contains no segments.");
            return false;
        }
        else if (!segments[segments.Length - 1].SegmentIsEndSegment())
        {
            Debug.Log("ComponentAssembler: INVALID CONFIG " +
                      "- Last segment is not an end segment.");
            return false;
        }
        else if (segments.Length == 1 && segments[0].SegmentIsEndSegment())
        {
            Debug.Log("ComponentAssembler: VALID CONFIG - Supplied Segment " +
                      "array contains only 1 blade segment which is a valid " +
                      "end segment.");
            return true;
        }
        else
        {
            float totalLength = 0.0f;

            // TODO: handle first segment here b/c it may not have a base ([0])
            // connection and that might be ok

            for (int i = 1; i < segments.Length; i++)
            {
                ItemPartSegment[] connections = segments[i].GetConnectedSegments();
                string[] connectionIDs = segments[i].GetConnectionIDs();
                for (int j = 1; j < connections.Length; j++)
                {
                    if (connections[j] == null)
                    {
                        Debug.Log("ComponentAssembler: INVALID CONFIG - Not" +
                                  "all segment connection points have " +
                                  "segments attached to them.");
                        return false;
                    }
                    if (connections[j].GetConnectionID(0) != connectionIDs[j])
                    {
                        Debug.Log("ComponentAssembler: INVALID CONFIG - " +
                                  "connectionID mismatch on segments: " +
                                   connections[j].gameObject.name + " and " +
                                   segments[i].gameObject.name + ".");
                        return false;
                    }
                }
                totalLength += segments[i].GetSegmentLength();
            }
            if (totalLength <= componentMaxLength)
                return true;
            else
                return false;
        }
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
    public Mesh WeldSegmentMeshes(ItemPartSegment[] segments)
    {
        // Lists of resulting vertices and tris
        List<Vector3> AllVertices = new List<Vector3>();
        List<Vector3> AllNormals = new List<Vector3>();
        List<int> AllTriangles = new List<int>();

        // Store vertices, normals, and triangles from all segments in the lists
        GetAllVertsNormsAndTris(segments, ref AllVertices, ref AllNormals, ref AllTriangles);

        // Initialize Lists to store final vertices and normals in.
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();

        // Initialize an array to map old, duplicate vertices to the final ones 
        // to use for correcting triangles (and UVs if copied from originals)
        int[] vertexMap = new int[AllVertices.Count];

        // Call helper function to filter duplicates and create the map
        MapAndFilterDuplicates(AllVertices, AllNormals, ref newVertices, ref newNormals, ref vertexMap);

        // Fix triangles
        FixTriangles(ref AllTriangles, vertexMap);

        // TODO: Fix UVs if copied from original. Otherwise, create new UVs

        // Create new mesh and assign the finalized vertices, normals & triangles.
        Mesh result = new Mesh();
        result.vertices = newVertices.ToArray();
        result.normals = newNormals.ToArray();
        result.triangles = AllTriangles.ToArray();

        return result;
    }

    /* Get All Verts, Norms, And Tris
     * Private helper method for combining all the vertices, normals, and 
     * triangles from each of the ComponentSegments in the segments array.
     * No return type; lists for vertices, normals, and triangles passed as ref
     * so they can be modified (added to) by this function.
     */
    private void GetAllVertsNormsAndTris(ItemPartSegment[] segments,
                                         ref List<Vector3> vertices,
                                         ref List<Vector3> normals,
                                         ref List<int> triangles)
    {
        // Used to offset the triangle indices by the correct amount
        int VertexCount = 0;

        // Add all vertices, triangles, and normals from each segment
        // to the final list (while offsetting their values correctly)
        foreach (ItemPartSegment segment in segments)
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

    /* Planar Map UVs
     * 
     * Calculates UVs for the supplied mesh based on simple planar
     * projection to the specified plane. After all vertices are
     * mapped, the UVs are assigned to the mesh that was passed in.
     * @Param xy - Bool indicating whether to project ot the xy plane.
     * @Param yz - Bool indicating whether to project ot the yz plane.
     * Pass false for both xy and yz if you want to project to xz plane.
     * @Param m - Mesh that needs UV projection.
     */
    public void PlanarMapUVs(bool xy, bool yz, ref Mesh m)
    {
        if (xy && yz)   // Can't use both xy and zy plane.
        {
            Debug.Log("ComponentAssembler: PlanarMapUVs: ERROR - Can't use " +
                      "both xy and zy plane for mapping; you must select one " +
                      "(or neither for xz).");
            return;
        }
        if (m == null)
        {
            Debug.Log("ComponentAssembler: PlanarMapUVs: ERROR - " +
                      "Supplied mesh was null.");
            return;
        }
        Vector3[] vertices = m.vertices;
        Vector3[] normals = m.normals;
        Vector2[] uvs = new Vector2[vertices.Length];
        if (vertices == null || normals == null)
        {
            Debug.Log("ComponentAssembler: PlanarMapUVs: ERROR - Mesh is " +
                      "missing necessary components.");
            return;
        }
        Vector3 bounds = m.bounds.size;
        if (xy)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                float dot = Vector3.Dot(normals[i], Vector3.forward);
                if (dot > 0)
                    uvs[i] = new Vector2(vertices[i].x / (bounds.x * 2), vertices[i].y / (bounds.x * 2));
                else
                    uvs[i] = new Vector2((vertices[i].x / (bounds.x * 2)) + 0.5f, vertices[i].y / (bounds.x * 2));
            }
        }
        else if (yz)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                float dot = Vector3.Dot(normals[i], Vector3.right);
                if (dot > 0)
                    uvs[i] = new Vector2(vertices[i].z / (bounds.z * 2), vertices[i].y / (bounds.z * 2));
                else
                    uvs[i] = new Vector2((vertices[i].z / (bounds.z * 2)) + 0.5f, vertices[i].y / (bounds.z * 2));
            }
        }
        else
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                float dot = Vector3.Dot(normals[i], Vector3.up);
                if (dot > 0)
                    uvs[i] = new Vector2(vertices[i].x / (bounds.x * 2), vertices[i].z / (bounds.x * 2));
                else
                    uvs[i] = new Vector2((vertices[i].x / (bounds.x * 2)) + 0.5f, vertices[i].z / (bounds.x * 2));
            }
        }
        m.uv = uvs;
    }

    /* Cube Map UVs
     * 
     * Calculates UVs for the supplied mesh using simplified cube projection.
     * @Param m - Mesh that needs UV projection.
     */
    public void CubeMapUVs(ref Mesh m)
    {
        if (m == null)
        {
            Debug.Log("ComponentAssembler: CubeMapUVs: ERROR - " +
                      "Supplied mesh was null.");
            return;
        }
        Vector3[] vertices = m.vertices;
        Vector3[] normals = m.normals;
        Vector2[] uvs = new Vector2[vertices.Length];
        
        //Vector3 bounds = m.bounds.size;
        float[] b = new float[3] { m.bounds.size.x,
                                   m.bounds.size.y,
                                   m.bounds.size.z };
        float bigBound = Mathf.Max(b);

        for (int i = 0; i < vertices.Length; i++)
        {
            // Determine which plane to map uvs to:
            char c = GetUVAxis(normals[i].x, normals[i].y, normals[i].z);
            switch(c)
            {
                case 'X':
                    {
                        uvs[i] = new Vector2(vertices[i].z / (bigBound * 3), vertices[i].y / (bigBound * 3));
                        break;
                    }
                case 'Y':
                    {
                        uvs[i] = new Vector2((vertices[i].x / (bigBound * 3)) + 0.33333f, vertices[i].z / (bigBound * 3));
                        break;
                    }
                case 'Z':
                    {
                        uvs[i] = new Vector2((vertices[i].x / (bigBound * 3)) + 0.66667f, vertices[i].y / (bigBound * 3));
                        break;
                    }
                case 'x':
                    {
                        uvs[i] = new Vector2(-vertices[i].z / (bigBound * 3), vertices[i].y / (bigBound * 3));
                        break;
                    }
                case 'y':
                    {
                        uvs[i] = new Vector2((vertices[i].x / (bigBound * 3)) + 0.33333f, (-vertices[i].z / (bigBound * 3)) + 0.33333f);
                        break;
                    }
                case 'z':
                    {
                        uvs[i] = new Vector2((-vertices[i].x / (bigBound * 3)) + 0.66667f, (vertices[i].y / (bigBound * 3)) + 0.66667f);
                        break;
                    }
            }
        }

        m.uv = uvs;
    }

    private char GetUVAxis(float nX, float nY, float nZ)
    {
        float absX = Mathf.Abs(nX);
        float absY = Mathf.Abs(nY);
        float absZ = Mathf.Abs(nZ);
        bool XgtY = (absX > absY);
        bool XgtZ = (absX > absZ);
        bool YgtZ = (absX > absZ);
        if (XgtY && XgtZ)
        {
            if (nX > 0)
                return 'X';     // 'X' for +X
            else
                return 'x';     // 'x' for -X
        }
        else if (!XgtZ && !YgtZ)
        {
            if (nZ > 0)
                return 'Z';     // 'Z' for +Z
            else
                return 'z';     // 'z' for -Z
        }
        else if (!XgtY && XgtZ && YgtZ)
        {
            if (nZ > 0)
                return 'Y';     // 'Y' for +Y
            else
                return 'y';     // 'y' for -Y
        }
        return '0';
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
            Debug.Log("GetUVBounds: Supplied UV array was empty.");
            return null;
        }
    }

}
