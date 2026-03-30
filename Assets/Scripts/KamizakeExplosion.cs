using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class KamizakeExplosion : ExplosiveBase
{
    [Header("Settings")]
    [SerializeField] private float explosionDistance = 1.5f;

    [Header("References")]
    private Transform playerTransform;
    private EntityStats stats;
    private NavMeshAgent agent;

    void Start()
    {
        stats = GetComponent<EntityStats>();
        agent = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    protected override GameObject GetVisualEffect()
    {
        return VFXManager.Instance.GetKamikazeExplosion(transform.position);
    }

    void Update()
    {
        if (playerTransform == null || hasExploded) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= explosionDistance)
        {
            if (agent != null && agent.enabled) agent.isStopped = true;

            TriggerExplosion();
            GlobalEvents.OnEnemyKilled?.Invoke();
            stats.Death();
        }
    }
}
