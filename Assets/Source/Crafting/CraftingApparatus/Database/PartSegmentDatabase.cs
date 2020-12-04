using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSegmentDatabase : MonoBehaviour
{
    [SerializeField] private SegmentDesignFamily[] designFamilies;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(designFamilies != null && designFamilies.Length > 0);
    }
    
    public GameObject[] GetSegmentsByPartTypeAndFamily(string partType, string familyName)
    {
        List<SegmentDesignFamily> families = GetDesignFamiliesForPartType(partType);
        for(int i = 0; i < families.Count; i++)
        {
            if (families[i].designFamilyName == familyName)
                return families[i].segmentPrefabs;
        }
        return null;
    }

    public GameObject[] GetSegmentsForDesignFamily(string familyName)
    {
        for (int i = 0; i < designFamilies.Length; i++)
        {
            if (designFamilies[i].designFamilyName == familyName)
                return designFamilies[i].segmentPrefabs;
        }
        return null;
    }

    public List<string> GetDesignFamilyNamesByPartType(string partType)
    {
        List<string> familyNames = new List<string>();
        for (int i = 0; i < designFamilies.Length; i++)
        {
            if (designFamilies[i].partType == partType)
                familyNames.Add(designFamilies[i].designFamilyName);
        }
        return familyNames;
    }

    //private SegmentDesignFamily[] GetDesignFamiliesForPartType(string partType)
    private List<SegmentDesignFamily> GetDesignFamiliesForPartType(string partType)
    {
        List<SegmentDesignFamily> families = new List<SegmentDesignFamily>();
        for (int i = 0; i < designFamilies.Length; i++)
        {
            if (designFamilies[i].partType == partType)
                families.Add(designFamilies[i]);
        }
        return families;
        //return families.ToArray();
    }

}
