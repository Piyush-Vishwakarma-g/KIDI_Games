using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this from React Native via unityRef.current.postMessage('RNBridgeManager', 'LoadSceneFromRN', sceneName)
    /// </summary>
    public void LoadSceneFromRN(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneManagerScript] Scene name parameter is null or empty.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"[SceneManagerScript] Scene '{sceneName}' was not found. Make sure it is added to Build Settings.");
        }
    }

    /// <summary>
    /// Call this from Unity UI Button OnClick() to switch back to React Native
    /// </summary>
    public void BackToReactNative()
    {
        #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (UnityMessageManager.Instance != null)
        {
            UnityMessageManager.Instance.SendMessageToRN("CLOSE_UNITY");
        }
        else
        {
            Debug.LogWarning("[SceneManagerScript] UnityMessageManager instance is null.");
        }
        #else
        Debug.Log("[SceneManagerScript] BackToReactNative called in Editor/Desktop build.");
        #endif
    }

    
}