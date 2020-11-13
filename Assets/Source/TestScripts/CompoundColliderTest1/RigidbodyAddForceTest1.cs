using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyAddForceTest1 : MonoBehaviour
{
    public float delay;
    public Vector3 f1;
    public Vector3 f2;
    public Vector3 f2Pos;
    private Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        StartCoroutine(TestCoroutine());
    }

    IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(delay);
        r.useGravity = true;
        r.AddForce(f1);
        r.AddForceAtPosition(f2, f2Pos);
        yield break;
    }

}
