using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDesignLayout : MonoBehaviour
{
    [SerializeField] private GameObject[] partSlots;
    [SerializeField] private Vector2[] slotPositions;

    void OnValidate()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(partSlots.Length == slotPositions.Length);

        for (int i = 0; i < partSlots.Length; i++)
        {
            PartSlot ps = partSlots[i].GetComponent<PartSlot>();
            if (ps == null)
            {
                Debug.LogError("ItemDesignLayout: UI object does NOT have a " +
                               "PartSlot attached to it!");
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
