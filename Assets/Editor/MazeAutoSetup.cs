using UnityEngine;
using UnityEditor;

public class MazeAutoSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup Pacman Maze Game")]
    public static void SetupMazeGame()
    {
        int wallsCount = 0;
        int targetsCount = 0;

        // 1. Find Character in Scene
        GameObject character = GameObject.FindWithTag("Player");
        if (character == null)
        {
            character = GameObject.Find("Character") ?? GameObject.Find("Avatar") ?? GameObject.Find("Player");
        }

        if (character != null)
        {
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            if (rb == null) rb = character.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.useFullKinematicContacts = true;

            if (character.GetComponent<Collider2D>() == null)
            {
                character.AddComponent<BoxCollider2D>();
            }

            if (character.GetComponent<PacmanMazeCharacterMover>() == null)
            {
                character.AddComponent<PacmanMazeCharacterMover>();
            }

            Debug.Log($"✔ Character '{character.name}' configured successfully!");
        }
        else
        {
            Debug.LogWarning("⚠️ Character nahi mila! Apne player GameObject ka naam 'Player' ya 'Character' rakhein.");
        }

        // 2. Setup Maze Walls and Targets across all objects in scene
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            string objName = obj.name.ToLower();
            Collider2D col = obj.GetComponent<Collider2D>();

            if (col != null)
            {
                // Target check
                if (objName.Contains("target") || objName.Contains("destination") || objName.Contains("item") || objName.Contains("headphones"))
                {
                    obj.tag = "mazeTarget";
                    col.isTrigger = true;
                    targetsCount++;
                }
                // Wall / Obstacle check (Any collidable object that is not Player or Target)
                else if (!obj.CompareTag("Player") && !obj.name.Contains("Background") && !obj.name.Contains("Canvas"))
                {
                    col.isTrigger = false;
                    wallsCount++;
                }
            }
        }

        Debug.Log($"🎉 Setup Complete! Walls updated: {wallsCount}, Targets updated: {targetsCount}");
    }
}