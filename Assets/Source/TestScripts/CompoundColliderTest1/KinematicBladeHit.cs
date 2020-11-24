using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBladeHit : MonoBehaviour
{
    KinematicMove parentScript;

    void Start()
    {
        parentScript = transform.parent.GetComponent<KinematicMove>();
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    parentScript.OnChildCollision(this.gameObject, collision);
    //    Debug.Log("Kinematic Blade Hit");
    //}

    void OnTriggerEnter(Collider collider)
    {
        parentScript.OnChildCollision(this.gameObject, collider);
        Debug.Log("Kinematic Blade Hit");
    }

}
