using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// TODO: Should the setters validate the data? Is value = 0 acceptable? Is value > 1 acceptable?

[Serializable]
public class ConductivityProperties
{
    [SerializeField] private float _acousticConductivity;
    public float acousticConductivity
    {
        get => _acousticConductivity;
        set => _acousticConductivity = value;
    }

    [SerializeField] private float _electricalConductivity;
    public float electricalConductivity
    {
        get => _electricalConductivity;
        set => _electricalConductivity = value;
    }

    [SerializeField] private float _metaphysicalConductivity;
    public float metaphysicalConductivity
    {
        get => _metaphysicalConductivity;
        set => _metaphysicalConductivity = value;
    }

    [SerializeField] private float _thermalConductivity;
    public float thermalConductivity
    {
        get => _thermalConductivity;
        set => _thermalConductivity = value;
    }
}
