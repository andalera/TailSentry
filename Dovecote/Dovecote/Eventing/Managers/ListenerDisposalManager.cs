using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Retlang.Core;

namespace Dovecote.Eventing.Managers
{
    public class ListenerDisposalManager : IListenerDisposerManager
    {
        private readonly object _lock = new object();
        private readonly IKernel _kernel;
        private readonly Dictionary<Type, List<IUnsubscriber>> _disposers = new Dictionary<Type, List<IUnsubscriber>>();

        public ListenerDisposalManager(IKernel kernel)
        {
            _kernel = kernel;
        }

        public Dictionary<Type, List<IUnsubscriber>> AllDisposers
        {
            get { lock(_lock){return _disposers;} }
        }

        public void AddListenerToRemoveLater(Type type, IUnsubscriber unsubscriber)
        {
            lock (_lock)
            {
                if (!AllDisposers.ContainsKey(type))
                {
                    AllDisposers.Add(type, new List<IUnsubscriber>());
                }
                AllDisposers[type].Add(unsubscriber);
            }
        }

        public void RemoveListenersFor(Type type)
        {
            lock (_lock)
            {
                if (!AllDisposers.ContainsKey(type)) return;

                AllDisposers[type].ForEach(unsubscriber => unsubscriber.Dispose());
                AllDisposers.Remove(type);
            }
        }
    }
}