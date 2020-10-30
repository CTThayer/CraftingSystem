using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingApparatus : MonoBehaviour
{
    /************************* User Configured Fields *************************/

    // Is this apparatus is used to make parts, items, or both?
    [SerializeField] private bool isPartBuilder;                                // TODO: Consider using custom editor script to dynamically hide/unhide the supported 
    [SerializeField] private bool isItemBuilder;                                // types and other part/item specific data based on which of these is checked.
                                                                                // See: https://answers.unity.com/questions/192895/hideshow-properties-dynamically-in-inspector.html
    /* Supported Type Fields
     * These govern what types of items/parts this crafting apparatus can make
     * and what types of materials it can use to make them.
     */
    [SerializeField] private string[] _supportedItemTypes;
    public string[] supportedItemTypes { get; private set; }

    [SerializeField] private string[] _supportedPartTypes;
    public string[] supportedPartTypes { get; private set; }

    [SerializeField] private string[] _supportedMaterialTypes;                  // TODO: Should this be an Enum?
    public string[] supportedMaterialTypes { get; private set; }


    /* Required Skill Info
     * These fields are used to specify what type of skill this apparatus 
     * requires and the minimum level in that skill that is required to use it.
     */
    [SerializeField] private string _skillType;
    public string skillType { get; private set; }

    [SerializeField] private float _requiredSkillLevel;
    public float requiredSkillLevel { get; private set; }


    /* Use Time Modifier
     * The base multiplier for how long it takes to use this apparatus. This is
     * used in the calculations of how long it takes to craft an item.
     */
    [SerializeField] private float _usageTimeModifier;
    public float usageTimeModifier { get; private set; }

    // TODO: Add UI Manager Reference(s)
    // TODO: Add DesignRequirements / PartRequirements database reference(s) for retrieving requirements
    // TODO: Add ItemFactory Reference (if necessary)
    // TODO: Add PartFactory Reference (if necessary)
    //      TODO: Do these need a reference to a procedural texturing controller of some kind? Or will all of that be handled in crafting material?
    // TODO: Add required tools field AND tool check methods
    // TODO: Add a spawn location field for where items/parts are created when finished
    //      TODO: Do we need an option to drop items/parts directly into player inventory??
    // TODO: Add CraftingResource input fields??

    /*********************** END User Configured Fields ***********************/

    // Internal fields
    private PartRequirements selectedPartReqs;
    private DesignRequirements selectedDesignReqs;
    private GameObject resultObject;                                            // TODO: Is this necessary? Or will factory scripts take care of this?


    // Start is called before the first frame update
    void Start()
    {
        // TODO: Validate field values
        // TODO: Check references
        // TODO: Verify components (e.g. has an Interactable component, etc.)
    }


    // TODO: Fill in these stub methods
    public void Use() { }                       // For Interactable to connect to
    public void Craft() { }                     // Method to call when finalizing an item/part.
    public void LoadRequirements() { }          // Loads the selected part/design reqs and updates scene/UI accordingly

        // TODO: Should these be in the UI Manager?
    public void LaunchUI() { }                  // Launch UI for apparatus & hide other UI
    public void ExitUI() { }                    // Closes the UI for the apparatus and reactivates previous UI
    
        // TODO: Should these be in some sort of Input/Camera Manager?
    public void SetCameraToCraftingMode() { }   // Switches the camera to crafting mode (camera scripts should be on the camera and activated/deactivated from here)
    public void ResetCameraMode() { }           // Switches back to previous camera mode (camera scripts should be on the camera and activated/deactivated from here)
    public void SetInputToCraftingMode() { }    // Switches input handling to crafting mode control scheme
    public void ResetInputMode() { }            // Switches back to the previous input mode

    private void PlayerHasRequiredSkill() { }   // Checks if the player trying to use the apparatus has the correct skill for it


}
