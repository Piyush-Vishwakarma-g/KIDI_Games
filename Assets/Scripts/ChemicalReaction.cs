using UnityEngine;
using System.Collections;

public class ChemicalReaction : MonoBehaviour
{
    public bool hasVinegar = false;
    public bool hasBakingSoda = false;

    public ParticleSystem foamParticles;
    public AudioSource audioSource;
    public AudioClip fizzingSound;

    public void AddIngredient(string ingredientName)
    {
        if (ingredientName == "Vinegar") hasVinegar = true;
        if (ingredientName == "BakingSoda") hasBakingSoda = true;

        if (hasVinegar && hasBakingSoda)
        {
            if (foamParticles != null) foamParticles.Play();
            if (audioSource != null && fizzingSound != null)
            {
                audioSource.clip = fizzingSound;
                audioSource.Play();
            }
        }
    }
}