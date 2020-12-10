using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartCreatorUIController : MonoBehaviour
{
    [SerializeField] private PartCraftingApparatus _craftingApparatus;

    [SerializeField] private PartCreator _partCreator;
    public PartCreator partCreator { get => _partCreator; }

    [SerializeField] private ResourcePanel _resourcePanel;
    public ResourcePanel resourcePanel { get => _resourcePanel; }

    [SerializeField] private StorageUI _storageUI;
    [SerializeField] private PartDesignSelectorMenu _partDesignSelectorMenu;
    [SerializeField] private CraftingTextIOPanel _textIOPanel;
    [SerializeField] private Button _craftPartButton;

    private bool _isInitialized;
    public bool isInitialized { get => _isInitialized; }

    private PartCraftingApparatusUIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }

    public void Initialize(PartCraftingApparatusUIManager manager)
    {
        uiManager = manager;
        Initialize();
    }

    public void Initialize()
    {
        if (uiManager == null)
            Debug.LogError("ERROR: PartCreatorUIController: Initialize() - " +
                           "Could not find UI manager.");

        if (_craftingApparatus == null)
        {
            _craftingApparatus = uiManager.craftingApparatus as PartCraftingApparatus;
        }
        Debug.Assert(_craftingApparatus != null);

        if (_partCreator == null)
        {
            _partCreator = _craftingApparatus.partCreatorObj.GetComponent<PartCreator>();
        }
        Debug.Assert(_partCreator != null);

        if (_resourcePanel == null)
        {
            _resourcePanel = transform.GetComponentInChildren<ResourcePanel>();
        }
        Debug.Assert(_resourcePanel != null);

        if (_storageUI == null)
        {
            _storageUI = transform.GetComponentInChildren<StorageUI>();
        }
        Debug.Assert(_storageUI != null);

        if (_partDesignSelectorMenu == null)
        {
            _partDesignSelectorMenu = transform.GetComponentInChildren<PartDesignSelectorMenu>();
        }
        Debug.Assert(_partDesignSelectorMenu != null);

        if (_textIOPanel == null)
        {
            _textIOPanel = transform.GetComponentInChildren<CraftingTextIOPanel>();
        }
        Debug.Assert(_textIOPanel != null);

        if (_craftPartButton == null)
        {
            Button[] buttons = transform.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.name == "CraftPart_Button")
                    _craftPartButton = button;
            }
        }
        Debug.Assert(_craftPartButton != null);

        _craftPartButton.onClick.AddListener(_craftingApparatus.Craft);

        _isInitialized = true;
    }

    public void LaunchPartCreatorUI()
    {
        _partDesignSelectorMenu.transform.parent.gameObject.SetActive(true);
        _textIOPanel.SetInteractivity(false);
        _craftPartButton.interactable = false;
    }

    public void ActivateBackgroundUI()
    {
        _textIOPanel.SetInteractivity(true);
        _craftPartButton.interactable = true;
        _storageUI.SetStorage(_craftingApparatus.characterUsingApp.inventory);
    }

    //public void LoadResourceSlots(GameObject resourceSlotPrefab, PartRequirements reqs)
    public void LoadResourceSlots(PartRequirements reqs)
    {
        _resourcePanel.Initialize();
        //_resourcePanel.LoadResourceSlots(resourceSlotPrefab, reqs);
        _resourcePanel.LoadResourceSlots(reqs);
        SetResourcePanelSlotCallbacks();
    }

    public void DisplayOutput(string s)
    {
        _textIOPanel.DisplayOutputMessage(s);
    }

    public void SetResourcePanelSlotCallbacks()
    {
        _resourcePanel.SetResourceMaterialCallbacks(_partCreator.ApplyMaterial,
                                                    _partCreator.RemoveMaterial);
    }

    public void OnExit()
    {
        //_partDesignSelectorMenu.gameObject.SetActive(true);
    }

}