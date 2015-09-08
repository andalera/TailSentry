using System.Collections.Generic;

namespace Tests
{
    public class MatchedNotificationQueue : IMatchedNotification
    {
        //this class may not be needed.
        private static readonly object Locker = new object();
        private readonly Queue<INotification> _notificationQueue;

        public MatchedNotificationQueue()
        {
            _notificationQueue = new Queue<INotification>();
        }

        #region IMatchedNotification Members

        public MatchedNotificationQueue Enqueue(INotification notification)
        {
            lock (Locker)
            {
                _notificationQueue.Enqueue(notification);
            }
            return this;
        }

        public INotification Dequeue()
        {
            lock (Locker)
            {
                return _notificationQueue.Dequeue();
            }
        }

        #endregion
    }
}