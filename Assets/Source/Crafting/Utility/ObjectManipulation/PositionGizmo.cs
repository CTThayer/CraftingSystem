using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionGizmo : MonoBehaviour
{
    [SerializeField] private GameObject[] GizmoManipulators;
    [SerializeField] private float Sensitivity;

    private GameObject SelectedObject;
    private XYZRange Constraints;
    private Vector3 OriginalPosition;
    
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
            OriginalPosition = selection.transform.localPosition;
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
            Vector3 NewPosition = this.transform.localPosition;
            float MouseDeltaX = Input.GetAxis("Mouse X");
            float x = NewPosition.x + (MouseDeltaX * Time.deltaTime * Sensitivity);
            if (Constraints.ContainsXValue(x))
            {
                NewPosition.x = x;
                this.transform.localPosition = NewPosition;
                if (SelectedObject != null)
                    SelectedObject.transform.localPosition = NewPosition;
            }
        }
        else if (axis == 'Y')
        {
            Vector3 NewPosition = this.transform.localPosition;
            float MouseDeltaY = Input.GetAxis("Mouse Y");
            float y = NewPosition.y + (MouseDeltaY * Time.deltaTime * Sensitivity);
            if (Constraints.ContainsYValue(y))
            {
                NewPosition.y = y;
                this.transform.localPosition = NewPosition;
                if (SelectedObject != null)
                    SelectedObject.transform.localPosition = NewPosition;
            }
        }
        else if (axis == 'Z')
        {
            Vector3 NewPosition = this.transform.localPosition;
            float MouseDeltaZ = Input.GetAxis("Mouse X");
            float z = NewPosition.z + (MouseDeltaZ * Time.deltaTime * Sensitivity);
            if (Constraints.ContainsZValue(z))
            {
                NewPosition.z = z;
                this.transform.localPosition = NewPosition;
                if (SelectedObject != null)
                    SelectedObject.transform.localPosition = NewPosition;
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
