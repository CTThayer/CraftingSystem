using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTextIOPanel : MonoBehaviour
{
    [SerializeField] private CraftingApparatus _craftingApparatus;
    public CraftingApparatus craftingApparatus
    {
        get => _craftingApparatus;
        set
        {
            if (value != null)
                _craftingApparatus = value;
        }
    }
    [SerializeField] private InputField _partNameInputField;
    [SerializeField] private InputField _partDescInputField;
    [SerializeField] private Text _outputText;

    private bool _isInitialized;

    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }

    public void Initialize()
    {
        if (_craftingApparatus == null)
        {
            ItemCraftingApparatusUIManager uiManager = 
                transform.parent.GetComponent<ItemCraftingApparatusUIManager>();
            _craftingApparatus = uiManager.craftingApparatus;
        }
        Debug.Assert(_craftingApparatus != null);

        if (_partNameInputField == null || _partDescInputField == null)
        {
            InputField[] inputFields = GetComponents<InputField>();
            foreach (InputField field in inputFields)
            {
                if (field.gameObject.name == "Name_InputField")
                    _partNameInputField = field;
                else if (field.gameObject.name == "Description_InputField")
                    _partDescInputField = field;
            }
        }
        Debug.Assert(_partNameInputField != null);
        Debug.Assert(_partDescInputField != null);

        if (_outputText == null)
        {
            Text[] textFields = GetComponents<Text>();
            foreach (Text field in textFields)
            {
                if (field.gameObject.name == "Output_Text")
                    _outputText = field;
            }
        }

        _isInitialized = true;
    }
    
    public void OnEndNameEdit(string input)
    {
        if (input != null)
        {
            if (input.Length < 80)
            {
                _craftingApparatus.itemName = input;
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
        if (input != null)
        {
            if (input.Length < 512)
            {
                _craftingApparatus.itemDescription = input;
                DisplayOutputMessage("");
            }
            else
            {
                DisplayOutputMessage("The entered item desccription is too long.");
            }
        }
    }

    public void DisplayOutputMessage(string s)
    {
        if (s != null)
            _outputText.text = s;
    }

    public void SetInteractivity(bool on)
    {
        _partNameInputField.interactable = on;
        _partDescInputField.interactable = on;
    }

    public void ClearTextFields()
    {
        _partNameInputField.text = "";
        _partDescInputField.text = "";
        _outputText.text = "";
    }
}
