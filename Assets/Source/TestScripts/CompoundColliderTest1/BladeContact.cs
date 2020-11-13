using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeContact : MonoBehaviour
{
    ParentHitManager parentHitManager;
    //public string result;

    void Start()
    {
        // Note this could all be done in the parent object and this script
        // could be eliminated but since I wanted to keep the NOTE about the
        // collider functionality with parents that have rigidbodies, I decided
        // to leave all of the test code in the file but commented out.
        parentHitManager = transform.parent.GetComponent<ParentHitManager>();
        parentHitManager.blade = this.gameObject;
    }

    // NOTE: The OnCollisionEnter() and OnTriggerEnter methods DO NOT work if
    // the script containing them is on a child of an object with a rigidbody.
    // All handling of hit information MUST be done in scripts on the parent
    // that has the rigidbody. Therefore, the following methods don't work

    //void OnCollisionEnter(Collision collision)
    //{
    //    parentHitManager.OnBladeHit(this, "Blade hit " + collision.gameObject.name +
    //                " with relative velocity: " + collision.relativeVelocity);
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    result = "Blade hit " + collision.gameObject.name + " with relative velocity: " + collision.relativeVelocity;
    //    parentHitManager.OnBladeHit(this);
    //}
}
