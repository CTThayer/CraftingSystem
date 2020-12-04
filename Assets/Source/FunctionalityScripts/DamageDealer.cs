using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Chopping,
    Cold,
    Corrosion,
    Electrical,
    Heat,
    Light,
    Metaphysical,
    Piercing,
    Slashing,
    Smashing,
    Wear
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
        private set
        {
            if (value != null && value.Length > 0)
                _damageDealerColliders = value;
        }
    }

    // TODO: Add Animator reference to allow this script to affect the animation
    // state when hits occur. For example, it could trigger a rebound animation
    // when this object hits another object of similar hardness.

    // Private variables for tracking the current state
    private bool isDamageDealingAction;
    private bool isInitialHit;
    private Damagable thisDamagable;

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

    public void Initialize()
    {
        // TODO: Generalize the initializer so that it doesn't take parameters
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDamageDealingAction && isInitialHit)
        {
            int contactPoints = collision.contactCount;
            for (int i = 0; i < contactPoints; i++)
            {
                ContactPoint contact = collision.GetContact(i);
                if(ColliderIsDamageDealer(contact.thisCollider))
                {
                    Damagable hitDamagable = collision.collider.GetComponent<Damagable>();
                    if (hitDamagable != null)
                    {
                        DealDamage(hitDamagable);
                        ApplySelfDamage(thisDamagable, hitDamagable);
                        // TODO: Update animation state??
                        isInitialHit = false;
                        break;
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // TODO: Update animation state??
        isInitialHit = true;
    }

    public void Initialize(DamageType dType, float dBase, float dModifier)
    {
        damageType = dType;
        baseDamage = dBase;
        damageModifier = dModifier;
        isDamageDealingAction = false;
        isInitialHit = true;
        thisDamagable = GetComponent<Damagable>();
    }

    public void Initialize(DamageType dType, float dBase, float dModifier, Collider[] colliders)
    {
        Initialize(dType, dBase, dModifier);
        SetDamageDealerColliders(colliders);
    }

    public void SetDamageDealerColliders(Collider[] ddColliders)
    {
        if (ddColliders != null)
        {
            // TODO: check if these colliders actually belong to this gameObject??
            damageDealerColliders = ddColliders;
        }
    }

    // TODO: Fill in these stub methods
    private void DealDamage(Damagable otherObject)
    {
        float damageAmount = baseDamage * damageModifier;
        otherObject.OnDamageDealerHit(damageType, damageAmount);
    }

    private void ApplySelfDamage(Damagable thisObject, Damagable otherObject)
    {
        // TODO: How do we want to handle this?

        //float damageAmount = baseDamage * damageModifier;
        //thisObject.OnDamageDealerHit(damageAmount);
    }


    // NOTE: This might get really expensive if there are a lot of colliders on
    // an item or if it sweeps through something with a lot of colliders. If it
    // becomes an issue, caching damage dealer colliders in a HashTable might be
    // an effective optimization since only the hash of the collider parameter
    // would need to be checked making each contact check O(1)
    private bool ColliderIsDamageDealer(Collider collider)
    {
        for (int i = 0; i < _damageDealerColliders.Length; i++)
        {
            if (collider == _damageDealerColliders[i])
                return true;
        }
        return false;
    }

    // TODO: Add animator update methods that are called when certain collisions are detected.

    // TODO: Make methods virtual so that we can have different types of damage
    // dealers, i.e. MeleeDamageDealer, RangedDamageDealer, MagicDamageDealer, etc.
}
