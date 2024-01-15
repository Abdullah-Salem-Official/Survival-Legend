using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float pushForce = 0.5f;  // Force applied to push the player
    private Rigidbody2D playerRigidbody;  // Reference to the player's rigidbody
    private SpriteRenderer spriteRenderer;
    public float attackCooldown;

    private float _lastAttackTime;

    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the enemy touches the player's collider
        if (collision.CompareTag("Player"))
        {
            PushPlayer();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Abort if we already attacked recently.
        if (Time.time - _lastAttackTime < attackCooldown) return;

        // CompareTag is cheaper than .tag ==
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(25);
            }

            PushPlayer();

            // Remember that we recently attacked.
            _lastAttackTime = Time.time;
        }
    }

    private void PushPlayer()
    {
        // Calculate the direction to push the player based on the enemy's facing direction
        Vector2 pushDirection = Vector2.right;
        if (spriteRenderer.flipX == false)
        {
            pushDirection = Vector2.right;
        }
        else if (spriteRenderer.flipX)
        {
            pushDirection = Vector2.left;
        }

        // Apply the push force to the player's rigidbody
        playerRigidbody.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
    }
}