using UnityEngine;
using UnityEngine.Pool;

public class PooledVFX : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    private IObjectPool<GameObject> pool;

    public void SetPool(IObjectPool<GameObject> targetPool)
    {
        pool = targetPool;
        CancelInvoke();
        Invoke(nameof(ReturnToPool), duration);
    }

    private void ReturnToPool()
    {
        if (pool != null) pool.Release(gameObject);
    }
}
