using UnityEngine;

public class TrainLevelData : MonoBehaviour
{
     public static TrainLevelData instance;
    
    public int targetIndex = 0;
    public int levelIndex = 1;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        levelIndex = 1;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
