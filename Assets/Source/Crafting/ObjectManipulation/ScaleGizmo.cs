using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleGizmo : MonoBehaviour
{
    [SerializeField] private GameObject[] GizmoManipulators;
    [SerializeField] private float Sensitivity;

    private GameObject SelectedObject;
    private XYZRange Constraints;
    private Vector3 OriginalScale;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(GizmoManipulators != null);
        Debug.Assert(GizmoManipulators.Length == 3);
        Debug.Assert(GizmoManipulators[0].GetComponent<Collider>() != null);
        Debug.Assert(GizmoManipulators[1].GetComponent<Collider>() != null);
        Debug.Assert(GizmoManipulators[2].GetComponent<Collider>() != null);
        Debug.Assert(Sensitivity > 0f);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetSelectedObject(GameObject selection)
    {
        SelectedObject = selection;
        if (selection != null)
        {
            ActivateGizmo();
            OriginalScale = selection.transform.localScale;
            this.transform.localPosition = selection.transform.localPosition;
            this.transform.localScale = selection.transform.localScale;
        }
        else
        {
            DeactivateGizmo();
        }
    }

    public void SetConstraints(XYZRange constraints)
    {
        Constraints = constraints;
    }

    public void UseGizmo(char axis)
    {
        if (axis == 'X')
        {
            Vector3 NewScale = this.transform.localScale;
            float MouseDeltaX = Input.GetAxis("Mouse X");
            float x = NewScale.x + (MouseDeltaX * Time.deltaTime * Sensitivity);
            if (Constraints.ContainsXValue(x))
            {
                NewScale.x = x;
                this.transform.localScale = NewScale;
                if (SelectedObject != null)
                    SelectedObject.transform.localScale = NewScale;
            }
        }
        else if (axis == 'Y')
        {
            Vector3 NewScale = this.transform.localScale;
            float MouseDeltaY = Input.GetAxis("Mouse Y");
            float y = NewScale.y + (MouseDeltaY * Time.deltaTime * Sensitivity);
            if (Constraints.ContainsXValue(y))
            {
                NewScale.y = y;
                this.transform.localScale = NewScale;
                if (SelectedObject != null)
                    SelectedObject.transform.localScale = NewScale;
            }
        }
        else if (axis == 'Z')
        {
            Vector3 NewScale = this.transform.localScale;
            float MouseDeltaZ = Input.GetAxis("Mouse X");
            float z = NewScale.z + (MouseDeltaZ * Time.deltaTime * Sensitivity);
            if (Constraints.ContainsXValue(z))
            {
                NewScale.z = z;
                this.transform.localScale = NewScale;
                if (SelectedObject != null)
                    SelectedObject.transform.localScale = NewScale;
            }
        }
    }

    private void ActivateGizmo()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        for (int i = 0; i < 3; i++)
        {
            GizmoManipulators[i].GetComponent<MeshRenderer>().enabled = true;
            GizmoManipulators[i].GetComponent<Collider>().enabled = true;
        }
    }

    private void DeactivateGizmo()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        for (int i = 0; i < 3; i++)
        {
            GizmoManipulators[i].GetComponent<MeshRenderer>().enabled = false;
            GizmoManipulators[i].GetComponent<Collider>().enabled = false;
        }
    }
}
