using UnityEngine;

public abstract class ExplosiveBase : MonoBehaviour
{
    [Header("Base Explosion Settings")]
    [SerializeField] protected float radius;
    [SerializeField] protected float damage;
    [SerializeField] protected float knockback;

    protected bool hasExploded = false;

    //Forces each child class to implement its specific effect
    protected abstract GameObject GetVisualEffect();

    public virtual void TriggerExplosion()
    {
        if (hasExploded) return;
        hasExploded = true;

        GameObject vfx = GetVisualEffect();

        if (vfx != null && vfx.TryGetComponent<Explosion>(out Explosion explosion))
        {
            explosion.Setup(radius, damage, knockback);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
