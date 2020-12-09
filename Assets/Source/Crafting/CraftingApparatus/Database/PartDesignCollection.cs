using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class PartDesignCollection
{
    public string partType;
    public string partSubType;
    public List<GameObject> partDesignPrefabs;
    public PartRequirements partReqs;
    public GameObject resourceSlotLayout;
}
