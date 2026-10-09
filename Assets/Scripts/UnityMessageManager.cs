using UnityEngine;

public class UnityMessageManager : MonoBehaviour
{
    public static UnityMessageManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void SendMessageToRN(string message)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("com.azaron.unityview.UnityUtils"))
        {
            jc.CallStatic("postMessage", message);
        }
        #elif UNITY_IOS && !UNITY_EDITOR
        // iOS Native message call
        UnitySendMessageToRN(message);
        #endif
    }

    #if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void UnitySendMessageToRN(string message);
    #endif
}