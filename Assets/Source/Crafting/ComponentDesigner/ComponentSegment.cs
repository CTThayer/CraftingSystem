using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSegment : MonoBehaviour
{
    [SerializeField] private bool IsBaseSegment;
    [SerializeField] private bool IsEndSegment;
    [SerializeField] private float SegmentLength;
    [SerializeField] private float MaxLength;
    [SerializeField] private float MinLength;
    [SerializeField] private float DifficultyModifier;
    [SerializeField] private float DurabilityModifier;  // TODO: Do we need this? It seems useful.

    /* Array of GameObjects representing the location and orientation
     * of the connection point(s) between this segment and any others
     * All segments *should* have at least one connection point and 
     * most non-end segments will have at least two.
     * 0-index item should always be the "IN" connection point.
     */
    [SerializeField] private GameObject[] ConnectionPoints;

    /* Array of strings representing the connection ID of each of the 
     * connection point(s) this segment has. Indices must match the
     * index of the corresponding object in the ConnectionPoint array.
     * Used to validate that the segments can connect properly.
     */
    [SerializeField] private string[] ConnectionIDs;

    /* Array of booleans representing whether the connection point(s) has a
     * component segment attached to it currently. Length MUST equal the number
     * of connection points that this segment has.
     * Used to validate that the segments can connect properly.
     */
    [SerializeField] private bool[] ConnectionPointStatus;

    /* Array of ComponentSegments that are attached to this segment.
     * Max # of connected segments == number of connection points.
     * 0-index item should always be ComponentSegment that comes
     * before this segment in the series.
     * TODO: Remove this if we don't use it. Boolean array works fine for most 
     * cases. This is only useful for potential optimizations of the welding
     * algorithm.
     */
    [SerializeField] private ComponentSegment[] ConnectedSegments;

    // Tracks whether this segment's length be adjusted in a specified direction
    [SerializeField] private bool isLengthAdjustable;

    // Tracking the direction to scale in when length is changed.
    [SerializeField] private Vector3 lengthDir;

    // Stores the original color assigned to this component so that it can be
    // Reverted to this color after highlighting is removed.
    private Color originalColor;
    private Color tempColor;
    // State colors
    private static Color selectionColor = new Color(0.9f, 0.86f, 0.56f, 0.5f);
    private static Color errorColor = new Color(0.7f, 0.25f, 0.25f, 0.500f);

    private bool _isDisconnected;
    public bool isDisconnected
    {
        get => _isDisconnected;
        private set => _isDisconnected = value;
    }

    void Awake()
    {
        // Store original color for deselection/selection highlighting purposes
        // Also set tempColor equal to originalColor to start with.
        originalColor = GetComponent<Renderer>().material.color;
        tempColor = originalColor;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Can't be a base segment and an end segment
        Debug.Assert(!(IsBaseSegment && IsEndSegment));

        // Segment must have a positive value length.
        Debug.Assert(SegmentLength > 0.0f);

        // All connected segments data must be non-null & use same length arrays
        Debug.Assert(ConnectionPoints != null && ConnectionPoints.Length > 0);
        Debug.Assert(ConnectionIDs != null && ConnectionPointStatus != null);
        Debug.Assert(   ConnectionPoints.Length == ConnectionPointStatus.Length
                     && ConnectionPoints.Length == ConnectionIDs.Length);

        // ComponentSegment must have a mesh on the GameObject it is assigned to
        Debug.Assert(this.gameObject.GetComponent<MeshFilter>().mesh != null);

        // MaxLength and MinLength must be positive and MaxLength > MinLength
        Debug.Assert(   MaxLength > 0.0f 
                     && MinLength > 0.0f 
                     && MaxLength > MinLength);

        // Length Direction should be normalized
        if (lengthDir.magnitude != 1)
            lengthDir = lengthDir.normalized;


    }

    /******************************** Getters *********************************/
    // Get segment information/data
    public bool SegmentIsEndSegment() { return IsEndSegment; }
    public bool SegmentIsBaseSegment() { return IsBaseSegment; }
    public float GetSegmentLength() { return SegmentLength; }
    public float GetDifficultyModifier() { return DifficultyModifier; }
    public float GetDurabilityModifier() { return DurabilityModifier; }
    
    // Get ALL connection info
    public int GetNumberOfConnections() { return ConnectionPoints.Length - 1; }
    public GameObject[] GetConnectionPoints() { return ConnectionPoints; }
    public string[] GetConnectionIDs() { return ConnectionIDs; }
    public bool[] GetAllConnectionStatuses() { return ConnectionPointStatus; }
    public ComponentSegment[] GetConnectedSegments() { return ConnectedSegments; }

    // Get single connection info
    public GameObject GetConnectionPoint(int index) { return ConnectionPoints[index]; }
    public string GetConnectionID(int index) { return ConnectionIDs[index]; }
    //public bool HasConnectionAt(int index) { return ConnectionPointStatus[index]; }
    public bool HasConnectionAt(int index) { return ConnectedSegments[index] != null; }
    public ComponentSegment GetConnectedSegment(int index) { return ConnectedSegments[index]; }

    public int GetIndexOfConnectionPoint(GameObject cPoint)
    {
        for (int i = 0; i < ConnectionPoints.Length; i++)
        {
            if (ConnectionPoints[i] == cPoint)
                return i;
        }
        return -1;
    }

    /****************************** END Getters *******************************/


    /******************************** Setters *********************************/

    public void SetConnectionStatusAt(int index, bool status)
    {
        if (index >= 0 && index < ConnectionPointStatus.Length)
            ConnectionPointStatus[index] = status;
        else
            Debug.LogError("ComponentSegment: SetConnectionStatusAt(): " +
                           "Index Out of Bounds");
    }

    /****************************** END Setters *******************************/



    /********************** Connected Segment Management **********************/

    /* Add Connected Segment At
     * If the specified index is in bounds, the specified segment is added to
     * the ConnectedSegments array. If it isn't, an error is logged.
     * 
     * TODO: do we need additional bad data checking (i.e. should we check if
     * the connection is null before adding a new one or are "raw swaps" ok?
     */
    public void AddConnectedSegmentAt(int index, ComponentSegment SegToAdd)
    {
        if (index >= 0 && index < ConnectedSegments.Length)
        {
            ConnectedSegments[index] = SegToAdd;
        }
        else
        {
            Debug.LogError("ComponentSegment: AddConnectedSegmentAt(): " +
                           "Index Out of Bounds.");
        }
    }

    /* Remove Segment Connection At
     * Sets the ComponentSegment reference at the specified index of this 
     * segment's ConnectedSegments array to be NULL (if the index is in bounds).
     * Logs an error if the index is out of bounds and changes nothing.
     */
    public void RemoveSegmentConnectionAt(int index)
    {
        if (index >= 0 && index < ConnectedSegments.Length)
        {
            // Remove connection at specified index
            ConnectedSegments[index] = null;

            // If connection from "parent segment" is removed, flag this segment
            // as disconnected from the rest of the component.
            if (index == 0)
                OnDisconnection();
        }
        else
        {
            Debug.LogError("ComponentSegment: RemoveConnectedSegmentAt(): " +
                           "Index Out of Bounds.");
        }
    }

    /* Remove Connected Segment
     * Searches for the specified ComponentSegment reference in this segment's
     * ConnectedSegments array and sets it to NULL (if it exists in the array),
     * then returns true. If the segment isn't found, it logs an error and 
     * returns false.
     */
    public bool RemoveConnectedSegment(ComponentSegment segToRemove)
    {
        for (int i = 0; i < ConnectedSegments.Length; i++)
        {
            if (ConnectedSegments[i] == segToRemove)
            {
                ConnectedSegments[i] = null;
                return true;
            }
        }
        Debug.LogError("ComponentSegment: RemoveConnectedSegmentAt(): " +
                       "Specified segment was not found in ConnectedSegments.");
        return false;
    }

    /* Clear Connections
     * Used to clean up this ComponentSegment's references and any references to
     * it when it is being removed.
     * Returns an array of ComponentSegments containing the "downstream"
     * segments where the connections to the rest of the segment were broken.
     * NOTE: the returned array only contains the segments that were immediately
     * downstream from this one and NOT all further segments that they connect
     * to. If these segments are needed, use GetAllDownstreamSegments() to 
     * retrieve them.
     */
    public ComponentSegment[] ClearConnections()
    {
        List<ComponentSegment> disconnectedSegments = new List<ComponentSegment>();
        if (ConnectedSegments[0] != null)
        {
            ConnectedSegments[0].RemoveConnectedSegment(this);
            disconnectedSegments.Add(ConnectedSegments[0]);
        }
        for (int i = 1; i < ConnectedSegments.Length; i++)
        {
            if (ConnectedSegments[i] != null)
            {
                ConnectedSegments[i].RemoveSegmentConnectionAt(0);
                disconnectedSegments.Add(ConnectedSegments[i]);
            }
        }
        return disconnectedSegments.ToArray();
    }

    /* Get All Downstream Segments
     * Creates a list of containing the initial segment it is called on and all 
     * other segments that are "downstream" from it (meaning all segments that
     * are connected to the component through this initial segment). This is a 
     * recursive function that uses pre-order traversal to gather the segments.
     * @Param: segments - a ref list of ComponentSegments that is added to as
     * the functions run. (Must be ref because this is where results are stored)
     */
    public void GetAllDownstreamSegments(ref List<ComponentSegment> segments)
    {
        segments.Add(this);
        for (int i = 1; i < ConnectedSegments.Length; i++)
        {
            if (ConnectedSegments[i] != null)
            {
                ConnectedSegments[i].GetAllDownstreamSegments(ref segments);
            }
        }
    }

    /* Get Next Open Connection Point
     * 
     */
    public SegmentConnectionPoint GetNextOpenConnectionPoint()
    {
        for (int i = 0; i < ConnectedSegments.Length; i++)
        {
            if (ConnectedSegments[i] == null)
                return ConnectionPoints[i].GetComponent<SegmentConnectionPoint>();
        }
        return null;
    }

    /******************** END Connected Segment Management ********************/



    /****************************** Highlighting ******************************/

    /* OnSelect()
     * Changes the color of the GameObject to indicate it is currently selected. 
     */
    public void OnSelect()
    {
        GetComponent<Renderer>().material.color = selectionColor;
        ShowConnectionPoints(true);
    }

    /* OnDeselect()
     * Changes the color of the GameObject back to it's original color to 
     * indicate that it has been deselected.
     */
    public void OnDeselect()
    {
        GetComponent<Renderer>().material.color = tempColor;
        ShowConnectionPoints(false);
    }

    /* OnDisconnection()
    * Changes the color of the segment's GameObject when it becomes disconnected
    * from the rest of the component. 
    */
    public void OnDisconnection()
    {
        tempColor = errorColor;
        GetComponent<Renderer>().material.color = errorColor;
        isDisconnected = true;
    }

    /* OnReconnection()
    * Resets the color of the segment's GameObject to it's original color after
    * it has been reconnected to the rest of the segment.
    */
    public void OnReconnection()
    {
        tempColor = originalColor;
        GetComponent<Renderer>().material.color = originalColor;
        isDisconnected = false;
    }

    /* OnPlacementError()
    * Changes the color of this segment's GameObject when it enters an error
    * state (e.g. invalid intersection with another part of the component)
    */
    public void OnPlacementError()
    {
        tempColor = errorColor;
        GetComponent<Renderer>().material.color = errorColor;
    }

    private void ShowConnectionPoints(bool makeVisible)
    {
        for (int i = 0; i < ConnectionPoints.Length; i++)
        {
            if (makeVisible)
            {
                if (ConnectedSegments[i] == null)
                    ConnectionPoints[i].SetActive(true);
                else
                    ConnectionPoints[i].SetActive(false);
            }
            else
            {
                ConnectionPoints[i].SetActive(false);
            }

            //if (ConnectedSegments[i] == null && makeVisible)
            //{
            //    ConnectionPoints[i].SetActive(makeVisible);
            //}
            //else if (ConnectedSegments[i] != null && makeVisible)
            //{
            //    ConnectionPoints[i].SetActive(false);
            //}
            //else
            //{
            //    ConnectionPoints[i].SetActive(makeVisible);
            //}
        }
    }

    /**************************** END Highlighting ****************************/



    /**************************** Length Controls *****************************/
    /* Modify Segment Length By
     * Adds value to the "dominant" length of the segment (as specified
     * by the lengthIsX and lengthIsY fields). Scale of the appropriate
     * dimension is increased/decreased by 'l' if the result falls within
     * the min-max range for this segment. Then all "downstream" segments
     * are updated using OnParentLengthChange(). Also updates SegmentLength.
     * @Param l - float value to scale the segment's length by in the 
     * "length direction"
     * 
     * NOTE: This will only generate correct results if the direction vector
     * is in a cardinal direction (i.e. Vector3.up, <1, 0, 0>, etc.). In order
     * to make it work in an arbitrary direction and still have the segments
     * connect properly, some sort of vertex off-setting would have to be done
     * so that the next segment's connecting vertices would actually line up.
     * It is non-trivial to determine which vertices to change and how to do so
     * meaning it will likely be best to always use basis vectors for direction.
     * 
     * NOTE: This operation will cause distortion by it's very nature so, any
     * segments where distortion is undesireable should be marked as NOT length
     * adjustable (ESPECIALLY multi-connection segments since there is a high
     * probability that their connecting vertices will become warped in a way 
     * that will make them unable to weld properly).
     */
    public void ModifySegmentLengthBy(float lengthAdjustment)
    {
        if (isLengthAdjustable)
        {
            float newLength = SegmentLength * lengthAdjustment;
            if (newLength >= MinLength && newLength <= MaxLength)
            {
                // Adjust this segment's scale in the "length direction"
                Vector3 modifier = lengthDir * lengthAdjustment;
                Vector3 newScale = transform.localScale;
                newScale.x *= modifier.x;
                newScale.y *= modifier.y;
                newScale.z *= modifier.z;
                transform.localScale = newScale;

                // Offset the downstream segments' position by the length change
                for (int i = 1; i < ConnectedSegments.Length; i++)
                {
                    ConnectedSegments[i].OnParentLengthChange(modifier);
                }
            }
        }
    }

    /* On Parent Length Change
     * Moves all "downstream" segments to account for changes to an "upstream" 
     * segment's length. Note: this function should be called once per connected 
     * segment attached to the segment that was altered. All downstream segments 
     * will be handled recursively from there.
     * @Param offsetDelta - amount to shift the segment by to compensate for 
     * parent length change.
     */
    public void OnParentLengthChange(Vector3 offset)
    {
        this.gameObject.transform.position += offset;
        if (ConnectedSegments != null)
        {
            for (int i = 0; i < ConnectedSegments.Length; i++)
            {
                OnParentLengthChange(offset);
            }
        }
    }

    /*************************** END Length Controls **************************/


    
    /**************************** Contact Checking ****************************/

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Segments Touch");

    //    ComponentSegment otherSegment = collision.gameObject.GetComponent<ComponentSegment>();
    //    if (otherSegment == null)
    //        return;
    //    for (int i = 0; i < ConnectedSegments.Length; i++)
    //    {
    //        if (ConnectedSegments[i] == otherSegment)
    //            return;
    //    }
    //    // If we made it this far, this is an unregistered contact
    //    GameObject[] otherConnectionPoints = otherSegment.GetConnectionPoints();
    //    for (int j = 0; j < otherConnectionPoints.Length; j++)
    //    {
    //        for (int k = 0; k < ConnectionPoints.Length; k++)
    //        {
    //            if (otherConnectionPoints[j].transform.position == ConnectionPoints[k].transform.position
    //                && otherConnectionPoints[j].transform.localRotation == ConnectionPoints[k].transform.localRotation
    //                && otherSegment.GetConnectionID(j) == ConnectionIDs[k])
    //            {
    //                // Segments touch and can make a valid connection
    //                ConnectedSegments[k] = otherSegment;
    //                otherSegment.AddConnectedSegmentAt(j, this);
    //            }
    //            else
    //            {
    //                // Segments overlap INCORRECTLY
    //                this.OnPlacementError();
    //                otherSegment.OnPlacementError();
    //                Debug.Log("ComponentSegment: OnCollisionEnter: Segments " +
    //                          "overlap and cannot be connected correctly.");
    //            }
    //        }
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
        
    //}

    public void CheckForContact()
    {
        Vector3 midpoint = this.transform.position;
        midpoint.y = midpoint.y + (SegmentLength / 2);

        Collider[] hitColliders = Physics.OverlapBox(midpoint,
                                                     GetComponent<Collider>().bounds.extents, 
                                                     transform.rotation);
        if (hitColliders != null && hitColliders.Length > 0)
        {
            Debug.Log("ComponentSegment: CheckForContact(): Segment touches " +
                      "other collider(s).");

            // Validate/Invalidate contact with other segments
            for (int i = 0; i < hitColliders.Length; i++)
            {
                // Check if collision is with another ComponentSegment
                ComponentSegment otherSegment = hitColliders[i].gameObject.GetComponent<ComponentSegment>();
                if (otherSegment == null || otherSegment == this)
                    continue;

                // Check if connection to this segment already exists
                bool isExistingConnection = false;
                for (int c = 0; c < ConnectedSegments.Length; c++)
                {
                    if (otherSegment == ConnectedSegments[c])
                    {
                        isExistingConnection = true;
                        break;
                    }
                }
                if (isExistingConnection)
                    continue;

                // Check if object aligns with other object so that they can
                // connect properly.
                GameObject[] otherConnectionPoints = otherSegment.GetConnectionPoints();
                for (int j = 0; j < otherConnectionPoints.Length; j++)
                {
                    for (int k = 0; k < ConnectionPoints.Length; k++)
                    {
                        if (otherConnectionPoints[j].transform.position == ConnectionPoints[k].transform.position
                            && otherConnectionPoints[j].transform.localRotation == ConnectionPoints[k].transform.localRotation
                            && otherSegment.GetConnectionID(j) == ConnectionIDs[k])
                        {
                            // Segments touch and can make a valid connection
                            ConnectedSegments[k] = otherSegment;
                            otherSegment.AddConnectedSegmentAt(j, this);
                            if (otherSegment.isDisconnected)
                                otherSegment.OnReconnection();
                        }
                    }
                }
            }
            
        }
        return;
    }

    /************************** End Contact Checking **************************/

}
