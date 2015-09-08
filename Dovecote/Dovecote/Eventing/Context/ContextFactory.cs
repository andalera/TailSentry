using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Castle.MicroKernel;
using Dovecote.Eventing.Attributes;
using Retlang.Core;
using Retlang.Fibers;

namespace Dovecote.Eventing.Context
{
    public class ContextFactory : IContextFactory
    {
        private SynchronizationContext _syncContext;
        private SynchronizationFiber _syncfiber;

        private MarshalingControl marshaler = null;
        private FormFiber formFiber = null;

        //private IList<IFiber> poolfibers = null;

        public ContextFactory()
        {
            //poolfibers = new List<IFiber>();

            _syncfiber = null;
            _syncContext = null;
            DefaultContext = ContextType.New;
        }

        public ContextType DefaultContext { get; set; }

        public IFiber GetContext(ContextType contextType)
        {
            IFiber fiber = null;
            switch (contextType)
            {
                case ContextType.New:
                    fiber = new PoolFiber();
                    fiber.Start();
                    //poolfibers.Add(fiber);
                    break;
                case ContextType.Gui:
                    //if(formFiber == null)
                    //{
                    //    if (marshaler == null) marshaler = new MarshalingControl();    
                    //    formFiber = new FormFiber(marshaler, new BatchAndSingleExecutor());
                    //    formFiber.Start();
                    //}

                    //fiber = formFiber;

                    fiber = GetStartedGuiFiber();
                    break;
            }
            

            return fiber;
        }
        
        private IFiber GetStartedGuiFiber()
        {
            //this actually needs to have IKernel pulled into the constructor.
            //so a gui thread resolver can be resolved out of the kernel.
            //if it's null, then throw GuiNotStarted Exception that states that <something> must be provided to
            //grab the correct thread.
            
            //this needs to be changed...

            if(_syncContext== null)
            {
                using (new Form())
                {
                    _syncContext = SynchronizationContext.Current;
                }
            }

            if(_syncfiber == null)
            {
                _syncfiber = new SynchronizationFiber(_syncContext, new BatchAndSingleExecutor());
                _syncfiber.Start();
            }

            return _syncfiber;
            
        }
    }
}