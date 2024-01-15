using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public int energyValue = 25;  // Energy value to add when collected
    public GameObject collectedPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collided with the fruit
        if (collision.CompareTag("Player"))
        {
            // Get a reference to the player's energy script
            PlayerController playerController = collision.GetComponent<PlayerController>();

            // Add energy to the player
            if (playerController != null)
            {
                playerController.appleCount++;
            }

            Instantiate(collectedPrefab, transform.position, Quaternion.identity);

            // Destroy the fruit
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided with the fruit
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get a reference to the player's energy script
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            // Add energy to the player
            if (playerController != null)
            {
                playerController.appleCount++;
                // Update the UI Text
                playerController.appleCountText.text = ": " + playerController.appleCount;

                Instantiate(collectedPrefab, transform.position, Quaternion.identity);

                // Destroy the fruit
                Destroy(gameObject);
            }

        }
    }
}



