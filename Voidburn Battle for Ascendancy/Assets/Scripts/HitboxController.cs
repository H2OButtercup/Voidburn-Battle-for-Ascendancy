using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public float damage = 10f; // Damage dealt by this hitbox

    private Collider hitboxCollider;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider != null)
        {
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false; // Start with the hitbox disabled
        }
    }

    public void Activate()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
        }
    }

    public void Deactivate()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Health opponentHealth = other.GetComponent<Health>();
        if (opponentHealth != null)
        {
            opponentHealth.TakeDamage(damage);

            Deactivate();
        }
    }
}