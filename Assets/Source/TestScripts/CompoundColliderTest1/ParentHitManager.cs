using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentHitManager : MonoBehaviour
{
    public GameObject blade;

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject == blade)
        //    Debug.Log("Use collision.gameObject");
        if (collision.GetContact(0).thisCollider.gameObject == blade)           // This is the one that works.
            Debug.Log("Blade Hit: " + collision.gameObject.name);   // Might need to loop through Contacts[] in actual code.
        //if (collision.GetContact(0).otherCollider.gameObject == blade)
        //    Debug.Log("Use collision.GetContact(0).otherCollider.gameObject");
    }

    // NOTE: The OnCollisionEnter() and OnTriggerEnter methods DO NOT work if
    // the script containing them is on a child of an object with a rigidbody.
    // All handling of hit information MUST be done in scripts on the parent
    // that has the rigidbody. So, contrary to some of the posts online, the 
    // following methods don't seem to work.

    //public void OnBladeHit(string output)
    //{
    //    Debug.Log(output);
    //}

    //public void OnBladeHit(BladeContact bc, string output)
    //{
    //    Debug.Log(output);
    //}

    //public void OnBladeHit(BladeContact bc)
    //{
    //    Debug.Log(bc.result);
    //}

}
