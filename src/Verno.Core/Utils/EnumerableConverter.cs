using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Verno
{
    public static class EnumerableConverter
    {
#if NET35
        public static IEnumerable Convert<TElement, TResultElement>(this IEnumerable<TElement> value, Type conversionType, IValueConverter converter, CultureInfo culture)
#else
        public static IEnumerable Convert<TElement, TResultElement>(this IEnumerable<TElement> value, Type conversionType, IValueConverter converter = null, CultureInfo culture = null)
#endif
        {
            if (value == null) throw new ArgumentNullException("value");
            if (conversionType == null) throw new ArgumentNullException("conversionType");
            if (!typeof(IEnumerable).IsAssignableFrom(conversionType)) throw new ArgumentException("Target conversion type must be derived from IEnumerable.");

            if (culture == null)
                culture = CultureInfo.CurrentCulture;
            if (converter == null)
                converter = new SimpleValueConverter();

            Type targetElementType = typeof(TResultElement);
            var resultEnum = value.Select(i => (TResultElement)converter.Convert(i, targetElementType, culture));
            if (conversionType.IsArray)
                return resultEnum.ToArray();
            else if (conversionType.IsAssignableFrom(typeof(List<TResultElement>)))
                return resultEnum.ToList();
            else if (typeof(IList).IsAssignableFrom(conversionType))
            {
                var list = (IList)Activator.CreateInstance(conversionType);
                foreach (var item in resultEnum)
                    list.Add(item);
                return list;
            }
            else if (typeof(ICollection<TResultElement>).IsAssignableFrom(conversionType))
            {
                var list = (ICollection<TResultElement>)Activator.CreateInstance(conversionType);
                foreach (var item in resultEnum)
                    list.Add(item);
                return list;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(conversionType))
            {
                return resultEnum;
            }
            throw new NotSupportedException(string.Format("Cast to {0} is not supported.", conversionType.Name));
        }

#if NET35
        public static TResult Convert<TElement, TResult>(this IEnumerable<TElement> value, IValueConverter converter, CultureInfo culture)
#else
        public static TResult Convert<TElement, TResult>(this IEnumerable<TElement> value, IValueConverter converter = null, CultureInfo culture = null)
#endif
            where TResult : IEnumerable
        {
            if (value == null) throw new ArgumentNullException("value");

            var first = value.Cast<object>().FirstOrDefault(i => i != null);
            if (first == null) return Activator.CreateInstance<TResult>();

            Type elemType = first.GetType();
            Type targetElementType = GetElementType(typeof(TResult)) ?? elemType;
            var castMethod = typeof(EnumerableConverter).GetMethods()
                .Single(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 4)
                .MakeGenericMethod(elemType, targetElementType);
            return (TResult)castMethod.Invoke(null, new object[] { value, typeof(TResult), converter, culture });            
        }

#if NET35
        public static IEnumerable Convert(IEnumerable value, Type conversionType, IValueConverter converter, CultureInfo culture)
#else
        public static IEnumerable Convert(IEnumerable value, Type conversionType, IValueConverter converter = null, CultureInfo culture = null)
#endif
        {
            if (value == null) throw new ArgumentNullException("value");
            if (conversionType == null) throw new ArgumentNullException("conversionType");
            if (!typeof(IEnumerable).IsAssignableFrom(conversionType)) throw new ArgumentException("Target conversion type must be derived from IEnumerable.");

            var first = value.Cast<object>().FirstOrDefault(i => i != null);
            if (first == null) return (IEnumerable) Activator.CreateInstance(conversionType);

            Type elemType = first.GetType();
            Type targetElementType = GetElementType(conversionType) ?? elemType;
            var castMethod = typeof (EnumerableConverter).GetMethods()
                .Single(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 4)
                .MakeGenericMethod(elemType, targetElementType);
            return (IEnumerable)castMethod.Invoke(null, new object[] { value, conversionType, converter, culture });
        }

        private static Type GetElementType(Type resultType)
        {
            if (resultType.IsArray)
                return resultType.GetElementType();
            else if (resultType.IsGenericType)
                return resultType.GetGenericArguments()[0];
            else
                return null;
        }
    }
}