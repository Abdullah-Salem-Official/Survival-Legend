using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float flyDuration = 0.5f;  // Duration of the initial flying towards the player
    public float moveSpeed = 5f;  // Movement speed of the projectile after the initial fly

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Vector2 finalDirection;
    private float flyTimer;
    private bool isFlying = true;
    private bool isFlyingRight;

    public float pushForce = 0.5f;  // Force applied to push the player
    private Rigidbody2D playerRigidbody;  // Reference to the player's rigidbody

    public GameObject hitPrefab;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        rb = GetComponent<Rigidbody2D>();
        flyTimer = flyDuration;
    }

    private void Update()
    {
        if (isFlying)
        {
            FlyTowardsPlayer();
        }
        else
        {
            MoveAlongDirection();
        }


    }

    private void FlyTowardsPlayer()
    {
        flyTimer -= Time.deltaTime;

        if (flyTimer > 0f)
        {
            Vector2 flyDirection = (playerTransform.position - transform.position).normalized;
            rb.velocity = flyDirection * moveSpeed;

            // Rotate the projectile to face the player while tracking
            if (playerTransform != null)
            {
                Vector2 directionToPlayer = playerTransform.position - transform.position;
                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            isFlyingRight = rb.velocity.x > 0f;
        }
        else
        {
            isFlying = false;
            finalDirection = (playerTransform.position - transform.position).normalized;

            // Determine if the projectile is flying to the right
            isFlyingRight = finalDirection.x > 0f;
        }
    }

    private void MoveAlongDirection()
    {
        rb.velocity = finalDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Boss") && !collision.gameObject.CompareTag("Collectable") && !collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Projectile"))
        {
            // Check if the enemy touches the player's collider
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(25);
                }

                PushPlayer();
            }
         
            Instantiate(hitPrefab, transform.position, Quaternion.identity);

            // Detach the child objects from the parent before destroying it
            Transform[] childObjects = GetComponentsInChildren<Transform>();
            foreach (Transform child in childObjects)
            {
                child.SetParent(null);
            }

            Destroy(gameObject);
        }
    }

    private void PushPlayer()
    {
        // Calculate the direction to push the player based on the enemy's facing direction
        Vector2 pushDirection = Vector2.right;
        if (isFlyingRight)
        {
            pushDirection = Vector2.right;
        }
        else if (!isFlyingRight)
        {
            pushDirection = Vector2.left;
        }

        // Apply the push force to the player's rigidbody
        playerRigidbody.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
    }
}
