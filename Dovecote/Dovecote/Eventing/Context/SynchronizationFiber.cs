using System;
using System.Threading;
using Retlang.Core;
using Retlang.Fibers;

namespace Dovecote.Eventing.Context
{
    public class SynchronizationFiber : BaseFiber
    {
        /// <summary>
        /// Creates an instance.
        /// </summary>
        public SynchronizationFiber(SynchronizationContext context, IBatchAndSingleExecutor executor)
            : base(new GuiAdapter(context), executor)
        {
        }
        
    }

    internal class GuiAdapter : IThreadAdapter
    {
        private readonly SynchronizationContext _invoker;

        public GuiAdapter(SynchronizationContext invoker)
        {
            _invoker = invoker;
        }

        public void Invoke(Action action)
        {
            //_invoker.Send(delegate { action(); }, null);
            _invoker.Post(delegate { action(); }, null);
        }

    }
}