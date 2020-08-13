using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSegment : MonoBehaviour
{
    [SerializeField] private bool IsBaseSegment;            // Unnecessary?
    [SerializeField] private bool IsEndSegment;
    [SerializeField] private float SegmentLength;           // Unnecessary?
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
     * most non-end segments will usually have two.
     * 0-index item should always be the "IN" connection point.
     */
    [SerializeField] private GameObject[] ConnectionPoints;

    /* Array of strings representing the connection ID of each of the 
     * connection point(s) this segment has. Indices must match the
     * index of the corresponding object in the ConnectionPoint array.
     * Used to validate that the segments can connect properly.
     */
    [SerializeField] private string[] ConnectionIDs;
    
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


    }

    // Getters
    public bool SegmentIsEndSegment() { return IsEndSegment; }

    public float GetSegmentLength() { return SegmentLength; }
    public int GetConnectionID() { return ConnectionID; }
    public Transform GetInConnectionPoint() { return InConnectionPoint; }
    public Transform GetOutConnectionPoint() { return OutConnectionPoint; }
    
    // Get ALL connection info
    public GameObject[] GetConnectionPoints() { return ConnectionPoints; }
    public string[] GetConnectionIDs() { return ConnectionIDs; }
    // Get single connection info
    public GameObject GetConnectionPoint(int index) { return ConnectionPoints[index]; }
    public string GetConnectionID(int index) { return ConnectionIDs[index]; }

    // OnSelect()
    // Changes the color of the GameObject to indicate it is currently selected.
    public void OnSelect()
    {
        this.gameObject.GetComponent<Renderer>().material.color = new Color(0.902f, 0.863f, 0.560f, 0.500f);
    }

    // OnDeselect()
    // Changes the color of the GameObject back to it's original shade to 
    // indicate that it has been deselected.
    public void OnDeselect()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originalColor;
    }


}
