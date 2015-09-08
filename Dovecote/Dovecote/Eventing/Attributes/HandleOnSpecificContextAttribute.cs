using System;

namespace Dovecote.Eventing.Attributes
{
    public enum ContextType
    {
        Default,
        New,
        Gui
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class HandleOnSpecificContextAttribute : Attribute
    {
        public HandleOnSpecificContextAttribute(ContextType contextType)
        {
            Context = contextType;
        }
        
        public ContextType Context { get; private set; }
    }
}