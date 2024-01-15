using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class detector : MonoBehaviour
{
    private bool bossSpawned = false;
    private void Update()
    {
        if(GameObject.FindGameObjectWithTag("Boss") != null)
        {
            bossSpawned = true;
        }
        // Check if there are no objects with the "Player" tag
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            // Call a method or perform actions when there is no player object
            StartCoroutine(LoadRestartSceneAfterDelay(1f));
        }
        else if(GameObject.FindGameObjectWithTag("Boss") == null && bossSpawned)
        {
            // Call a method or perform actions when there is no boss object
            StartCoroutine(LoadEndSceneAfterDelay(1f));
        }
    }

    private System.Collections.IEnumerator LoadRestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Load the restart scene
        SceneManager.LoadScene("Restart Screen");
    }

    private System.Collections.IEnumerator LoadEndSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Load the restart scene
        SceneManager.LoadScene("End Screen");
    }
}
