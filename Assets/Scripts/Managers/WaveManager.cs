using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyDatabase
{
    public GameObject normalPrefab;
    public GameObject runnerPrefab;
    public GameObject bossPrefab;
}
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private EnemyDatabase database;
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Settings")]
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnBetweenGroups;

    [Header("Runner Progression")]
    [SerializeField] private float initialRunnerChance;
    [SerializeField] private float runnerChanceIncrease;
    [SerializeField] private int runnerIncreaseInterval;

    //Control
    private float hpMod = 1f;
    private float speedMod = 1f;
    private int enemiesKilled;
    private int totalEnemies;

    //Pools (Separate for each enemy type)
    private IObjectPool<GameObject> normalPool;
    private IObjectPool<GameObject> runnerPool;
    private IObjectPool<GameObject> bossPool;


    //Events
    public event Action OnWaveStarted;
    public event Action OnWaveEnded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        normalPool = CreateEnemyPool(database.normalPrefab);
        runnerPool = CreateEnemyPool(database.runnerPrefab);
        bossPool = CreateEnemyPool(database.bossPrefab);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalEvents.OnEnemyKilled += CheckWaveEnded;
    }

    IObjectPool<GameObject> CreateEnemyPool(GameObject prefab)
    {
        GameObject currentPrefab = prefab;

        IObjectPool<GameObject> newPool = null;
        newPool = new UnityEngine.Pool.ObjectPool<GameObject>(CreateNewObject, GetFromPool, BackToPool, OnDestroyObject, false, 20, 150); 

        return newPool;

        GameObject CreateNewObject()
        {
            GameObject enemyObj = Instantiate(currentPrefab);

            PooledEnemy pooledEnemy = enemyObj.AddComponent<PooledEnemy>();
            pooledEnemy.SetPool(newPool); //Tells the enemy which pool it came from

            return enemyObj; //Returns
        }

        void GetFromPool (GameObject obj) => obj.SetActive(true);
        void BackToPool(GameObject obj) => obj.SetActive(false);
        void OnDestroyObject (GameObject obj) => Destroy(obj);  
    }

    public void StartWave(int waveNumber)
    {
        //----- DIFICULT SCALING -----
        //Every 5 waves, increase enemy health modifier
        if (waveNumber > 1 && waveNumber % 5 == 0)
        {
            hpMod += 0.4f;
        }

        //----- ENEMY COUNT LOGIC -----
        //Total groups: (Base 2 + 1 every 3 rounds)
        int totalGroups = 2 + Mathf.FloorToInt(waveNumber / 3f);

        //Enemies per group: (Base 5 + 2 every 4 rounds)
        int enemiesPerGroup = 5 + (Mathf.FloorToInt(waveNumber / 4f) * 2);

        //----- RUNNER ENEMY SPAWN LOGIC -----
        //Runner enemy spawn chance
        float currentRunnerChance = initialRunnerChance;

        if(waveNumber > 1)
        {
            //Counts how many full 2-round cycles have passed
            int increases = (waveNumber - 1) / runnerIncreaseInterval;

            //Apply the increase
            currentRunnerChance += (increases * runnerChanceIncrease);
        }

        //Ensures the chance is never less than 0 or bigger than 1 (100%)
        currentRunnerChance = Mathf.Clamp01(currentRunnerChance);
        Debug.Log($"Wave {waveNumber}: Runner chance is: {currentRunnerChance * 100}%");

        // ----- TOTAL CALCULATION -----
        //Calculate total enemies for the wave
        totalEnemies = (totalGroups * enemiesPerGroup);
        if (waveNumber > 1 && waveNumber % 5 == 0) totalEnemies += 1; //+1 from Boss

        enemiesKilled = 0;

        Debug.Log($"Starting Wave {waveNumber}: {totalEnemies} total enemies.");
        OnWaveStarted?.Invoke();

        StartCoroutine(SpawnProceduralRoutine(totalGroups, enemiesPerGroup, currentRunnerChance, waveNumber));
    }

   
    IEnumerator SpawnProceduralRoutine(int totalGroups, int enemiesPerGroup, float runnerChance, int waveNumber)
    {
        //Loop through all groups
        for (int g = 0; g < totalGroups; g++)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);

            //Loop through all units inside the group
            for (int i = 0; i < enemiesPerGroup; i++)
            {
                GameObject prefabToSpawn;

                //Probability logic to choose between normal or runner
                if (Random.value < runnerChance) prefabToSpawn = database.runnerPrefab;
                else prefabToSpawn = database.normalPrefab;

                SpawnEnemy(prefabToSpawn, spawnIndex);
                yield return new WaitForSeconds(spawnRate);
            }

            yield return new WaitForSeconds(spawnBetweenGroups);
        }

        //Boss spawn logic (Every 5 rounds at the end of the wave???)
        if (waveNumber > 1 && waveNumber % 5 == 0)
        {
            int bossIndex = Random.Range(0, spawnPoints.Count);
            SpawnEnemy(database.bossPrefab, bossIndex); //Ver isso depois
        }

    }

    //Responsible for instantiating and configuring enemies
    void SpawnEnemy(GameObject prefab, int pointIndex)
    {
        //----- POOL SETTINGS ------
        if (prefab == null) return; 

        IObjectPool<GameObject> selectedPool = null;    
        if (prefab == database.normalPrefab) selectedPool = normalPool;
        else if (prefab == database.runnerPrefab) selectedPool = runnerPool;
        else if (prefab ==  database.bossPrefab) selectedPool = bossPool;

        if (selectedPool == null) return;

        //----- ENEMY SETTINGS -----
        GameObject enemy = selectedPool.Get();
        var agent = enemy.GetComponent<NavMeshAgent>();
        var stats = enemy.GetComponent<EntityStats>();
        var ai = enemy.GetComponent<EnemyAI>();

        if (agent != null) agent.enabled = false; //Disables the agent to teleport without errors

        //----- SPAWN POSITION -----
        Transform selectedPoint = spawnPoints[pointIndex];
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); //Small position variation to prevent them from spawning too close together
        float yOffset = 0.5f;
        Vector3 spawnPos = selectedPoint.position + randomOffset + (Vector3.up * yOffset);

        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;

        //----- DIFFICULTY SETTINGS AND RESET -----
        if (agent != null)
        {
            agent.enabled = true; //Reactivate after teleport
            agent.Warp(spawnPos); //Attaches/positions the enemy on the NavMesh
        }
        if (ai != null) ai.ResetEnemyAI();
        if (stats != null)
        {
            stats.SetupEnemyStats(hpMod, speedMod); //Accesses EntityStats public method
            Debug.Log($"HP {stats.MaxHp}, Speed {stats.MoveSpeed}");
        }
    }

    void CheckWaveEnded()
    {
        enemiesKilled++;
        Debug.Log($"Enemies: {enemiesKilled} / {totalEnemies}");

        if (enemiesKilled >= totalEnemies) OnWaveEnded?.Invoke();
    }

    void OnDisable()
    {
        GlobalEvents.OnEnemyKilled -= CheckWaveEnded;
    }
}
