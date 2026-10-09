using UnityEngine;
using UnityEngine.Events;

public class SafetyManager : MonoBehaviour
{
    public static SafetyManager Instance;

    [Header("Safety Items Status")]
    public bool hasGoggles = false;
    public bool hasGloves = false;

    [Header("UI / Game Events")]
    public UnityEvent OnSafetyCompleted;
    public AudioSource voiceoverSource;
    public AudioClip presentationVoiceover;

    private bool isCompleted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EquipItem(string itemType)
    {
        if (itemType == "Goggles") hasGoggles = true;
        if (itemType == "Gloves") hasGloves = true;

        CheckSafetyStatus();
    }

    private void CheckSafetyStatus()
    {
        if (hasGoggles && hasGloves && !isCompleted)
        {
            isCompleted = true;
            Debug.Log("Safety equipment complete! Unlocking chemicals...");
            
            OnSafetyCompleted?.Invoke();
            if (voiceoverSource != null && presentationVoiceover != null)
            {
                voiceoverSource.PlayOneShot(presentationVoiceover);
            }
        }
    }
}