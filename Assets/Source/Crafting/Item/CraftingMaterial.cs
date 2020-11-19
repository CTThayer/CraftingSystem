using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMaterial : MonoBehaviour
{
    /****************************** Core Stats ********************************/

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
    /****************************** Core Stats ********************************/



    /************************* Resistance / Weakness **************************/
    // TODO: What is the valid range for these? Can they ever be 0? What about 
    // -1 < values < 1 ? This could cause serious problems if used as a 
    // multiplier. Negative values could be ok if they are being added to 
    // another value instead of multiplied because this would decrement but 
    // multiplying by a negative wouldn't work in most cases. Similarly, 
    // fractional values could be problematic because they might cause something
    // to approach zero but never actually reach it.

    [SerializeField] private float _coldResistance;
    public float coldResistance { get => _coldResistance; } //set; }                               // TODO: should this be allowed to set?

    [SerializeField] private float _corrosionResistance;
    public float corrosionResistance { get => _corrosionResistance; } //set; }                          // TODO: should this be allowed to set?

    [SerializeField] private float _electricalResistance;
    public float electricalResistance { get => _electricalResistance; } //set; }                         // TODO: should this be allowed to set?

    [SerializeField] private float _heatResistance;
    public float heatResistance { get => _heatResistance; } //set; }                               // TODO: should this be allowed to set?

    [SerializeField] private float _impactResistance;                           // TODO: do we actually want this one?
    public float impactResistance { get => _impactResistance; } //set; }                             // TODO: should this be allowed to set?

    [SerializeField] private float _lightResistance;
    public float lightResistance { get => _lightResistance; } //set; }                              // TODO: should this be allowed to set?

    [SerializeField] private float _metaphysicalResistance;
    public float metaphysicalResistance { get => _metaphysicalResistance; } //set; }                       // TODO: should this be allowed to set?

    [SerializeField] private float _slashingResistance;
    public float slashingResistance { get => _slashingResistance; } //set; }                           // TODO: should this be allowed to set?

    [SerializeField] private float _wearResistance;
    public float wearResistance { get => _wearResistance; } //set; }                               // TODO: should this be allowed to set?
    /*********************** END Resistance / Weakness ************************/



    /****************************** Conductivity ******************************/

    [SerializeField] private float _acousticConductivity;
    public float acousticConductivity { get => _acousticConductivity; } // set; }                        // TODO: should this be allowed to set?

    [SerializeField] private float _electricalConductivity;
    public float electricalConductivity { get => _electricalConductivity; } // set; }                      // TODO: should this be allowed to set?

    [SerializeField] private float _metaphysicalConductivity;
    public float metaphysicalConductivity { get => _metaphysicalConductivity; } // set; }                    // TODO: should this be allowed to set?

    [SerializeField] private float _thermalConductivity;
    public float thermalConductivity { get => _thermalConductivity; } // set; }                         // TODO: should this be allowed to set?

    /*************************** END Conductivity *****************************/


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
        if (materialType != null)                                               // TODO: Implement checks against acceptable material types OR change to enum
            _materialType = materialType;
        else
            return false;

        if (materialName != null
            && materialName.Length > 0
            && ContainsAlphanumericChars(materialName))
        {
            _materialName = materialName;
        }
        else
        {
            return false;
        }

        if (materialDescription != null
            && materialDescription.Length > 0
            && ContainsAlphanumericChars(materialDescription))
        {
            _description = materialDescription;
        }
        else
        {
            return false;
        }

        // NOTE: Descriptor values are NOT validated, just assigned
        if (materialDescriptors != null && materialDescriptors.Length > 0)
        {
            _descriptors = materialDescriptors;
        }
        else
        {
            return false;
        }

        if (coreAtrributes.Length != 5)
            return false;
        else
        {
            baseDurability = coreAtrributes[0];
            baseValue = coreAtrributes[1];
            craftingDifficulty = coreAtrributes[2];
            density = coreAtrributes[3];
            rarity = coreAtrributes[4];
        }

        // NOTE: Supplied resistance values are NOT validated, just assigned
        if (resistances.Length != 9)
            return false;
        else
        {
            _coldResistance = resistances[0];
            _corrosionResistance = resistances[1];
            _electricalResistance = resistances[2];
            _heatResistance = resistances[3];
            _impactResistance = resistances[4];
            _lightResistance = resistances[5];
            _metaphysicalResistance = resistances[6];
            _slashingResistance = resistances[7];
            _wearResistance = resistances[8];
        }

        // NOTE: Supplied resistance values are NOT validated, just assigned
        if (conductivities.Length != 4)
            return false;
        else
        {
            _acousticConductivity = conductivities[0];
            _electricalConductivity = conductivities[1];
            _metaphysicalConductivity = conductivities[2];
            _thermalConductivity = conductivities[3];
        }
        return true;
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
