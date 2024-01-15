using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossLight2DFade : MonoBehaviour
{
    public Light2D light2D;
    public float targetIntensity = 1f;
    public float fadeOutDuration = 1f;

    private float initialIntensity;
    private float currentIntensity;
    private bool isFadingIn;
    private bool isFadingOut;
    private float fadeOutTimer;

    private void Start()
    {
        initialIntensity = 0f;
        currentIntensity = initialIntensity;
        isFadingIn = true;
        isFadingOut = false;
        fadeOutTimer = 0f;
        StartIntensityChange();
    }

    private void Update()
    {
        if (isFadingIn)
        {
            currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, Time.deltaTime / fadeOutDuration);
            light2D.intensity = currentIntensity;

            if (Mathf.Approximately(currentIntensity, targetIntensity))
            {
                isFadingIn = false;
                isFadingOut = true;
            }
        }
        else if (isFadingOut)
        {
            fadeOutTimer += Time.deltaTime;
            float normalizedTime = fadeOutTimer / fadeOutDuration;
            currentIntensity = Mathf.Lerp(targetIntensity, 0f, normalizedTime);
            light2D.intensity = currentIntensity;

            if (normalizedTime >= 1f)
            {
                isFadingOut = false;
                // Perform any desired actions after the fade-out completes
                // For example, you can destroy the light object or disable it
                Destroy(gameObject);
            }
        }
    }

    public void StartIntensityChange()
    {
        light2D.intensity = initialIntensity;
        isFadingIn = true;
    }
}
