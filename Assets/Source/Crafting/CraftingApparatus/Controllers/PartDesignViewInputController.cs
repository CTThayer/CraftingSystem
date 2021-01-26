using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDesignViewInputController : CraftingViewInputController
{
    //[SerializeField] private ItemPartDesignerController partDesigner;
    [SerializeField] private PartDesigner partDesigner;
    [SerializeField] private Camera cam;
    private Rect viewportRect;

    void Start()
    {
        base.Start();
        //Camera camera = craftingApparatus.craftingCamera.GetComponent<Camera>();
        viewportRect = cam.rect;
        //cam = craftingApparatus.craftingCamera.GetComponent<Camera>();
    }

    void Update()
    {
        HandleInput();
    }

    /* HandleInput()
     * Helper method for reading/responding to user input. Called
     * from Update() each frame. Deals with mouse click selection,
     * and arrow key input for selecting the AddLocation.
     */
    protected override void HandleInput()
    {
        // Manipulate Camera
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"),
                                                 Input.GetAxis("Mouse Y"));
                //craftingApparatus.camController.ProcessTumble(mouseDelta);
                craftingApparatus.camController.ProcessRotation(mouseDelta);
                return;     // Return to avoid multiple actions
            }
            if (Input.GetMouseButton(1))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"),
                                                 Input.GetAxis("Mouse Y"));
                craftingApparatus.camController.ProcessPan(mouseDelta);
                return;     // Return to avoid multiple actions
            }
            //if (Input.mouseScrollDelta.y != 0)
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            if (zoom != 0)
            {
                craftingApparatus.camController.ProcessZoom(zoom);
                return;     // Return to avoid multiple actions
            }
        }

        // Update Selection
        if (Input.GetMouseButtonDown(0))
        {
            LMBSelect();
        }

        // Handle Hotkey Presses
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            craftingApparatus.Exit();
        }
    }


    /* LMBSelect()
     * Updates the currently selected ComponentSegment using a raycast from
     * the cursor location. This method should be called when the left
     * mouse button has been pressed. 
     * Returns the name of the currently selected ComponentSegment (string).
     */
    private string LMBSelect()
    {
        string retVal = "";
        if (!CursorIsOverViewport())
            return retVal;
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.transform.gameObject;

            Debug.Log("Selected: " + hitObj.name);

            SegmentConnectionPoint SCP = hitObj.GetComponent<SegmentConnectionPoint>();
            ItemPartSegment NewSelection = hitObj.GetComponent<ItemPartSegment>();
            if (SCP == null && NewSelection == null)
            {
                SCP = hitObj.GetComponentInParent<SegmentConnectionPoint>();
                NewSelection = hitObj.GetComponentInParent<ItemPartSegment>();
            }

            // If a SegmentConnectionPoint was hit, update selection
            if (SCP != null)
            {
                if (SCP != partDesigner.SelectedConnectionPoint)
                {
                    if (partDesigner.SelectedConnectionPoint != null)
                        partDesigner.SelectedConnectionPoint.OnDeselection();
                    partDesigner.SelectedConnectionPoint = SCP;
                    partDesigner.SelectedConnectionPoint.OnSelection();
                    return SCP.gameObject.name; // early out
                }
            }
            else
            {
                if (partDesigner.SelectedConnectionPoint != null)
                {
                    partDesigner.SelectedConnectionPoint.OnDeselection();
                    partDesigner.SelectedConnectionPoint = SCP;
                }
            }

            // If a ComponentSegment was hit, update selection
            
            if (NewSelection != partDesigner.CurrentSelection)
            {
                if (partDesigner.CurrentSelection != null)
                {
                    partDesigner.CurrentSelection.OnDeselect();
                }
                if (NewSelection != null)
                {
                    NewSelection.OnSelect();
                    retVal = NewSelection.gameObject.name;

                    // update camera target location to be the transform of the 
                    // selected object
                    craftingApparatus.camController.ChangeLookAtPosition(NewSelection.transform.position);
                }
                partDesigner.CurrentSelection = NewSelection;

                // Auto-update connection point selection
                AutoUpdateConnectionPointSelection();
            }
        }
        // Else, nothing was hit so, clear selections
        else
        {
            if (partDesigner.SelectedConnectionPoint != null)
            {
                partDesigner.SelectedConnectionPoint.OnDeselection();
                partDesigner.SelectedConnectionPoint = null;
            }
            if (partDesigner.CurrentSelection != null)
            {
                partDesigner.CurrentSelection.OnDeselect();
                partDesigner.CurrentSelection = null;
            }
        }
        return retVal;
    }

    /* Auto-Update Connection Point Selection
     * Automatically updates the selected connection point based on the current
     * ComponentSegment selection (CurrentSelection). It will select the first
     * open connection point of the ComponentSegment (if there is one).
     */
    private void AutoUpdateConnectionPointSelection()
    {
        if (partDesigner.SelectedConnectionPoint != null)
            partDesigner.SelectedConnectionPoint.OnDeselection();
        if (partDesigner.CurrentSelection != null)
        {
            partDesigner.SelectedConnectionPoint = 
                partDesigner.CurrentSelection.GetNextOpenConnectionPoint();
            if (partDesigner.SelectedConnectionPoint != null)
                partDesigner.SelectedConnectionPoint.OnSelection();
        }
    }

    /* Cursor Is Over Viewport
     * Determine whether the mouse cursor is inside of the viewport area.
     */
    private bool CursorIsOverViewport()
    {
        Vector3 vpPoint = cam.ScreenToViewportPoint(Input.mousePosition);
        return viewportRect.Contains(vpPoint);
    }

    //private bool CursorIsOverUIPanel()
    //{
    //    Vector3 pos = Input.mousePosition;
    //    Rect panelCorners = RectTransformUtility.PixelAdjustRect(UIPanel, canvas);
    //    if (pos.x > panelCorners.xMin + UIPanel.anchoredPosition.x
    //        && pos.x < panelCorners.xMax + UIPanel.anchoredPosition.x
    //        && pos.y > panelCorners.yMin + UIPanel.anchoredPosition.y
    //        && pos.y < panelCorners.yMax + UIPanel.anchoredPosition.y)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
