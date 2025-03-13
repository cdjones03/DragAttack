using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private bool playOnStart = false;

    [SerializeField] private AudioSource punchSound;
    [SerializeField] private AudioSource kickSound;

    private float targetVolume;
    private Coroutine fadeCoroutine;

    public static AudioController Instance { get; private set; }

    private void Awake()
    {
        // Simple singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Store the target volume
        if (musicSource != null)
            targetVolume = musicSource.volume;

        // Initially set volume to 0 if we'll fade in
        if (fadeInDuration > 0 && !playOnStart)
            musicSource.volume = 0;

        // Subscribe to timeline finished event
        if (timeline != null)
            timeline.stopped += OnTimelineFinished;
            
        // Start music immediately if needed
        if (playOnStart)
            PlayMusic();
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (musicSource == null) return;
        
        // Stop any existing fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        musicSource.Play();
        
        if (fadeInDuration > 0)
            fadeCoroutine = StartCoroutine(FadeMusic(0, targetVolume, fadeInDuration));
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        
        // Stop any existing fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
            
        musicSource.Stop();
    }

    public void FadeOutMusic()
    {
        if (musicSource == null || !musicSource.isPlaying) return;
        
        // Stop any existing fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
            
        fadeCoroutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return FadeMusic(musicSource.volume, 0, fadeOutDuration);
        musicSource.Stop();
    }

    private IEnumerator FadeMusic(float startVolume, float endVolume, float duration)
    {
        float startTime = Time.time;
        
        // Set initial volume
        musicSource.volume = startVolume;
        
        while (Time.time < startTime + duration)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / duration;
            musicSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }
        
        // Ensure we reach the target volume
        musicSource.volume = endVolume;
    }

    public void CrossfadeToNewTrack(AudioClip newTrack, float crossfadeDuration = 2f)
    {
        if (musicSource == null) return;
        
        StartCoroutine(CrossfadeRoutine(newTrack, crossfadeDuration));
    }

    private IEnumerator CrossfadeRoutine(AudioClip newTrack, float duration)
    {
        // Create temporary audio source for new track
        GameObject tempObj = new GameObject("TempAudioSource");
        AudioSource newSource = tempObj.AddComponent<AudioSource>();
        
        // Copy settings from main source
        newSource.clip = newTrack;
        newSource.volume = 0;
        newSource.loop = musicSource.loop;
        newSource.Play();
        
        // Fade out current and fade in new
        float startTime = Time.time;
        float currentVol = musicSource.volume;
        
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            musicSource.volume = Mathf.Lerp(currentVol, 0, t);
            newSource.volume = Mathf.Lerp(0, targetVolume, t);
            yield return null;
        }
        
        // Swap tracks
        musicSource.Stop();
        musicSource.clip = newTrack;
        musicSource.volume = targetVolume;
        musicSource.Play();
        
        // Clean up temp object
        Destroy(tempObj);
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        if (timeline != null)
            timeline.stopped -= OnTimelineFinished;
    }

    public void PlayPunchSound()
    {
        punchSound.Play();
    }

    public void PlayKickSound()
    {
        kickSound.Play();
    }
}
