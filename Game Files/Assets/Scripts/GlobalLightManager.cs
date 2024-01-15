using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; //2019 VERSIONS

public class GlobalLightManager : MonoBehaviour
{
    public Light2D globalLight;  // Reference to the global light
    public float bossIntensity = 0.29f;  // Intensity for the light when the boss spawns
    public Color bossColor = Color.red;  // Color for the light when the boss spawns
    public float intensityChangeSpeed = 1f;  // Speed at which the intensity changes

    private float defaultIntensity;  // Default intensity of the light
    private Color defaultColor;  // Default color of the light
    private float targetIntensity;  // Target intensity for the light
    private bool isChangingIntensity;  // Flag to track if the intensity is currently changing

    private void Start()
    {
        // Store the default intensity and color of the light
        defaultIntensity = globalLight.intensity;
        defaultColor = globalLight.color;

        // Set the initial target intensity to the default intensity
        targetIntensity = defaultIntensity;

        // Subscribe to the boss spawn event
        SpawnManager.OnBossSpawn += OnBossSpawn;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the boss spawn event when the script is destroyed
        SpawnManager.OnBossSpawn -= OnBossSpawn;
    }

    private void Update()
    {
        // Smoothly change the intensity towards the target intensity
        if (isChangingIntensity)
        {
            float currentIntensity = globalLight.intensity;
            float newIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, intensityChangeSpeed * Time.deltaTime);
            globalLight.intensity = newIntensity;

            // Check if the intensity has reached the target intensity
            if (Mathf.Approximately(newIntensity, targetIntensity))
            {
                isChangingIntensity = false;
            }
        }
    }

    private void OnBossSpawn()
    {
        // Set the target intensity and start changing the intensity
        targetIntensity = bossIntensity;
        isChangingIntensity = true;

        // Change the color of the light when the boss spawns
        globalLight.color = bossColor;
    }

    private void ResetLight()
    {
        // Reset the intensity and color of the light to the default values
        globalLight.intensity = defaultIntensity;
        globalLight.color = defaultColor;
    }
}
