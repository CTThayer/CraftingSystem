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
    [SerializeField] private PartTypeSelectorMenu _partSelectorMenu;
    [SerializeField] private CraftingTextIOPanel _textIOPanel;
    [SerializeField] private Button _craftPartButton;

    private bool _isInitialized;
    public bool isInitialized { get => _isInitialized; }

    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }

    public void Initialize()
    {
        PartCraftingApparatusUIManager uiManager =
            transform.parent.GetComponent<PartCraftingApparatusUIManager>();

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
            _resourcePanel = transform.parent.GetComponent<ResourcePanel>();
        }
        Debug.Assert(_resourcePanel != null);

        if (_partSelectorMenu == null)
        {
            _partSelectorMenu = transform.parent.GetComponent<PartTypeSelectorMenu>();
        }
        Debug.Assert(_partSelectorMenu != null);

        if (_textIOPanel == null)
        {
            _textIOPanel = transform.parent.GetComponent<CraftingTextIOPanel>();
        }
        Debug.Assert(_textIOPanel != null);

        if (_craftPartButton == null)
        {
            Button[] buttons = transform.parent.GetComponentsInChildren<Button>();
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
        _partSelectorMenu.gameObject.SetActive(true);
        _textIOPanel.SetInteractivity(false);
        _craftPartButton.interactable = false;
    }

    public void ActivateBackgroundUI()
    {
        _textIOPanel.SetInteractivity(true);
        _craftPartButton.interactable = true;
    }

    public void DisplayOutput(string s)
    {
        _textIOPanel.DisplayOutputMessage(s);
    }

}