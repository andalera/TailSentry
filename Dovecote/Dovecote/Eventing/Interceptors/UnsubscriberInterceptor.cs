using Castle.Core.Interceptor;
using Castle.MicroKernel;
using Dovecote.Eventing.Managers;

namespace Dovecote.Eventing.Interceptors
{
    public class UnsubscriberInterceptor : IInterceptor
    {
        private readonly IKernel _kernel;

        public UnsubscriberInterceptor(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Intercept(IInvocation invocation)
        {
            if(_kernel.HasComponent(typeof (IListenerDisposerManager)))
            {
                if(invocation.Method.Name.ToLower() == "dispose")
                {
                    var listenerDisposer = _kernel.Resolve<IListenerDisposerManager>();
                    //listenerDisposer.RemoveListenersFor(invocation.Method.DeclaringType);
                    listenerDisposer.RemoveListenersFor(invocation.TargetType);
                }
            }
            invocation.Proceed();
        }
    }
}