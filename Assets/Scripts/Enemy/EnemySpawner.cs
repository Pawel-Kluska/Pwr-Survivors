using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int minimumWaveQuota;
        public float spawnInterval;
        public int spawnCount;
        public int spawnTime;
        public bool isBossWave = false;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;
    }

    public List<Wave> waves;
    private List<Wave> activeWaves;
    public int currentWaveCount;

    [Header("Spawner Attributes")]
    float spawnTimer;
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval;
    public bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        activeWaves = new List<Wave>(waves.Count);
        currentWaveCount = 0;

        // CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnTime <= GameManager.instance.stopwatchTime)
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTimer += Time.deltaTime;

        StartCoroutine(SpawnEnemies());

        if (maxEnemiesReached)
            StopCoroutine(SpawnEnemies());
    }

    IEnumerator BeginNextWave()
    {
        activeWaves.Add(waves[currentWaveCount]);
        currentWaveCount++;

        yield return new WaitForSeconds(waveInterval);
        // CalculateWaveQuota();
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].minimumWaveQuota = currentWaveQuota;
    }

    IEnumerator SpawnEnemies()
    {
        foreach (Wave wave in activeWaves.ToList())
        {
            if (wave.isBossWave)
            {
                Instantiate(wave.enemyGroups[0].enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);
                enemiesAlive++;
                wave.spawnCount++;
                activeWaves.Remove(wave);
                continue;
            }

            if (!maxEnemiesReached)
            {
                foreach (EnemyGroup enemyGroup in wave.enemyGroups)
                {
                    // Spawnuj dopóki jeszcze nie wyspawnowało max przeciwnika i gdy jest mniej przeciwników niż minimum fali
                    while (enemyGroup.spawnCount < enemyGroup.enemyCount && (wave.spawnCount < wave.minimumWaveQuota || enemiesAlive < wave.minimumWaveQuota))
                    {
                        Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                        enemyGroup.spawnCount++;
                        wave.spawnCount++;
                        enemiesAlive++;
                        yield return new WaitForSeconds(0.4f);

                    }

                    // Jeśli został jakiś przeciwnik do wyspawnowania i minął czas, to zrób to
                    if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                    {
                        Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                        enemyGroup.spawnCount++;
                        wave.spawnCount++;
                        enemiesAlive++;
                        yield return new WaitForSeconds(wave.spawnInterval);
                    }

                    if (enemiesAlive >= maxEnemiesAllowed)
                        maxEnemiesReached = true;
                }
            }
        }
        yield return null;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
