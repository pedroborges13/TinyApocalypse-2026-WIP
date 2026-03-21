using UnityEngine;

public class Grenade : ExplosiveBase
{
    [SerializeField] private float delay;

    protected override GameObject GetVisualEffect()
    {
        return VFXManager.Instance.GetSmallExplosion(transform.position);
    }

    void Start()
    {
        Invoke("TriggerExplosion", delay);
    }
}
