using System;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dovecote.Eventing.Aggregator;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Facilities;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using NUnit.Framework;

namespace Dovecote.Tests.EventAggregatorTests
{
    [TestFixture]
    public class AutoRegistrationWithUnsubscriberFacilityTests
    {
        [Test]
        public void TestInterception()
        {
            IWindsorContainer container = new WindsorContainer();
            // need to have the eventaggregator registered completely before other stuff.
            container
                .AddFacility<EventAutoRegistrationFacility>()
                .AddFacility<UnsubscriberInterceptorFacility>()
                .Register(
                Component.For<IChannelManager>().ImplementedBy<ChannelManager>().LifeStyle.Singleton,
                Component.For<IListenerDisposerManager>().ImplementedBy<ListenerDisposalManager>().LifeStyle.Singleton,
                Component.For<IEventAggregator>().ImplementedBy<EventAggregator>(),
                Component.For<IPublisher>().ImplementedBy<EventPublisher>(),
                Component.For<IContextFactory>().ImplementedBy<ContextFactory>()
                );
            container
                .Register(
                Component.For<DisposableSubscriber>().LifeStyle.Transient
                );

            var listenerDisposerManager = container.Resolve<IListenerDisposerManager>();

            using (var disposablesubscriber1 = container.Resolve<DisposableSubscriber>())
            {
                //at this point, the Auto Registration Facility should have registered 
                //the unsubscriber in the listener disposal manager.
                Assert.IsTrue(listenerDisposerManager.AllDisposers.Count == 1);
            } //right here should end this subscriber.. it should auto deregister.

            //should not have anything in listenerDisposer now..
            Assert.IsTrue(listenerDisposerManager.AllDisposers.Count == 0);

            //send a message through publisher.
            var broker = container.Resolve<IPublisher>();
            broker.SendMessage(new UnsubscriberTestsMessage()); //should not have anything respond..

            //DisposableSubscriber.Handle should *not* be called.
        }

    }

    public class UnsubscriberTestsMessage { /* just an empty class to use as the message*/ }

    public class DisposableSubscriber : IListenFor<UnsubscriberTestsMessage>, IDisposable
    {
        public void Handle(UnsubscriberTestsMessage message)
        {
            Assert.Fail("This should NOT be called.  Should be disposed of before hand.");
        }

        //gotta mark virtual due to way I registered on container.. (no interface)
        //wish dynamic proxy could intercept methods not marked virtual from concrete objects... 
        //maybe in a future version... 
        public virtual void Dispose()
        {
            //don't have to do anything special here.. just marking either IDisposable or IListenerDisposer 
            //will work to auto unsubscribe any Handlers.
            
        }
    }
}