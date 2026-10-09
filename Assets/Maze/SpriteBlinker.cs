using System.Collections;
using UnityEngine;

public class SpriteBlinker : MonoBehaviour
{
    [Header("Components")]
public SpriteRenderer spriteRenderer;
    public float blinkDuration = 2f;
    public float blinkInterval = 1f;

    private Coroutine coroutine;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ContextMenu("Test Blink")]
    public void StartBlinking()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            // Toggles sprite visibility
            spriteRenderer.enabled = !spriteRenderer.enabled;
            
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; // Ensure it stays visible at the end
    }
}