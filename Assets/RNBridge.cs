using UnityEngine;

public class RNBridge : MonoBehaviour
{
    // Yeh method React Native se call hoga scene switch karne ke liye (Optional)
    public void OpenUnityScene(string message)
    {
        Debug.Log("Message from React Native: " + message);
    }

    // Unity Button Click Event Listener
    public void OnUnityButtonClick()
    {
        // UnityView package React Native mein 'UnityBridge' NativeMessage listen karta hai
        #if UNITY_ANDROID || UNITY_IOS
        UnityNativeMessage("CLOSE_UNITY");
        #endif
    }

    private void UnityNativeMessage(string message)
    {
        // React Native Unity library `UnityPostMessage` handler ka use karti hai
        UnityMessageManager.Instance.SendMessageToRN(message);
    }
}