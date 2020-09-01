using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] private string ComponentType;
    [SerializeField] private string MaterialType;

    //[SerializeField] private float ComponentDurability_MAX;
    //[SerializeField] private float ComponentDurability_Current;

    private Color originalColor;
    private float ComponentVolume;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(this.GetComponent<MeshFilter>() != null);
        Debug.Assert(this.GetComponent<MeshFilter>().mesh != null);
        Debug.Assert(this.GetComponent<MeshRenderer>() != null);
        Debug.Assert(this.GetComponent<Collider>() != null);

        originalColor = this.GetComponent<MeshRenderer>().material.color;

        ComponentVolume = CalculateExactComponentVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* OnSelect()
     * Changes the color of the GameObject to indicate it is currently selected. 
     */
    public void OnSelect()
    {
        this.GetComponent<MeshRenderer>().material.color = new Color(0.902f, 0.863f, 0.560f, 0.500f);
    }

    /* OnDeselect()
     * Changes the color of the GameObject back to it's original color to 
     * indicate that it has been deselected.
     */
    public void OnDeselect()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originalColor;
    }

    // Getters
    public string GetComponentType() { return ComponentType; }
    public string GetMaterialType() { return MaterialType; }


    /* Calculate Exact Component Volume
     * Calculates the exact volume of the mesh attached to this ItemComponent 
     * using signed volume of tetrahedra. Code based on the formula found here:
     * http://mathcentral.uregina.ca/QQ/database/QQ.09.09/h/ozen1.html
     * NOTE: This method works best if the origin is contained in the mesh and
     * the mesh is completely closed. However, it should (in theory) still work
     * if the origin is not contained in the mesh as long as the mesh isn't a
     * long way from the origin.
     */
    private float CalculateExactComponentVolume()
    {
        float volume = 0f;
        Vector3[] verts = this.GetComponent<MeshFilter>().mesh.vertices;
        int[] tris = this.GetComponent<MeshFilter>().mesh.triangles;
        for(int i = 0; i < tris.Length; i += 3)
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
}
