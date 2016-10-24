using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Verno
{
    public static class EnumerableExtensions
    {
        public static IList ToList(this IEnumerable source, Type itemType)
        {
            Type enumerableType = typeof(Enumerable);
            MethodInfo methodTemplate = enumerableType.GetMethod("ToList");
            MethodInfo method = methodTemplate.MakeGenericMethod(itemType);
            return (IList) method.Invoke(null, new object[]{source});
        }

        /// <summary>
        /// В случае, если условие <param name="condition">condition</param> соблюдается, то вызывается функция <param name="conversion">conversion</param>,
        /// которая возвращает свой результат, являющийся результатом IfThen. Если <param name="condition">condition</param> не удовлетворяет условию,
        /// то возвращается <param name="source">source</param>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        public static IEnumerable<T> IfThen<T>(this IEnumerable<T> source, Predicate<IEnumerable<T>> condition, Func<IEnumerable<T>, IEnumerable<T>> conversion)
        {
            if (condition(source))
                return conversion(source);
            else
                return source;
        }

        public static IList<T> ToList<T>(this IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }
    }
}