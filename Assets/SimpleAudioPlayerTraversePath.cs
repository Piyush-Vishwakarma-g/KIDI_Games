using UnityEngine;

public class SimpleAudioPlayerTraversePath : MonoBehaviour
{ 
    public static SimpleAudioPlayerTraversePath instance;

    [Header("Audio Components")]
    public AudioSource audioSource;
    public AudioClip soundClip1;
    public AudioClip soundClip2;

    private void Awake()
    {
        // Singleton pattern assign in Awake for early initialization
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-get AudioSource component if missing in Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlaySound1()
    {
        if (audioSource != null && soundClip1 != null)
        {
            audioSource.PlayOneShot(soundClip1);
        }
        else
        {
            Debug.LogWarning("SimpleAudioPlayer: AudioSource ya SoundClip1 missing hai!");
        }
    }

    public void PlaySound2()
    {
        if (audioSource != null && soundClip2 != null)
        {
            audioSource.PlayOneShot(soundClip2);
        }
        else
        {
            Debug.LogWarning("SimpleAudioPlayer: AudioSource ya SoundClip2 missing hai!");
        }
    }
}