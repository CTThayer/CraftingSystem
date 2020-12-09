using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePanel : MonoBehaviour, ISlotPanelIO
{
    private ResourceSlot[] _slots;
    public ResourceSlot[] slots { get => _slots; }

    public TwoPanelController panelController;

    private bool _isInitialized;
    public bool isInitialized { get => _isInitialized; }

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    //void OnValidate()
    //{
    //    Initialize();
    //}

    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }

    public void Initialize()
    {
        if (panelController == null)
            panelController = transform.parent.gameObject.GetComponent<TwoPanelController>();
        
        Debug.Assert(panelController != null);

        if (_slots == null || _slots.Length == 0)
        {
            int childCount = transform.childCount;
            _slots = transform.gameObject.GetComponentsInChildren<ResourceSlot>(true);
        }
        if (_slots != null && _slots.Length > 0)
        {
            ConfigurSlotDelegates();
        }

        _isInitialized = true;
    }

    public void LoadResourceSlots(GameObject resourceSlotsPrefab, PartRequirements partReqs)
    {
        ResourceSlot[] prefabSlots = resourceSlotsPrefab.GetComponentsInChildren<ResourceSlot>();
        if (prefabSlots == null || prefabSlots.Length == 0)
            return;

        //// Clear any old ui objects
        //ClearSlotLayout();

        //// Copy the slots prefab and set
        //GameObject slotsPrefab = Instantiate(resourceSlotsPrefab);
        //slotsPrefab.transform.position = transform.position;
        //slotsPrefab.transform.parent = transform;
        //_slots = prefabSlots;

        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].SetAllowedResourceTypes(partReqs.allowedMaterials);
        }
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
            DestroySlotLayout();
        }
    }

    private void DestroySlotLayout()
    {
        //foreach (Transform t in this.transform)
        foreach (RectTransform t in this.transform)
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

    public ItemSlot CanAdd(ItemSlot input)
    {
        if (input != null)
        {
            CraftingResource inputCR = input.storedItem as CraftingResource;
            if (inputCR != null)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].ResourceIsAllowedType(inputCR))
                    {
                        int count = slots[i].GetResourceCountInSlot();
                        if (count == 0)
                            return slots[i];
                        else
                        {
                            if (inputCR.resourceType == slots[i].GetCurrentResourceType()
                                && count < slots[i].GetResourceCountMax())
                            {
                                return slots[i];
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    public bool SetDelegateActions(Action<ItemSlot>[] delegates)
    {
        if (delegates != null && delegates.Length == 7)
        {
            OnPointerEnterEvent += delegates[0];
            OnPointerExitEvent += delegates[1];
            OnRightClickEvent += delegates[2];
            OnBeginDragEvent += delegates[3];
            OnEndDragEvent += delegates[4];
            OnDragEvent += delegates[5];
            OnDropEvent += delegates[6];
            return true;
        }
        return false;
    }

    public void ConfigurSlotDelegates()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            _slots[i].OnPointerExitEvent += OnPointerExitEvent;
            _slots[i].OnRightClickEvent += OnRightClickEvent;
            _slots[i].OnBeginDragEvent += OnBeginDragEvent;
            _slots[i].OnEndDragEvent += OnEndDragEvent;
            _slots[i].OnDragEvent += OnDragEvent;
            _slots[i].OnDropEvent += OnDropEvent;
        }
    }

    public void SetResourceMaterialCallbacks(MaterialDelegate onAddResource,
                                             MaterialDelegate onRemoveResource)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].OnResourceAddedCallback = onAddResource;
            _slots[i].OnResourceRemovedCallback = onRemoveResource;
        }
    }
}
