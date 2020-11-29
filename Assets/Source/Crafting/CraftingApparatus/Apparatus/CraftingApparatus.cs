using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CraftingSkill
{
    Assembly,
    Blacksmithing,
    Glassblowing,
    Leatherworking,
    Woodworking
}

public abstract class CraftingApparatus : MonoBehaviour, IActionable
{
    /************************* User Configured Fields *************************/

    /* Supported Material Types
     * Governs what types of CRAFTING MATERIALS this crafting apparatus can use
     * TODO: Consider making this use an enum instead of strings to simplify data validation.
     */
    [SerializeField] private string[] _supportedMaterialTypes;
    public string[] supportedMaterialTypes
    {
        get => _supportedMaterialTypes;
        private set
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == null || value[i].Length == 0)
                        return;
                }
                _supportedMaterialTypes = value;
            }
        }
    }

    /* Skill Type
     * Specify the type of skill the player needs in order to use this apparatus 
     * TODO: Consider making this use an enum instead of strings to simplify data validation.
     */
    [SerializeField] private CraftingSkill _skillType;
    public CraftingSkill skillType
    {
        get => _skillType;
        private set { _skillType = value;}
    }

    /* Required Skill Level
     * Governs the amount of skill in _skillType that the player needs in order
     * to use this apparatus.
     */
    [SerializeField] private float _requiredSkillLevel;
    public float requiredSkillLevel
    {
        get => _requiredSkillLevel;
        private set
        {
            if (value > 0)
                _requiredSkillLevel = value;
        }
    }

    /* Use Time Modifier
     * The base multiplier for how long it takes to use this apparatus. This is
     * used in the calculations of how long it takes to craft an item.
     */
    [SerializeField] private float _usageTimeModifier;
    public float usageTimeModifier
    {
        get => _usageTimeModifier;
        private set
        {
            if (value > 0)
                _usageTimeModifier = value;
        }
    }

    // TODO: Add required tools field AND tool check methods

    public Transform spawnLocation;     // Location where the item is spawned at creation time
    public Transform buildLocation;     // Location where the item construction starts
    public GameObject craftingCamera;   // GameObject containing the camera to use with this apparatus. Can be set externally with 

    /*********************** END User Configured Fields ***********************/

    [SerializeField] protected CraftingApparatusUIManager _uiManager;
    public CraftingApparatusUIManager uiManager { get => _uiManager; }

    [SerializeField] protected CraftingViewInputController _inputController;
    public CraftingViewInputController inputController { get => _inputController; }

    [SerializeField] protected CraftingCameraController _camController;
    public CraftingCameraController camController { get => _camController; }

    protected GameObject resultObject;                                            // TODO: Is this necessary? Or will factory scripts take care of this?


    /**************************** Public Functions ****************************/

    public abstract void LoadRequirements(GameObject reqsObject);               // Loads the requirements object
    public abstract void Craft();                                               // Crafts the current object
    public abstract string Use(PlayerCharacter PCC);                            // For Interactable
    public abstract void Exit();                                                // Exits the apparatus

    /**************************** Public Functions ****************************/



    /********************* IActionable Interface Members **********************/
    /* IActionable is used by Interactable to get all actions that can be taken
     * on any given object. These methods MUST be implemented for interaction 
     * to work correctly.                                                     */

    public ActionDelegate[] GetActions()
    {
        ActionDelegate[] actions = new ActionDelegate[1] { Use };
        return actions;
    }

    public string[] GetActionNames()
    {
        string[] actionNames = new string[1] { "Use Crafting Apparatus" };
        return actionNames;
    }

}
