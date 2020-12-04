using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingResource : MonoBehaviour
{
    public string resourceType
    {
        get
        {
            if (_craftingMaterial != null)
                return _craftingMaterial.materialType;
            else
                return "NOT SET";
        }
    }

    [SerializeField] private CraftingMaterial _craftingMaterial;
    public CraftingMaterial craftingMaterial { get => _craftingMaterial; }

    [SerializeField] private float _volume;
    public float volume { get => _volume; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_craftingMaterial != null);
        Debug.Assert(_volume > 0);
    }

    private void OnValidate()
    {
        if (_craftingMaterial != null 
            && _craftingMaterial.materialSubstanceGraph != null)
        {
            Material mat = _craftingMaterial.materialSubstanceGraph.material;
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && mat != null)
                renderer.material = mat;
        }
    }

}
