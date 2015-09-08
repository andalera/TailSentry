using System;
using System.Collections.Generic;
using Retlang.Core;

namespace Dovecote.Eventing.Managers
{
    public interface IListenerDisposerManager
    {
        void RemoveListenersFor(Type type);
        void AddListenerToRemoveLater(Type type, IUnsubscriber unsubscriber);
        Dictionary<Type, List<IUnsubscriber>> AllDisposers { get; }
    }
}