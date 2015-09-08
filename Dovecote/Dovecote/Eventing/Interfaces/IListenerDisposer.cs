using System;

namespace Dovecote.Eventing.Interfaces
{
    /// <summary>
    /// Interface denoting that Listeners will be cleaned up when the implementing class is GC'd
    /// </summary>
    public interface IListenerDisposer : IDisposable
    {
    }
}