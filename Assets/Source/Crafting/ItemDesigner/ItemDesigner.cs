using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDesigner : MonoBehaviour
{
    private ItemFactory itemFactory;

    private DesignRequirements selectedDesignReqs;

    // Camera Controller
    [SerializeField] private CameraController camControl;

    // UI Fields
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform UIPanel;
    [SerializeField] private Text outputText;
    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField descInput;

    private GameObject resultItem;

    void Awake()
    {
        itemFactory = new ItemFactory();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Assert required fields are filled
        
        // Initialize camera controller settings (which functions are enabled)
        
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    // TODO: Move input handling for designer classes into it's own class and
    //       reuse InputHandler_Designer for this class and component designer.
    public void HandleInput()
    {

    }

    public void LoadDesign(DesignRequirements designReqs)
    {
        if (designReqs != null)
            selectedDesignReqs = designReqs;

    }

    public void CraftItem()
    {
        //itemFactory.CreateItemFromParts(selectedDesignReqs,
        //                                ItemPart[] parts,
        //                                string name,
        //                                string description,
        //                                out GameObject resultItem,
        //                                out string outputString)
    }

}
