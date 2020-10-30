using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentConnectionPoint : MonoBehaviour
{
    private float scaleModifier = 1.25f;    // For selection adjustments

    private bool isSelected = false;
    //private bool _isSelected;
    //public bool isSelected
    //{
    //    get => _isSelected;
    //    private set => isSelected = value;
    //}

    private int _indexInConnections;
    public int indexInConnections
    {
        get => _indexInConnections;
        private set => _indexInConnections = value;
    }

    private Color originalColor;
    private static Color selectionColor = new Color(0.8f, 0.6f, 0.4f, 0.5f);

    void Awake()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        ItemPartSegment parentSeg = GetComponentInParent<ItemPartSegment>();
        Debug.Assert(parentSeg != null);

        indexInConnections = parentSeg.GetIndexOfConnectionPoint(this.gameObject);
        Debug.Assert(indexInConnections >= 0);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void OnSelection()
    {
        if (!isSelected)
        {
            // Change color to selected color
            GetComponent<Renderer>().material.color = selectionColor;

            // Increase scale slightly while offsetting collider size.
            transform.localScale *= scaleModifier;
            SphereCollider C = GetComponent<SphereCollider>();
            C.radius /= scaleModifier;

            isSelected = true;
        }
    }

    public void OnDeselection()
    {
        if (isSelected)
        {
            // Reset color
            GetComponent<Renderer>().material.color = originalColor;

            // Reset scale and collider.
            transform.localScale /= scaleModifier;
            SphereCollider C = GetComponent<SphereCollider>();
            C.radius *= scaleModifier;

            isSelected = false;
        }
    }

}
