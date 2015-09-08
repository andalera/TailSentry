using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Facilities;
using Dovecote.Eventing.Attributes;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Interfaces;
using Dovecote.Eventing.Managers;
using Dovecote.Eventing.Rules;
using Dovecote.Extensions;
using Retlang.Core;
using Retlang.Fibers;

namespace Dovecote.Eventing.Facilities
{
    /// <summary>
    /// When a component is created in the in Container, this facility runs immediately after.
    /// We check to see if the new instance implements certain generic event handler types we've defined and register them with the broker if found
    /// </summary>
    public class EventAutoRegistrationFacility : AbstractFacility
    {
        private IEventAggregator _broker;

        protected override void Init()
        {
            Kernel.ComponentCreated += OnComponentCreated;
        }

        private void OnComponentCreated(Castle.Core.ComponentModel model, object instance)
        {
            //test to make sure the instance implements IListenFor<T> (T will be an actual Type)
            if (!instance.Implements(typeof (IListen)))
            {
                return;
            }

            if (_broker == null) _broker = Kernel.Resolve<IEventAggregator>();

            //We have to get a reference to the ConvertToHandlerAndRegister method below.  we're gonna
            //use it to pass the T part of IListenFor<T>.  We have to do this because there's no other way 
            //to actually pass the actual T type that I know of.. See the next comment for more...
            var converterMethod = GetType().GetMethods()
                .Where(m => m.Name == "ConvertToHandlerAndRegister")
                //.Where(m => m.GetParameters().Length == 3)
                .First();

            //retrieve the T types implemented by IListenFor<T> and register each one.  
            //for instance, if instance implements IListenFor<AbcMessage>, then the call will be:
            // ConvertToHandlerAndRegister<AbcMessage>(model, instance)
            //We're actually calling the method with the correct type T for the handler.
            //Yes, this means that IListenFor<T> is just a marker to assemble all messages under...  
            //sneaky, yes?

            foreach (var type in instance.GetDeclaredTypesForGeneric(typeof(IListenFor<>)))
            {
                converterMethod.MakeGenericMethod(new[] {type})
                    .Invoke(this, new[] {model, instance, type, typeof(IListenFor<>)});
            }

            foreach (var type in instance.GetDeclaredTypesForGeneric(typeof(IListenForInBatches<>)) )
            {
                //converterMethod.MakeGenericMethod(typeof (IList<>).MakeGenericType(new[] {type}))
                //    .Invoke(this, new[] {model, instance, type, typeof(IListenForInBatches<>)});
                converterMethod.MakeGenericMethod(new[]{type})
                    .Invoke(this, new[] { model, instance, type, typeof(IListenForInBatches<>) });
            }
        }

        /// <summary>
        /// This method is not called directly.. it is called *indirectly* through reflection so that T types can be passed to it correctly.
        /// </summary>
        /// <typeparam name="T">The type of Handler Message</typeparam>
        /// <param name="model">Castle Component Model</param>
        /// <param name="instance">Instance just created from container</param>
        /// <param name="listenerType">Type of Listener that is being processed.</param>
        /// <param name="declaredType">The enumerated type from getdeclaredtypesforgeneric</param>
        public void ConvertToHandlerAndRegister<T>(Castle.Core.ComponentModel model, object instance, Type declaredType, Type listenerType)
        {
            if(listenerType == typeof(IListenFor<>))
            {
                var context = GetExecutionContext<T>(model.Implementation);
                var disposer = AddListenerToEventAggregator((IListenFor<T>)instance, _broker, context);
                HandleDisposer(model, disposer);
            }

            if(listenerType == typeof(IListenForInBatches<>))
            {
                var context = GetExecutionContext<IList<T>>(model.Implementation);
                BatchOptionsAttribute batchOptionsAttribute = GetBatchOptionsAttribute<IList<T>>(model.Implementation);
                var interval = 10;
                interval = batchOptionsAttribute != null ? batchOptionsAttribute.Interval : 10;
                var disposer = AddBatchedListenerToEventAggregator((IListenForInBatches<T>) instance, _broker, context,
                                                                   interval);
                HandleDisposer(model, disposer);
                
            }

        }

        private void HandleDisposer(ComponentModel model, IDisposable disposer)
        {
            if (Kernel.HasComponent(typeof(IListenerDisposerManager)))
            {
                var listenerDisposerManager = Kernel.Resolve<IListenerDisposerManager>();

                if (new ListenerSpec().And(new DisposableSpec()).IsSatisfiedBy(model.Implementation))
                {
                    listenerDisposerManager.AddListenerToRemoveLater(model.Implementation, (IUnsubscriber)disposer);
                }
            }
        }

        private IDisposable AddBatchedListenerToEventAggregator<T>(IListenForInBatches<T> listener, IEventAggregator broker, ContextType contextType, int interval)
        {
            IDisposable disposer = null;
            if (broker != null)
            {
                if (contextType == ContextType.Default)
                {
                    disposer = broker.AddBatchedListener<T>(listener.Handle, interval);
                }
                else
                {
                    //IExecutionContext executionContext = new ExecutionContextFactory().GetContextByType(context);
                    IFiber fiber = Kernel.Resolve<IContextFactory>().GetContext(contextType);
                    disposer = broker.AddBatchedListener<T>(fiber, listener.Handle, interval);
                }
            }
            return disposer;
        }

        private IDisposable AddListenerToEventAggregator<T>(IListenFor<T> listener, IEventAggregator broker, ContextType contextType)
        {
            IDisposable disposer = null;
            if (broker != null)
            {
                if (contextType == ContextType.Default)
                {
                    disposer = broker.AddListener<T>(listener.Handle);
                }
                else
                {
                    //IExecutionContext executionContext = new ExecutionContextFactory().GetContextByType(context);
                    IFiber fiber = Kernel.Resolve<IContextFactory>().GetContext(contextType);
                    disposer = broker.AddListener<T>(fiber, listener.Handle);
                }
            }
            return disposer;
        }

        private static BatchOptionsAttribute GetBatchOptionsAttribute<T>(Type implementationType)
        {
            Type t = implementationType;
            var batchingMethods = t.GetMethods()
                .Where(method => method.Name.ToLower() == "handle")
                .Where(method =>
                       method.GetCustomAttributes(typeof (BatchOptionsAttribute), true)
                           .Any(obj => obj is BatchOptionsAttribute));
                        
            var hasbatchingoptions = batchingMethods.Any(
                method => method.GetParameters().First().ParameterType == typeof (T));
            
            BatchOptionsAttribute attribute = null;

            if(hasbatchingoptions)
            {
                attribute = (BatchOptionsAttribute) batchingMethods
                                                        .Single(method =>
                                                                method.GetParameters()
                                                                    .First()
                                                                    .ParameterType == typeof (T))
                                                        .GetCustomAttributes(typeof (BatchOptionsAttribute), true)
                                                        .First();
            }

            return attribute;

        }

        private static ContextType GetExecutionContext<T>(Type implementationType)
        {
            ContextType defaultContext = ContextType.Default;

            //check to see if the class itself needs a new context.
            System.Reflection.MemberInfo info = implementationType;
            var attributes = info.GetCustomAttributes(typeof (HandleOnSpecificContextAttribute), true);
            if(attributes.Count() == 1)
            {
                //we have a marker on the class itself.. need to change defaultContext now.
                defaultContext = ((HandleOnSpecificContextAttribute) attributes[0]).Context;
            }


            //handle cases where a method needs it's own context. (long running methods)
            //All Handle methods have one parameter, the message.  
            //the type of this message is T, passed above.
            //we'll see if the attribute HandleOnNewContext is set on any of the methods in instance, then
            //compare T to the parameter type and see if we have a match.

            //gotta first grab all the methods that are attributed with HandleOnAnotherContext.
            Type t = implementationType; //listener.GetType();
            var methodsThatNeedNewContext = t.GetMethods()
                .Where(method => method.Name.ToLower() == "handle")
                .Where(method =>
                       method.GetCustomAttributes(typeof(HandleOnSpecificContextAttribute), true)
                           .Any(obj => obj is HandleOnSpecificContextAttribute));

            //Then we check to see if the method containing signature of <T> needs new context.
            //We know that all Handle Methods will have <T> as the first parameter.
            //check to see if any of the methods match <T> as a parameter type.
            //for instance, The handle method for IListenFor<MyMessage> will be Handle(MyMessage message)
            //since only one handler of a specific type can be in a class, we can find if it has attributes by checking its parameters against <T>.
            //confusing, yes? this whole thing is smelly? Yes? Not really, but kinda...
            var methodNeedsNewContext = methodsThatNeedNewContext.Any(
                method => method.GetParameters().First().ParameterType == typeof(T));

            if (methodNeedsNewContext)
            {
                var attribute = (HandleOnSpecificContextAttribute)methodsThatNeedNewContext
                                                                      .Single(method =>
                                                                              method.GetParameters().
                                                                                  First().
                                                                                  ParameterType == typeof(T))
                                                                      .GetCustomAttributes(typeof(HandleOnSpecificContextAttribute), true)
                                                                      .First();  //should be only one attribute of type HandleOnSpecificContextAttribute on any given method.

                return attribute.Context;
            }

            return defaultContext;

        }

    }
}