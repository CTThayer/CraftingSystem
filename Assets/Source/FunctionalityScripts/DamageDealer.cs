using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Blunt,
    Chopping,
    Piercing,
    Slashing
}

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private DamageType _damageType;
    public DamageType damageType
    {
        get => _damageType;
        private set => _damageType = value;
    }

    [SerializeField] private float _baseDamage;
    public float baseDamage
    {
        get => _baseDamage;
        private set => _baseDamage = value;
    }

    [SerializeField] private float _damageModifier;
    public float damageModifier
    {
        get => _damageModifier;
        set => _damageModifier = value;
    }

    [SerializeField] private Collider[] _damageDealerColliders;
    public Collider[] damageDealerColliders
    {
        get => _damageDealerColliders;
        set
        {
            if (value != null && value.Length > 0)
                _damageDealerColliders = value;
        }
    }

    // TODO: Add Animator reference to allow this script to affect the animation
    // state when hits occur. For example, it could trigger a rebound animation
    // when this object hits another object of similar hardness.

    // Start is called before the first frame update
    void Start()
    {
        // TODO: This only works if the DamageDealer can be on the PART that 
        // does the damage, however, it likely will not be possible to do this
        // because the script needs to be at the same level as the Rigidbody in
        // order to recieve collider events. This means that the code to gather
        // the colliders for a DamageDealer needs to be moved to another script
        // that is on the part but passes the colliders to DamageDealer on the
        // top-level of the final item where the Rigidbody lives.
        if (damageDealerColliders == null || damageDealerColliders.Length == 0)
        {
            Collider[] topLevel = GetComponents<Collider>();
            List<Collider> colliders = new List<Collider>(topLevel);
            Collider[] nextLevel = GetComponentsInChildren<Collider>();
            colliders.AddRange(nextLevel);
            // TODO: Check for grandchildren, great-grandchildren, etc??

            damageDealerColliders = colliders.ToArray();
        }
        Debug.Assert(damageDealerColliders != null && damageDealerColliders.Length > 0);
        Debug.Assert(baseDamage > 0);
    }

    // TODO: Add on collision enter to apply damage (and possibly update animation state)

    // TODO: Add on collision exit?

    // TODO: Make methods virtual so that we can have different types of damage
    // dealers, i.e. MeleeDamageDealer, RangedDamageDealer, MagicDamageDealer, etc.

    // TODO: Add animator update methods that are called when certain collisions are detected.

}
