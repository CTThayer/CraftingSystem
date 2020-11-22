using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    MeshUtility meshUtil;

    public ItemFactory()
    {
        meshUtil = new MeshUtility();                                           // TODO: Consider making MeshUtility static
    }

    /* Create Item From Parts
     * This method assembles an Item GameObject from an array of ItemParts. The 
     * ItemParts array is validated against the design requirements for this
     * type of item and, if valid, the Item is created using the inputs.
     * @Param: reqs - the design requirements for this type of item.
     * @Param: parts - the array of item parts to be combined into an item. It 
     * is critical that the parts array is passed into this function in the same
     * order that the corresponding parts requirement array is in, otherwise it
     * will fail.
     * @Param: name - a string for the item's name (often user generated)
     * @Param: description - a string for item's description (often user 
     * generated)
     * @Param: resultItem - the out parameter for the resulting gameObject that 
     * is the finalized item.
     * @Param: outputString - the out parameter for any output text that should 
     * be returned and displayed to the user (e.g. errors, success text, etc.)
     * @Return: bool - a boolean for the success or failure of item creation.
     */
    public bool CreateItemFromParts(DesignRequirements reqs,
                                    ItemPart[] parts,
                                    string name,
                                    string description,
                                    out GameObject resultItem,
                                    out string outputString)
    {
        // Validate parts against design requirements
        string validationResult;
        if (reqs.ValidatePartConfig(parts, out validationResult))
        {
            // NOTE: Assumes that manipulators[0] is the primary manipulator
            GameObject itemParent = reqs.manipulators[0];
            itemParent.transform.parent = null;
            Debug.Assert(itemParent.transform.localScale == Vector3.one);

            //Sum part data for item totals
            float totalVolume = 0;
            float totalMass = 0;
            float totalValue = 0;
            for (int p = 0; p < parts.Length; p++)
            {
                parts[p].transform.parent = itemParent.transform;

                Mesh pMesh = parts[p].GetComponent<MeshFilter>().mesh;
                float pVol = meshUtil.CalculateMeshVolume(pMesh);
                totalVolume += pVol > 0f ? pVol : 0;

                float pMass = pVol * parts[p].craftingMaterial.density;
                totalMass += pMass > 0f ? pMass : 0;

                float pVal = parts[p].partQuality * parts[p].craftingMaterial.baseValue;
                totalValue += pVal > 0f ? pVal : 0;
            }

            // Add the Item.cs script to the primaryManipulator gameObject so 
            // that it is attached to the top level of the item's hierarchy
            itemParent.AddComponent<Item>();
            Item item = itemParent.AddComponent<Item>();

            // Initialize item's attributes
            string id = GenerateItemID(itemParent);
            item.Initialize(id,
                            name,
                            description,
                            totalMass,
                            totalVolume,
                            totalValue,
                            reqs.manipulators,
                            parts);

            // Add required component scripts to the item
            AddComponentsToItem(ref itemParent, reqs.requiredScripts);

            // Deactivate components in parts
            PartRequirements[] partReqs = reqs.partLayout.GetPartRequirements();
            for (int i = 0; i < parts.Length; i++)
            {
                // NOTE: Parts need to be caches in Item before doing this.
                DeactivateComponentsOfPart(parts[i].gameObject, 
                                           partReqs[i].componentsToDeactivate);
            }

            // Return results
            resultItem = itemParent;
            outputString = validationResult;
            return true;
        }
        else
        {
            resultItem = null;
            outputString = validationResult;
            return false;
        }
    }

    /* Add Components To Item
     * Adds the specified components to the specified GameObject which should
     * always be the top-level GameObject of the item's hierarchy.
     * @Param: item - a reference to the GameObject for the item
     * @Param: reqComponents - an array of strings corresponding to the type 
     * values of the components (scripts) that should be attached to the item.
     * NOTE: All failures of component additions are silent. Any strings that 
     * are invalid names types will simply be skipped.
     */
    public void AddComponentsToItem(ref GameObject item, string[] reqComponents)
    {
        for (int i = 0; i < reqComponents.Length; i++)
        {
            System.Type componentType = System.Type.GetType(reqComponents[i]);
            if (componentType != null)
                item.AddComponent(componentType);
            // TODO: Handle functionality script initialization here
        }
    }

    /* Deactivate Components Of Part
     * Deactivates the specified components of the specified GameObject 
     * that is a part of the item being created.
     * @Param: part - a reference to the GameObject for the part whose 
     * components need to be disabled.
     * @Param: compsToDeactivate - an array of strings corresponding to the type 
     * values of the components (scripts) that should be attached to the item.
     * NOTE: All failures of component removals are SILENT. Any strings that 
     * are incorrect or invalid names/types will simply be skipped.
     */
    public void DeactivateComponentsOfPart(GameObject part, 
                                           string[] compsToDeactivate)
    {
        if (part == null || compsToDeactivate == null || compsToDeactivate.Length == 0)
            return;
        for (int i = 0; i < compsToDeactivate.Length; i++)
        {
            System.Type componentType = System.Type.GetType(compsToDeactivate[i]);
            if (componentType != null)
            {
                Component component = part.GetComponent(componentType);
                Behaviour behaviour = component as Behaviour;
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
                else if (compsToDeactivate[i] == "Rigidbody")
                {
                    if (component != null)
                        Destroy(component);
                    // NOTE: It might be important to cache rigidbody data in
                    // the ItemPart before doing this in case the rigidbody has
                    // to be recreated later (most data should already be there)
                }
            }
        }
    }

    /* Generate Item ID
     * Returns a unique ID for the supplied item.
     * @Param: item - GameObject of the Item that needs a unique item ID.
     * @Return: unique item ID string.
     * NOTE: THIS IS TEMPORARY
     * TODO: Change this to use a function call to get a unique ID from the 
     * database guaranteeing it is unique for any and all clients. Also consider
     * using an int instead of a string for added efficiency.
     */
    private string GenerateItemID(GameObject item)
    {
        string itemID = "Item-" + item.GetInstanceID().ToString();
        return itemID;
    }

}
