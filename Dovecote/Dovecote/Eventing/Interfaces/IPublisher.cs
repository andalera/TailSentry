namespace Dovecote.Eventing.Interfaces
{
    public interface IPublisher
    {
        void SendMessage<T>(T message);
    }
}