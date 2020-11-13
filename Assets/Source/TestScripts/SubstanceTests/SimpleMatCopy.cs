using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMatCopy : MonoBehaviour
{
    public GameObject ObjA;
    public GameObject ObjB;
    
    // Start is called before the first frame update
    void Start()
    {
        Material mat = new Material(ObjA.GetComponent<Renderer>().material);
        mat.CopyPropertiesFromMaterial(ObjA.GetComponent<Renderer>().material);
        mat.name = "ObjB_mat";
        ObjB.GetComponent<Renderer>().material = mat;
    }

}
