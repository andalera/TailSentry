using Castle.Core;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Dovecote.Eventing.Interceptors;
using Dovecote.Eventing.Rules;

namespace Dovecote.Eventing.Facilities
{
    public class UnsubscriberInterceptorFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(Component.For<UnsubscriberInterceptor>());
            Kernel.ComponentModelCreated += OnComponentModelCreated;
        }

        void OnComponentModelCreated(ComponentModel model)
        {
            if (new ListenerSpec().And(new DisposableSpec()).IsSatisfiedBy(model.Implementation))
            {
                model.Interceptors.Add(new InterceptorReference(typeof(UnsubscriberInterceptor)));
            }
        }
    }
}