using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyFade : MonoBehaviour
{
    public float fadeSpeed = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float initialAlpha;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialAlpha = spriteRenderer.color.a;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }

    private void Update()
    {
        if (spriteRenderer.color.a < initialAlpha)
        {
            float newAlpha = Mathf.Lerp(spriteRenderer.color.a, initialAlpha, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
        }
    }
}