using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDesignDatabase : MonoBehaviour
{
    [SerializeField] private PartDesignCollection[] partDesignCollections;

    void Start()
    {
        Debug.Assert(   partDesignCollections != null 
                     && partDesignCollections.Length > 0);

    }

    public GameObject GetPartDesign(string partType, 
                                    string partSubtype, 
                                    string partName)
    {
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (   partDesignCollections[i].partType == partType
                && partDesignCollections[i].partSubType == partSubtype)
            {
                List<GameObject> designs = partDesignCollections[i].partDesignPrefabs;
                for (int j = 0; j < designs.Count; j++)
                {
                    if (designs[j].name == partName)
                        return designs[j];
                }
            }
        }
        return null;
    }

    public GameObject GetPartDesign(string partType,
                                    string partSubtype,
                                    string partName,
                                    out PartRequirements partReqs,
                                    out GameObject resourceSlotPrefab)
    {
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == partType
                && partDesignCollections[i].partSubType == partSubtype)
            {
                List<GameObject> designs = partDesignCollections[i].partDesignPrefabs;
                for (int j = 0; j < designs.Count; j++)
                {
                    if (designs[j].name == partName)
                    {
                        partReqs = partDesignCollections[i].partReqs;
                        resourceSlotPrefab = partDesignCollections[i].resourceSlotLayout;
                        return designs[j];
                    }
                }
            }
        }
        partReqs = null;
        resourceSlotPrefab = null;
        return null;
    }


    public GameObject[] GetDesignsByType(string partType)
    {
        List<GameObject> designs = new List<GameObject>();
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == partType)
                designs.AddRange(partDesignCollections[i].partDesignPrefabs);
        }
        return designs.ToArray();
    }

    public GameObject[] GetDesignsByTypeAndSubType(string partType, 
                                                   string subType)
    {
        List<GameObject> designs = new List<GameObject>();
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == partType
                && partDesignCollections[i].partSubType == subType)
            {
                designs.AddRange(partDesignCollections[i].partDesignPrefabs);
            }
        }
        return designs.ToArray();
    }

    public List<string> GetSubtypesByType(string partType)
    {
        List<string> subtypes = new List<string>();
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == partType)
                subtypes.Add(partDesignCollections[i].partSubType);
        }
        return subtypes;
    }

    public List<string> GetNamesByTypeAndSubtype(string type, string subtype)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == type
                && partDesignCollections[i].partSubType == subtype)
            {
                List<GameObject> partDesigns = partDesignCollections[i].partDesignPrefabs;
                foreach (GameObject go in partDesigns)
                {
                    names.Add(go.name);
                }
            }
        }
        return names;
    }


    public bool AddPartDesignToDatabase(GameObject partDesign,
                                        string partType,
                                        string subType)
    {
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == partType
                && partDesignCollections[i].partSubType == subType)
            {
                partDesignCollections[i].partDesignPrefabs.Add(partDesign);
                return true;
            }
        }
        return false;
    }

    public bool AddPartDesignToDatabase(GameObject partDesign, string partType)
    {
        for (int i = 0; i < partDesignCollections.Length; i++)
        {
            if (partDesignCollections[i].partType == partType)
            {
                partDesignCollections[i].partDesignPrefabs.Add(partDesign);
                return true;
            }
        }
        return false;
    }

}
