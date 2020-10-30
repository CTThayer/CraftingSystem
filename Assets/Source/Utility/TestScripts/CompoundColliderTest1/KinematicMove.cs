using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicMove : MonoBehaviour
{
    public float delay;
    public float speed;
    public float mass;
    public Vector3 targetRotation;
    //public Vector3 force1;

    private Vector3 initialRotation;
    private bool collided = false;
    private bool notFinishedRotation = true;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.up;
        StartCoroutine(StartTestCoroutine());
    }

    IEnumerator StartTestCoroutine()
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(RotateCoroutine());
        yield break;
    }

    IEnumerator RotateCoroutine()
    {
        while (notFinishedRotation)
        {
            //Vector3 newDirection = Vector3.RotateTowards(transform.up, Vector3.forward, speed * Time.deltaTime, 0.0f);
            //transform.rotation = Quaternion.LookRotation(newDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), speed * Time.deltaTime * 120f);
            if (transform.rotation == Quaternion.Euler(targetRotation))
                notFinishedRotation = false;
            yield return null;
        }
        //else
        //{
            StartCoroutine(SpringForwardXCoroutine());
            yield break;
        //}
    }

    IEnumerator SpringForwardXCoroutine()
    {
        while (!collided)
        {
            Vector3 newPos = transform.position;
            newPos.x += Time.deltaTime * speed * 6;
            transform.position = newPos;
            yield return null;
        }
        //else
        //{
            GetComponent<Rigidbody>().isKinematic = false;
            //collided = false;
            yield break;
        //}
    }

    //private void SpringForwardRigidbody()
    //{
    //    Rigidbody r = GetComponent<Rigidbody>();
    //    r.isKinematic = false;
    //    //r.AddForce(1250f, 0f, 0f);
    //    r.AddForce(force1);
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    collided = true;
    //}

    public void OnChildCollision(GameObject child, Collision collision)
    {
        collided = true;
        collision.rigidbody.AddForce(mass * speed, 0f, 0f);
    }

    public void OnChildCollision(GameObject child, Collider collider)
    {
        collided = true;
        Debug.Log("KinematicMove: OnChildCollision(): Hit Collider GameObject = " + collider.gameObject.name);
        collider.GetComponent<Rigidbody>().AddForce(mass * speed * 1000f, 0f, 0f);
    }

}
