using UnityEngine;
using UnityEngine.Pool;

public class PooledEnemy : MonoBehaviour
{
    private IObjectPool<GameObject> pool;

    //Called by the WaveManager to set the pool reference
    public void SetPool(IObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }

    public void ReturnToPool()
    {
        if (pool != null) pool.Release(gameObject); //Sends back to the pool
        else Destroy(gameObject); //Destroy as fallback when pool unavailable 
    }
}
