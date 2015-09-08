using System;
using System.Collections.Generic;

namespace Tests
{
    public static class Extensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
            return source;
        }

        //public static IList<T> Each<T>(this IList<T> list, Action<T> action)
        //{
        //    if (list == null) return null;

        //    foreach (T t in list)
        //    {
        //        action(t);
        //    }
        //    return list;
        //}

    }
}