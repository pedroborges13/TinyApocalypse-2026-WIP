using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject explosionPrefab;

    //Pools
    private IObjectPool<GameObject> bloodPool;
    private IObjectPool<GameObject> explosionPool;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        bloodPool = new ObjectPool<GameObject>(CreateBlood, GetFromPool, BackToPool, OnDestroyVFX, false, 10, 25);
        explosionPool = new ObjectPool<GameObject>(CreateExplosion, GetFromPool, BackToPool, OnDestroyVFX, false, 5, 15);
    }

    // ----- OBJECT CREATION -----
    GameObject CreateBlood() 
    {
        return Instantiate(bloodPrefab);
    }

    GameObject CreateExplosion()
    {
        return Instantiate(explosionPrefab);    
    }

    // ----- BEHAVIOU METHODS -----
    void GetFromPool(GameObject vfx)
    {
        vfx.SetActive(true);
    }

    void BackToPool(GameObject vfx)
    {
        vfx.SetActive(false);
    }

    void OnDestroyVFX(GameObject vfx)
    {
        Destroy(vfx);
    }

    // ----- PUBLIC ACCESS METHODS -----
    public GameObject GetBloodVFX(Vector3 position, Quaternion rotation)
    {
        GameObject vfx = bloodPool.Get();
        vfx.transform.position = position;
        vfx.transform.rotation = rotation;

        vfx.GetComponent<PooledVFX>().SetPool(bloodPool);
        return vfx;
    }

    public GameObject GetExplosionVFX(Vector3 position)
    {
        GameObject vfx = explosionPool.Get();
        vfx.transform.position = position;

        vfx.GetComponent <PooledVFX>().SetPool(explosionPool);
        return vfx;
    }
}
