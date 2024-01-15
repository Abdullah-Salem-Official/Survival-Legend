using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float targetProjectionSize = 9f;  // The desired projection size
    public float projectionSmoothing = 0.1f;  // Smoothing factor for projection size changes

    private Camera mainCamera;
    private float initialProjectionSize;
    private float currentProjectionSize;

    private bool isChangingProjectionSize = false;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        initialProjectionSize = mainCamera.orthographicSize;
        currentProjectionSize = initialProjectionSize;

        // Subscribe to the OnBossSpawn event
        SpawnManager.OnBossSpawn += ChangeCameraOnBossSpawn;
        
    }

    private void Update()
    {
        if (isChangingProjectionSize)
        {
            // Smoothly increase the camera's projection size
            currentProjectionSize = Mathf.Lerp(currentProjectionSize, targetProjectionSize, projectionSmoothing * Time.deltaTime);
            mainCamera.orthographicSize = currentProjectionSize;

            // Check if the target projection size is reached
            if (Mathf.Abs(currentProjectionSize - targetProjectionSize) <= 0.01f)
            {
                isChangingProjectionSize = false;
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnBossSpawn event when the script is destroyed
        SpawnManager.OnBossSpawn -= ChangeCameraOnBossSpawn;
    }

    private void ChangeCameraOnBossSpawn()
    {
        isChangingProjectionSize = true;
    }
}
