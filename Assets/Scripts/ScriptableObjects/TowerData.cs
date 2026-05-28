using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower/TowerData")]
public class TowerData : PlaceableItemData
{
    [Header("Visuals")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private SoundType fireSound;

    [Header("Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRateRPM;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private bool isAutomatic;


    //Visuals
    public GameObject ProjectilePrefab => projectilePrefab;
    public SoundType FireSound => fireSound;

    //Stats
    public float Damage => damage;
    public float FireInterval => 60f / fireRateRPM; //Já entrega o cálculo pronto para o script Weapon
    public float KnockbackForce => knockbackForce;
    public float ProjectileSpeed => projectileSpeed;
    public bool IsAutomatic => isAutomatic;
}
