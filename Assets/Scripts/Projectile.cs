using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float lifeTime;
    private Rigidbody rb;
    private TrailRenderer trail;
    private MeshRenderer meshRenderer;

    [Header("VFX")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private float vfxDuration;

    //Internal variables to store data from WeaponData
    private float damage;
    private float knockback;
    private float speed;
    private int currentPierce;

    private IObjectPool<Projectile> projPool;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetPool(IObjectPool<Projectile> pool)
    {
        projPool = pool;
    }

    public void Setup(WeaponData data)
    {
        damage = data.Damage;
        knockback = data.KnockbackForce;  
        currentPierce = data.PierceCount;
        speed = data.ProjectileSpeed;

        if (meshRenderer != null) meshRenderer.enabled = true; //Ensures it is visible
        if (trail != null) trail.Clear(); //Clears old trail and avoid drawing lines when teleporting


        //Raycast to check if the bullet is spawning inside the barrier (hence the * 0.5, the check starts slightly behind)
        if (Physics.Raycast(transform.position - transform.forward * 0.65f, transform.forward, out RaycastHit hit, 0.75f, hitLayers))
        {
            transform.position = hit.point;
            ProcessCollision(hit.collider, hit.normal);
            return;
        }

        //Resets visual and physical state for Pooling
        rb.linearVelocity = transform.forward * data.ProjectileSpeed;

        CancelInvoke(nameof(ReturnToPool)); //Cancels previous Invokes to avoid bugs
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.1f) return; //Skips processing if velocity is too low (performance optimisation)

        float travelDistance = speed * Time.fixedDeltaTime;
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, travelDistance, hitLayers))
        {
            transform.position = hit.point; //Moves to the impact point 
            rb.linearVelocity = Vector3.zero;
            ProcessCollision(hit.collider, hit.normal);
        }
    }

    void ProcessCollision(Collider other, Vector3 hitNormal)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EntityStats>().TakeDamage(damage, transform.forward, knockback);

            //VFX
            if (bloodPrefab != null)
            {
                GameObject newBlood = VFXManager.Instance.GetBloodVFX(transform.position, Quaternion.LookRotation(hitNormal));
                newBlood.transform.parent = other.transform;
                //PooledVFX handles "Destroy" (Release) automatically
            }

            if (currentPierce <= 0)
            {
                ReturnToPool();
            }
            else
            {
                currentPierce--; //Loses 1 "pierce" when passing through an enemy
                rb.linearVelocity = transform.forward * speed;
                transform.position += transform.forward * 0.1f; //Prevents the projectile from getting stuck in the hit enemy
            }
        }
        else if (other.CompareTag("ExplosiveBarrel"))
        {
            other.GetComponent<ExplosiveBarrel>().TakeDamage(damage);
            ReturnToPool();
        }
        else if (other.CompareTag("Barrier"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool)); //Cancels the lifetime timer if it's still running (in case it hit something before the time)
        if (projPool != null) projPool.Release(this); //Sends back to the pool
        else Destroy(gameObject); //Destroy as fallback when pool unavailable 
    }
}
