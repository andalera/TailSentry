using System;
using System.Collections.Generic;

namespace Dovecote.Extensions
{
    public static class TypeExtensions
    {   
        //from here to end of class,
        //from http://dimebrain.com/2008/12/how-to-determine-generic-inheritance-and-generic-parameters-at-runtime.html
        public static bool Implements(this object instance, Type interfaceType)
        {
            return interfaceType.IsGenericTypeDefinition
                       ? instance.ImplementsGeneric(interfaceType)
                       : interfaceType.IsAssignableFrom(instance.GetType());
        }

        public static bool Implements<T>(this object instance)
        {
            var interfaceType = typeof(T);
            return instance.Implements(interfaceType);
        }

        public static bool Implements<T>(this object instance, T interfaceType)
        {
            return instance.Implements<T>();
        }

        public static bool Implements<T>(this Type type, T interfaceType)
        {
            return Implements<T>(type);
        }

        public static bool Implements<T>(this Type type)
        {
            var interfaceType = typeof(T);
            return interfaceType.IsGenericTypeDefinition
                       ? type.ImplementsGeneric(interfaceType)
                       : interfaceType.IsAssignableFrom(type);
        }

        public static Type GetDeclaredTypeForGeneric(this object instance, Type interfaceType)
        {
            return instance.GetType().GetDeclaredTypeForGeneric(interfaceType);
        }

        public static Type GetDeclaredTypeForGeneric<T>(this object instance)
        {
            var interfaceType = typeof(T);
            return instance.GetDeclaredTypeForGeneric(interfaceType);
        }

        public static Type GetDeclaredTypeForGeneric<T>(this object instance, T interfaceType)
        {
            return instance.GetDeclaredTypeForGeneric(typeof(T));
        }

        public static Type GetDeclaredTypeForGeneric<T>(this Type type)
        {
            var interfaceType = typeof(T);
            return type.GetDeclaredTypeForGeneric(interfaceType);
        }

        public static Type GetDeclaredTypeForGeneric<T>(this Type type, T interfaceType)
        {
            return type.GetDeclaredTypeForGeneric(typeof(T));
        }

        public static Type GetDeclaredTypeForGeneric(this Type baseType, Type interfaceType)
        {
            var type = default(Type);

            if (baseType.ImplementsGeneric(interfaceType))
            {
                var generic = baseType.GetInterface(interfaceType.FullName);
                if (generic.IsGenericType)
                {
                    var args = generic.GetGenericArguments();
                    if (args.Length == 1)
                    {
                        type = args[0];
                    }
                }
            }

            return type;
        }

        public static IEnumerable<Type> GetDeclaredTypesForGeneric(this object instance, Type interfaceType)
        {
            return instance.GetType().GetDeclaredTypesForGeneric(interfaceType);
        }

        public static IEnumerable<Type> GetDeclaredTypesForGeneric<T>(this object instance)
        {
            var interfaceType = typeof(T);
            return instance.GetType().GetDeclaredTypesForGeneric(interfaceType);
        }

        public static IEnumerable<Type> GetDeclaredTypesForGeneric<T>(this object instance, T interfaceType)
        {
            return instance.GetDeclaredTypesForGeneric<T>();
        }

        public static IEnumerable<Type> GetDeclaredTypesForGeneric<T>(this Type type)
        {
            var interfaceType = typeof(T);
            return type.GetDeclaredTypesForGeneric(interfaceType);
        }

        public static IEnumerable<Type> GetDeclaredTypesForGeneric<T>(this Type type, T interfaceType)
        {
            return type.GetDeclaredTypesForGeneric(typeof(T));
        }

        public static IEnumerable<Type> GetDeclaredTypesForGeneric(this Type type, Type interfaceType)
        {
            foreach (var generic in type.GetGenericInterfacesFor(interfaceType))
            {
                foreach (var arg in generic.GetGenericArguments())
                {
                    yield return arg;
                }
            }
        }

        private static IEnumerable<Type> GetGenericInterfacesFor(this Type type, Type interfaceType)
        {
            var candidates = type.GetInterfaces();
            foreach (var candidate in candidates)
            {
                if (!candidate.IsGenericType)
                {
                    continue;
                }

                var definition = candidate.GetGenericTypeDefinition();
                if (definition == interfaceType)
                {
                    yield return candidate;
                }
            }
        }

        private static bool ImplementsGeneric(this Type type, Type targetType)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }

                if (interfaceType.GetGenericTypeDefinition() == targetType)
                {
                    return true;
                }
            }

            var baseType = type.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return baseType.IsGenericType
                       ? baseType.GetGenericTypeDefinition() == targetType
                       : baseType.ImplementsGeneric(targetType);
        }

        private static bool ImplementsGeneric(this object instance, Type targetType)
        {
            return instance.GetType().ImplementsGeneric(targetType);
        }
    }
}