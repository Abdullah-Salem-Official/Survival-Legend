using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class FinalBoss : MonoBehaviour
{
    public Transform playerTransform;  // Reference to the player's transform
    public float movementSpeed = 3f;   // Movement speed of the boss
    public GameObject projectilePrefab;  // Prefab for the boss's projectile
    public float projectileSpeed = 5f;  // Speed of the projectile
    public Transform projectileSpawnPoint;  // Spawn point for the projectiles
    public float shootInterval = 3f;  // Interval between each projectile shoot

    private float shootTimer = 0f;  // Timer to control projectile shooting

    public float movementSmoothing = 0.1f;

    public delegate void BossDeath();  // Delegate for the boss death event
    public static event BossDeath OnBossDeath;  // Event triggered when the boss dies


    public int maxHitPoints = 4;  // Maximum number of hits before destruction

    public int currentHitPoints = 0;  // Current number of hits taken

    public GameObject hitPrefab; // Hit Prefab

    public float blinkDuration = 0.2f;
    public Color hitColor = Color.red;
    private Color defaultColor;

    private SpriteRenderer enemyRenderer;


    public GameObject deathExplosionPrefab;
    public float deathExplosionYOffset = 0f;

    public GameObject healthBarPrefab;  // Prefab of the health bar
    private GameObject healthBarInstance;  // Instance of the health bar
    public Canvas canvas;

    public Slider healthSlider;

    public Vector3 healthBarOffset = new Vector3(0f, 1f, 0f);  // Offset for the health bar position

    private void Start()
    {
        // Find the Canvas GameObject by name
        GameObject canvasGameObject = GameObject.Find("Canvas");

        // Check if the GameObject was found
        if (canvasGameObject != null)
        {
            // Get the Canvas component
            canvas = canvasGameObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Canvas not found in the scene!");
        }

        if (canvas != null)
        {
            // Instantiate the health bar prefab as a child of the Canvas
            healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
            RectTransform cooldownBarRectTransform = healthBarInstance.GetComponent<RectTransform>();
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out Vector2 localPosition);
            cooldownBarRectTransform.anchoredPosition = localPosition;
        }


        enemyRenderer = GetComponent<SpriteRenderer>();
        defaultColor = enemyRenderer.color;

        // Update the UI slider to represent the remaining dash duration
        if (healthBarInstance != null)
        {
            healthSlider = healthBarInstance.GetComponent<Slider>();
            healthSlider.maxValue = maxHitPoints;
            healthSlider.minValue = 0;
        }
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = maxHitPoints - currentHitPoints;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, movementSmoothing * Time.deltaTime);
            transform.position = smoothedPosition;
        }

        // Update the shoot timer
        shootTimer += Time.deltaTime;

        // Check if it's time to shoot
        if (shootTimer >= shootInterval)
        {
            Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            // Reset the timer
            shootTimer = 0f;
        }

        // Update the position of the cooldown bar instance
        if (healthBarInstance != null)
        {
            // Calculate the position of the cooldown bar relative to the player's position
            Vector3 cooldownBarPosition = transform.position + healthBarOffset;

            // Convert the world position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(cooldownBarPosition);

            // Convert the screen position to UI canvas space
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out canvasPosition);

            // Set the cooldown bar's anchored position within the canvas
            RectTransform cooldownBarRectTransform = healthBarInstance.GetComponent<RectTransform>();
            cooldownBarRectTransform.anchoredPosition = canvasPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile collided with the enemy
        if (collision.CompareTag("Projectile") || collision.CompareTag("RangeAttack"))
        {

            // Add hit points
            currentHitPoints++;

            UpdateHealthUI();

            // Check if hit points reached zero
            if (currentHitPoints == maxHitPoints)
            {
                // Trigger the enemy death event
                OnBossDeath?.Invoke();

                SpawnDeathExplosion();

                Light2D lightComponent = GetComponentInChildren<Light2D>();
                if (lightComponent != null)
                {
                    lightComponent.transform.SetParent(null);
                }

                // Destroy the enemy
                Destroy(gameObject);
                Destroy(healthBarInstance);
            }
            else
            {

                SpawnHit();


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

    private void SpawnDeathExplosion()
    {
        // Instantiate the smoke
        Vector3 spawnPosition = transform.position + new Vector3(0f, deathExplosionYOffset, 0f);
        GameObject smoke = Instantiate(deathExplosionPrefab, spawnPosition, Quaternion.identity);
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    public void SetCanvas(Canvas _canvas)
    {
        canvas = _canvas;
    }
}