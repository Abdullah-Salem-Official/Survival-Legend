using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DFade : MonoBehaviour
{
    public Light2D light2D;
    public float targetIntensity = 1f;
    public float changeSpeed = 1f;

    private float initialIntensity;
    private float currentIntensity;
    private bool isChangingIntensity;

    private void Start()
    {
        light2D.intensity = initialIntensity = 0f;
        currentIntensity = initialIntensity;
        isChangingIntensity = false;

        // Subscribe to the boss spawn event
        SpawnManager.OnBossSpawn += StartIntensityChange;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the boss spawn event when the script is destroyed
        SpawnManager.OnBossSpawn -= StartIntensityChange;
    }

    private void Update()
    {
        if (isChangingIntensity)
        {
            currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, changeSpeed * Time.deltaTime);
            light2D.intensity = currentIntensity;

            if (Mathf.Approximately(currentIntensity, targetIntensity))
            {
                isChangingIntensity = false;
            }
        }
    }

    public void StartIntensityChange()
    {
        light2D.intensity = initialIntensity;
        isChangingIntensity = true;
    }
}
