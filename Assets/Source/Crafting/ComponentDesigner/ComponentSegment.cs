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

    [SerializeField] private int ConnectionID;              // Unnecessary?

    /* These transforms are used to ensure the correct orientation 
     * of the segment In connection point may not be necessary if 
     * assumptions are made about the default orientation of the 
     * segment (i.e. the mesh base is always at y = 0 and obj transform
     * is always the origin with mesh oriented towards +y.)
     * These transforms also need to be PARENTED to the GameObject 
     * that the script is attached to so that the transforms are 
     * positioned correctly regardless of GameObject positioning.
     */
    [SerializeField] private Transform InConnectionPoint;   // Unnecessary?
    [SerializeField] private Transform OutConnectionPoint;  // Unnecessary?

    /* Array of GameObjects representing the location and orientation
     * of the connection point(s) between this segment and any others
     * All segments *should* have at least one connection point and 
     * most non-end segments will have at least two.
     * 0-index item should always be the "IN" connection point.
     */
    [SerializeField] private GameObject[] ConnectionPoints;

    /* Array of ComponentSegments that are attached to this segment.
     * Max # of connected segments == number of connection points.
     * 0-index item should always be ComponentSegment that comes
     * before this segment in the series.
     */
    [SerializeField] private ComponentSegment[] ConnectedSegments;


    /* Array of strings representing the connection ID of each of the 
     * connection point(s) this segment has. Indices must match the
     * index of the corresponding object in the ConnectionPoint array.
     * Used to validate that the segments can connect properly.
     */
    [SerializeField] private string[] ConnectionIDs;

    [SerializeField] bool lengthIsX;
    [SerializeField] bool lengthIsY;

    private Color originalColor;

    private void Awake()
    {
        // Store original color (for deselection/selection highlighting purposes).
        originalColor = this.gameObject.GetComponent<Renderer>().material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Can't be a base segment and an end segment
        Debug.Assert(!(IsBaseSegment && IsEndSegment));

        // Segment must have a positive value length.
        Debug.Assert(SegmentLength > 0.0f);

        // ConnectionID must be a value >= zero.
        Debug.Assert(ConnectionID >= 0);

        // Inbound ConnectionPoint must not be null.
        Debug.Assert(InConnectionPoint != null);

        // Outbound ConnectionPoint can only be null if it is an end segment.
        if (!IsEndSegment)
            Debug.Assert(OutConnectionPoint != null);

        // ComponentSegment must have a mesh on the GameObject it is assigned to.
        Debug.Assert(this.gameObject.GetComponent<MeshFilter>().mesh != null);

        // MaxLength and MinLength must be positive and MaxLength > MinLength
        Debug.Assert(MaxLength > 0.0f && MinLength > 0.0f && MaxLength > MinLength);

        // Length adjustment can't be set to more than one axis
        // Set both lengthIsX and lengthIsY to false for length = z;
        Debug.Assert(!(lengthIsX && lengthIsY));
    }

    // Getters
    public bool SegmentIsEndSegment() { return IsEndSegment; }
    public bool SegmentIsBaseSegment() { return IsBaseSegment; }
    public int GetConnectionID() { return ConnectionID; }
    public Transform GetInConnectionPoint() { return InConnectionPoint; }
    public Transform GetOutConnectionPoint() { return OutConnectionPoint; }
    public float GetSegmentLength() { return SegmentLength; }
    
    // Get ALL connection info
    public GameObject[] GetConnectionPoints() { return ConnectionPoints; }
    public string[] GetConnectionIDs() { return ConnectionIDs; }
    public ComponentSegment[] GetConnectedSegments() { return ConnectedSegments; }

    // Get single connection info
    public GameObject GetConnectionPoint(int index) { return ConnectionPoints[index]; }
    public string GetConnectionID(int index) { return ConnectionIDs[index]; }
    public ComponentSegment GetConnectedSegment(int index) { return ConnectedSegments[index]; }

    /* OnSelect()
     * Changes the color of the GameObject to indicate it is currently selected. 
     */
    public void OnSelect()
    {
        this.gameObject.GetComponent<Renderer>().material.color = new Color(0.902f, 0.863f, 0.560f, 0.500f);
    }

    /* OnDeselect()
     * Changes the color of the GameObject back to it's original  
     * color to indicate that it has been deselected.
     */
    public void OnDeselect()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originalColor;
    }

    /* On Parent Length Change
     * Moves all "downstream" segments to account for changes
     * to an "upstream" segment's length. Note: this function
     * should be called once per connected segment attached to
     * the segment that was altered. All downstream segments 
     * will be handled recursively from there.
     * @Param offsetDelta - amount to shift the segment by to 
     * compensate for parent length change.
     */
    public void OnParentLengthChange(Vector3 offsetDelta)
    {
        this.gameObject.transform.position += offsetDelta;
        if (ConnectedSegments != null)
        {
            for (int i = 0; i < ConnectedSegments.Length; i++)
            {
                OnParentLengthChange(offsetDelta);
            }
        }
    }

    /* Modify Segment Length By
     * Adds value to the "dominant" length of the segment (as specified
     * by the lengthIsX and lengthIsY fields). Scale of the appropriate
     * dimension is increased/decreased by 'l' if the result falls within
     * the min-max range for this segment. Then all "downstream" segments
     * are updated using OnParentLengthChange(). Also updates SegmentLength.
     * @Param l - float value to add to the segment's length direction.
     */
    public void ModifySegmentLengthBy(float l)
    {
        Vector3 offset = Vector3.zero;
        if (lengthIsX)
        {
            float newScaleX = this.transform.localScale.x + l;
            if (newScaleX >= MinLength && newScaleX <= MaxLength)
            {
                SegmentLength += l;
                this.transform.localScale = new Vector3(newScaleX, this.transform.localScale.y, this.transform.localScale.z);
                offset = new Vector3(l, 0f, 0f);
            }
        }
        else if (lengthIsY)
        {
            float newScaleY = this.transform.localScale.y + l;
            if (newScaleY >= MinLength && newScaleY <= MaxLength)
            {
                SegmentLength += l;
                this.transform.localScale = new Vector3(this.transform.localScale.x, newScaleY, this.transform.localScale.z);
                offset = new Vector3(0f, l, 0f);
            }
        }
        else
        {
            float newScaleZ = this.transform.localScale.z + l;
            if (newScaleZ >= MinLength && newScaleZ <= MaxLength)
            {
                SegmentLength += l;
                this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, newScaleZ);
                offset = new Vector3(0f, 0f, l);
            }
        }
        // Update downstream segments
        for (int i = 1; i < ConnectedSegments.Length; i++)
        {
            ConnectedSegments[i].OnParentLengthChange(offset);
        }
    }

}
