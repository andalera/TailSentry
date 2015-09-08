using System;
using System.Collections.Generic;
using Retlang.Channels;

namespace Dovecote.Eventing.Managers
{
    public class ChannelManager : IChannelManager
    {
        private readonly Dictionary<Type, object> _channels = new Dictionary<Type, object>();

        public Dictionary<Type, object> AllChannels
        {
            get { return _channels; }
        }

        public IChannel<T> GetChannel<T>()
        {
            Type passedType = typeof (IChannel<T>);
            
            if(!AllChannels.ContainsKey(passedType))
            {
                var channel = new Channel<T>();
                AllChannels.Add(passedType, channel);
            }

            return (IChannel<T>)AllChannels[passedType];
        }



    }
}