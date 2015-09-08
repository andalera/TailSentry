using System;
using System.Collections.Generic;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using Retlang.Fibers;

namespace Dovecote.Eventing.Aggregator
{
    /// <summary>
    /// This aggregator is a mishmash of different ideas from different people.
    /// The idea for this class and the IListenFor interface are from
    /// Jeremy Miller's Event Aggregator in the Build your own CAB series.  
    /// Probably going to use a bunch more of his ideas on screens and such..
    /// Retlang is used as the backend for the Aggregator.  It seemlessly handles threading and such.
    /// I *really* like it.  Couldn't really get my head around it though till I saw a powerpoint presentation 
    /// at http://www.mikebroberts.com/blogtastic/wp-content/uploads/2009/03/retlang-and-jetlang.pdf
    /// Besides, Mike Rettig keeps saying in Jeremy and Ayende's blog comments that his library would be good 
    /// for pubsub stuff.  I'm going to try and put it to the test.  (but keep it abstracted out behind an aggregator)
    /// This class is used to subscribe and publish Messages.
    /// HOWEVER, I've made a EventAutoRegistrationFacility to be registererd with Windsor to ***AUTO*** subscribe objects
    /// registered on the container that implement certain Interfaces (IListenFor..)
    /// The point is that your classes that only subscribe to events/messages don't have to know about the event
    /// aggregator at all!  I think of Message subscription as a crosscutting concern that shouldn't be worried about in classes.
    /// Classes that publish events should take a IPublisher object in from constructor.
    /// Publishers won't recieve *any* return values from a message being published.  This is *one way* pub/sub in this
    /// implementation.
    /// If a class needs to know if something happens as a result of something being published, make it *subscribe*
    /// to another message!  Some classes will be both subscribers and publishers!
    /// Message objects that are published should be IMMUTABLE for thread safety.  that means properties in your 
    /// message object should implement a getter, but not a setter.  You shouldn't be able to *change* a Message object
    /// from the subscriber.  The subscriber uses it to act upon.
    /// see the AutoRegistration test for example.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly IFiber _defaultContext;
        private readonly IChannelManager _channelManager;
        
        public EventAggregator(IChannelManager channelManager, IContextFactory contextFactory) : this(contextFactory.GetContext(contextFactory.DefaultContext), channelManager)
        {
        }

        public EventAggregator(IFiber defaultContext, IChannelManager channelManager)
        {
            _defaultContext = defaultContext;
            _channelManager = channelManager;
            //_defaultContext.Start();
        }

        public IDisposable AddListener<T>(Action<T> @delegate)
        {
            return AddListener(_defaultContext, @delegate);
        }

        public IDisposable AddListener<T>(IFiber context, Action<T> @delegate)
        {
            var channel = _channelManager.GetChannel<T>();
            return channel.Subscribe(context, @delegate);
        }

        public void SendMessage<T>(T message)
        {
            var channel = _channelManager.GetChannel<T>();
            channel.Publish(message);
        }

        public IDisposable AddBatchedListener<T>(Action<IList<T>> @delegate, int interval)
        {
            return AddBatchedListener(_defaultContext, @delegate, interval);
        }

        public IDisposable AddBatchedListener<T>(IFiber context, Action<IList<T>> action, int interval)
        {
            var channel = _channelManager.GetChannel<T>();
            return channel.SubscribeToBatch(context, action, interval);
        }
        
    }
}