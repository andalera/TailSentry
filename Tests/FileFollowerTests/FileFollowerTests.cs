using System;
using System.Collections.Generic;
using System.Threading;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dovecote;
using Dovecote.Eventing.Aggregator;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Facilities;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;
using TailSentry.FileFollowing;

namespace Tests.FileFollowerTests
{
    [TestFixture]
    public class FileFollowerTests
    {
        [Test]
        public void TestLongRunningConceptWithRetlang()
        {
            var longrunningContext = new PoolFiber();
            longrunningContext.Start();
            var regularContext = new PoolFiber();
            regularContext.Start();

            var startFollowingFileChannel = new Channel<StartFollowingFileMessage>();
            var stopFollowingFileChannel = new Channel<StopFollowingFileMessage>();
            var followerStartedChannel = new Channel<FollowerStartedMessage>();
            var followerStoppedChannel = new Channel<FollowerStoppedMessage>();
            var longrunningTest = new LongRunningTest(followerStartedChannel, followerStoppedChannel);

            //this one runs in it's own context cause it won't end until a flag is set.
            startFollowingFileChannel.Subscribe(longrunningContext, longrunningTest.Handle);
            //this is registered on another context because if it's on the longrunning one, it won't ever run.
            stopFollowingFileChannel.Subscribe(regularContext, longrunningTest.Handle);

            var reset = new AutoResetEvent(false);
            Guid identifier;

            followerStartedChannel.Subscribe(regularContext, message =>
                                                                 {
                                                                     Console.WriteLine("Follower Started" + message.Identifier);
                                                                     identifier = message.Identifier;
                                                                     stopFollowingFileChannel.Publish(new StopFollowingFileMessage(identifier));
                                                                 });
            followerStoppedChannel.Subscribe(regularContext, message =>
                                                                 {
                                                                     Console.WriteLine("Follower stopped" + message.Identifier);
                                                                     reset.Set();
                                                                 });

            startFollowingFileChannel.Publish(new StartFollowingFileMessage(@"ExampleFiles/ExampleLogFiles/LotroLog.txt"));

            Assert.IsTrue(reset.WaitOne(5000, false));

        }

        public class LongRunningTest
        {
            private readonly Channel<FollowerStartedMessage> _followerStartedChannel;
            private readonly Channel<FollowerStoppedMessage> _followerStoppedChannel;
            private Guid _identifier = Guid.NewGuid();

            public LongRunningTest(Channel<FollowerStartedMessage> followerStartedChannel,
                                   Channel<FollowerStoppedMessage> followerStoppedChannel)
            {
                _followerStartedChannel = followerStartedChannel;
                _followerStoppedChannel = followerStoppedChannel;
            }

            bool _keepRunning = true;
            public void Handle(StartFollowingFileMessage message)
            {
                Console.WriteLine("In Start Handler");
                _followerStartedChannel.Publish(new FollowerStartedMessage(_identifier));
                StartLongRunningCode();
            }

            private void StartLongRunningCode()
            {
                while (_keepRunning)
                {
                    Thread.Sleep(1000);
                }
            }

            public void Handle(StopFollowingFileMessage message)
            {
                Console.WriteLine("In Stop Handler");
                _keepRunning = false;
                _followerStoppedChannel.Publish(new FollowerStoppedMessage(_identifier));
            }
        }

        [Test]
        public void TestFileFollowerwithEventAggregator()
        {
            var broker = new EventAggregator(new ChannelManager(), new ContextFactory());
            var publisher = new EventPublisher(broker);
            var filefollower = new FileFollower(publisher);
            Guid identifier = Guid.Empty;

            var longrunningcontext = new PoolFiber();
            longrunningcontext.Start();

            //register with longrunning context.
            broker.AddListener<StartFollowingFileMessage>(longrunningcontext, filefollower.Handle);

            //register with normal context.
            broker.AddListener<StopFollowingFileMessage>(filefollower.Handle);

            var reset = new AutoResetEvent(false);
            broker.AddListener<FollowerStartedMessage>(message =>
                                                           {
                                                               Console.WriteLine("Follower Started" + message.Identifier);
                                                               identifier = message.Identifier;
                                                               broker.SendMessage(new StopFollowingFileMessage(identifier));
                                                           });
            broker.AddListener<FollowerStoppedMessage>(message =>
                                                           {
                                                               if (message.Identifier == identifier)
                                                               {
                                                                   Console.WriteLine("Follower stopped" + message.Identifier);
                                                                   reset.Set();
                                                               }
                                                           });

            publisher.SendMessage(new StartFollowingFileMessage(@"ExampleFiles/ExampleLogFiles/LotroLog.txt"));
            Assert.IsTrue(reset.WaitOne(5000, false));

        }

        [Test]
        public void TestFileFollowerWithAutoRegistrationAndWindsor()
        {
            IWindsorContainer container = new WindsorContainer();
            // need to have the eventaggregator registered completely before other stuff.
            //container
            //    .AddFacility<EventAutoRegistrationFacility>()
            //    .AddFacility<UnsubscriberInterceptorFacility>()
            //    .Register(
            //    Component.For<IContextFactory>().ImplementedBy<ContextFactory>(),
            //    Component.For<IChannelManager>().ImplementedBy<ChannelManager>().LifeStyle.Singleton,
            //    Component.For<IListenerDisposerManager>().ImplementedBy<ListenerDisposalManager>().LifeStyle.Singleton,
            //    Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifeStyle.Singleton,
            //    Component.For<IPublisher>().ImplementedBy<EventPublisher>().LifeStyle.Singleton
            //    );

            container.Install(new DovecoteInstaller());

            container
                .Register(
                Component.For<IFileFollower>().ImplementedBy<FileFollower>()
                );

            var follower = container.Resolve<IFileFollower>(); //it should do the right thing!

            var broker = container.Resolve<IEventAggregator>();
            var reset = new AutoResetEvent(false);
            Guid identifier = Guid.Empty;
            broker.AddListener<FollowerStartedMessage>(message =>
                                                           {
                                                               Console.WriteLine("Follower Started" + message.Identifier);
                                                               identifier = message.Identifier;
                                                               broker.SendMessage(new StopFollowingFileMessage(identifier));
                                                           });
            broker.AddListener<FollowerStoppedMessage>(message =>
                                                           {
                                                               if (message.Identifier == identifier)
                                                               {
                                                                   Console.WriteLine("Follower stopped" + message.Identifier);
                                                                   reset.Set();
                                                               }
                                                           });

            broker.SendMessage(new StartFollowingFileMessage(@"ExampleFiles/ExampleLogFiles/LotroLog.txt"));

            Assert.IsTrue(reset.WaitOne(5000, false));

        }

        [Test]
        public void TypeFactoryTest()
        {
            IWindsorContainer container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();
            container.Install(new DovecoteInstaller());

            container.Register(
                Component.For<IFileFollowerFactory>().AsFactory(),
                Component.For<IFileFollower>().ImplementedBy<FileFollower>().LifeStyle.Transient
            );

            var factory = container.Resolve<IFileFollowerFactory>();
            var follower = factory.FetchFollower();

            Console.WriteLine(follower.Identifier.ToString());

            Assert.IsNotNull(follower);

        }

        [Test]
        public void TestFollowerManager()
        {
            
            //Todo: implement Follower manager test

        }

    }

    public interface IFileFollowerFactory
    {
        IFileFollower FetchFollower();
    }

    public class LogFollowerManager
    {
        private readonly IFileFollowerFactory _followerFactory;
        private readonly IPublisher _publisher;
        private readonly IDictionary<string, IFileFollower> _followers;

        public LogFollowerManager(IFileFollowerFactory followerFactory, IPublisher publisher)
        {
            _followers = new Dictionary<string, IFileFollower>();
            _followerFactory = followerFactory;
            _publisher = publisher;
        }

        public void Handle(CreateNewFollowerMessage message)
        {
            if(!_followers.ContainsKey(message.Path))
            {
                _followers.Add(message.Path, _followerFactory.FetchFollower());
            }
            else
            {
                _publisher.SendMessage(new FollowerAlreadyExistsMessage(message.Path, _followers[message.Path].Identifier));
            }
        }
            
        public void Handle(StartLogFollowerMessage message)
        {
            _publisher.SendMessage(new StartFollowingFileMessage(message.Path));
        }

        public void Handle(StopFollowingSpecificLogMessage message)
        {
            
        }

    }

    public class StopFollowingSpecificLogMessage
    {
        public string Path { get; private set; }

        public StopFollowingSpecificLogMessage(string path)
        {
            Path = path;
        }
    }

    public class StartLogFollowerMessage
    {
        public StartLogFollowerMessage(string path)
        {
            Path = path;
        }

        public string Path{ get; private set; }
    }

    public class FollowerAlreadyExistsMessage
    {
        public string Path { get; private set; }
        public Guid Identifier { get; private set; }

        public FollowerAlreadyExistsMessage(string path, Guid identifier)
        {
            Path = path;
            Identifier = identifier;
        }
    }

    public class CreateNewFollowerMessage
    {
        public CreateNewFollowerMessage(string path)
        {
            Path = path;
        }

        public string Path
        {
            get; private set;
        }
    }
}