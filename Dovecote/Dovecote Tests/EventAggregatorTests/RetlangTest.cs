using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Core;
using Retlang.Fibers;

namespace Dovecote.Tests.EventAggregatorTests
{
    [TestFixture]
    public class RetlangTest
    {
        [Test]
        public void TestFromRetlangSitePoolFiber()
        {
            //the test from retlang basic example
            using (var fiber = new PoolFiber())
            {
                fiber.Start();
                var channel = new Channel<string>();

                var reset = new AutoResetEvent(false);
                channel.Subscribe(fiber, delegate { reset.Set(); });
                channel.Publish("hello");

                Assert.IsTrue(reset.WaitOne(5000, false));

            }
        }

        [Test]
        public void MyOwnBatchingTestToMakeSureIUnderstandRetlangsBatchingStuff()
        {
            using (var fiber = new PoolFiber())
            {
                fiber.Start();
                var reset = new ManualResetEvent(false);

                var channel = new Channel<BatchTestMessage>();
                channel.SubscribeToBatch(fiber, BatchSubscriberMethod, 1);

                for(var i=0; i < 10; i++) // <---  increase to 1000000 or so to see it make 3 or 4 calls to BatchSubscriber!
                {
                    channel.Publish(new BatchTestMessage("loop " + i));
                }

                //Assert.IsTrue(reset.WaitOne(10000, false));
                
                reset.WaitOne(1000, false);
            }
        }

        class BatchTestMessage
        {
            public string Message { get; private set; } // <-- as immutable as I can make it..
            public BatchTestMessage(string message)
            {
                Message = message;
            }
        }

        private void BatchSubscriberMethod(IList<BatchTestMessage> batchedMessages)
        {
            Console.WriteLine("Called Batch Subscriber Method");
            Console.WriteLine("have " + batchedMessages.Count + " messages");
            //foreach(var message in batchedMessages)
            //{
            //    Console.WriteLine("Message Contents: " + message.Message);
            //}
        }

        [Test]
        public void BatchingTestFromRetlangTests()
        {
            using (var fiber = new ThreadFiber())
            {
                fiber.Start();
                var counter = new Channel<int>();
                var reset = new ManualResetEvent(false);
                var total = 0;
                Action<IList<int>> cb = delegate(IList<int> batch)
                                            {
                                                total += batch.Count;
                                                if (total == 10)
                                                {
                                                    reset.Set();
                                                }
                                            };

                counter.SubscribeToBatch(fiber, cb, 1);

                for (var i = 0; i < 10; i++)
                {
                    counter.Publish(i);
                }

                Assert.IsTrue(reset.WaitOne(10000, false));
            }
        }

    }

   
}