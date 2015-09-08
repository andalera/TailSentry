using System;
using System.Collections.Generic;
using System.Threading;
using Dovecote.Eventing.Aggregator;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using NUnit.Framework;

namespace Dovecote.Tests.EventAggregatorTests
{
    [TestFixture]
    public class EventingTests
    {
        [Test]
        public void BasicEventAggregatorTest()
        {
            var broker = new EventAggregator(new ChannelManager(), new ContextFactory());
            var tester = new Tester();
            var tester2 = new Tester2();
            var reset = new AutoResetEvent(false);

            broker.AddListener<TestMessage>(tester2.Handle);
            broker.AddListener<TestMessage>(tester.Handle);
            broker.AddListener<string>(x => Console.WriteLine("I'm a string .. Here's the message that was sent:" + x));
            broker.SendMessage<string>("test");
            broker.SendMessage<TestMessage>(new TestMessage("Hi there", reset));

            Assert.IsTrue(reset.WaitOne(5000, false));
        }

        [Test]
        public void WhatHappensWhenASubscriberGetsGCd()
        {
            var broker = new EventAggregator(new ChannelManager(), new ContextFactory());
            var reset = new AutoResetEvent(false);

            using (var tester2 = new Tester2())
            {
                var disposer = broker.AddListener<TestMessage>(tester2.Handle);
                tester2.ListenerDisposers = new List<IDisposable> {disposer};
            }

            GC.Collect();
            var tester = new Tester();
            broker.AddListener<TestMessage>(tester.Handle);

            broker.AddListener<string>(x => Console.WriteLine("I'm a string .. Here's the message that was sent:" + x));
            broker.SendMessage<string>("test");
            broker.SendMessage<TestMessage>(new TestMessage("Hi there", reset));

            Assert.IsTrue(reset.WaitOne(5000, false));
        }

        
    }

    public class Tester : IListenFor<TestMessage>
    {
        public void Handle(TestMessage message)
        {
            Console.WriteLine(message.Msg1 + " I'm from Tester..");
            Assert.IsTrue(message.Msg1 == "Hi there");
            message.Event.Set();
        }
    }

    public class Tester2 : IListenFor<TestMessage>, IListenerDisposer
    {
        public IList<IDisposable> ListenerDisposers { get; set; }

        public void Handle(TestMessage message)
        {
            Console.WriteLine(message.Msg1 + " I'm number from tester2!");
        }

        public void Dispose()
        {
            if (ListenerDisposers != null)
                foreach (var disposer in ListenerDisposers)
                {
                    disposer.Dispose();
                }
        }
    }

    public class TestMessage
    {
        private readonly string _msg;
        private readonly AutoResetEvent _event;

        public TestMessage(string msg, AutoResetEvent @event)
        {
            _msg = msg;
            _event = @event;
        }

        public AutoResetEvent Event
        {
            get { return _event; }
        }

        public string Msg1
        {
            get { return _msg; }
        }
    
    }
}