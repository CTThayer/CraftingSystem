using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingCam : MonoBehaviour
{
    [SerializeField] private Camera craftingCam;
    
    // Start is called before the first frame update
    void Start()
    {
        if (craftingCam == null)
        {
            Camera cam = GetComponent<Camera>();
            if (cam == null)
                Debug.LogError("ERROR: CraftingCam: No camera was set for the" +
                               " CraftingCam and the parent object does not " +
                               "contain a Camera object!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SetScreenSpace(Vector2 topLeftCorner, Vector2 dimensions)
    {
        if (   dimensions.x > 0.0f && dimensions.x <= 1.0f 
            && dimensions.y > 0.0f && dimensions.y <= 1.0f
            && topLeftCorner.x >= 0.0f && topLeftCorner.x <= 1.0f
            && topLeftCorner.y >= 0.0f && topLeftCorner.y <= 1.0f)
        {
            Rect screenSpace = new Rect(topLeftCorner, dimensions);
            craftingCam.rect = screenSpace;
            return true;
        }
        else
        {
            return false;
        }
    }
}
