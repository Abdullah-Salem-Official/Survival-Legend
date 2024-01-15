using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform playerTransform;  // Reference to the player's transform
    public float movementSpeed = 5f;    // Movement speed of the enemy

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Enemy enemyScript;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyScript = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (playerTransform != null && !enemyScript.getPush)
        {
            Vector2 direction = playerTransform.position - transform.position;
            direction.Normalize();

            // Move towards the player
            rb.velocity = direction * movementSpeed;

            // Flip the sprite based on the movement direction
            if (direction.x > 0)
                spriteRenderer.flipX = false;  // Face right
            else if (direction.x < 0)
                spriteRenderer.flipX = true;   // Face left
        }
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}