using UnityEngine;

public class RNManager : MonoBehaviour
{
    // Ye method Unity ke UI Button ke 'OnClick()' par call karein
    public void GoBackToReactNative()
    {
        // React Native ko message bhejne ke liye
        #if UNITY_ANDROID || UNITY_IOS
        UnityMessageManager.Instance.SendMessageToRN("CLOSE_UNITY");
        #endif
    }
}
