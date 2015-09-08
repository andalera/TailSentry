namespace Tests
{
    public interface IMatchedNotification
    {
        MatchedNotificationQueue Enqueue(INotification notification);
        INotification Dequeue();
    }
}