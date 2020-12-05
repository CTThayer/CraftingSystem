using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingReticlePositioning : MonoBehaviour
{
    [SerializeField] private Camera characterCamera;
    [SerializeField] private GameObject raycastOrigin;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(raycastOrigin != null);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = characterCamera.WorldToScreenPoint(raycastOrigin.transform.position);
        v.z = 0;
        this.transform.position = v;
    }
}
