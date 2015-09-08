using System;
using System.Runtime.Remoting.Messaging;
using Dovecote.Eventing.Attributes;
using Dovecote.Eventing.Interfaces;
using FileMonitoring;

namespace TailSentry.FileFollowing
{
    public interface IFileFollower : IDisposable, 
        IListenFor<StartFollowingFileMessage>, 
        IListenFor<StopFollowingFileMessage>
    {
        Guid Identifier { get; }
        FollowerState State { get; }
        String Path { get; }
    }

    public enum FollowerState
    {
        Idle,
        Started,
        InError,
        Stopped
    }

    public class FileFollower : IFileFollower
    {
        private readonly IPublisher _publisher;
        private readonly Tail _tail;

        public Guid Identifier { get; private set; }
        public FollowerState State { get; private set; }
        public string Path { get; private set; }

        public FileFollower(IPublisher publisher)
        {
            _publisher = publisher;
            Identifier = Guid.NewGuid();
            _tail = new Tail();
            _tail.OutputRecievedEvent += OnOutputRecievedEvent;
            State = FollowerState.Idle;
        }

        void OnOutputRecievedEvent(object sender, OutputRecievedEvent e)
        {
            _publisher.SendMessage(new FollowerDataRecievedMessage(e.Output, Identifier, DateTime.Now));
        }

        [HandleOnSpecificContext(ContextType.New)]
        public void Handle(StartFollowingFileMessage message)
        {
            try
            {
                if(State != FollowerState.Idle) return; //shortcircuit...

                //startup
                State = FollowerState.Started;
                Path = message.Path;
                _publisher.SendMessage(new FollowerStartedMessage(Identifier));
                _tail.Start(message.Path); //will block, which is fine??
            }
            catch (Exception e)
            {
                ErrorHandler(e);
            }
        }

        public void Handle(StopFollowingFileMessage message)
        {
            try
            {
                if (message.Identifier == Identifier && _tail.State == TailState.Started)
                {
                    _tail.Stop();
                    
                    while(_tail.State != TailState.Stopped)
                    {
                        //wait on tail to finish.
                    }

                    State = FollowerState.Stopped;
                    Path = "";
                    _publisher.SendMessage(new FollowerStoppedMessage(Identifier));
                }
            }
            catch(Exception e)
            {
                ErrorHandler(e);
            }
        }

        private void ErrorHandler(Exception e)
        {
            State = FollowerState.InError;
            _publisher.SendMessage(new FollowerErrorMessage(Identifier, e));
        }

        public virtual void Dispose()
        { 
            //make sure tail is stopped...
            State = FollowerState.Stopped;
            _tail.Stop();
        }
    }

    public class FollowerErrorMessage
    {
        public Guid Identifier { get; private set; }
        public Exception Error { get; private set; }
        
        public FollowerErrorMessage(Guid identifier, Exception exception)
        {
            Identifier = identifier;
            Error = exception;
        }

    }

    public class FollowerStoppedMessage
    {
        public Guid Identifier { get; private set; }
        
        public FollowerStoppedMessage(Guid identifier)
        {
            Identifier = identifier;
        }

    }

    public class FollowerStartedMessage
    {
        public Guid Identifier { get; private set; }

        public FollowerStartedMessage(Guid guid)
        {
            Identifier = guid;
        }

    }

    public class StopFollowingFileMessage
    {
        public Guid Identifier { get; private set; }

        public StopFollowingFileMessage(Guid identifier)
        {
            Identifier = identifier;
        }
    }

    public class FollowerDataRecievedMessage
    {
        public DateTime RecievedDateTime { get; private set; }
        public Guid Identifier { get; private set; }
        public string Output { get; private set; }

        public FollowerDataRecievedMessage(string output, Guid identifier, DateTime time)
        {
            Output = output;
            Identifier = identifier;
            RecievedDateTime = time;
        }

    }

    public class StartFollowingFileMessage 
    {
        public string Path { get; private set; }
        
        public StartFollowingFileMessage(string path)
        {
            Path = path;
        }

    }
}