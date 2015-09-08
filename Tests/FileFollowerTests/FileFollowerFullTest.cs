using System;
using System.IO;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dovecote.Eventing.Aggregator;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Facilities;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using NUnit.Framework;
using TailSentry.FileFollowing;

namespace Tests.FileFollowerTests
{
    [TestFixture]
    public class FileFollowerFullTest
    {
        
        [Test]
        public void VerifyFollowerThrowsErrorCorrectly()
        {
            IWindsorContainer container = new WindsorContainer();
            // need to have the eventaggregator registered completely before other stuff.
            container
                .AddFacility<EventAutoRegistrationFacility>()
                .AddFacility<UnsubscriberInterceptorFacility>()
                .Register(
                Component.For<IContextFactory>().ImplementedBy<ContextFactory>(),
                Component.For<IChannelManager>().ImplementedBy<ChannelManager>().LifeStyle.Singleton,
                Component.For<IListenerDisposerManager>().ImplementedBy<ListenerDisposalManager>().LifeStyle.Singleton,
                Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifeStyle.Singleton,
                Component.For<IPublisher>().ImplementedBy<EventPublisher>().LifeStyle.Singleton
                );
            container
                .Register(
                Component.For<IFileFollower>().ImplementedBy<FileFollower>()
                );
            var reset = new AutoResetEvent(false);

            var follower = container.Resolve<IFileFollower>();

            var broker = container.Resolve<IEventAggregator>();
            
            broker.AddListener<FollowerErrorMessage>(message => 
                                                         {
                                                             Assert.IsTrue(message.Error != null);
                                                             if(message.Error != null) reset.Set();
                                                         });
            
            broker.SendMessage(new StartFollowingFileMessage("afilethatshouldntexist.log"));

            reset.WaitOne(2000, false);

        }

        [Test, Ignore("Long running Test..")]
        public void TestFollowerWithExampleLog()
        {
            IWindsorContainer container = new WindsorContainer();
            // need to have the eventaggregator registered completely before other stuff.
            container
                .AddFacility<EventAutoRegistrationFacility>()
                .AddFacility<UnsubscriberInterceptorFacility>()
                .Register(
                Component.For<IContextFactory>().ImplementedBy<ContextFactory>(),
                Component.For<IChannelManager>().ImplementedBy<ChannelManager>().LifeStyle.Singleton,
                Component.For<IListenerDisposerManager>().ImplementedBy<ListenerDisposalManager>().LifeStyle.Singleton,
                Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifeStyle.Singleton,
                Component.For<IPublisher>().ImplementedBy<EventPublisher>().LifeStyle.Singleton
                );
            container
                .Register(
                Component.For<IFileFollower>().ImplementedBy<FileFollower>()
                );
            var reset = new AutoResetEvent(false);

            using (var follower = container.Resolve<IFileFollower>())
            {
                var broker = container.Resolve<IEventAggregator>();

                broker.AddListener<FollowerErrorMessage>(message => Console.WriteLine(message.Error.Message));

                broker.AddListener<FollowerStoppedMessage>(message =>
                                                               {
                                                                   Console.WriteLine("Follower Stopped");
                                                                   reset.Set();
                                                               });

                broker.AddListener<FollowerDataRecievedMessage>(message =>
                                                                Console.WriteLine(message.RecievedDateTime.ToString() +
                                                                                  ":" +
                                                                                  message.Output));

                File.Delete("followerwithexamplelog.txt");
                using (var testLog = new CreateTestLog("followerwithexamplelog.txt"))
                {
                    broker.SendMessage(new StartFollowingFileMessage("followerwithexamplelog.txt"));

                    testLog.StartWritingToTestLog(10);
                    testLog.Stop();
                }

                broker.SendMessage(new StopFollowingFileMessage(follower.Identifier));

            }

            File.Delete("followerwithexamplelog.txt");

            Assert.IsTrue(reset.WaitOne(30000, false));
        }

    }
}