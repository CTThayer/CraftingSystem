using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Substance.Game;

public class PartCreator : MonoBehaviour
{
    [SerializeField] private PartCraftingApparatus _craftingApparatus;
    public PartCraftingApparatus craftingApparatus { get => _craftingApparatus; }

    [SerializeField] private PartCreatorUIController _partCreatorUI;

    [SerializeField] private TextureVariationFactory textureFactory;

    private bool _isInitialized;
    public bool isInitialized { get => _isInitialized; }

    private bool _materialsAreSet;
    public bool materialsAreSet { get => _materialsAreSet; }

    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitialized)
            Initialize(transform.parent.GetComponent<PartCraftingApparatus>());
    }

    public void Initialize(PartCraftingApparatus apparatus)
    {
        if (apparatus != null)
        {
            _craftingApparatus = apparatus;
            Initialize();
        }
    }

    public void Initialize()
    {
        Debug.Assert(_craftingApparatus != null);

        if (_partCreatorUI == null)
            _partCreatorUI = _craftingApparatus.uiManager.partCreatorUIController;
        Debug.Assert(_partCreatorUI != null);

        if (textureFactory == null)
            textureFactory = GetComponent<TextureVariationFactory>();
        if (textureFactory == null)
            textureFactory = gameObject.AddComponent<TextureVariationFactory>();
        Debug.Assert(textureFactory != null);

        _materialsAreSet = false;

        _isInitialized = true;
    }

    public void OnLoadDesign()
    {
        _materialsAreSet = false;
    }

    public void TransferDesignFromDesigner(GameObject partDesign,
                                           string partType)
    {
        _craftingApparatus.LoadDesign(partDesign);
        _materialsAreSet = false;
    }

    public void ApplyMaterialToPart(GameObject part)
    {
        CraftingMaterial cMat = _partCreatorUI.resourcePanel.GetResourceMaterialFromSlot(0);
        if (cMat != null)
        {
            Substance.Game.SubstanceGraph sgo = cMat.materialSubstanceGraph;
            if (sgo != null)
            {
                textureFactory.GetVariationAndApplyToObject(sgo, part);
                _materialsAreSet = true;
            }
        }
    }

    public bool FinishPart(GameObject part)
    {
        Storable storable = part.GetComponent<Storable>();
        if (storable == null)
            return false;
        float v = storable.objectPhysicalStats.volume;
        int numResources = _partCreatorUI.resourcePanel.GetQuantityRequiredForVolume(0, v);
        if (_partCreatorUI.resourcePanel.UseResourcesFromSlot(0, numResources))
        {
            // TODO: What else need to be done to complete this part?
            return true;
        }
        return false;
    }
}
