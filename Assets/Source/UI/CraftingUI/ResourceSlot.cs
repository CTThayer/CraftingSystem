using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MaterialDelegate(CraftingMaterial resourceMaterial, int index);

public class ResourceSlot : ItemSlot
{
    [SerializeField] private string[] _allowedResourceTypes;
    public string[] allowedResourceTypes { get => _allowedResourceTypes; }

    [SerializeField] private CraftingMaterial _resourceMaterial;
    public CraftingMaterial resourceMaterial { get => _resourceMaterial; }

    public MaterialDelegate OnResourceAddedCallback;
    public MaterialDelegate OnResourceRemovedCallback;

    private Material originalMaterial;

    private List<Storable> resources = new List<Storable>();
    private int maxCount = 64;

    public override bool CanReceiveItem(Storable storableObject)
    {
        if (storableObject == null)     // I guess??
            return true;

        if (resources.Count == 0)
        {
            CraftingResource resource = storableObject as CraftingResource;
            return resource != null && ResourceIsAllowedType(resource);
        }
        else if (resources.Count > 0 && resources.Count < maxCount)
        {
            CraftingResource resource = storableObject as CraftingResource;
            CraftingResource first = resources[0] as CraftingResource;
            return resource != null
                    && resource.resourceType == first.resourceType;
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
                {
                    storedItem = storableObject;
                    _resourceMaterial = resource.craftingMaterial;
                    OnResourceAddedCallback(_resourceMaterial, index);
                }
                resources.Add(storableObject);
            }
        }
        else
        {
            if (storedItem != null || resources.Count > 0)
            {
                Debug.LogError("Attempting to add a null resource to a " +
                               "non-empty resource slot. Resource slots " +
                               "should always be emptied before adding.");
            }
        }
    }

    public override Storable RemoveFromSlot()                                   // TODO: I think this needs to remove ALL things in slot.
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
            OnResourceRemovedCallback(resourceMaterial, index);
            resources.RemoveAt(0);
            storedItem = null;
        }
        return s;
    }

    public List<Storable> RemoveAllFromSlot()
    {
        List<Storable> copy = new List<Storable>(resources);
        resources.Clear();
        return copy;
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

    public int GetResourceCountMax()
    {
        return maxCount;
    }

    public string GetCurrentResourceType()
    {
        CraftingResource cr = resources[0] as CraftingResource;
        return cr.resourceType;
    }

    public bool ResourceIsAllowedType(CraftingResource resource)
    {
        for (int i = 0; i < _allowedResourceTypes.Length; i++)
        {
            if (resource.resourceType == _allowedResourceTypes[i])
                return true;
        }
        return false;
    }

    public void SetAllowedResourceTypes(string[] allowedTypes)
    {
        if (allowedTypes != null && allowedTypes.Length > 0)
        {
            _allowedResourceTypes = allowedTypes;
        }
    }

    /**************************** EDITOR FUNCTIONS ****************************/
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    /************************** END EDITOR FUNCTIONS **************************/
}
