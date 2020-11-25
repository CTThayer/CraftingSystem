using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingApparatusUIManager : MonoBehaviour
{
    [SerializeField] private CraftingApparatus _craftingApparatus;
    public CraftingApparatus craftingApparatus { get => _craftingApparatus; }

    [SerializeField] private Canvas _craftingUICanvas;
    public Canvas craftingUICanvas
    {
        get => _craftingUICanvas;
        set { if (value != null) _craftingUICanvas = value; }
    }

    [SerializeField] private InputField _nameInputField;
    public InputField nameInputField
    {
        get => _nameInputField;
        set { if (value != null) _nameInputField = value; }
    }

    [SerializeField] private InputField _descInputField;
    public InputField descInputField
    {
        get => _descInputField;
        set { if (value != null) _descInputField = value; }
    }

    [SerializeField] private Button _craftItemButton;
    public Button craftItemButton { get => _craftItemButton; }

    [SerializeField] private TwoPanelController _panelController;
    public TwoPanelController panelController
    {
        get => _panelController;
        set { if (value != null) _panelController = value; }
    }

    [SerializeField] private PartPanel_Test _partsPanel;
    public PartPanel_Test partsPanel
    {
        get => _partsPanel;
        set { if (value != null) _partsPanel = value; }
    }

    [SerializeField] private Text _outputText;
    public Text outputText
    {
        get => _outputText;
    }

    void Awake()
    {
        Debug.Assert(_craftingApparatus != null);
        Debug.Assert(craftingUICanvas != null);
        Debug.Assert(nameInputField != null);
        Debug.Assert(descInputField != null);
        Debug.Assert(_craftItemButton != null);
        Debug.Assert(partsPanel != null);

        //craftItemButton.onClick.AddListener(OnCraftItemButtonClick);

        nameInputField.onEndEdit.AddListener(OnEndNameEdit);
        descInputField.onEndEdit.AddListener(OnEndDescriptionEdit);
    }

    public void ActivateUI()
    {
        if (!craftingUICanvas.enabled)
            craftingUICanvas.enabled = true;
    }

    public void DeactivateUI()
    {
        if (craftingUICanvas.enabled)
            craftingUICanvas.enabled = false;
    }

    public void ClearPartsPanel()
    {
        bool partPanelIsA = partsPanel.gameObject == panelController.panelA;
        PartSlot_Test[] partsPanelSlots = partsPanel.partSlots;
        for (int i = 0; i < partsPanelSlots.Length; i++)
        {
            panelController.MoveItemBetweenPanels(partsPanelSlots[i], partPanelIsA);
        }
    }

    public void DisplayOutputMessage(string output)
    {
        _outputText.text = output;
    }

    public void OnEndNameEdit(string input)
    {
        ItemCraftingApparatus ICA = craftingApparatus as ItemCraftingApparatus;
        if (ICA != null)
        {
            if (input.Length < 80)
            {
                ICA.itemName = input;
                DisplayOutputMessage("");
            }
            else
            {
                DisplayOutputMessage("The entered item name is too long.");
            }
        }
    }

    public void OnEndDescriptionEdit(string input)
    {
        ItemCraftingApparatus ICA = craftingApparatus as ItemCraftingApparatus;
        if (ICA != null)
        {
            if (input.Length < 80)
            {
                ICA.itemDescription = input;
                DisplayOutputMessage("");
            }
            else
            {
                DisplayOutputMessage("The entered item desccription is too long.");
            }
        }
    }

    public void OnCraftItemButtonClick()
    {
        craftingApparatus.Craft();
    }
}
