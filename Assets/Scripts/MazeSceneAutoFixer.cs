using UnityEngine;

public class MazeSceneAutoFixer : MonoBehaviour
{
    [Header("Settings")]
    public string targetLayerName = "Obstacle";
    public string targetTagName = "MazeWall";

    [ContextMenu("Auto Fix Scene Colliders & Layers")]
    public void AutoFixMazeScene()
    {
        int fixedCount = 0;
        int layerIndex = LayerMask.NameToLayer(targetLayerName);

        if (layerIndex == -1)
        {
            Debug.LogError($"Layer '{targetLayerName}' nahi mili! Kripya Project Settings -> Tags & Layers me '{targetLayerName}' layer create karein.");
            return;
        }

        // Scene ke saare 2D Colliders find karein
        Collider2D[] allColliders = FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Collider2D col in allColliders)
        {
            if (col.gameObject.name.Contains("character") || col.gameObject.name.Contains("Waypoint") || col.gameObject.name.Contains("Player"))
            {
                continue;
            }

            GameObject obj = col.gameObject;

            // 1. Layer & Tag Assign
            obj.layer = layerIndex;
            
            // 2. Kinematic Rigidbody2D Add / Configure
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = obj.AddComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true;

            // 3. Z Position Snap to 0
            Vector3 pos = obj.transform.position;
            pos.z = 0f;
            obj.transform.position = pos;

            fixedCount++;
        }

        Debug.Log($"<color=green>SUCCESS:</color> {fixedCount} Maze Collider Objects Auto-Fix ho gaye hain!");
    }
}