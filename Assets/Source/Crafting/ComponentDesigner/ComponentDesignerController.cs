using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentDesignerController : MonoBehaviour
{
    // Manually Configured Fields
    [SerializeField] private Transform DefaultAddLocation;
    [SerializeField] private GameObject AddLocIndicatorPrefab;
    [SerializeField] private float MaxLength;
    [SerializeField] private Material FinishedComponentMat;

    // Manually Assigned Dependencies
    [SerializeField] private ComponentAssembler CompAssembler;
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
    private List<ComponentSegment> Segments = new List<ComponentSegment>();
    private ComponentSegment CurrentSelection;

    // Replacing Old Add Segment Code with Multi-Connect Code
    // TODO: Replace/Remove AddLocation
    // TODO: Update all methods to use Multi-Connect
    // MULTI CONNECTION Variables
    private int AddLocIndex;
    private int AddLocIndex_MAX;
    //private Transform AddLocation;

    private GameObject AddLocationIndicator;
    private bool isFirstSegment;
    
    // Start is called before the first frame update
    void Start()
    {
        // Ensure manually assigned fields are not null
        Debug.Assert(DefaultAddLocation != null);
        Debug.Assert(AddLocIndicatorPrefab != null);
        Debug.Assert(OutputText != null);
        Debug.Assert(SelectedSegmentText != null);
        Debug.Assert(GameObjectSelector != null);
        Debug.Assert(FinishedComponentMat != null);
        Debug.Assert(CamControl != null);

        // isFirstSegment should always start out as true
        isFirstSegment = true;

        // Set DropdownGameObjectSelector's delegate function to SetComponentToAdd()
        GameObjectSelector.SetDropdownDelegate(SetComponentToAdd);

        //Set ComponentToAdd to match the initial value of the dropdown
        ComponentToAdd = GameObjectSelector.GetObjectFromDropdown(0);

        // Set SelectedConnection to 1 b/c it will usually start w/first index
        AddLocIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    /*************************** Public Methods *******************************/

    /* SetComponentToAdd()
     * Public facing method for setting the ComponentToAdd GameObject reference
     * to point to whatever prefab the user selected from the UI menu(s).
     */
    public void SetComponentToAdd(GameObject go)
    {
        if (go != null)
        {
            ComponentSegment newSeg = go.GetComponent<ComponentSegment>();
            if (newSeg != null)
            {
                ComponentToAdd = go;
                return;
            }
            else
            {
                Debug.Log("ComponentDesignerController: SetComponentToAdd() - Supplied GameObject is not a ComponentSegment.");
                return;
            }
        }
        else
        {
            Debug.Log("ComponentDesignerController: SetComponentToAdd() - Supplied GameObject is null.");
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

    /* RemoveSegment()
     * Public facing method for removing a segment (for use in UI).
     * Removes the currently selected segment.
     */
    public void RemoveSegment()
    {
        if (CurrentSelection != null)
        {
            // Clean-up segment connections.
            CurrentSelection.RemoveConnections();

            // NOTE: Does NOT currently have any functionality for tracking
            // downstream references to objects that were previously connected
            // to this one. This could cause problems with adding segments.

            // Remove from Segments list and destroy the selected GameObject.
            int index = Segments.IndexOf(CurrentSelection);
            Segments.Remove(CurrentSelection);
            Destroy(CurrentSelection.transform.gameObject);

            // Select the object that precedes this one (if it exists).
            if (index > 0)
            {
                CurrentSelection = Segments[index - 1];
                CurrentSelection.OnSelect();
            }

            // NOTE: We are NOT shifting segments down to ensure that
            // everything connects because there is no guarantee that the
            // next segment will be compatible and doing so makes 
            // assumptions about the user's intent. It would also prohibit 
            // simply swapping out a segment. This means that validating 
            // the final segment configuration is CRITICAL!
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
    public void AssembleComponent()
    {
        ComponentSegment[] segArray = Segments.ToArray();
        bool validConfig = CompAssembler.ValidateSegmentConfiguration(segArray, MaxLength);
        if(validConfig)
        {
            // For test purposes, setup a new GameObject with the new mesh 
            // and move it off the side so that it can be viewed.
            //Mesh m = CompAssembler.CombineSegmentGeometry(segArray);
            Mesh m = CompAssembler.WeldSegmentMeshes(segArray);
            GameObject finishedComponent = new GameObject();
            finishedComponent.AddComponent<MeshFilter>();
            MeshRenderer MR = finishedComponent.AddComponent<MeshRenderer>();
            MR.material = FinishedComponentMat;
            finishedComponent.GetComponent<MeshFilter>().mesh = m;
            Vector3 componentDisplayPos = DefaultAddLocation.position;
            componentDisplayPos.x += 0.1f;
            finishedComponent.transform.position = componentDisplayPos;

            // TODO: Mesh should be stored into a component database once this is configured.
        }
        else
        {
            OutputText.text = "Invalid segment configuration. Component can't be constructed.";
            Debug.Log("ComponentDesigner: Segment configuration was not valid.");
        }
    }

    /**************************** Private Methods ******************************/

    /* AddComponentSegment()
     * Adds the chosen ComponentSegment to the workspace (and Segments list) at
     * the currently selected segment/add location. This also validates a number
     * of conditions and returns a string detailing success or any errors.
     */
    private string AddComponentSegment()
    {
        if (isFirstSegment)
        {
            // NOTE: If we decide to use base-segments then we need to ensure
            // that the segment we are adding is a base segment here.
            GameObject firstSeg = Instantiate(ComponentToAdd.gameObject, 
                                              DefaultAddLocation.position, 
                                              DefaultAddLocation.rotation);
            firstSeg.name = "Segment1";
            isFirstSegment = false;
            Segments.Add(firstSeg.GetComponent<ComponentSegment>());
        }
        else
        {
            if (CurrentSelection == null)
            {
                return "An existing ComponentSegment is not selected. ComponentSegment cannot be added.";
            }
            if (ComponentToAdd == null)
            {
                return "No new ComponentSegment to add was selected. ComponentSegment cannot be added.";
            }

            /*************************** OLD CODE *****************************/
            /*          TODO: Remove when Multi-Connect is complete           */

            //if (CurrentSelection.GetConnectionID() != ComponentToAdd.GetComponent<ComponentSegment>().GetConnectionID())
            //{
            //    return "Incompatible ComponentSegments selected! (ConnectionID Mismatch) ComponentSegment cannot be added.";
            //}
            //if (CurrentSelection.GetComponent<ComponentSegment>().SegmentIsEndSegment())
            //{
            //    return "Selected ConnectionSegment being added to is an end segment. Therefore, ComponentSegment cannot be added.";
            //}
            //if (AddLocation == null)
            //{
            //    if (CurrentSelection != null)   // Technically unnecessary??
            //        AddLocation = CurrentSelection.GetOutConnectionPoint();
            //    else
            //        return "No connection location was selected. Therefore, ComponentSegment cannot be added.";
            //}
            //
            //GameObject seg = Instantiate(ComponentToAdd, AddLocation.position, AddLocation.rotation);
            //Segments.Add(seg.GetComponent<ComponentSegment>());
            //seg.name = "Segment" + Segments.Count;

            /************************ END OLD CODE ****************************/

            // TODO: Implement checks to verify that we are not adding a base-
            // segment or end-segment inbetween two normal segments.

            if (!CurrentSelection.HasConnectionAt(AddLocIndex))
            {
                if (CurrentSelection.GetConnectionID(AddLocIndex) == ComponentToAdd.GetComponent<ComponentSegment>().GetConnectionID(0))
                {
                    CurrentSelection.SetConnectionStatusAt(AddLocIndex, true);
                    Transform AddLocation = CurrentSelection.GetConnectionPoint(AddLocIndex).transform;
                    GameObject segObj = Instantiate(ComponentToAdd, AddLocation.position, AddLocation.rotation);
                    ComponentSegment segment = segObj.GetComponent<ComponentSegment>();
                    Segments.Add(segment);
                    segObj.name = "Segment" + Segments.Count;
                    CurrentSelection.AddConnectedSegmentAt(AddLocIndex, segment);
                    segment.AddConnectedSegmentAt(0, CurrentSelection);
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
        CurrentSelection = Segments[Segments.Count - 1].GetComponent<ComponentSegment>();
        CurrentSelection.OnSelect();
        AddLocIndex_MAX = CurrentSelection.GetNumberOfConnections();
        AddLocIndex = (AddLocIndex_MAX > 0) ? 1 : 0;

        return "ComponentSegment was added.";
    }

    /* HandleInput()
     * Helper method for reading/responding to user input. Called
     * from Update() each frame. Deals with mouse click selection,
     * and arrow key input for selecting the AddLocation.
     * 
     * TODO: Consider alternative selection methods for AddLocation.
     */
    private void HandleInput()
    {
        // Manipulate Camera
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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
            if (CurrentSelection != null)
            {
                AddLocIndex_MAX = CurrentSelection.GetNumberOfConnections();
                AddLocIndex = (AddLocIndex_MAX > 0) ? 1 : 0;
            }
            else
            {
                AddLocIndex_MAX = -1;
                AddLocIndex = -1;
            }
        }

        // Update AddLocIndex based on user input
        if (CurrentSelection != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (AddLocIndex + 1 < AddLocIndex_MAX)
                    AddLocIndex++;
                else
                    AddLocIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (AddLocIndex - 1 > 0)
                    AddLocIndex--;
                else
                    AddLocIndex = AddLocIndex_MAX;
            }
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
            ComponentSegment NewSelection = hit.transform.GetComponent<ComponentSegment>();
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
                    // update camera target location to be the transform of the selected object
                    CamControl.SetLookAtPosition(NewSelection.transform.position);
                }
                CurrentSelection = NewSelection;
            }
        }
        return retVal;
    }


    /******************************* OLD CODE *********************************/
    /*              TODO: Remove when Multi-Connect is complete               */

    /* SetAddLocation()
     * Updates the AddLocation and AddLocationIndicator used to
     * display the place where the new segment will be added.
     */
    //private void SetAddLocation(Transform t)
    //{
    //    if (t != null)
    //    {
    //        AddLocation = t;
    //        if (AddLocationIndicator == null)
    //            AddLocationIndicator = Instantiate(AddLocIndicatorPrefab);
    //        AddLocationIndicator.transform.position = t.position;
    //        AddLocationIndicator.transform.rotation = t.rotation;
    //    }
    //    else
    //    {
    //        Destroy(AddLocationIndicator);
    //    }
    //}

    /**************************** END OLD CODE ********************************/

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
        if (   pos.x > panelCorners.xMin + UIPanel.anchoredPosition.x
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
