using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Unity.Cinemachine;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private bool canReceiveKnockback;
    [SerializeField] private EntityStatsData data;

    //References
    [SerializeField] private CameraAnimationShake cameraAnimation;
    private CharacterAnimationController anim;
    private NavMeshAgent agent;
    private CharacterController playerController;

    //Local variables allow modifications without altering the ScriptableObject
    private float maxHp;
    private float moveSpeed;

    public float CurrentHp {  get; private set; }
    public bool IsDead { get; private set; }
    private bool CanRegenerate;
    private bool isPlayer;
    private Coroutine regenerationTimer;

    //Getters
    public float MaxHp => maxHp;
    public float Damage => data.Damage;
    public float MoveSpeed => moveSpeed;

    //Events
    public event Action OnHealthChanged;
    public event Action OnPlayerDeath;


    void Awake()
    {
        if (data != null)
        {
            maxHp = data.MaxHp;
            moveSpeed = data.MoveSpeed;
            CurrentHp = maxHp;
            //Debug.Log("EntityStats: Health: " + maxHp +  " Speed: " + moveSpeed + " CurrentHealth: " + CurrentHp);
        }
    }

    void Start()
    {
        anim = GetComponent<CharacterAnimationController>();
        if (CompareTag("Enemy")) agent = GetComponent<NavMeshAgent>();
        else if (CompareTag("Player")) playerController = GetComponent<CharacterController>();

        CanRegenerate = true;
        isPlayer = CompareTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space ) && cameraAnimation != null)
        {
            cameraAnimation.PlayShake();
        }

        if (IsDead || !isPlayer) return; 

        if (CanRegenerate && CurrentHp < MaxHp)
        {
            CurrentHp += 0.75f * Time.deltaTime;
            CurrentHp = Mathf.Min(CurrentHp, MaxHp);
        }
    }

    public void SetupEnemyStats(float hpMod, float speedMod)
    {
        IsDead = false;

        maxHp = data.MaxHp * hpMod;
        moveSpeed = data.MoveSpeed * speedMod;

        //New current health calculation
        CurrentHp = maxHp;

        //Updates NavMeshAgent
        if(TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.speed = moveSpeed;
        }
        if (TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            collider.enabled = true;
        }

    }
    public void TakeDamage(float damage, Vector3 initialPosition = default, float kbForce = 0) //Default and 0 for when the player takes damage
    {
        if(IsDead) return;

        CurrentHp -= damage;

        if (kbForce > 0 && TryGetComponent<EnemyAI>(out EnemyAI ai))
        {
            if(canReceiveKnockback) ai.ApplyKnockback(initialPosition, kbForce);
        }

        if (CurrentHp > 0 && canReceiveKnockback) anim.PlayHit();

        if (CompareTag("Player")) 
        {
            if (regenerationTimer != null) StopCoroutine(regenerationTimer);

            regenerationTimer = StartCoroutine(HealthRegenerationRoutine());

            if (cameraAnimation != null)
            {
               cameraAnimation.PlayShake();
            }
        }


        if (CurrentHp <= 0)
        {
           anim.PlayDeath();

            if (CompareTag("Enemy"))
            {
                GlobalEvents.OnEnemyKilled?.Invoke();
                DisableNavMesh();

                if (TryGetComponent<EnemyDrop>(out EnemyDrop drop))
                {
                    drop.DropReward();
                }
                if (TryGetComponent<BoxCollider>(out BoxCollider collider))
                {
                    collider.enabled = false;
                }
            }
            else if (CompareTag("Player"))
            {
                OnPlayerDeath?.Invoke();
            }
           Death();
         }
    }

    IEnumerator HealthRegenerationRoutine()
    {
        CanRegenerate = false;

        yield return new WaitForSeconds(3f);

        CanRegenerate = true;
    }

    void DisableNavMesh()
    {
        if(agent != null)
        {
            agent.speed = 0;
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

    public void Death()
    {
        IsDead = true;
        CanRegenerate = false;
        if (CompareTag("Player")) playerController.enabled = false;
        else if (CompareTag("Enemy")) StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        if (TryGetComponent<PooledEnemy>(out PooledEnemy pooledEnemy))
        {
            pooledEnemy.ReturnToPool();
        }
        else Destroy(pooledEnemy, 1);
    }
}
