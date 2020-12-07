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
                originalCamRotation = _originalLookAtObj.transform.rotation;
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
    [SerializeField] private float PanDistance_MAX;

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
            //originalRotation = _originalLookAtObj.transform.rotation;
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
     * Moves the camera AND lookAt point based on mouse movement delta and
     * current camera positioning. Also ensures it stays within the maximum
     * panning distance from it's original location.
     * TODO: Test this method
     * TODO: Consider using sqrMagnitude instead of magnitude to speed up calcs
     */
    public void ProcessPan(Vector2 mouseDelta)
    {
        if ((originalLookAtPosition - lookAtPosition).magnitude.Equals(PanDistance_MAX))
            return;

        Vector3 horizontal = transform.right * -mouseDelta.x;
        Vector3 vertical = transform.up * -mouseDelta.y;
        Vector3 panDirection = horizontal + vertical;
        panDirection = panDirection.normalized;
        Vector3 move = panDirection * PanSpeed * Time.deltaTime;
        if ((originalLookAtPosition - (lookAtPosition + move)).magnitude < PanDistance_MAX)
        {
            lookAtPosition += move;
            transform.localPosition += move;
        }
        else
        {
            move = panDirection * (PanDistance_MAX - (originalLookAtPosition - lookAtPosition).magnitude);
            lookAtPosition += move;
            transform.localPosition += move;
        }
    }

    public void ProcessRotation(Vector2 delta)
    {
        float pivotAngleDelta = RotationSpeed * Time.deltaTime * delta.x;
        float tiltAngleDelta = RotationSpeed * Time.deltaTime * -delta.y;
        transform.RotateAround(lookAtPosition, Vector3.up, pivotAngleDelta);
        if (CanRotate(transform.forward, tiltAngleDelta))
            return;
        transform.RotateAround(lookAtPosition, transform.right, tiltAngleDelta);
    }

    public void ProcessZoom(float delta)
    {
        Vector3 lookVector = lookAtPosition - transform.position;
        float distance = lookVector.magnitude;
        float change = (delta * ZoomSpeed * 1.0f * Time.deltaTime);
        float newDistance = distance + change;
        if (newDistance >= ZoomDistance_MIN && newDistance <= ZoomDistance_MAX)
        {
            transform.position += lookVector.normalized * change;
        }
    }

    private bool CanRotate(Vector3 currentForward, float delta)
    {
        float theta;
        if (delta > 0)
        {
            theta = Vector3.Angle(currentForward, Vector3.down) - delta;
            return theta < PolarDeadzone_TOP;
        }
        else
        {
            theta = Vector3.Angle(currentForward, Vector3.up) + delta;
            return theta < PolarDeadzone_BOT;
        }
    }

    public void ChangeLookAtPosition(Vector3 newLookAt)
    {
        Vector3 posDelta = newLookAt - lookAtPosition;
        Vector3 resultCamPos = transform.position + posDelta;
        StartCoroutine("slerpToNewPosition", resultCamPos);
    }

    private IEnumerator lerpToNewPosition(Vector3 destination)
    {
        while (transform.position != destination)
        {
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime);
            yield return new WaitForSeconds(0f);
        }
        yield break;
    }

    public void ResetCameraTransform()
    {
        craftingCam.transform.position = originalCamPosition;
        craftingCam.transform.rotation = originalCamRotation;
        //craftingCam.transform.LookAt(originalLookAtPosition);
    }

}
