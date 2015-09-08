using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dovecote.Eventing.Aggregator;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Facilities;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;

namespace Dovecote
{
    public class DovecoteInstaller: IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .AddFacility<EventAutoRegistrationFacility>()
                .AddFacility<UnsubscriberInterceptorFacility>()
                .Register(
                    Component.For<IChannelManager>().ImplementedBy<ChannelManager>().LifeStyle.Singleton,
                    Component.For<IListenerDisposerManager>().ImplementedBy<ListenerDisposalManager>().LifeStyle.Singleton,
                    Component.For<IEventAggregator>().ImplementedBy<EventAggregator>(),
                    Component.For<IPublisher>().ImplementedBy<EventPublisher>(),
                    Component.For<IContextFactory>().ImplementedBy<ContextFactory>()
                );
        }
    }
}