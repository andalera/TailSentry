using Dovecote.Eventing.Interfaces;

namespace Dovecote.Eventing.Aggregator
{
    public class EventPublisher : IPublisher
    {
        private readonly IEventAggregator _eventAggregator;

        public EventPublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void SendMessage<T>(T message)
        {
            _eventAggregator.SendMessage(message);
        }
    }
}