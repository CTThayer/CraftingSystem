using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    [SerializeField] private bool isPrimary;
    [SerializeField] private bool isVisible;
    [SerializeField] private bool mustTouchObject;
    [SerializeField] private float testRadius;
    [SerializeField] private float maxDistanceToObj;

    [SerializeField] private GameObject _controlledObj;
    public GameObject controlledObj
    {
        get => _controlledObj;
        private set => _controlledObj = value;
    }

    [SerializeField] private Color errorColor;
    private Color originalColor;

    private void Awake()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }

    public void CenterOnObject(GameObject gameObj)
    {
        if (gameObj != null)
        {
            this.transform.position = gameObj.transform.position;
            this.transform.rotation = gameObj.transform.rotation;
        }
    }

    public void ChangeHandlePosition(Vector3 newPosition)
    {
        this.transform.position = newPosition;
        if (isVisible)
        {
            if (!ValidatePosition())
                GetComponent<Renderer>().material.color = errorColor;
            else
                GetComponent<Renderer>().material.color = originalColor;
        }
    }

    public void Show(bool visible)
    {
        GetComponent<Renderer>().enabled = visible;
        isVisible = visible;
    }

    public bool ValidatePosition()
    {
        if (mustTouchObject)
        {
            return ValidatePositionTouches();
        }
        else
        {
            return ValidatePositionDistance();
        }
    }

    private bool ValidatePositionTouches()
    {
        if (controlledObj == null)
            return false;
        Collider[] touches = Physics.OverlapSphere(this.transform.position, testRadius);
        if (touches == null || touches.Length <= 0)
            return false;
        List<Collider> objColliders = new List<Collider>(controlledObj.GetComponents<Collider>());
        objColliders.AddRange(controlledObj.GetComponentsInChildren<Collider>());
        for (int i = 0; i < objColliders.Count; i++)
        {
            for (int j = 0; j < touches.Length; j++)
            {
                if (objColliders[i] == touches[j])
                    return true;
            }
        }
        return false;
    }

    // Note: This currently only checks distance to the object's transform, NOT
    // the surface of the object (which would be a much more complicated check)
    private bool ValidatePositionDistance()
    {
        if (controlledObj == null)
            return false;
        Vector3 diffVector = controlledObj.transform.position - this.transform.position;
        float distance = diffVector.magnitude;
        if (distance > maxDistanceToObj)
            return false;
        else
            return true;
    }
}
