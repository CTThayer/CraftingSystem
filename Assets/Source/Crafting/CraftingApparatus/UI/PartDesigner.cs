using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDesigner : MonoBehaviour
{
    // Manually Configured Fields
    [SerializeField] private PartCraftingApparatus _craftingApparatus;
    public PartCraftingApparatus craftingApparatus { get => _craftingApparatus; }

    [SerializeField] private PartDesignerUIController _uiController;
    public PartDesignerUIController uiController { get => _uiController; }

    [SerializeField] private ItemPartAssembler partAssembler;
    [SerializeField] private PartDesignDatabase designDB;
    [SerializeField] private Transform defaultAddLocation;
    [SerializeField] private Material defaultMat;
    [SerializeField] private float MaxLength;

    // Public Variables
    public ItemPartSegment CurrentSelection;
    public SegmentConnectionPoint SelectedConnectionPoint;
    public string partType;

    // Private Variables
    private GameObject ComponentToAdd;
    private bool isFirstSegment;
    private List<ItemPartSegment> segments = new List<ItemPartSegment>();
    private GameObject result;

    /* Start
     * Start is called before the first frame update
     */
    void Start()
    {
        Debug.Assert(_craftingApparatus != null);
        Debug.Assert(_uiController != null);
        Debug.Assert(partAssembler != null);
        Debug.Assert(defaultAddLocation != null);
        Debug.Assert(defaultMat != null);
        Debug.Assert(MaxLength > 0);

        isFirstSegment = true;

        if (_uiController.isInitialized)
        {
            //ComponentToAdd = _uiController.segmentSelector.GetObjectFromDropdown(0);
        }
        else
        {
            _uiController.Initialize();
            //_uiController.OnDesignFamilySelection(0);
            //ComponentToAdd = _uiController.segmentSelector.GetObjectFromDropdown(0);
        }
    }

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

    /* AddPartSegment()
     * Public facing method for adding a part at the currently selected 
     * connection point.
     * TODO: Consider combining with AddSegment() since there really isn't any
     * reason to hid the method like this. Also consider making it return string
     */
    public void AddPartSegment()
    {
        string output = AddSegment();
        _uiController.SetOutputText(output);
    }

    /* AddComponentSegment()
     * Adds the chosen ComponentSegment to the workspace (and Segments list) at
     * the currently selected segment/add location. This also validates a number
     * of conditions and returns a string detailing success or any errors.
     */
    private int count = 0;
    private string AddSegment()
    {
        if (ComponentToAdd == null)
            return "Cannot add segment because no segment is selected.";
        if (isFirstSegment)
        {
            // NOTE: If base-segments are used then the first segment needs to
            // be ensured to be a base segment at this point.

            GameObject firstSeg = Instantiate(ComponentToAdd.gameObject,
                                              defaultAddLocation.position,
                                              defaultAddLocation.rotation);
            firstSeg.name = "Segment0";
            isFirstSegment = false;
            segments.Add(firstSeg.GetComponent<ItemPartSegment>());
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
                    segments.Add(newSegment);

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
        CurrentSelection = segments[segments.Count - 1].GetComponent<ItemPartSegment>();
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
            int index = segments.IndexOf(CurrentSelection);
            segments.Remove(CurrentSelection);
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

    /* Assemble Part Design
     * Attemtps to assemble the current segment arrangement into a new part 
     * design. 
     */
    public bool AssemblePartDesign()
    {
        ItemPartSegment[] segs = segments.ToArray();
        if (partAssembler.ValidateSegmentConfiguration(segs, MaxLength))
        {
            // Weld the mesh and add it to the result object
            Mesh m = partAssembler.WeldSegmentMeshes(segs);
            result = new GameObject();
            result.transform.position = _craftingApparatus.buildLocation.transform.position;
            MeshFilter mf = result.AddComponent<MeshFilter>();
            mf.mesh = m;

            // Set the part material to the default material
            Renderer r = result.AddComponent<Renderer>();
            r.material = defaultMat;
            
            // Set up a compound collider for this part
            GameObject[] colliderObjs = partAssembler.GetCompoundColliderFromSegments(segs);
            foreach (GameObject go in colliderObjs)
            {
                go.transform.parent = result.transform;
            }
            Rigidbody rigidbody = result.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            ClearSegments();
            return true;
        }
        result = null;
        return false;
    }

    /* Add Result To Part Database
     * Adds the result GameObject to the part design database and outputs to the
     * UI output text based on the success/failure of the operation.
     */
    public void AddResultToPartDatabase()
    {
        bool b = designDB.AddPartDesignToDatabase(result, _uiController.partType);
        if (b)
            _uiController.SetOutputText("Part design added to batabase.");
        else
            _uiController.SetOutputText("Could not add part design to database");
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

    /* Clear Segments
     * Clears all the current segments that are in the workspace (in the 
     * segments array). This should be used whenever an item is created or when
     * exiting the PartDesigner.
     */
    private void ClearSegments()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            Destroy(segments[i]);
        }
        segments.Clear();
    }

    /* OutputDebugComparisonInfo()
     * Helper method for outputing and comparing the vertex, triangle, 
     * and normal counts for the final mesh that was generated and all
     * of the segments.
     */
    private void OutputDebugComparisonInfo(GameObject FinalizedComponent)
    {
        Debug.Log("Number of Segments: " + segments.Count);
        int segTotalVerts = 0;
        int segTotalTris = 0;
        int segTotalNorms = 0;
        for (int i = 0; i < segments.Count; i++)
        {
            Mesh segMesh = segments[i].gameObject.GetComponent<MeshFilter>().mesh;
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
}
