using UnityEngine;

public class BagLevelData : MonoBehaviour
{
    public static BagLevelData instance;
    
    public int targetIndex = 0;
    public int levelIndex = 2;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        levelIndex = 2;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}