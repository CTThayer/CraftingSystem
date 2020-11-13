using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingApparatusUIManager : MonoBehaviour
{
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
        set { if (value != null) _nameInputField= value; }
    }

    [SerializeField] private InputField _descInputField;
    public InputField descInputField
    {
        get => _descInputField;
        set { if (value != null) _descInputField = value; }
    }

    [SerializeField] private PartPanel _partsPanel;
    public PartPanel partsPanel
    {
        get => _partsPanel;
        set { if (value != null) _partsPanel = value; }
    }

    [SerializeField] private TwoPanelController _panelController;
    public TwoPanelController panelController
    {
        get => _panelController;
        set { if (value != null) _panelController = value; }
    }

    void Awake()
    {
        Debug.Assert(craftingUICanvas != null);
        Debug.Assert(nameInputField != null);
        Debug.Assert(descInputField != null);
        Debug.Assert(partsPanel != null);
    }

    public void ActivateUI()
    {
        if (!craftingUICanvas.enabled)
            craftingUICanvas.enabled = true;
    }

    public void ClearPartsPanel()
    {
        bool partPanelIsA = partsPanel.gameObject == panelController.panelA;
        PartSlot[] partsPanelSlots = partsPanel.partSlots;
        for (int i = 0; i < partsPanelSlots.Length; i++)
        {
            panelController.MoveItemBetweenPanels(partsPanelSlots[i], partPanelIsA);
        }
    }

}
