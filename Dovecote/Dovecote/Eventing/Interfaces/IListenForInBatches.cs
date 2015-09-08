using System.Collections.Generic;

namespace Dovecote.Eventing.Interfaces
{
    public interface IListenForInBatches<T> : IListen
    {
        void Handle(IList<T> messages);
    }
}