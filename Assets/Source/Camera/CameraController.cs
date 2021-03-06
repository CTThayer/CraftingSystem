﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject Cam;
    [SerializeField] private Vector3 LookAtPosition;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float ZoomSpeed;
    [SerializeField] private float ZoomDistance_MIN;
    [SerializeField] private float ZoomDistance_MAX;

    // Degrees in any direction from the y-axis poles that can't be entered
    [SerializeField] private float PolarDeadzone;


    // Use this for initialization
    void Start()
    {
        Debug.Assert(LookAtPosition != null);
        Debug.Assert(RotationSpeed > 0f);
        Debug.Assert(ZoomSpeed > 0f);
        Debug.Assert(PolarDeadzone > 0f && PolarDeadzone < 45f);
        Debug.Assert(ZoomDistance_MIN > 0 && ZoomDistance_MAX > 0 && ZoomDistance_MAX > ZoomDistance_MIN);
        transform.LookAt(LookAtPosition);
    }

    public void ProcessZoom(float delta)
    {
        Vector3 v = LookAtPosition - transform.localPosition;
        float dist = v.magnitude + (delta * ZoomSpeed * -1.0f);
        if (dist < ZoomDistance_MAX && dist > ZoomDistance_MIN)
            transform.localPosition = LookAtPosition - dist * v.normalized;
    }

    //public void ProcessTumble(Vector3 delta)
    public void ProcessTumble(Vector2 delta)
    {
        // 1.a. Rotation of the viewing direction by right axis
        Quaternion q = Quaternion.AngleAxis(delta.x * RotationSpeed * Time.deltaTime, transform.up);
        // 1.b. Rotation of the viewing direction around y-axis
        Quaternion q2;
        if (!IsInPolarDeadzone())
        {
            q2 = Quaternion.AngleAxis(delta.y * RotationSpeed * Time.deltaTime * -1f, transform.right);
        }
        else
        {
            q2 = Quaternion.identity;
        }
        // 1.c. Concatenate quaternions
        q = q * q2;

        // 2.a. Create the rotation matrix that rotates the camera position around the pivot
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        Matrix4x4 pivot = Matrix4x4.TRS(LookAtPosition, Quaternion.identity, Vector3.one);
        Matrix4x4 invPivot = Matrix4x4.TRS(-LookAtPosition, Quaternion.identity, Vector3.one);
        rotationMatrix = pivot * rotationMatrix * invPivot;
        // 2.b. Rotate the camera position around the pivot location (camera target)
        transform.localPosition = rotationMatrix.MultiplyPoint(transform.localPosition);

        // 3.a. Update camera orientation (rotation) to aim directly at the target using LookAt()
        transform.LookAt(LookAtPosition);
    }

    private bool IsInPolarDeadzone()
    {
        //float thetaPosY = Mathf.Abs(Mathf.Acos(Vector3.Dot(transform.forward, Vector3.up)));
        //float thetaPosY = Mathf.Abs(Mathf.Acos(Vector3.Dot(transform.forward, -Vector3.up)));

        float thetaPosY = Vector3.Angle(this.transform.forward, Vector3.up);
        float thetaNegY = Vector3.Angle(this.transform.forward, -Vector3.up);

        if (thetaPosY < PolarDeadzone || thetaNegY < PolarDeadzone)
            return true;
        else
            return false;
    }

    const float TrackModifier = 1f / 100f;  // about 10-degress per second
    public void ProcessTrack(Vector3 delta)
    {
        Vector3 newCameraPos;
        newCameraPos.x = transform.localPosition.x + (delta.x * TrackModifier);
        newCameraPos.y = transform.localPosition.y + (delta.y * TrackModifier);
        newCameraPos.z = transform.localPosition.z;
        Vector3 newLookAtPos;
        newLookAtPos.x = LookAtPosition.x + (delta.x * TrackModifier);
        newLookAtPos.y = LookAtPosition.y + (delta.y * TrackModifier);
        newLookAtPos.z = LookAtPosition.z;
        transform.localPosition = newCameraPos;
        LookAtPosition = newLookAtPos;
    }

    public void SetLookAtPosition(Vector3 target)
    {
        if (target != null)
            LookAtPosition = target;
    }

    public void SetZoom(float distance)
    {
        if (distance > ZoomDistance_MIN && distance < ZoomDistance_MAX)
        {
            Vector3 v = transform.localPosition - LookAtPosition;
            transform.position = LookAtPosition + (v.normalized * distance);
        }
    }

}
