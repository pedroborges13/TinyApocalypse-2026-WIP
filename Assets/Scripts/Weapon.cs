using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform muzzlePoint;

    [Header("VFX")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private float muzzleFlashDuration;

    [Header("Aim Layer")]
    [SerializeField] private LayerMask aimLayerMask;

    [Header("Pool settings")]
    [SerializeField] private int defaultPoolCapacity;
    [SerializeField] private int maxPoolSize;

    private float fireTime;
    private int currentAmmo;
    private bool isReloading;

    private IObjectPool<Projectile> projectilePool; //The Object Pool that holds the Projectile components

    public bool IsAutomatic => weaponData.IsAutomatic;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => weaponData.MagazineSize;
    public bool IsReloading => isReloading;

    public static event Action<string> OnWeaponEquipped; //Name
    public static event Action<int, int> OnAmmoChanged; //Current, max
    public static event Action<float> OnReloadStart; //Reload time

    void Awake()
    {
        //Pool new ObjectPool<GameObject>(Create, Get, Release, Destroy, false, min, max)
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, GetFromPool, BackToPool, OnDestroyPoolObject, false, defaultPoolCapacity, maxPoolSize);
    }
    void Start()
    {
        currentAmmo = weaponData.MagazineSize;

        //Pistol
        OnWeaponEquipped?.Invoke(weaponData.WeaponName);
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
    }

    public int GetPrice()
    {
        return weaponData.Price;
    }
    public void OnEquip()
    {   
        Cursor.visible = true;

        gameObject.SetActive(true);
        isReloading = false;

        PlayerController.OnWeaponReloaded += Reload;

        OnWeaponEquipped?.Invoke(weaponData.WeaponName);
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
    }

    public void OnUnequip()
    {
        PlayerController.OnWeaponReloaded -= Reload;

        gameObject.SetActive(false);
        StopAllCoroutines(); //Stops reload if player switches weapons
        isReloading = false;
    }

    public void TryShoot()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        if (Time.time >= fireTime)
        {
            Shoot();
            fireTime = Time.time + weaponData.FireInterval;
        }
    }

    void Shoot()
    {
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
        SpawnMuzzleFlash();

        //Plays the sound defined in this weapon's ScriptableObject
        AudioManager.Instance.PlaySound(weaponData.FireSound, muzzlePoint.position);

        //The animations were shaking the rotation too much, so it was necessary to base it on the player's rotation
        Quaternion playerRotation = transform.root.rotation;

        for (int i = 0; i < weaponData.ProjPerShot; i++)
        {
            //Calculate random spread based on weapon data
            float horizontalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            float verticalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(horizontalSpread, verticalSpread, 0);

            //Combine the stable player rotation with the randomized spred
            Quaternion finalRotation = playerRotation * spreadRotation;

            Projectile newProj = projectilePool.Get();

            newProj.transform.position = muzzlePoint.position;
            newProj.transform.rotation = finalRotation;

            newProj.Setup(weaponData);

            /*Instantiate the projectile at the muzzle position, but using the stabilized rotation
            GameObject newProj = Instantiate(weaponData.ProjectilePrefab, muzzlePoint.position, finalRotation);
            newProj.GetComponent<Projectile>().Setup(weaponData);*/
        }
    }

    void Reload()
    {
        if (currentAmmo >= weaponData.MagazineSize) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        OnReloadStart?.Invoke(weaponData.ReloadTime);

        yield return new WaitForSeconds(weaponData.ReloadTime);

        currentAmmo = weaponData.MagazineSize;
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
        isReloading = false;
    }

    void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab  != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);

            Destroy(flash, muzzleFlashDuration);
        }
    }

    //Called when the pool is empty and needs to create a brand new object
    Projectile CreateProjectile()
    {
        //Instantiate the prefab (only happens a few times until the pool is full)
        GameObject projInstance = Instantiate(weaponData.ProjectilePrefab);

        //Get the script and inject the pool reference into the projectile
        Projectile projComponent = projInstance.GetComponent<Projectile>();
        projComponent.SetPool(projectilePool);

        return projComponent;
    }

    //Called when I grab an object from pool using pool.Get()
    void GetFromPool(Projectile proj)
    {
        proj.gameObject.SetActive(true);
    }

    //Called when the projectile is returned to the pool using pool.Release()
    void BackToPool(Projectile proj)
    {
        proj.gameObject.SetActive(false);
    }

    //Called if I try to return an object, but the pool is already full (maxPoolSize)
    void OnDestroyPoolObject(Projectile proj)
    {
        Destroy(proj.gameObject);
    }
}
