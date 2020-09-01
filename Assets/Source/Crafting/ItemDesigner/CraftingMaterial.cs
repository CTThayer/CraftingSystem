using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMaterial : MonoBehaviour
{
    [SerializeField] private string MaterialType;   // Change to Enum??

    [SerializeField] private float _density;
    public float density
    {
        get => _density;
        set => _density = value > 0 ? value : 0;
    }

    [SerializeField] private float _baseDurability;
    public float baseDurability
    {
        get => _baseDurability;
        set => _baseDurability = value > 0 ? value : 0;
    }

    [SerializeField] private float _valueModifier;
    public float valueModifier
    {
        get => _valueModifier;
        set => _valueModifier = value > 0 ? value : 0;
    }

    [SerializeField] private float _craftingDifficulty;
    public float craftingDifficulty
    {
        get => _craftingDifficulty;
        set => _craftingDifficulty = value > 0 ? value : 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
