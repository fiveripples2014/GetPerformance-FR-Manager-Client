using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Threading;

namespace FRManagerClient
{
    public static class Speak
    {
        private static SpeechSynthesizer speech = new SpeechSynthesizer();

        public static void greeting()
        {
            speak("Welcome to your workplace", VoiceGender.Female);
            Thread.Sleep(2);
            speak("Please enter your user name and password", VoiceGender.Female);
        }

        public static void speak(String message)
        {
            speech.Speak(message);
        }

        public static void speak(String message, VoiceGender voice)
        {
            speech.SelectVoiceByHints(voice);
            speak(message);
        }

        public static void speak(String message, VoiceGender voice, int rate)
        {
            speech.Rate = rate;
            speak(message, voice);
        }
    }
}

