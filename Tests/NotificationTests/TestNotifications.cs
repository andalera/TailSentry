using NUnit.Framework;

namespace Tests.NotificationTests
{
    [TestFixture]
    public class TestNotifications
    {
        [Test]
        public void CanCreateWatcherInstance()
        {
            Assert.DoesNotThrow(delegate
                                    {
                                        INotification notification = new Notification
                                                                         {
                                                                             Name = "Test",
                                                                             Active = true,
                                                                             Pattern = "pattern",
                                                                             PatternType = PatternType.ExactMatch,
                                                                             SoundToPlayPath = @"c:\path\to\sound",
                                                                             TextToConvertToSpeech =
                                                                                 "Some text to convert to Speech"
                                                                         };
                                    }, "Could not create instance of Notification.");
        }

        [Test]
        public void WatcherClassInstanceImplementsIWatcher()
        {
            INotification notification = new Notification
                                             {
                                                 Name = "Test",
                                                 Active = true,
                                                 Pattern = "pattern",
                                                 PatternType = PatternType.ExactMatch,
                                                 SoundToPlayPath = @"c:\path\to\sound",
                                                 TextToConvertToSpeech = "Some text to convert to Speech"
                                             };
            Assert.IsInstanceOf(typeof (INotification), notification, "Notification instance does not implement INotification");
        }
    }
}