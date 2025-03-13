using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fadePanel;
    
    [Header("Settings")]
    [SerializeField] public float defaultFadeDuration = 1f;
    [SerializeField] private bool startFadedIn = false;
    [SerializeField] private bool dontDestroyOnLoad = true;

    // Singleton instance
    public static ScreenFade Instance { get; private set; }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initial fade state
        Color startColor = fadePanel.color;
        fadePanel.color = new Color(startColor.r, startColor.g, startColor.b, startFadedIn ? 1 : 0);
    }

    private void Start()
    {
        // If we start faded in, automatically fade out
        if (startFadedIn)
        {
            FadeOut();
        }
    }

    /// <summary>
    /// Fade from transparent to black
    /// </summary>
    public void FadeIn(float duration = -1)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0, 1, duration > 0 ? duration : defaultFadeDuration));
    }

    /// <summary>
    /// Fade from black to transparent
    /// </summary>
    public void FadeOut(float duration = -1)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1, 0, duration > 0 ? duration : defaultFadeDuration));
    }

    /// <summary>
    /// Fade to a specific alpha value
    /// </summary>
    public void FadeTo(float targetAlpha, float duration = -1)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(fadePanel.color.a, targetAlpha, duration > 0 ? duration : defaultFadeDuration));
    }

    /// <summary>
    /// Core fade routine
    /// </summary>
    public IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0;
        Color currentColor = fadePanel.color;
        
        // Set initial alpha
        fadePanel.color = new Color(currentColor.r, currentColor.g, currentColor.b, startAlpha);
        
        // Fade over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            fadePanel.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        // Ensure we reach the target alpha exactly
        fadePanel.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }

    /// <summary>
    /// Fade in, wait, then fade out
    /// </summary>
    public IEnumerator FadeInAndOut(float delayBetween = 0.5f, float fadeInDuration = -1, float fadeOutDuration = -1)
    {
        yield return StartCoroutine(FadeRoutine(0, 1, fadeInDuration > 0 ? fadeInDuration : defaultFadeDuration));
        yield return new WaitForSeconds(delayBetween);
        yield return StartCoroutine(FadeRoutine(1, 0, fadeOutDuration > 0 ? fadeOutDuration : defaultFadeDuration));
    }
} 