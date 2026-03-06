using UnityEngine;

public class Landmine : ExplosiveBase
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TriggerExplosion();
            AudioManager.Instance.PlaySound(SoundType.Landmine, transform.position);
        }
    }
}
