using System;
using Dovecote.Eventing.Interfaces;
using Dovecote.Extensions;
using Dovecote.Specification;

namespace Dovecote.Eventing.Rules
{
    public class ListenerSpec : Specification<Type>
    {
        public override bool IsSatisfiedBy(Type obj)
        {
            //return obj.GetInterfaces().Any(x =>
            //                               x.IsGenericType &&
            //                               x.GetGenericTypeDefinition() == typeof(IListenFor<>));

            //return obj.GetInterfaces().Any(x => x.Implements(IListen));

            return obj.Implements<IListen>();
        }
    }
}