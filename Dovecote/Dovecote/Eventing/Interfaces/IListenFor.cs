namespace Dovecote.Eventing.Interfaces
{
    public interface IListenFor<T> : IListen
    {
        void Handle(T message);
    }
}