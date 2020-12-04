using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Substance.Game;

public class CraftingMaterial : MonoBehaviour
{
    /*************************** Descriptors Etc. *****************************/
    [SerializeField] private string _materialType;                              // TODO: Change to Enum??
    public string materialType { get => _materialType; }

    [SerializeField] private string _materialName;
    public string materialName { get => _materialName; }

    [SerializeField] private string _description;
    public string description { get => _description; }

    [SerializeField] private string[] _descriptors;
    public string[] descriptors { get => _descriptors; }
    public string GetDescriptor(int index)
    {
        return _descriptors[index];
    }

    /****************************** Core Stats ********************************/
    [SerializeField] private float _baseDurability;
    public float baseDurability
    {
        get => _baseDurability;
        set => _baseDurability = value > 0 ? value : 0;
    }

    [SerializeField] private float _baseValue;
    public float baseValue
    {
        get => _baseValue;
        set => _baseValue = value > 0 ? value : 0;
    }

    [SerializeField] private float _craftingDifficulty;
    public float craftingDifficulty
    {
        get => _craftingDifficulty;
        set => _craftingDifficulty = value > 0 ? value : 0;
    }

    [SerializeField] private float _density;
    public float density
    {
        get => _density;
        set => _density = value > 0 ? value : 0;
    }

    [SerializeField] private float _rarity;
    public float rarity
    {
        get => _rarity;
        set => _rarity = value >= 0 ? value : 0;
    }

    /************************* Resistance / Weakness **************************/
    public ResistanceProperties resistance;

    /****************************** Conductivity ******************************/
    public ConductivityProperties conductivity;

    public Substance.Game.Substance materialSubstance;
    public Substance.Game.SubstanceGraph materialSubstanceGraph;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /* Initialize Crafting Material
     * Validates and initializes the values of the crafting material properties.
     * This is method is to be used for materials that are created at runtime 
     * and it acts as a sort of "constructor." 
     * NOTE: The order of the values in the float arrays is crucial because this
     * function does NOT have any way of verifying that the values match the
     * properties they are intended for. It simply assumes that the values are
     * in the correct order to match what they are assigned to. Therefore this
     * is primarily designed for internal use by a "CraftingMaterial Factory." 
     */
    public bool InitializeCraftingMaterial(string materialType,
                                           string materialName,
                                           string materialDescription,
                                           string[] materialDescriptors,
                                           float[] coreAtrributes,
                                           float[] resistances,
                                           float[] conductivities)
    {
        if (materialType == null                                             // TODO: Implement checks against acceptable material types OR change to enum
            || materialName == null
            || materialName.Length == 0
            || !ContainsAlphanumericChars(materialName)
            || materialDescription == null
            || materialDescription.Length == 0
            || !ContainsAlphanumericChars(materialDescription)
            || materialDescriptors == null
            || materialDescriptors.Length == 0
            || coreAtrributes.Length != 5
            || resistances.Length != 11
            || conductivities.Length != 4)
        {
            return false;
        }
        else
        {
            // Set string values
            _materialName = materialName;
            _description = materialDescription;
            _descriptors = materialDescriptors;
            // Set core stats
            baseDurability = coreAtrributes[0];
            baseValue = coreAtrributes[1];
            craftingDifficulty = coreAtrributes[2];
            density = coreAtrributes[3];
            rarity = coreAtrributes[4];
            // Set resistances
            resistance.choppingResistance = resistances[0];
            resistance.coldResistance = resistances[1];
            resistance.corrosionResistance = resistances[2];
            resistance.electricalResistance = resistances[3];
            resistance.heatResistance = resistances[4];
            resistance.lightResistance = resistances[5];
            resistance.metaphysicalResistance = resistances[6];
            resistance.piercingResistance = resistances[7];
            resistance.slashingResistance = resistances[8];
            resistance.smashingResistance = resistances[9];
            resistance.wearResistance = resistances[10];
            // Set conductivities
            conductivity.acousticConductivity = conductivities[0];
            conductivity.electricalConductivity = conductivities[1];
            conductivity.metaphysicalConductivity = conductivities[2];
            conductivity.thermalConductivity = conductivities[3];
            return true;
        }
    }

    /* Contains Alphanumeric Chars
     * A helper method for determining where there is at least one alphanumeric
     * character in a given string.
     * @Param: s - a string to check for alphanumeric characters
     * Returns: true if ANY character in the string is alphanumeric, otherwise
     *          it returns false.
     */
    private bool ContainsAlphanumericChars(string s)
    {
        if (s != null)
        {
            char[] chars = s.ToCharArray();
            for (int c = 0; c < chars.Length; c++)
            {
                if ((chars[c] >= 48 && chars[c] <= 57)
                    || (chars[c] >= 65 && chars[c] <= 90)
                    || (chars[c] >= 97 && chars[c] <= 122))
                    return true;
            }
        }
        return false;
    }

}
