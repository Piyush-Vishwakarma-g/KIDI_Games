using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class MobileTTS : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _NativeTTS_Speak(string text);
#endif

    private AndroidJavaClass androidTTS;

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            androidTTS = new AndroidJavaClass("com.yourcompany.nativeplugin.NativeTTS");
            androidTTS.CallStatic("init", activity);
        }
#endif
    }

    public void Speak()
    {
        if (GameManager.instance == null || GameManager.instance.title == null) return;
        
        string text = GameManager.instance.title.text;
        if (string.IsNullOrEmpty(text)) return;

#if UNITY_EDITOR_WIN
        SpeakInWindowsEditor(text);
        return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        if (androidTTS != null)
        {
            androidTTS.CallStatic("speak", text);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        _NativeTTS_Speak(text);
#endif
    }

#if UNITY_EDITOR_WIN
    private void SpeakInWindowsEditor(string message)
    {
        string command = $"Add-Type -AssemblyName System.Speech; $syn = New-Object System.Speech.Synthesis.SpeechSynthesizer; $syn.Speak('{message}');";
        
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{command}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        Process.Start(psi);
        UnityEngine.Debug.Log("[Windows Editor TTS]: " + message);
    }
#endif
}