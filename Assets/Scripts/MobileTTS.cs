using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
    public void SpeakTxt(string _text)
    {
        
        string text = _text;
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
        string escapedMessage = message.Replace("'", "''");
        string command = $"$ErrorActionPreference = 'Stop'; $syn = $null; $failed = $false; try {{ Add-Type -AssemblyName System.Speech; $syn = New-Object System.Speech.Synthesis.SpeechSynthesizer; $syn.Volume = 100; $syn.SetOutputToDefaultAudioDevice(); $syn.Speak('{escapedMessage}'); }} catch {{ [Console]::Error.WriteLine($_.Exception.ToString()); $failed = $true; }} finally {{ if ($syn -ne $null) {{ $syn.Dispose(); }} }}; if ($failed) {{ exit 1; }}";
        string encodedCommand = Convert.ToBase64String(Encoding.Unicode.GetBytes(command));

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -NonInteractive -EncodedCommand {encodedCommand}",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true
        };

        Process process = new Process
        {
            StartInfo = psi
        };
        StringBuilder errorOutput = new StringBuilder();
        process.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                lock (errorOutput)
                {
                    errorOutput.AppendLine(args.Data);
                }
            }
        };

        if (!process.Start())
        {
            process.Dispose();
            UnityEngine.Debug.LogError("Could not start PowerShell for Windows text-to-speech.");
            return;
        }

        process.BeginErrorReadLine();
        Task.Run(() =>
        {
            process.WaitForExit();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error;
                lock (errorOutput)
                {
                    error = errorOutput.ToString();
                }

                UnityEngine.Debug.LogError("Windows text-to-speech failed: " + error);
            }

            process.Dispose();
        });
        UnityEngine.Debug.Log("[Windows Editor TTS] Speech request started: " + message);
    }
#endif
}