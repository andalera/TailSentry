using Dovecote.Eventing.Attributes;
using Retlang.Fibers;

namespace Dovecote.Eventing.Context
{
    public interface IContextFactory
    {
        IFiber GetContext(ContextType contextType);
        ContextType DefaultContext { get; set; }
    }
}