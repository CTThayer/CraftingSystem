using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InitialPartCraftingMenu : MonoBehaviour
{
    [SerializeField] private PartCraftingApparatusUIManager apparatusUIController;
    [SerializeField] private Canvas initialMenuCanvas;
    [SerializeField] private Button createNewDesignButton;
    [SerializeField] private Button useExistingDesignButton;

    public UnityAction createNew;
    public UnityAction useExisting;

    private bool _isInitialized = false;
    public bool isInitialized { get => _isInitialized; }

    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitialized)
            Initialize();
    }

    public void Initialize()
    {
        Debug.Assert(apparatusUIController != null);
        Debug.Assert(initialMenuCanvas != null);
        Debug.Assert(createNewDesignButton != null);
        Debug.Assert(useExistingDesignButton != null);

        createNew += apparatusUIController.OnSelectPartDesigner;
        useExisting += apparatusUIController.OnSelectPartCreator;

        createNewDesignButton.onClick.AddListener(createNew);
        useExistingDesignButton.onClick.AddListener(useExisting);
        _isInitialized = true;
    }

}
