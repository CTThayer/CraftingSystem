using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy was hit by " + collision.gameObject.name);
    }
}
