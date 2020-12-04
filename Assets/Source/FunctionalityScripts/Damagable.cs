using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    // TODO: What data do we need here?? Item ref? ItemPart ref? Local durability/health?

    [SerializeField] private DamageType[] immunities;
    [SerializeField] private DamageType[] resistances;
    [SerializeField] private float[] resistanceModifiers;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(resistances.Length == resistanceModifiers.Length);
    }

    public void Initialize()
    {
        // TODO: Generalize the initializer so that it doesn't take parameters
    }

    public void Initialize(DamageType[] immunitiesArray,
                           DamageType[] resistancesArray,
                           float[] resistanceModsArray)
    {
        if (immunitiesArray != null)
            immunities = immunitiesArray;
        if (resistancesArray.Length == resistanceModsArray.Length)
        {
            resistances = resistancesArray;
            resistanceModifiers = resistanceModsArray;
        }
    }

    // TODO: Fill in these stub methods
    public void OnDamageDealerHit(DamageType dType, float damage)
    {
        if (damage <= 0)
            return;
        if (IsImmune(dType))
            return;
        int resistIndex;
        if (IsResistant(dType, out resistIndex))
            damage -= GetDamageReduction(resistIndex, damage);
        ApplyDamage(damage);
    }
    
    // TODO: Fill in this stub method
    private void ApplyDamage(float damage)
    {

    }

    private float GetDamageReduction(int resistIndex, float damageAmount)
    {
        if (resistIndex >= 0 || resistIndex < resistanceModifiers.Length)
        {
            return resistanceModifiers[resistIndex] * damageAmount;
        }
        return 0.0f;
    }

    private bool IsImmune(DamageType damageType)
    {
        for (int i = 0; i < immunities.Length; i++)
        {
            if (immunities[i] == damageType)
                return true;
        }
        return false;
    }

    private bool IsResistant(DamageType damageType, out int index)
    {
        for (int i = 0; i < resistances.Length; i++)
        {
            if (resistances[i] == damageType)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    // TODO: Fill in this stub method
    private void OnDestruction()
    {

    }

    // TODO: Fill in this stub method
    private void CalculateResistances()
    {

    }

}
