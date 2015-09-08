using System;
using Dovecote.Specification;

namespace Dovecote.Eventing.Rules
{
    public class DisposableSpec : Specification<Type>
    {
        public override bool IsSatisfiedBy(Type obj)
        {
            return typeof (IDisposable).IsAssignableFrom(obj);
        }
    }
}