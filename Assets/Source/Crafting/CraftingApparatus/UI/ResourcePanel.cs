using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePanel : MonoBehaviour
{
    private ResourceSlot[] _slots;
    public ResourceSlot[] slots { get => _slots; }

    public TwoPanelController panelController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadResourceSlots(GameObject resourceSlotsPrefab)
    {
        // Clear any old ui objects
        ClearSlotLayout();

        // Copy the slots prefab and set
        GameObject slotsPrefab = Instantiate(resourceSlotsPrefab);
        slotsPrefab.transform.position = transform.position;
        slotsPrefab.transform.parent = transform;
        _slots = resourceSlotsPrefab.GetComponentsInChildren<ResourceSlot>();
    }

    public CraftingMaterial GetResourceMaterialFromSlot(int index)
    {
        if (index < _slots.Length && _slots[index] != null)
            return _slots[index].resourceMaterial;
        else
            return null;
    }

    public float GetResourceVolumeFromSlot(int index)
    {
        if (index < _slots.Length && _slots[index] != null)
            return _slots[index].storedItem.objectPhysicalStats.volume;
        else
            return -1f;
    }

    public List<CraftingMaterial> GetMaterialsFromResourceSlots()
    {
        List<CraftingMaterial> materials = new List<CraftingMaterial>();
        for (int i = 0; i < _slots.Length; i++)
        {
            materials.Add(_slots[i].resourceMaterial);
        }
        return materials;
    }

    public List<float> GetVolumesFromSlots()
    {
        List<float> volumes = new List<float>();
        for (int i = 0; i < _slots.Length; i++)
        {
            volumes.Add(_slots[i].storedItem.objectPhysicalStats.volume);
        }
        return volumes;
    }

    public void GetSlotContentData(out List<CraftingMaterial> materials,
                                   out List<float> volumes)
    {
        materials = new List<CraftingMaterial>();
        volumes = new List<float>();
        for (int i = 0; i < _slots.Length; i++)
        {
            materials.Add(_slots[i].resourceMaterial);
            volumes.Add(_slots[i].storedItem.objectPhysicalStats.volume);
        }
    }

    public int GetQuantityRequiredForVolume(int slotIndex, float targetVolume)
    {
        int rCount = _slots[slotIndex].GetResourceCountInSlot();
        if (rCount == 0)
            return -1;
        float rVolume = _slots[slotIndex].storedItem.objectPhysicalStats.volume;
        if (rVolume > targetVolume)
            return 1;
        else
            return (int)Mathf.Ceil(targetVolume / rVolume);
    }

    public bool UseResourcesFromSlot(int slot, int numResouces)
    {
        if (slot < _slots.Length)
        {
            if (_slots[slot].GetResourceCountInSlot() >= numResouces)
            {
                _slots[slot].ConsumeResources(numResouces);
                return true;
            }
        }
        return false;
    }



    public bool SlotsAreEmpty()
    {
        for(int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].storedItem != null)
                return false;
        }
        return true;
    }

    private void ClearSlotLayout()
    {
        if (SlotsAreEmpty())
        {
            DestroySlotLayout();
        }
        else
        {
            ClearSlotContents();
        }
    }

    private void DestroySlotLayout()
    {
        foreach (Transform t in this.transform)
        {
            Destroy(t.gameObject);                                          // TODO: Is this how we want to handle this?
        }
    }

    private void ClearSlotContents()
    {
        bool isA = panelController.IsPanelA(this.gameObject);
        for (int i = 0; i < _slots.Length; i++)
        {
            ItemSlot slot = _slots[i] as ItemSlot;
            panelController.MoveItemBetweenPanels(slot, isA);
        }
    }
}
