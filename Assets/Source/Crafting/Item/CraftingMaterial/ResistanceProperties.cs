using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ResistanceProperties
{
    [SerializeField] private float _choppingResistance;
    public float choppingResistance
    {
        get => _choppingResistance;
        set => _choppingResistance = ValidateValue(value);
    }

    [SerializeField] private float _coldResistance;
    public float coldResistance
    {
        get => _coldResistance; 
        set => _coldResistance = ValidateValue(value);
    }

    [SerializeField] private float _corrosionResistance;
    public float corrosionResistance
    {
        get => _corrosionResistance;
        set => _corrosionResistance = ValidateValue(value);
    }

    [SerializeField] private float _electricalResistance;
    public float electricalResistance
    {
        get => _electricalResistance;
        set => _electricalResistance = ValidateValue(value);
    }

    [SerializeField] private float _heatResistance;
    public float heatResistance
    {
        get => _heatResistance;
        set => _heatResistance = ValidateValue(value);
    }

    [SerializeField] private float _lightResistance;
    public float lightResistance
    {
        get => _lightResistance;
        set => _lightResistance = ValidateValue(value);
    }

    [SerializeField] private float _metaphysicalResistance;
    public float metaphysicalResistance
    {
        get => _metaphysicalResistance;
        set => _metaphysicalResistance = ValidateValue(value);
    }

    [SerializeField] private float _piercingResistance;
    public float piercingResistance
    {
        get => _piercingResistance;
        set => _piercingResistance = ValidateValue(value);
    }

    [SerializeField] private float _slashingResistance;
    public float slashingResistance
    {
        get => _slashingResistance;
        set => _slashingResistance = ValidateValue(value);
    }

    [SerializeField] private float _smashingResistance;
    public float smashingResistance
    {
        get => _smashingResistance;
        set => _smashingResistance = ValidateValue(value);
    }

    [SerializeField] private float _wearResistance;
    public float wearResistance
    {
        get => _wearResistance;
        set => _wearResistance = ValidateValue(value);
    }

    /* Validate Value
     * Validates the supplied value for use as a resistance property.
     * The default value is 0.0f which applies no resistances but also no 
     * weaknesses to the object. Values of 1.0f give the object immunity to all
     * damage of a given type. Negative values give the object weakness to
     * damage of a given type. Currently there is no floor on negative values
     * (aside from the lower limit of floats) BUT, you should be very careful
     * when using weaknesses (negative values). They can cause an item  or part 
     * to break VERY easily if set even slightly too high. You will likely have
     * to tune your values quite a bit to get the desired results.
     */
    private float ValidateValue(float value)
    {
        return value < 1.0f ? value : 0.0f;
    }
}
