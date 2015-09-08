using System;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dovecote.Eventing.Aggregator;
using Dovecote.Eventing.Attributes;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Facilities;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using NUnit.Framework;

namespace Dovecote.Tests.EventAggregatorTests
{
    [TestFixture]
    public class AutoRegistrationTest
    {
        private IWindsorContainer container = null;

        [SetUp]
        public void Setup()
        {
            container = new WindsorContainer();
            // need to have the eventaggregator registered completely before other stuff.
            container.
                AddFacility<EventAutoRegistrationFacility>().
                Register(
                Component.For<IChannelManager>().ImplementedBy<ChannelManager>().LifeStyle.Singleton,
                Component.For<IEventAggregator>().ImplementedBy<EventAggregator>(),
                Component.For<IPublisher>().ImplementedBy<EventPublisher>(),
                Component.For<IContextFactory>().ImplementedBy<ContextFactory>()
                ); 
        }

        [Test]
        public void BasicPubSubTest()
        {
            container
                .Register(
                Component.For<TestPublisher>(),
                Component.For<TestSubscriber>(),
                Component.For<TestSubscriber2>()
                );

            var reset = new AutoResetEvent(false);

            var testsubscriber = container.Resolve<TestSubscriber>();
            var testsubscriber2 = container.Resolve<TestSubscriber2>();

            Console.WriteLine("Publisher thread: " + Thread.CurrentThread.ManagedThreadId);
            var testpublisher = container.Resolve<TestPublisher>();
            testpublisher.SendAMessage();

            //reset.Set();
            reset.WaitOne(1000, false);

        }
        
        [Test]
        public void SubscriberGetsCalledMultipleTimes()
        {
            container
                .Register(
                Component.For<TestSubscriber>()
                );

            var reset = new AutoResetEvent(false);

            var testsubscriber = container.Resolve<TestSubscriber>();

            var publisher = container.Resolve<IPublisher>();
            Console.WriteLine("Publisher Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            for (var i=0; i<=10; i++)
            {
                publisher.SendMessage(new TesterMessage("message sent " + i));
            }
                

            //reset.Set();
            reset.WaitOne(2000, false);
        }

    }

    public class TestSubscriber2 : IListenFor<TesterMessage>
    {
        public void Handle(TesterMessage message)
        {
            Console.WriteLine("I'm listening to!" + message.Message);
            Console.WriteLine("Subscriber Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }
    }

    public class TestSubscriber : IListenFor<TesterMessage>, IListenFor<AnotherMessage>
    {
        public void Handle(TesterMessage message)
        {
            Console.WriteLine("A Message: " + message.Message);
            Console.WriteLine("Subscriber Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }

        public void Handle(AnotherMessage message)
        {
            Console.WriteLine("Another Message: " + message.Message);
            Console.WriteLine("Subscriber Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }
    }

    public class TestPublisher
    {
        private readonly IPublisher _publisher;

        public TestPublisher(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void SendAMessage()
        {
            _publisher.SendMessage<TesterMessage>(new TesterMessage("Here's a Message!!"));
            _publisher.SendMessage<AnotherMessage>(new AnotherMessage("another message..."));
        }
    }

    public class TesterMessage
    {
        private readonly string _aMessage;

        public TesterMessage(string aMessage)
        {
            _aMessage = aMessage;
        }

        public string Message
        {
            get { return _aMessage; }
        }
    }

    public class AnotherMessage
    {
        private readonly string _aMessage;

        public AnotherMessage(string aMessage)
        {
            _aMessage = aMessage;
        }

        public string Message
        {
            get { return _aMessage; }
        }
    }
}