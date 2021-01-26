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

    private bool _isInitialized = false;
    public bool isInitialized { get => _isInitialized; }

    private bool _materialsAreSet = false;
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

    // TODO: Maybe we don't need this method??
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
        float v = storable.physicalStats.volume;
        if (v <= 0)
        {
            // TODO: Add mesh volume calculation here when Mesh Utility is fixed
            Debug.Log("PartCreator: FinishPart: Volume of storable was zero.");
        }
        CraftingMaterial craftingMaterial = _partCreatorUI.resourcePanel.GetResourceMaterialFromSlot(0);
        int numResources = _partCreatorUI.resourcePanel.GetQuantityRequiredForVolume(0, v);
        if (_partCreatorUI.resourcePanel.UseResourcesFromSlot(0, numResources))
        {
            ItemPart partScript = part.GetComponent<ItemPart>();
            if (partScript == null)
                partScript = part.AddComponent<ItemPart>();

            storable.physicalStats.mass = craftingMaterial.density * v;
            partScript.physicalStats.mass = craftingMaterial.density * v;
            partScript.physicalStats.volume = v;
            partScript.craftingMaterial = craftingMaterial;
            partScript.maxDurability = craftingMaterial.baseDurability * v * 1000000f;
            partScript.currentDurability = partScript.maxDurability;
            partScript.partQuality = craftingMaterial.rarity;                   // TODO: Implement actual checks against skills, tools used, etc. once these are in the system.
            
            // TODO: What else need to be done to complete this part?

            return true;
        }
        return false;
    }

    public void ApplyMaterial(CraftingMaterial craftingMat, int index)
    {
        if (craftingMat == null || index < 0)
            return;

        SubstanceGraph sgo =  craftingMat.materialSubstanceGraph;

        GameObject obj = _craftingApparatus.GetLoadedDesign();

        textureFactory.GetVariationAndApplyToObject(sgo, obj);

        _materialsAreSet = true;
    }

    public void RemoveMaterial(CraftingMaterial craftingMat, int index)
    {
        if (index < 0)
            return;

        GameObject obj = _craftingApparatus.GetLoadedDesign();

        textureFactory.ResetObjectToDefaultMaterial(obj);

        _materialsAreSet = false;
    }
}
