using NUnit.Framework;

namespace Tests.NotificationTests
{
    [TestFixture]
    public class NotificationQueueTests
    {
        [Test]
        public void NotificationrQueueTest()
        {
            IMatchedNotification s = new MatchedNotificationQueue().Enqueue(new Notification());
            INotification w = s.Dequeue();
            Assert.IsInstanceOf(typeof (INotification), w);
        }
    }
}