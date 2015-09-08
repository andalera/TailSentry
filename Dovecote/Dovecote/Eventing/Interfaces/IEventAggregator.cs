using System;
using System.Collections.Generic;
using Retlang.Fibers;

namespace Dovecote.Eventing.Interfaces
{
    public interface IEventAggregator
    {
        // Sending messages
        void SendMessage<T>(T message);

        //void SendMessage<T>() where T : new(); //we'll always send a message?

        // Explicit registration
        //void AddListener(object listener);
        IDisposable AddListener<T>(Action<T> @delegate);
        IDisposable AddListener<T>(IFiber context, Action<T> @delegate);
        IDisposable AddBatchedListener<T>(Action<IList<T>> action, int interval);
        IDisposable AddBatchedListener<T>(IFiber context, Action<IList<T>> action, int interval);
    }
}