using UnityEngine;
using System.Collections; // For coroutines

/// <summary>
/// Manages all audio playback in the game, including background music and sound effects.
/// This singleton ensures a consistent and serene audio experience, aligning with V7 Games' "Joyfully Calm" brand voice.
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the AudioManager.
    /// </summary>
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("The AudioSource component used for playing background music.")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("The AudioSource component used for playing one-shot sound effects.")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [Tooltip("The master volume level for all background music.")]
    [SerializeField] private float musicVolume = 0.7f;
    [Range(0f, 1f)]
    [Tooltip("The master volume level for all sound effects.")]
    [SerializeField] private float sfxVolume = 0.8f;

    // Keys for PlayerPrefs to persist volume settings
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    private void Awake()
    {
        // Implement the singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads

            // Ensure AudioSources are assigned or created
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true; // Music typically loops
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            // Load and apply previously saved volume settings
            LoadVolumeSettings();
            ApplyVolumeSettings();
        }
    }

    /// <summary>
    /// Loads the music and sound effect volume settings from PlayerPrefs.
    /// Default values are used if no settings are saved.
    /// </summary>
    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, sfxVolume);
    }

    /// <summary>
    /// Applies the current music and sound effect volume settings to their respective AudioSources.
    /// </summary>
    private void ApplyVolumeSettings()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Plays a background music clip. If a different music clip is already playing,
    /// it will gracefully fade out the current music and fade in the new one.
    /// </summary>
    /// <param name="musicClip">The AudioClip to play as background music.</param>
    /// <param name="fadeDuration">The duration (in seconds) over which to fade the music in/out. Defaults to 1.0f.</param>
    public void PlayMusic(AudioClip musicClip, float fadeDuration = 1.0f)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play a null music clip. Please provide a valid AudioClip.");
            return;
        }

        // If the same music is already playing, do nothing
        if (musicSource.clip == musicClip && musicSource.isPlaying)
        {
            return;
        }

        // Stop any existing fade coroutine to prevent conflicts
        StopAllCoroutines();
        StartCoroutine(FadeMusicCoroutine(musicClip, fadeDuration));
    }

    /// <summary>
    /// Coroutine to handle the fading of music clips for smooth transitions.
    /// </summary>
    /// <param name="newClip">The new AudioClip to transition to.</param>
    /// <param name="fadeDuration">The duration of the fade.</param>
    private IEnumerator FadeMusicCoroutine(AudioClip newClip, float fadeDuration)
    {
        float currentVolume = musicSource.volume;

        // Fade out current music if playing
        if (musicSource.isPlaying)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(currentVolume, 0f, timer / fadeDuration);
                yield return null;
            }
            musicSource.Stop();
        }

        // Set the new clip and play, then fade in
        musicSource.clip = newClip;
        musicSource.Play();
        
        float targetVolume = musicVolume; // Use the stored master music volume as the target
        float timerIn = 0f;
        while (timerIn < fadeDuration)
        {
            timerIn += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, timerIn / fadeDuration);
            yield return null;
        }
        musicSource.volume = targetVolume; // Ensure target volume is reached precisely
    }

    /// <summary>
    /// Plays a sound effect one time.
    /// </summary>
    /// <param name="sfxClip">The AudioClip to play as a sound effect.</param>
    /// <param name="volumeScale">An optional multiplier (0-1) for the SFX volume, applied on top of the master SFX volume. Defaults to 1.0f.</param>
    public void PlaySFX(AudioClip sfxClip, float volumeScale = 1.0f)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play a null SFX clip. Please provide a valid AudioClip.");
            return;
        }

        // PlayOneShot is ideal for sound effects as it doesn't interrupt currently playing clips on the same source.
        sfxSource.PlayOneShot(sfxClip, sfxVolume * volumeScale);
    }

    /// <summary>
    /// Sets the master music volume and persists it using PlayerPrefs.
    /// </summary>
    /// <param name="volume">The desired music volume, clamped between 0 and 1.</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save(); // Ensure the setting is saved immediately
    }

    /// <summary>
    /// Sets the master sound effect volume and persists it using PlayerPrefs.
    /// </summary>
    /// <param name="volume">The desired SFX volume, clamped between 0 and 1.</param>
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save(); // Ensure the setting is saved immediately
    }

    /// <summary>
    /// Retrieves the current master music volume.
    /// </summary>
    /// <returns>The current music volume (0-1).</returns>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    /// <summary>
    /// Retrieves the current master sound effect volume.
    /// </summary>
    /// <returns>The current SFX volume (0-1).</returns>
    public float GetSfxVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    /// <summary>
    /// Stops all currently playing sound effects on the SFX AudioSource.
    /// </summary>
    public void StopSFX()
    {
        sfxSource.Stop();
    }
}