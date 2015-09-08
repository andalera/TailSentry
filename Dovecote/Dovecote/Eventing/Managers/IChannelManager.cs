using System;
using System.Collections.Generic;
using Retlang.Channels;

namespace Dovecote.Eventing.Managers
{
    public interface IChannelManager
    {
        IChannel<T> GetChannel<T>();
        Dictionary<Type, object> AllChannels { get; }
    }
}