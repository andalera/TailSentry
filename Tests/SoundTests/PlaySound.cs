using System;
using System.Media;
using NUnit.Framework;

namespace Tests.SoundTests
{
    [TestFixture]
    public class PlaySound
    {
        private const string Testwav = @"ExampleFiles\wavFiles\chimes.wav";

        [Test]
        public void Sound()
        {
            Console.WriteLine("This test should play a sound.");
            var player = new SoundPlayer(Testwav);
            player.PlaySync();
        }
    }
}