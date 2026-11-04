
using UnityEngine;

public class FoamParticleCollisionHandler : MonoBehaviour
{
    public ParticleSystem[] allFoamSystems;

    private bool hasCollided;

    private void OnParticleCollision(GameObject other)
    {
        if (hasCollided)
            return;

        // Stop the complete foam effect on first collision.
        hasCollided = true;

        foreach (ParticleSystem ps in allFoamSystems)
        {
            if (ps != null)
            {
                ps.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }
        }

        Debug.Log("Foam particles collided. All foam systems stopped.");
    }
}
