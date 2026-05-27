using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class TowerBehaviour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float findTargetInterval;
    [SerializeField] private float rotationSpeed;

    [Header("References")]
    [SerializeField] private TowerData towerData;
    [SerializeField] private Transform turretHead;
    [SerializeField] private Transform muzzlePoint;

    [Header("Pool settings")]
    [SerializeField] private int defaultPoolCapacity;
    [SerializeField] private int maxPoolSize;

    private GameObject currentTarget;
    private float nextFireTime;
    private float searchTimer;

    private IObjectPool<Projectile> projectilePool;

    void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, GetFromPool, BackToPool, OnDestroyPoolObject, false, defaultPoolCapacity, maxPoolSize);
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            //Check if the target was destroyed or disabled
            if (currentTarget == null || !currentTarget.activeInHierarchy)
            {
                currentTarget = null;
                return;
            }

            //If the target dies or moves out of range, lose the lock-on
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange)
            {
                currentTarget = null;
                return;
            }

            RotateTowardsTarget();
            TryShoot();
        }
    }

    void FindTarget()
    {
        searchTimer += Time.deltaTime;

        if (searchTimer >= findTargetInterval)
        {
            searchTimer = 0f; //Reset timer immediately

            Collider[] targetColliders = Physics.OverlapSphere(transform.position, attackRange);
            float closestDistance = float.MaxValue;
            GameObject bestTarget = null;

            foreach (var target in targetColliders)
            {
                if (target.CompareTag("Enemy"))
                {
                    //Calculate distance to find the actual closest enemy
                    float distanceToEnemy = Vector3.Distance(transform.position, target.transform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        closestDistance = distanceToEnemy;
                        bestTarget = target.gameObject;
                    }
                }
            }

            currentTarget = bestTarget; //Locked onto the closest target found.
        }
    }

    void RotateTowardsTarget()
    {
        if (turretHead == null) return;

        //Calculate direction pointing at the target, ignoring the Y axis so the turret doesn't tilt weirdly
        Vector3 targetDirection = currentTarget.transform.position - turretHead.position;
        targetDirection.y = 0f;

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            //Multiply by a -90 degree offsett on the X because the model's natural "forward"position requires an initial X rotation of -90.
            Quaternion correctionOffset = Quaternion.Euler(-75f, 0f, 0f);
            targetRotation = targetRotation * correctionOffset;

            //Smoothly rotation
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + towerData.FireInterval;
        }
    }

    void Shoot()
    {
        if (muzzlePoint == null || towerData == null) return;

        AudioManager.Instance.PlaySound(towerData.FireSound, muzzlePoint.position);

        Projectile newProj = projectilePool.Get();
        newProj.transform.position = muzzlePoint.position;
        newProj.transform.rotation = muzzlePoint.rotation;

        newProj.SetupTower(towerData);
    }

    //Called when the pool is empty and needs to create a brand new object
    Projectile CreateProjectile()
    {
        //Instantiate the prefab (only happens a few times until the pool is full)
        GameObject projInstance = Instantiate(towerData.ProjectilePrefab);

        //Get the script and inject the pool reference into the projectile
        Projectile projComponent = projInstance.GetComponent<Projectile>();
        projComponent.SetPool(projectilePool);

        return projComponent;
    }

    //Called when I grab an object from pool using pool.Get()
    void GetFromPool(Projectile proj) => proj.gameObject.SetActive(true);

    //Called when the projectile is returned to the pool using pool.Release()
    void BackToPool(Projectile proj) => proj.gameObject.SetActive(false);

    //Called if I try to return an object, but the pool is already full (maxPoolSize)
    void OnDestroyPoolObject(Projectile proj) => Destroy(proj.gameObject);


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
