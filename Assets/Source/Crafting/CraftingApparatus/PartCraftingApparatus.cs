using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartCraftingApparatus : CraftingApparatus
{
    /* Supported Part Types
     * Governs what types of PARTS this crafting apparatus can make.
     * TODO: Consider making this use an enum instead of strings to simplify data validation.
     */
    [SerializeField] private string[] _supportedPartTypes;
    public string[] supportedPartTypes
    {
        get => _supportedPartTypes;
        private set
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == null || value[i].Length == 0)
                        return;
                }
                _supportedPartTypes = value;
            }
        }
    }

    // TODO: Add PartFactory Reference (if necessary)
    //      TODO: Does this need a ref to a procedural texturing controller of
    //            some kind? Or will that be handled in crafting material?

    // TODO: Add CraftingResource input(s) fields??

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // TODO: Fill in stub methods

    // Loads the reqquirements
    public override void LoadRequirements(GameObject reqsObject)
    {
        DesignRequirements designReqs = reqsObject.GetComponent<DesignRequirements>();
        if (designReqs != null)
        {
            uiManager.partsPanel.LoadPartLayout(designReqs.partLayout.prefabPartLayoutUI);
            reqsObject.transform.position = buildLocation.transform.position;
            reqsObject.transform.rotation = buildLocation.transform.rotation;
        }
        else
        {
            Debug.LogError("ERROR: ItemCraftingApparatus: Supplied " +
                           "requirements are not DesignRequirements and " +
                           "cannot be loaded.");
            return;
        }
    }
    //public override void LoadRequirements(Requirements reqs)
    //{
    //    PartRequirements partReqs = reqs as PartRequirements;
    //    if (partReqs != null)
    //    {
    //                                                                            // TODO: Load requirements here
    //    }
    //    else
    //    {
    //        Debug.LogError("ERROR: PartCraftingApparatus: Supplied " +
    //                       "requirements are not PartRequirements and cannot" +
    //                       "be loaded.");
    //        return;
    //    }
    //}

    // Crafts the current object
    public override void Craft()
    {

    }

    // For use by Interactable
    public override void Use()
    {

    }

    // Exits the apparatus
    public override void Exit()
    {

    }

}
