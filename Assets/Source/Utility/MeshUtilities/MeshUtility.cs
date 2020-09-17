using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtility
{
    /* Calculate Mesh Volume
     * Calculates the exact volume of the specified mesh using signed volume of 
     * tetrahedra. Code based on the formula found here:
     * http://mathcentral.uregina.ca/QQ/database/QQ.09.09/h/ozen1.html
     * NOTE: This method works best if the origin is contained in the mesh and
     * the mesh is completely closed. However, it should (in theory) still work
     * if the origin is not contained in the mesh as long as the mesh is closed
     * and isn't a long way from the origin.
     */
    public float CalculateMeshVolume(Mesh mesh)
    {
        float volume = 0f;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            float f1 = verts[i].x * verts[i + 1].y * verts[i + 2].z;
            float f2 = verts[i].y * verts[i + 1].z * verts[i + 2].x;
            float f3 = verts[i].z * verts[i + 1].x * verts[i + 2].y;
            float f4 = verts[i].x * verts[i + 1].z * verts[i + 2].y;
            float f5 = verts[i].y * verts[i + 1].x * verts[i + 2].z;
            float f6 = verts[i].z * verts[i + 1].y * verts[i + 2].x;
            float vol = (f1 + f2 + f3 - f4 - f5 - f6) / 6.0f;
            volume += vol;
        }
        return Mathf.Abs(volume); // Abs to ensure positive volume
    }


    //public bool MeshContact(Mesh A, Mesh B)
    //{

    //}

    //public Vector3[] MeshIntersection(Mesh A, Mesh B)
    //{

    //}


    //public Vector3 GetSurfacePosNearestToPoint(Mesh surface, Vector3 point)
    //{
    //    // NOTE: Only works with an active mesh collider and nothing between the
    //    // point and the surface.
    //    //int i = GetVertexNearestToPoint(surface, point);
    //    //Vector3[] verts = surface.vertices;
    //    //int[] tris = surface.triangles;
    //    //Vector3 direction = (verts[i] - point).normalized;
    //    //RaycastHit hit;
    //    //if (Physics.Raycast(point, direction, out hit, 1.0f))
    //    //{
    //    //    tris[hit.triangleIndex];
    //    //}

    //}

    // Naive Implementation - acceleration structures like an Octree would speed
    // this up dramatically.
    public int GetVertexNearestToPoint(Mesh m, Vector3 point)
    {
        int indexOfNearest = -1;
        if (m != null)
        {
            Vector3[] verts = m.vertices;
            float minSqrDistance = Mathf.Infinity;
            for (int i = 0; i < verts.Length; i++)
            {
                float currentSqrDist = (verts[i] - point).sqrMagnitude;
                if (currentSqrDist < minSqrDistance)
                {
                    minSqrDistance = currentSqrDist;
                    indexOfNearest = i;
                }
            }
        }
        return indexOfNearest;
    }
}
