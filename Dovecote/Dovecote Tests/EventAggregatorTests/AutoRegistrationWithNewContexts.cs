using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Linq;


namespace Dovecote.Tests.EventAggregatorTests
{
    [TestFixture]
    public class AutoRegistrationWithNewContexts
    {
        [Test]
        public void TestWithNewContexts()
        {
            IWindsorContainer container = new WindsorContainer();
            // need to have the eventaggregator registered completely before other stuff.
            container.
                AddFacility<EventAutoRegistrationFacility>().
                Register(
                Component.For<IChannelManager>()
                    .ImplementedBy<ChannelManager>()
                    .LifeStyle.Singleton,
                Component.For<IEventAggregator>()
                    .ImplementedBy<EventAggregator>(),
                Component.For<IPublisher>()
                    .ImplementedBy<EventPublisher>(),
                Component.For<IListenerDisposerManager>()
                    .ImplementedBy<ListenerDisposalManager>(),
                Component.For<IContextFactory>()
                    .ImplementedBy<ContextFactory>()
                    
                ); 
            container
                .Register(
                Component.For<Test3>(),
                Component.For<Test2>(),
                Component.For<Test1>(),
                Component.For<Pub1>()
                );

            var reset = new AutoResetEvent(false);
            var testsubscriber2 = container.Resolve<Test2>();
            var testsubscriber3 = container.Resolve<Test3>();
            var testsubscriber = container.Resolve<Test1>();
            
            var testpublisher = container.Resolve<Pub1>();
            testpublisher.SendAMessage();

            //reset.Set();
            reset.WaitOne(2000, false);

        }
    }

    public class Pub1
    {
        private readonly IPublisher _publisher;


        public Pub1 (IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void SendAMessage()
        {
            _publisher.SendMessage("Hi there");
            Console.WriteLine("Publisher Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }
    }

    public class Test1 : IListenFor<string>
    {
        [HandleOnSpecificContext(ContextType.New)]
        public void Handle(string message)
        {
            Console.WriteLine("From Test1: " + message);
            Console.WriteLine("Test 1 Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }
    }

    [HandleOnSpecificContext(ContextType.New)]
    public class Test2 : IListenFor<string>
    {
        public void Handle(string message)
        {
            Console.WriteLine("From Test2: " + message);
            Console.WriteLine("Test 2 Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            
        }
    }

    public class Test3 : IListenForInBatches<string>
    {
        [HandleOnSpecificContext(ContextType.New)]
        [BatchOptions(Interval = 1)]
        public void Handle(IList<string> messages)
        {
            Console.WriteLine("From Test3: ");
            foreach (var message in messages)
            {
                Console.WriteLine("message: " + message);
            }
            Console.WriteLine("Test 3 Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}