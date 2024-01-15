using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHitPoints = 4;  // Maximum number of hits before destruction

    public int currentHitPoints;  // Current number of hits taken

    public delegate void EnemyDeath();  // Delegate for the enemy death event
    public static event EnemyDeath OnEnemyDeath;  // Event triggered when the enemy dies
    public GameObject hitPrefab; // Hit Prefab

    public float blinkDuration = 0.2f;
    public Color hitColor = Color.red;
    private Color defaultColor;

    private SpriteRenderer enemyRenderer;
    private Rigidbody2D enemyRigidbody;
    public float pushForce = 0.5f;

    public bool getPush = false;

    public GameObject deathSmokePrefab;
    public float deathSmokeYOffset = 0f;

    public SpawnManager spawnManager;

    private void Start()
    {
        currentHitPoints = maxHitPoints;
        enemyRenderer = GetComponent<SpriteRenderer>();
        defaultColor = enemyRenderer.color;
        enemyRigidbody = GetComponent<Rigidbody2D>();

        GameObject spawnManagerGameObject = GameObject.Find("Spawn Manager");

        // Check if the GameObject was found
        if (spawnManagerGameObject != null)
        {
            // Get the Spawn Manager component

            spawnManager = spawnManagerGameObject.GetComponent<SpawnManager>();
        }
        else
        {
            Debug.LogError("Spawn Manager not found in the scene!");
        }
    }

    private void Update()
    {
        if (enemyRigidbody.velocity.x == 0 && getPush)
        {
            getPush = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile collided with the enemy
        if (collision.CompareTag("Projectile") || collision.CompareTag("RangeAttack"))
        {

            // Reduce hit points
            currentHitPoints--;

            // Check if hit points reached zero
            if (currentHitPoints <= 0)
            {
                // Trigger the enemy death event
                OnEnemyDeath?.Invoke();

                SpawnDeathSmoke();

                // Destroy the enemy
                Destroy(gameObject);


                if (spawnManager.spawnDelay > 4f)
                {
                    spawnManager.spawnDelay -= 0.2f;
                }

                spawnManager.enemyCounter++;
            }
            else
            {

                SpawnHit();

                Hit();

                ShowHitEffect();
            }
        }
    }

    private void SpawnHit()
    {
        // Instantiate the smoke
        GameObject hit = Instantiate(hitPrefab, transform.position, Quaternion.identity);
    }

    public void ShowHitEffect()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private System.Collections.IEnumerator BlinkCoroutine()
    {
        enemyRenderer.color = hitColor;
        yield return new WaitForSeconds(blinkDuration);
        enemyRenderer.color = defaultColor;
    }

    private void Hit()
    {
        getPush = true;

        // Calculate the direction to push the enemy
        Vector2 pushDirection = Vector2.right;
        if (enemyRenderer.flipX == false)
        {
            pushDirection = Vector2.left;
        }
        else if (enemyRenderer.flipX)
        {
            pushDirection = Vector2.right;
        }

        // Apply the push force to the enemy's rigidbody
        enemyRigidbody.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
    }

    private void SpawnDeathSmoke()
    {
        // Instantiate the smoke
        Vector3 spawnPosition = transform.position + new Vector3(0f, deathSmokeYOffset, 0f);
        GameObject smoke = Instantiate(deathSmokePrefab, spawnPosition, Quaternion.identity);
    }
}