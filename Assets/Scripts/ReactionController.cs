using UnityEngine;

public class ReactionController : MonoBehaviour
{
    public static ReactionController Instance;

    public bool hasVinegar = false;
    public bool hasBakingSoda = false;

    public ParticleSystem foamParticleSystem;
    public AudioSource reactionAudioSource;
    public AudioClip fizzingSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}