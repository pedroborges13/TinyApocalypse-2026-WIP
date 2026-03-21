using UnityEngine;

public class Landmine : ExplosiveBase
{
    protected override GameObject GetVisualEffect()
    {
        return VFXManager.Instance.GetSmallExplosion(transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TriggerExplosion();
            AudioManager.Instance.PlaySound(SoundType.Landmine, transform.position);
        }
    }
}
