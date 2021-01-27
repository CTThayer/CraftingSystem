using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingCameraController : MonoBehaviour
{
    [SerializeField] private Camera craftingCam;

    [SerializeField] private GameObject _originalLookAtObj;
    public GameObject originalLookAtObj
    {
        get => _originalLookAtObj;
        set
        {
            if (value != null)
            {
                _originalLookAtObj = value;
                originalLookAtPosition = _originalLookAtObj.transform.position;
                //originalCamRotation = _originalLookAtObj.transform.rotation;
            }
        }
    }
    private Vector3 lookAtPosition;
    private Vector3 originalLookAtPosition;
    private Vector3 originalCamPosition;

    [SerializeField] private float RotationSpeed;
    [SerializeField] private float PolarDeadzone_TOP;
    [SerializeField] private float PolarDeadzone_BOT;
    private Quaternion originalCamRotation;

    [SerializeField] private float ZoomSpeed;
    [SerializeField] private float ZoomDistance_MIN;
    [SerializeField] private float ZoomDistance_MAX;

    [SerializeField] private float PanSpeed;
    //[SerializeField] private float PanDistance_MAX;

    [SerializeField] private GameObject cameraBoundsObj;
    [SerializeField] private Bounds cameraBounds;

    void OnValidate()
    {
        if (cameraBoundsObj != null)
            cameraBounds = cameraBoundsObj.GetComponent<MeshRenderer>().bounds;
        else
            Debug.LogError("ERROR: Bounds object was not set for crafting apparatus.");
    }

    // Use this for initialization
    void Start()
    {
        if (craftingCam == null)
        {
            craftingCam = GetComponent<Camera>();
            if (craftingCam == null)
                Debug.LogError("ERROR: CraftingCam: No camera was set for the" +
                               " CraftingCam and the parent object does not " +
                               "contain a Camera object!");
        }

        Debug.Assert(_originalLookAtObj != null);
        if (_originalLookAtObj != null)
        {
            originalLookAtPosition = _originalLookAtObj.transform.position;
            lookAtPosition = originalLookAtPosition;
        }

        originalCamRotation = craftingCam.transform.rotation;
        originalCamPosition = craftingCam.transform.position;

        Debug.Assert(lookAtPosition != null);
        Debug.Assert(RotationSpeed > 0f);
        Debug.Assert(PolarDeadzone_TOP >= 0f);
        Debug.Assert(PolarDeadzone_BOT >= 0f);
        Debug.Assert(ZoomSpeed > 0f);
        Debug.Assert(ZoomDistance_MIN > 0 && ZoomDistance_MAX > 0 && ZoomDistance_MAX > ZoomDistance_MIN);
        transform.LookAt(lookAtPosition);
    }

    /* Process Pan
     * Moves the camera AND lookAtPosition based on mouse movement delta and
     * current camera positioning. Also ensures the lookAtPosition stays within 
     * the specified Bounds (cameraBounds).
     */
    public void ProcessPan(Vector2 mouseDelta)
    {
        Vector3 horizontal = transform.right * -mouseDelta.x;
        Vector3 vertical = transform.up * -mouseDelta.y;
        Vector3 panDirection = (horizontal + vertical).normalized;
        Vector3 move = panDirection * PanSpeed * Time.deltaTime;
        Vector3 nextLookPos = lookAtPosition + move;
        if (cameraBounds.Contains(nextLookPos))
        {
            lookAtPosition += move;
            transform.localPosition += move;
        }
    }

    /* Process Rotation
     * Rotates the camera around the lookAtPosition based on mouse delta.
     * Rotation is clamped via the CanTilt() method.
     */
    public void ProcessRotation(Vector2 delta)
    {
        float pivotAngleDelta = RotationSpeed * Time.deltaTime * delta.x;
        float tiltAngleDelta = RotationSpeed * Time.deltaTime * -delta.y;
        transform.RotateAround(lookAtPosition, Vector3.up, pivotAngleDelta);
        if (CanTilt(transform.forward, tiltAngleDelta))
            transform.RotateAround(lookAtPosition, transform.right, tiltAngleDelta);
        return;
    }

    /* Process Zoom
     * Moves the camera closer to the lookAtPosition (or further from it) based
     * on the delta parameter (which usually corresponds to the scroll wheel but
     * could be any other float value that made sense).
     */
    public void ProcessZoom(float delta)
    {
        float change = delta * ZoomSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + (transform.forward * change);
        float newDistance = (lookAtPosition - newPos).magnitude;
        //float oldDistance = (lookAtPosition - transform.position).magnitude;
        //Debug.Log("oldDist = " + oldDistance + " newDist  = " + newDistance + " change = " + (newDistance - oldDistance));
        if (newDistance > ZoomDistance_MIN && newDistance < ZoomDistance_MAX)
            transform.position = newPos;
    }

    /* Can Tilt
     * Validation method for checking whether the camera can be rotated up or 
     * down around the lookAtPosition (rotated around the current 
     * horizontal axis of the transform, i.e. transform.right). This method 
     * checks to see if changing the rotation around this axis by the supplied 
     * delta parameter will move the camera into either of the Polar Deadzones.
     * If it does not, the method returns true; otherwise it returns false.
     */
    private bool CanTilt(Vector3 currentForward, float delta)
    {
        float theta;
        if (delta > 0)
        {
            theta = Vector3.Angle(currentForward, Vector3.down) - delta;
            return theta > PolarDeadzone_TOP;
        }
        else
        {
            theta = Vector3.Angle(currentForward, Vector3.up) + delta;
            return theta > PolarDeadzone_BOT;
        }
    }

    // TODO: Add validation to check whether the lookAtPosition/camera can be 
    // moved to the newLookAt position.
    public void ChangeLookAtPosition(Vector3 newLookAt)
    {
        Vector3 posDelta = newLookAt - lookAtPosition;
        Vector3 resultCamPos = transform.position + posDelta;
        StartCoroutine("lerpToNewPosition", resultCamPos);
    }

    // TODO: Is this the cause of the camera not updateing when a new segment
    // is selected?
    private IEnumerator lerpToNewPosition(Vector3 destination)
    {
        while (transform.position != destination)
        {
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime);
            yield return new WaitForSeconds(0f);
        }
        yield break;
    }

    // TODO: Is this the cause of the bug where the camera is not positioned
    // totally correctly after exiting the apparatus and re-entering it?
    public void ResetCameraTransform()
    {
        craftingCam.transform.position = originalCamPosition;
        craftingCam.transform.rotation = originalCamRotation;
        //craftingCam.transform.LookAt(originalLookAtPosition);
    }

}
