using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject bigExplosionPrefab;
    [SerializeField] private GameObject smallExplosionPrefab;
    [SerializeField] private GameObject kamikazeExplosionPrefab;

    //Pools
    private IObjectPool<GameObject> bloodPool;
    private IObjectPool<GameObject> bigExplosionPool;
    private IObjectPool<GameObject> smallExplosionPool;
    private IObjectPool<GameObject> kamikazeExplosionPool;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        bloodPool = new ObjectPool<GameObject>(CreateBlood, GetFromPool, BackToPool, OnDestroyVFX, false, 10, 25);
        bigExplosionPool = new ObjectPool<GameObject>(CreateBigExplosion, GetFromPool, BackToPool, OnDestroyVFX, false, 5, 15);
        smallExplosionPool = new ObjectPool<GameObject>(CreateSmallExplosion, GetFromPool, BackToPool, OnDestroyVFX, false, 5, 15);
        kamikazeExplosionPool = new ObjectPool<GameObject>(CreateKamikazeExplosion, GetFromPool, BackToPool, OnDestroyVFX, false, 5, 15);

    }

    // ----- OBJECT CREATION -----
    GameObject CreateBlood() => Instantiate(bloodPrefab);
    GameObject CreateBigExplosion() => Instantiate(bigExplosionPrefab);
    GameObject CreateSmallExplosion() => Instantiate(smallExplosionPrefab);
    GameObject CreateKamikazeExplosion() => Instantiate(kamikazeExplosionPrefab);


    // ----- BEHAVIOU METHODS -----
    void GetFromPool(GameObject vfx) => vfx.SetActive(true);
    void BackToPool(GameObject vfx) => vfx.SetActive(false);
    void OnDestroyVFX(GameObject vfx) => Destroy(vfx);

    // ----- PUBLIC ACCESS METHODS -----
    public GameObject GetBloodVFX(Vector3 position, Quaternion rotation)
    {
        GameObject vfx = bloodPool.Get();
        vfx.transform.position = position;
        vfx.transform.rotation = rotation;

        vfx.GetComponent<PooledVFX>().SetPool(bloodPool);
        return vfx;
    }

    public GameObject GetBigExplosion(Vector3 position)
    {
        GameObject vfx = bigExplosionPool.Get();
        vfx.transform.position = position;

        vfx.GetComponent<PooledVFX>().SetPool(bigExplosionPool);
        return vfx;
    }

    public GameObject GetSmallExplosion(Vector3 position)
    {
        GameObject vfx = smallExplosionPool.Get();
        vfx.transform.position = position;

        vfx.GetComponent<PooledVFX>().SetPool(smallExplosionPool);
        return vfx;
    }

    public GameObject GetKamikazeExplosion(Vector3 position)
    {
        if (kamikazeExplosionPool == null)
        {
            Debug.LogError("A Pool do Kamikaze está NULA! Verifique se ela foi inicializada no Awake.");
            return null;
        }

        GameObject vfx = kamikazeExplosionPool.Get();
        vfx.transform.position = position;

        vfx.GetComponent<PooledVFX>().SetPool(kamikazeExplosionPool);
        return vfx;
    }
}
