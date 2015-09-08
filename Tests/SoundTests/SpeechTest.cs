using System;
using NUnit.Framework;
using SpeechLib;

namespace Tests.SoundTests
{
    [TestFixture]
    public class SpeechTest
    {
        [Test]
        public void Speek()
        {
            Console.WriteLine("This test should play a sound.");
            SpeechVoiceSpeakFlags SpFlags = SpeechVoiceSpeakFlags.SVSFDefault;
            var speech = new SpVoice();
            speech.Speak("You are being followed", SpFlags);
            //speech.Speak("You are were eaten by a Grue", SpFlags);
        }
    }
}