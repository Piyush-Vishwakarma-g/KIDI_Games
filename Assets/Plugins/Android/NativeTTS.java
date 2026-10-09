package com.yourcompany.nativeplugin;

import android.content.Context;
import android.speech.tts.TextToSpeech;
import java.util.Locale;

public class NativeTTS {
    private static TextToSpeech tts;
    private static boolean isReady = false;

    public static void init(Context context) {
        if (tts == null) {
            tts = new TextToSpeech(context, status -> {
                if (status == TextToSpeech.SUCCESS) {
                    tts.setLanguage(Locale.US);
                    isReady = true;
                }
            });
        }
    }

    public static void speak(String text) {
        if (isReady && tts != null) {
            tts.speak(text, TextToSpeech.QUEUE_FLUSH, null, "TTS_ID");
        }
    }
}