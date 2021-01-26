using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPartDesignerController : MonoBehaviour
{
    // Manually Configured Fields
    [SerializeField] private Transform DefaultAddLocation;
    [SerializeField] private float MaxLength;
    [SerializeField] private Material defaultMat;

    // Manually Assigned Dependencies
    [SerializeField] private ItemPartAssembler CompAssembler;
    [SerializeField] private CameraController CamControl;

    // UI Fields
    [SerializeField] private Text OutputText;
    [SerializeField] private Text SelectedSegmentText;
    [SerializeField] private DropdownGameObjectSelector GameObjectSelector;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform UIPanel;
    [SerializeField] private Slider SegmentLengthSlider;

    // Public Variables
    public GameObject ComponentToAdd;

    // Private Variables
    private bool isFirstSegment;
    private List<ItemPartSegment> Segments = new List<ItemPartSegment>();

    // Public Variables
    public ItemPartSegment CurrentSelection;
    public SegmentConnectionPoint SelectedConnectionPoint;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Ensure manually assigned fields are not null
        Debug.Assert(DefaultAddLocation != null);
        //Debug.Assert(AddLocIndicatorPrefab != null);
        Debug.Assert(OutputText != null);
        Debug.Assert(SelectedSegmentText != null);
        Debug.Assert(GameObjectSelector != null);
        Debug.Assert(defaultMat != null);
        //Debug.Assert(CamControl != null);

        // isFirstSegment should always start out as true
        isFirstSegment = true;

        // Set DropdownGameObjectSelector's delegate function to SetComponentToAdd()
        GameObjectSelector.SetDropdownDelegate(SetComponentToAdd);

        //Set ComponentToAdd to match the initial value of the dropdown
        ComponentToAdd = GameObjectSelector.GetObjectFromDropdown(0);

        // Set SelectedConnection to 1 b/c it will usually start w/first index
        //SelectedAddLocIndex = 1;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    HandleInput();
    //}

    /*************************** Public Methods *******************************/

    /* SetComponentToAdd()
     * Public facing method for setting the ComponentToAdd GameObject reference
     * to point to whatever prefab the user selected from the UI menu(s).
     */
    public void SetComponentToAdd(GameObject go)
    {
        if (go != null)
        {
            ItemPartSegment newSeg = go.GetComponent<ItemPartSegment>();
            if (newSeg != null)
            {
                ComponentToAdd = go;
                return;
            }
            else
            {
                Debug.Log("ComponentDesignerController: SetComponentToAdd() " +
                          "- Supplied GameObject is not a ComponentSegment.");
                return;
            }
        }
        else
        {
            Debug.Log("ComponentDesignerController: SetComponentToAdd() " +
                      "- Supplied GameObject is null.");
            return;
        }
    }

    /* AddSegment()
     * Public facing method for adding a segment (for use in UI).
     * Adds the chosen segment using AddComponentSegment() and sets the output
     * text field's to the returned output string.
     */
    public void AddSegment()
    {
        OutputText.text = AddComponentSegment();
    }

    /* AddComponentSegment()
     * Adds the chosen ComponentSegment to the workspace (and Segments list) at
     * the currently selected segment/add location. This also validates a number
     * of conditions and returns a string detailing success or any errors.
     */
    private int count = 0;
    private string AddComponentSegment()
    {
        if (isFirstSegment)
        {
            // NOTE: If base-segments are used then the first segment needs to
            // be ensured to be a base segment at this point.

            GameObject firstSeg = Instantiate(ComponentToAdd.gameObject,
                                              DefaultAddLocation.position,
                                              DefaultAddLocation.rotation);
            firstSeg.name = "Segment0";
            isFirstSegment = false;
            Segments.Add(firstSeg.GetComponent<ItemPartSegment>());
            count++;
        }
        else
        {
            if (CurrentSelection == null)
            {
                return "An existing ComponentSegment is not selected. " +
                       "ComponentSegment cannot be added.";
            }
            if (ComponentToAdd == null)
            {
                return "No new ComponentSegment to add was selected. " +
                       "ComponentSegment cannot be added.";
            }

            // Get a reference to the ComponentSegment script of the new segment
            ItemPartSegment segToAdd = ComponentToAdd.GetComponent<ItemPartSegment>();

            // Get indices to connect segment at
            int AddIndexInSelected;
            if (SelectedConnectionPoint != null)
                AddIndexInSelected = SelectedConnectionPoint.indexInConnections;
            else                                                                // TODO: Should we error out here instead?
            {
                // if no connection point is selected and selected segment has 
                // more than 1 connection point, select the first non-parent
                // facing connection point (index 1), else select index 0
                int numConnections = CurrentSelection.GetConnectedSegments().Length;
                AddIndexInSelected = numConnections > 1 ? 1 : 0;
            }
            int AddIndexInNewSeg = AddIndexInSelected > 0 ? 0 : 1;

            // Verify that the new segment can actually connect at this point
            if (!CurrentSelection.HasConnectionAt(AddIndexInSelected))
            {
                if (CurrentSelection.GetConnectionID(AddIndexInSelected)
                    == segToAdd.GetConnectionID(AddIndexInNewSeg))
                {
                    // Setup AddLocation transform
                    Vector3 AddPos = CurrentSelection.GetConnectionPoint(AddIndexInSelected).transform.position;
                    Quaternion AddRot = CurrentSelection.GetConnectionPoint(AddIndexInSelected).transform.rotation;
                    if (AddIndexInSelected == 0)
                    {
                        Vector3 newSegConnPos = segToAdd.GetConnectionPoint(AddIndexInNewSeg).transform.position;
                        AddPos = AddPos - newSegConnPos;
                    }

                    // Instantiate & name new segment
                    GameObject segObj = Instantiate(ComponentToAdd, AddPos, AddRot);
                    segObj.name = "Segment" + count;

                    // Store new segment's reference in the segments list
                    ItemPartSegment newSegment = segObj.GetComponent<ItemPartSegment>();
                    Segments.Add(newSegment);

                    // Update segment connections
                    CurrentSelection.AddConnectedSegmentAt(AddIndexInSelected, newSegment);
                    newSegment.AddConnectedSegmentAt(AddIndexInNewSeg, CurrentSelection);

                    // Reset highlighting if the current segment was previously 
                    // a disconnected segment
                    if (CurrentSelection.isDisconnected)
                        CurrentSelection.OnReconnection();

                    // Check for contact with other segments and update any
                    // connections if applicable.
                    newSegment.CheckForContact();

                    // Update count used for naming segments
                    count++;
                }
                else
                {
                    return "Connection ID Mismatch";
                }
            }
            else
            {
                return "Segment already exists at selected connection point.";
            }
        }

        // Update selection to be newly added object
        if (CurrentSelection != null)
        {
            CurrentSelection.OnDeselect();
        }
        CurrentSelection = Segments[Segments.Count - 1].GetComponent<ItemPartSegment>();
        CurrentSelection.OnSelect();

        // Update ConnectionPoint selection to be next open connection
        AutoUpdateConnectionPointSelection();

        return "ComponentSegment was added.";
    }

    /* RemoveSegment()
     * Public facing method for removing a segment (for use in UI).
     * Removes the currently selected segment.
     * NOTE: We are NOT shifting segments down to ensure that everything 
     * connects because there is no guarantee that the next segment will be 
     * compatible and doing so makes assumptions about the user's intent. It 
     * would also prohibit simply swapping out a segment. This means that 
     * validating the final segment configuration is CRITICAL!
     * NOTE: This does NOT include any functionality for tracking downstream 
     * references to objects that were previously connected to this one. Segment
     * connections are validated differently in several other places.
     */
    public void RemoveSegment()
    {
        if (CurrentSelection != null)
        {
            ItemPartSegment parentSeg = CurrentSelection.GetConnectedSegment(0);
            
            // Clean-up segment connections.
            CurrentSelection.ClearConnections();

            // Remove from Segments list and destroy the selected GameObject.
            int index = Segments.IndexOf(CurrentSelection);
            Segments.Remove(CurrentSelection);
            Destroy(CurrentSelection.transform.gameObject);

            // Select the object that precedes this one (if it exists).
            if (index > 0)
            {
                CurrentSelection = parentSeg;
                if (CurrentSelection != null)
                    CurrentSelection.OnSelect();
                AutoUpdateConnectionPointSelection();
            }
        }
    }

    /* AssembleComponent()
     * Public facing method for assembling the final mesh (for use in UI).
     * Uses the ComponentAssembler reference (CompAssembler) to run final
     * validation on the segment configuration and then assemble a final
     * mesh from the segments if the config is valid. It also creates a new
     * GameObject and assigns the resulting mesh to it.
     * 
     * TODO: The mesh data/resulting component object should be stored in an
     * asset database and/or pushed to the server once these features are
     * fully implemented.
     */
    public GameObject AssembleComponent()
    {
        ItemPartSegment[] segArray = Segments.ToArray();
        bool validConfig = CompAssembler.ValidateSegmentConfiguration(segArray, MaxLength);
        if(validConfig)
        {
            // For test purposes, setup a new GameObject with the new mesh 
            // and move it off the side so that it can be viewed.
            Mesh m = CompAssembler.WeldSegmentMeshes(segArray);
            GameObject finishedComponent = new GameObject();
            finishedComponent.AddComponent<MeshFilter>();
            MeshRenderer MR = finishedComponent.AddComponent<MeshRenderer>();
            MR.material = defaultMat;
            finishedComponent.GetComponent<MeshFilter>().mesh = m;
            Vector3 componentDisplayPos = DefaultAddLocation.position;
            componentDisplayPos.x += 1.5f;
            finishedComponent.transform.position = componentDisplayPos;         // TODO: remove this once debugging is complete
            return finishedComponent;
        }
        else
        {
            OutputText.text = "Invalid segment configuration. Component " +
                              "can't be constructed.";
            Debug.Log("ComponentDesigner: Segment configuration was not valid.");
            return null;
        }
    }

    /* HandleInput()
     * Helper method for reading/responding to user input. Called
     * from Update() each frame. Deals with mouse click selection,
     * and arrow key input for selecting the AddLocation.
     */
    private void HandleInput()
    {
        // Manipulate Camera
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"),
                                                 Input.GetAxis("Mouse Y"));
                CamControl.ProcessTumble(mouseDelta);
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                CamControl.ProcessZoom(Input.mouseScrollDelta.y);
            }
            return;     // Return to avoid accidentally changing selection
        }

        // Update Selection
        if (Input.GetMouseButtonDown(0))
        {
            LMBSelect();
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
        if (CursorIsOverUIPanel())
            return retVal;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.transform.gameObject;

            Debug.Log("Selected: " + hitObj.name);

            // If a SegmentConnectionPoint was hit, update selection
            SegmentConnectionPoint SCP = hitObj.GetComponent<SegmentConnectionPoint>();
            if (SCP != null)
            {
                if (SCP != SelectedConnectionPoint)
                {
                    if (SelectedConnectionPoint != null)
                        SelectedConnectionPoint.OnDeselection();
                    SelectedConnectionPoint = SCP;
                    SelectedConnectionPoint.OnSelection();
                    return SCP.gameObject.name; // early out
                }
            }
            else
            {
                if (SelectedConnectionPoint != null)
                {
                    SelectedConnectionPoint.OnDeselection();
                    SelectedConnectionPoint = SCP;
                }
            }

            // If a ComponentSegment was hit, update selection
            ItemPartSegment NewSelection = hitObj.GetComponent<ItemPartSegment>();
            if (NewSelection != CurrentSelection)
            {
                if (CurrentSelection != null)
                {
                    CurrentSelection.OnDeselect();
                }
                if (NewSelection != null)
                {
                    NewSelection.OnSelect();
                    retVal = NewSelection.gameObject.name;

                    // update camera target location to be the transform of the 
                    // selected object
                    CamControl.SetLookAtPosition(NewSelection.transform.position); // TODO: NOT WORKING? Fix ASAP
                }
                CurrentSelection = NewSelection;

                // Auto-update connection point selection
                AutoUpdateConnectionPointSelection();
            }
        }
        // Else, nothing was hit so, clear selections
        else
        {
            if (SelectedConnectionPoint != null)
            {
                SelectedConnectionPoint.OnDeselection();
                SelectedConnectionPoint = null;
            }
            if (CurrentSelection != null)
            {
                CurrentSelection.OnDeselect();
                CurrentSelection = null;
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
        if (SelectedConnectionPoint != null)
            SelectedConnectionPoint.OnDeselection();
        if (CurrentSelection != null)
        {
            SelectedConnectionPoint = CurrentSelection.GetNextOpenConnectionPoint();
            if (SelectedConnectionPoint != null)
                SelectedConnectionPoint.OnSelection();
        }
    }

    /* OutputDebugComparisonInfo()
     * Helper method for outputing and comparing the vertex, triangle, 
     * and normal counts for the final mesh that was generated and all
     * of the segments.
     */
    private void OutputDebugComparisonInfo(GameObject FinalizedComponent)
    {
        Debug.Log("Number of Segments: " + Segments.Count);
        int segTotalVerts = 0;
        int segTotalTris = 0;
        int segTotalNorms = 0;
        for (int i = 0; i < Segments.Count; i++)
        {
            Mesh segMesh = Segments[i].gameObject.GetComponent<MeshFilter>().mesh;
            segTotalVerts += segMesh.vertices.Length;
            segTotalTris += segMesh.triangles.Length;
            segTotalNorms += segMesh.normals.Length;
        }
        Debug.Log("Total Vertices in All Segments: " + segTotalVerts);
        Debug.Log("Total Triangles in All Segments: " + segTotalTris);
        Debug.Log("Total Normals in All Segments: " + segTotalNorms);

        Mesh finalMesh = FinalizedComponent.gameObject.GetComponent<MeshFilter>().mesh;
        Debug.Log("Total Vertices in Result Mesh: " + finalMesh.vertices.Length);
        Debug.Log("Total Triangles in Result Mesh: " + finalMesh.triangles.Length);
        Debug.Log("Total Normals in Result Mesh: " + finalMesh.normals.Length);
    }

    private bool CursorIsOverUIPanel()
    {
        Vector3 pos = Input.mousePosition;
        Rect panelCorners = RectTransformUtility.PixelAdjustRect(UIPanel, canvas);
        if (pos.x > panelCorners.xMin + UIPanel.anchoredPosition.x
            && pos.x < panelCorners.xMax + UIPanel.anchoredPosition.x
            && pos.y > panelCorners.yMin + UIPanel.anchoredPosition.y
            && pos.y < panelCorners.yMax + UIPanel.anchoredPosition.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
