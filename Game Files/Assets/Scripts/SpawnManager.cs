using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject applePrefab;  // Prefab of the apple to spawn
    public float yPoint = 2.7f;    // y-coordinate for apple spawn
    public float minXRange = -3f;   // Minimum x-coordinate range for apple spawn
    public float maxXRange = 3f;    // Maximum x-coordinate range for apple spawn

    public GameObject enemyPrefab;        // Prefab of the enemy to spawn
    public Transform playerTransform;   // Reference to the player's transform
    public Transform spawnPointA;         // Transform representing spawn point A
    public Transform spawnPointB;         // Transform representing spawn point B
    public float spawnDelay = 5f;         // Delay between enemy spawns

    private float spawnTimer = 0f;        // Timer to track spawn delay
    private bool isSpawnPointA = true;    // Flag to determine the active spawn point

    public delegate void BossSpawn();  // Delegate for the boss spawn event
    public static event BossSpawn OnBossSpawn;  // Event triggered when the boss spawn

    public int enemyCounter = 0;
    public float bossSpawnY = 7;
    public GameObject bossPrefab;        // Prefab of the boss to spawn
    private bool bossSpawned = false;
    public Canvas canvas;
    public int bossSpawnLimit = 10;

    private void OnEnable()
    {
        // Subscribe to the enemy death event
        Enemy.OnEnemyDeath += SpawnApple;
    }

    private void OnDisable()
    {
        // Unsubscribe from the enemy death event
        Enemy.OnEnemyDeath -= SpawnApple;
    }

    private void Start()
    {
        SpawnEnemy();
    }

    private void Update()
    {

        if(enemyCounter >= bossSpawnLimit && !bossSpawned)
        {
            SpawnBoss();
            bossSpawned = true;

            // Trigger the enemy spawn event
            OnBossSpawn?.Invoke();
        }
        else
        {

            // Increment the spawn timer
            spawnTimer += Time.deltaTime;

            // Check if the spawn timer exceeds the spawn delay
            if (spawnTimer >= spawnDelay && GameObject.FindGameObjectWithTag("Player") != null)
            {
                SpawnEnemy();

                // Reset the spawn timer
                spawnTimer = 0f;
            }
        }
    }

    private void SpawnApple()
    {
        // Generate a random x-coordinate within the specified range
        float randomX = Random.Range(minXRange, maxXRange);

        // Calculate the spawn position using the random x-coordinate and the spawn point's y-coordinate
        Vector3 spawnPosition = new Vector3(randomX, yPoint, 0f);

        // Instantiate the apple at the spawn position
        Instantiate(applePrefab, spawnPosition, Quaternion.identity);
    }
    private void SpawnEnemy()
    {
        // Determine the active spawn point
        Transform activeSpawnPoint = isSpawnPointA ? spawnPointA : spawnPointB;

        // Instantiate the enemy at the active spawn point
        GameObject enemy = Instantiate(enemyPrefab, activeSpawnPoint.position, Quaternion.identity);
        enemy.GetComponent<EnemyMovement>().SetPlayerTransform(playerTransform);

        // Toggle the flag for the next spawn
        isSpawnPointA = !isSpawnPointA;
    }

    private void SpawnBoss()
    {
        // Instantiate the enemy at the active spawn point
        Vector3 spawnPoint = new Vector3(playerTransform.transform.position.x, bossSpawnY, playerTransform.transform.position.z);
        GameObject boss = Instantiate(bossPrefab, spawnPoint, Quaternion.identity);

        boss.GetComponent<FinalBoss>().SetPlayerTransform(playerTransform);
    }
}