#import <AVFoundation/AVFoundation.h>

extern "C" {
    void _NativeTTS_Speak(const char* text) {
        NSString *speakText = [NSString stringWithUTF8String:text];
        AVSpeechUtterance *utterance = [AVSpeechUtterance speechUtteranceWithString:speakText];
        utterance.voice = [AVSpeechSynthesisVoice voiceWithLanguage:@"en-US"];
        utterance.rate = AVSpeechUtteranceDefaultSpeechRate;

        AVSpeechSynthesizer *synthesizer = [[AVSpeechSynthesizer alloc] init];
        [synthesizer speakUtterance:utterance];
    }
}