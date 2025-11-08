using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource audioSource; // The single shared AudioSource

    [Header("UI Sounds")]
    public AudioClip hoverSfx;
    public AudioClip clickSfx;

    [Range(0f, 1f)] public float hoverVolume = 0.5f;
    [Range(0f, 1f)] public float clickVolume = 0.8f;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persist across scenes

        // Auto-setup
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D
        }
    }

    public void PlayHover()
    {
        if (hoverSfx && audioSource)
            audioSource.PlayOneShot(hoverSfx, hoverVolume);
    }

    public void PlayClick()
    {
        if (clickSfx && audioSource)
            audioSource.PlayOneShot(clickSfx, clickVolume);
    }
}
