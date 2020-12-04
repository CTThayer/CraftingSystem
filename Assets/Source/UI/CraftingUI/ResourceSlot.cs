using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSlot : ItemSlot
{
    [SerializeField] private string[] _allowedResourceTypes;
    public string[] allowedResourceTypes { get => _allowedResourceTypes; }

    [SerializeField] private CraftingMaterial _resourceMaterial;
    public CraftingMaterial resourceMaterial { get => _resourceMaterial; }

    private Material originalMaterial;

    private List<Storable> resources = new List<Storable>();
    private int maxCount = 64;

    public override bool CanReceiveItem(Storable storableObject)
    {
        if (storableObject == null)
        { 
            if (resources.Count == 0)
            {
                CraftingResource resource = storableObject.GetComponent<CraftingResource>();
                return resource != null && ResourceIsAllowedType(resource);
            }
            else if (resources.Count > 0 && resources.Count < maxCount)
            {
                CraftingResource resource = storableObject.GetComponent<CraftingResource>();
                CraftingResource first = resources[0].GetComponent<CraftingResource>();
                return resource != null
                       && resource.resourceType == first.resourceType;
            }
        }
        return false;
    }

    public override void AddToSlot(Storable storableObject)
    {
        if (storableObject != null)
        {
            CraftingResource resource = storableObject.gameObject.GetComponent<CraftingResource>();
            if (CanReceiveItem(storableObject))
            {
                if (resources.Count == 0)
                    storedItem = storableObject;
                resources.Add(storableObject);
                _resourceMaterial = resource.craftingMaterial;
            }
        }
    }

    public override Storable RemoveFromSlot()
    {
        Storable s = null;
        if (resources.Count > 1)
        {
            s = resources[resources.Count - 1];
            resources.RemoveAt(resources.Count - 1);
        }
        else if (resources.Count == 1)
        {
            s = resources[0];
            resources.RemoveAt(0);
            storedItem = null;
        }
        return s;
    }

    public bool ConsumeResources(int quantity)
    {
        if (quantity < 0 || quantity > resources.Count)
            return false;
        for (int i = 0; i < quantity; i++)
        {
            Storable s = resources[resources.Count - 1 - i];
            resources.RemoveAt(resources.Count - 1 - i);
            if (s = storedItem)
                storedItem = null;
            Destroy(s.gameObject);
        }
        return true;
    }

    public int GetResourceCountInSlot()
    {
        return resources.Count;
    }

    private bool ResourceIsAllowedType(CraftingResource resource)
    {
        for (int i = 0; i < _allowedResourceTypes.Length; i++)
        {
            if (resource.resourceType == _allowedResourceTypes[i])
                return true;
        }
        return false;
    }

    /**************************** EDITOR FUNCTIONS ****************************/
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    /************************** END EDITOR FUNCTIONS **************************/
}
