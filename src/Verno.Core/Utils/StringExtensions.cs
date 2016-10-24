using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace Verno
{
    public static class StringExtensions
    {
        public static string ReadWord(this string sourceString, ref int startIdx)
        {
            return ReadWhile(sourceString, ref startIdx, char.IsLetterOrDigit);
        }

        public static string ReadRegex(this string sourceString, ref int startIdx, Regex regex)
        {
            var match = regex.Match(sourceString, startIdx);
            if (match.Success)
            {
                startIdx = match.Index + match.Value.Length;
                return match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
            }
            return "";
        }

        public static string ReadRegex(this string sourceString, string pattern)
        {
            int idx = 0;
            return ReadRegex(sourceString, ref idx, new Regex(pattern));
        }

        public static string ReadFirstWord(this string sourceString)
        {
            int startIdx = 0;
            return ReadWhile(sourceString, ref startIdx, char.IsLetterOrDigit);
        }

        public static string ReadWhile(this string sourceString, ref int startIdx, Predicate<char> whilePredicate)
        {
            return ReadWhile(sourceString, ref startIdx, (int idx) => whilePredicate(sourceString[idx]));
        }

        private static string ReadWhile(this string sourceString, ref int startIdx, Predicate<int> whilePredicate)
        {
            int start = FindFrom(sourceString, startIdx, whilePredicate);
            for (int i = start; i < sourceString.Length; i++)
            {
                if (!whilePredicate(i))
                {
                    startIdx = i;
                    return sourceString.Substring(start, i - start).Trim();
                }
            }
            startIdx = sourceString.Length;
            return sourceString.Substring(start).Trim();
        }

        public static string ReadTo(this string sourceString, ref int startIdx, params string[] toStrings)
        {
            /*return ReadWhile(sourceString, ref startIdx,
                (int idx) => toStrings.Any(pred => sourceString.Length >= idx + pred.Length
                                                   && sourceString.Substring(idx, pred.Length) == pred));*/

            int start = startIdx;
            for (int i = startIdx; i < sourceString.Length; i++)
            {
                if (toStrings.Any(pred => sourceString.Length >= i + pred.Length && sourceString.Substring(i, pred.Length) == pred))
                {
                    startIdx = i + 1;
                    return sourceString.Substring(start, i - start).Trim();
                }
            }
            startIdx = sourceString.Length;
            return sourceString.Substring(start).Trim();
        }

        public static int FindFrom(this string sourceString, int startIdx, Predicate<char> toPredicate)
        {
            return FindFrom(sourceString, startIdx, (int idx) => toPredicate(sourceString[idx]));
        }

        private static int FindFrom(this string sourceString, int startIdx, Predicate<int> toPredicate)
        {
            for (int i = startIdx; i < sourceString.Length; i++)
            {
                if (toPredicate(i))
                    return i;
            }
            return sourceString.Length;
        }

        /// <summary>
        /// Matching all capital letters in the input and seperate them with spaces to form a sentence.
        /// If the input is an abbreviation text, no space will be added and returns the same input.
        /// </summary>
        /// <example>
        /// input : HelloWorld
        /// output : Hello World
        /// </example>
        /// <example>
        /// input : BBC
        /// output : BBC
        /// </example>
        /// <param name="input" />
        /// <returns/>
        public static string ToSentence(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            //return as is if the input is just an abbreviation
            if (Regex.Match(input, "[0-9A-Z]+$").Success)
                return input;
            //add a space before each capital letter, but not the first one.
            var result = Regex.Replace(input, "(\\B[A-Z])", " $1");
            return result;
        }

        /// <summary>
        /// Return last "howMany" characters from "input" string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="howMany">Characters count to return</param>
        /// <returns></returns>
        public static string GetLast(this string input, int howMany)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var value = input.Trim();
            return howMany >= value.Length ? value : value.Substring(value.Length - howMany);
        }

        /// <summary>
        /// Return first "howMany" characters from "input" string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="howMany">Characters count to return</param>
        /// <returns></returns>
        public static string GetFirst(this string input, int howMany)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var value = input.Trim();
            return howMany >= value.Length ? value : input.Substring(0, howMany);
        }

        public static bool IsNumber(this string input)
        {
            var match = Regex.Match(input, @"^[0-9]+$", RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static bool IsEmail(this string input)
        {
            var match = Regex.Match(input,
              @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static bool IsPhone(this string input)
        {
            var match = Regex.Match(input,
              @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static int ExtractNumber(this string input, int def)
        {
            if (string.IsNullOrEmpty(input)) return 0;
            
            var match = Regex.Match(input, "[0-9]{1,9}", RegexOptions.IgnoreCase);
            return match.Success ? int.Parse(match.Value) : def;
        }

        /// <summary>
        /// Checks string object's value to array of string values
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="stringValues">Array of string values to compare</param>
        /// <returns>Return true if any string value matches</returns>
        public static bool In(this string value, params string[] stringValues)
        {
            return stringValues.Any(otherValue => String.CompareOrdinal(value, otherValue) == 0);
        }
        
        public static bool IsNullOrWhiteSpace(this string input)
        {
            return string.IsNullOrEmpty(input) || input.Trim() == string.Empty;
        }

        /// <summary>
        /// Formats the string according to the specified mask
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="mask">The mask for formatting. Like "A##-##-T-###Z"</param>
        /// <returns>The formatted string</returns>
        public static string FormatWithMask(this string input, string mask)
        {
            if (input.IsNullOrWhiteSpace()) return input;
            var output = string.Empty;
            var index = 0;
            foreach (var m in mask)
            {
                if (m == '#')
                {
                    if (index < input.Length)
                    {
                        output += input[index];
                        index++;
                    }
                }
                else
                    output += m;
            }
            return output;
        }

        public static string Format(this string stringFormat, params object[] pars)
        {
            return string.Format(stringFormat, pars);
        }

        /// <summary>
        /// Supplements String.Format by letting you get properties from objects 
        /// </summary>
        /// <example>
        /// var sourceObject = new { simpleString = "string", integer = 3, Date = new DateTime(2013, 08, 19) };
        /// Debug.WriteLine("Gettings property by name : '{0:simpleString}' event cast insensitive : {0:date}".Inject(sourceObject));
        /// Debug.WriteLine("The property can be formatted by appending standard String.Format syntax after the property name like this {0:date:yyyy-MM-dd}".Inject(sourceObject));
        /// Debug.WriteLine("Use culture info to format the value to a specific culture '{0:date:dddd}'".Inject(CultureInfo.GetCultureInfo("da-DK"), sourceObject));
        /// Debug.WriteLine("Inject more values and event build in types {0:integer} {1} with build in properties {1:length}".Inject(sourceObject, "simple string"));
        /// </example>
        /// <param name="source">A composite format string</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatInject(this string source, IFormatProvider formatProvider, params object[] args)
        {
            var objectWrappers = new object[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                objectWrappers[i] = new ObjectWrapper(args[i]);
            }

            return string.Format(formatProvider, source, objectWrappers);
        }

        public static string FormatInject(this string source, params object[] args)
        {
            return FormatInject(source, CultureInfo.CurrentUICulture, args);
        }

        private class ObjectWrapper : IFormattable
        {
            private readonly object _wrapped;
            private static readonly Dictionary<string, FormatInfo> Cache = new Dictionary<string, FormatInfo>();

            public ObjectWrapper(object wrapped)
            {
                _wrapped = wrapped;
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (string.IsNullOrEmpty(format))
                {
                    return _wrapped.ToString();
                }

                var type = _wrapped.GetType();
                var key = type.FullName + ":" + format;

                FormatInfo wrapperCache;
                lock (Cache)
                {
                    if (!Cache.TryGetValue(key, out wrapperCache))
                    {
                        wrapperCache = CreateFormatInfo(format, type);
                        Cache.Add(key, wrapperCache);
                    }
                }

                var value = wrapperCache.GetValueDelegate != null ? wrapperCache.GetValueDelegate.DynamicInvoke(_wrapped) : _wrapped;
                var outputFormat = wrapperCache.OutputFormat;
     
                return string.Format(formatProvider, outputFormat, value);
            }

            private delegate object GetDictionaryValueDelegate(IDictionary dic);

            private static FormatInfo CreateFormatInfo(string format, Type type)
            {
                var split = format.Split(new[] { ':' }, 2);
                var param = split[0];
                var hasSubFormat = split.Length == 2;
                var subFormat = hasSubFormat ? split[1] : string.Empty;

                string outputFormat = "";
                Delegate valueDelegate = null;
                if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    valueDelegate = (GetDictionaryValueDelegate)delegate(IDictionary dic)
                    {
                        var key = param.ReadFirstWord();
                        var item = dic[key];
                        var parameter = Expression.Parameter(item.GetType(), key);
                        var expression = DynamicExpression.ParseLambda(new[] { parameter }, null, param);
                        return expression.Compile().DynamicInvoke(item);
                    };
                    outputFormat = hasSubFormat ? "{0:" + subFormat + "}" : "{0}";
                }
                else
                {
                    string exp = "obj." + param;
                    var parameter = Expression.Parameter(type, "obj");
                    var expression = DynamicExpression.ParseLambda(new[] { parameter }, null, exp);
                    outputFormat = expression != null ? (hasSubFormat ? "{0:" + subFormat + "}" : "{0}") : "{0:" + format + "}";
                    valueDelegate = expression != null ? expression.Compile() : null;
                }

                return new FormatInfo(valueDelegate, outputFormat);
            }

            private class FormatInfo
            {
                public FormatInfo(Delegate getValueDelegate, string outputFormat)
                {
                    GetValueDelegate = getValueDelegate;
                    OutputFormat = outputFormat;
                }

                public Delegate GetValueDelegate { get; private set; }
                public string OutputFormat { get; private set; }
            }
        }

#if WindowsCE
        public static string[] Split(this string str, string[] separator, StringSplitOptions options)
        {
            int numReplaces = 0;
            int length = separator.Length;
            int[] sepList = new int[str.Length];
            int[] lengthList = new int[str.Length];
            for (int i = 0; (i < str.Length) && (numReplaces < length); i++)
            {
                for (int j = 0; j < separator.Length; j++)
                {
                    string sep = separator[j];
                    if (!string.IsNullOrEmpty(sep))
                    {
                        int num5 = sep.Length;
                        if (((str[i] == sep[0]) && (num5 <= (str.Length - i))) && ((num5 == 1) || (string.CompareOrdinal(str, i, sep, 0, num5) == 0)))
                        {
                            sepList[numReplaces] = i;
                            lengthList[numReplaces] = num5;
                            numReplaces++;
                            i += num5 - 1;
                            break;
                        }
                    }
                }
            }

            int num = (numReplaces < int.MaxValue) ? (numReplaces + 1) : int.MaxValue;
            string[] strArray = new string[num];
            int startIndex = 0;
            int num3 = 0;
            for (int i = 0; (i < numReplaces) && (startIndex < str.Length); i++)
            {
                if ((sepList[i] - startIndex) > 0)
                {
                    strArray[num3++] = str.Substring(startIndex, sepList[i] - startIndex);
                }
                startIndex = sepList[i] + ((lengthList == null) ? 1 : lengthList[i]);
                if (num3 == (int.MaxValue - 1))
                {
                    while ((i < (numReplaces - 1)) && (startIndex == sepList[++i]))
                    {
                        startIndex += (lengthList == null) ? 1 : lengthList[i];
                    }
                    break;
                }
            }
            if (startIndex < str.Length)
            {
                strArray[num3++] = str.Substring(startIndex);
            }
            string[] strArray2 = strArray;
            if (num3 != num)
            {
                strArray2 = new string[num3];
                for (int j = 0; j < num3; j++)
                {
                    strArray2[j] = strArray[j];
                }
            }
            return strArray2;
        }
#endif
    }

#if WindowsCE
    public enum StringSplitOptions
    {
        None,
        RemoveEmptyEntries
    }
#endif
}