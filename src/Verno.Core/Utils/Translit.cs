using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Verno
{
    public static class Transliteration
    {
        /// <summary>
        /// Transliterate cyrillyc string to latin.
        /// </summary>
        /// <param name="cyrillicSource">Source string.</param>
        /// <param name="language">Specify it to determine correct transliteration rules 
        /// (it can be a little bit defferent for languages).</param>
        /// <returns>Transliterated string.</returns>
        public static string CyrillicToLatin(string cyrillicSource, Language language = Language.Russian)
        {
            return new CyrillicToLatinConverter(cyrillicSource, language)
                .Convert();
        }

        /// <summary>
        /// Transliterate latin string to cyrillyc.
        /// </summary>
        /// <param name="latinSource">Source string.</param>
        /// <param name="language">Specify it to determine correct transliteration rules 
        /// (it can be a little bit defferent for languages).</param>
        /// <returns>Cyrillyc string.</returns>
        public static string LatinToCyrillyc(string latinSource, Language language = Language.Russian)
        {
            return new LatinToCyrillicConverter(latinSource, language)
                .Convert();
        }
    }

    public enum Language
    {
        /// <summary>
        /// Unknown language. Most common rules will be used for transliteration.
        /// </summary>
        Unknown,

        /// <summary>
        /// Russain language.
        /// </summary>
        Russian,

        /*/// <summary>
        /// Belorussian language.
        /// </summary>
        Belorussian,

        /// <summary>
        /// Ukranian language.
        /// </summary>
        Ukrainian,

        /// <summary>
        /// Bulgarian language.
        /// </summary>
        Bulgarian,

        /// <summary>
        /// Macedonian language.
        /// </summary>
        Macedonian*/
    }

    internal partial struct CyrillicToLatinConverter
    {
        private readonly Language _lang;
        private readonly string _src;
        private readonly Dictionary<char, string> _ruleSet;

        private StringBuilder _sb;

        /// <summary>
        /// Create an instance of algorithm.
        /// </summary>
        public CyrillicToLatinConverter(string source, Language lang)
        {
            Debug.Assert(Language.Unknown == 0);
            Debug.Assert((int) Language.Russian == 1);
            /*Debug.Assert((int) Language.Belorussian == 2);
            Debug.Assert((int) Language.Ukrainian == 3);
            Debug.Assert((int) Language.Bulgarian == 4);
            Debug.Assert((int) Language.Macedonian == 5);*/

            _ruleSet = Rules[(int) lang];
            _lang = lang;
            _src = source;
            _sb = null;
        }

        /// <summary>
        /// Should be invoked only once.
        /// </summary>
        public string Convert()
        {
            Debug.Assert(_src != null);
            Debug.Assert(_ruleSet != null);

            if (string.IsNullOrEmpty(_src))
                return _src;

            _sb = new StringBuilder();

            for (var srcIndex = 0; srcIndex < _src.Length; srcIndex++)
            {
                string substitute;
                if (_ruleSet.TryGetValue(_src[srcIndex], out substitute))
                {
                    var nextChar = (_src.Length > (srcIndex + 1)) ? _src[srcIndex + 1] : ' ';
                    substitute = CheckSpecificRules(substitute, nextChar);
                    _sb.Append(substitute);
                }
                else
                {
                    _sb.Append(_src[srcIndex]);
                }
            }

            return _sb.ToString();
        }


        private static readonly HashSet<char> RuleCzCheck = new HashSet<char> { 'Е', 'Ё', 'И', 'Й', 'I','Ы','Э','Ю','Я','е','ё','и','й','i','ы','э','ю','я','ѣ','Ѣ','ѵ','Ѵ'};

        private string CheckSpecificRules(string substitue, char nextSourceChar)
        {
            if (substitue.Length == 2)
            {
                // Ц	cz, c	cz, c	cz, c	cz, c	cz, c	рекомендуется использовать С перед буквами I, Е, Y, J; в остальных случаях CZ
                if ((substitue[1] == 'z') && (RuleCzCheck.Contains(nextSourceChar)))
                {
                    return substitue.Substring(0, 1);
                }
            }
            return substitue;
        }

        private static readonly Dictionary<char, string>[] Rules =
        {
            new Dictionary<char, string> // Unknown
            {
                {'а', @"a"},
                {'А', @"A"},
                {'б', @"b"},
                {'Б', @"B"},
                {'в', @"v"},
                {'В', @"V"},
                {'г', @"g"},
                {'Г', @"G"},
                {'ѓ', @"g`"},
                {'Ѓ', @"G`"},
                {'ґ', @"g`"},
                {'Ґ', @"G`"},
                {'д', @"d"},
                {'Д', @"D"},
                {'е', @"e"},
                {'Е', @"E"},
                {'ё', @"yo"},
                {'Ё', @"Yo"},
                {'є', @"ye"},
                {'Є', @"Ye"},
                {'ж', @"zh"},
                {'Ж', @"Zh"},
                {'з', @"z"},
                {'З', @"Z"},
                {'s', @"z`"},
                {'S', @"Z`"},
                {'и', @"i"},
                {'И', @"I"},
                {'й', @"j"},
                {'Й', @"J"},
                {'j', @"j"},
                {'J', @"J"},
                {'i', @"i"},
                {'I', @"I"},
                {'ї', @"yi"},
                {'Ї', @"Yi"},
                {'к', @"k"},
                {'К', @"K"},
                {'ќ', @"k`"},
                {'Ќ', @"K`"},
                {'л', @"l"},
                {'Л', @"L"},
                {'љ', @"l`"},
                {'Љ', @"L`"},
                {'м', @"m"},
                {'М', @"M"},
                {'н', @"n"},
                {'Н', @"N"},
                {'њ', @"n`"},
                {'Њ', @"N`"},
                {'о', @"o"},
                {'О', @"O"},
                {'п', @"p"},
                {'П', @"P"},
                {'р', @"r"},
                {'Р', @"R"},
                {'с', @"s"},
                {'С', @"S"},
                {'т', @"t"},
                {'Т', @"T"},
                {'у', @"u"},
                {'У', @"U"},
                {'ў', @"u`"},
                {'Ў', @"U`"},
                {'ф', @"f"},
                {'Ф', @"F"},
                {'х', @"x"},
                {'Х', @"X"},
                {'ц', @"cz"},
                {'Ц', @"Cz"},
                {'ч', @"ch"},
                {'Ч', @"Ch"},
                {'џ', @"dh"},
                {'Џ', @"Dh"},
                {'ш', @"sh"},
                {'Ш', @"Sh"},
                {'щ', @"shh"},
                {'Щ', @"Shh"},
                {'ъ', @"``"},
                {'Ъ', @"``"},
                {'ы', @"y`"},
                {'Ы', @"Y`"},
                {'ь', @"`"},
                {'Ь', @"`"},
                {'э', @"e`"},
                {'Э', @"E`"},
                {'ю', @"yu"},
                {'Ю', @"Yu"},
                {'я', @"ya"},
                {'Я', @"Ya"},
                {'’', @"'"},
                {'ѣ', @"ye"},
                {'Ѣ', @"Ye"},
                {'ѳ', @"fh"},
                {'Ѳ', @"Fh"},
                {'ѵ', @"yh"},
                {'Ѵ', @"Yh"},
                {'ѫ', @"о`"},
                {'Ѫ', @"О`"},
                {'№', @"#"},
            },
            new Dictionary<char, string> // ru-RU
            {
                {'а', @"a"},
                {'А', @"A"},
                {'б', @"b"},
                {'Б', @"B"},
                {'в', @"v"},
                {'В', @"V"},
                {'г', @"g"},
                {'Г', @"G"},
                {'д', @"d"},
                {'Д', @"D"},
                {'е', @"e"},
                {'Е', @"E"},
                {'ё', @"yo"},
                {'Ё', @"Yo"},
                {'ж', @"zh"},
                {'Ж', @"Zh"},
                {'з', @"z"},
                {'З', @"Z"},
                {'и', @"i"},
                {'И', @"I"},
                {'й', @"j"},
                {'Й', @"J"},
                {'i', @"i"},
                {'I', @"I"},
                {'к', @"k"},
                {'К', @"K"},
                {'л', @"l"},
                {'Л', @"L"},
                {'м', @"m"},
                {'М', @"M"},
                {'н', @"n"},
                {'Н', @"N"},
                {'о', @"o"},
                {'О', @"O"},
                {'п', @"p"},
                {'П', @"P"},
                {'р', @"r"},
                {'Р', @"R"},
                {'с', @"s"},
                {'С', @"S"},
                {'т', @"t"},
                {'Т', @"T"},
                {'у', @"u"},
                {'У', @"U"},
                {'ф', @"f"},
                {'Ф', @"F"},
                {'х', @"x"},
                {'Х', @"X"},
                {'ц', @"cz"},
                {'Ц', @"Cz"},
                {'ч', @"ch"},
                {'Ч', @"Ch"},
                {'ш', @"sh"},
                {'Ш', @"Sh"},
                {'щ', @"shh"},
                {'Щ', @"Shh"},
                {'ъ', @"``"},
                {'Ъ', @"``"},
                {'ы', @"y`"},
                {'Ы', @"Y`"},
                {'ь', @"`"},
                {'Ь', @"`"},
                {'э', @"e`"},
                {'Э', @"E`"},
                {'ю', @"yu"},
                {'Ю', @"Yu"},
                {'я', @"ya"},
                {'Я', @"Ya"},
                {'’', @"'"},
                {'ѣ', @"ye"},
                {'Ѣ', @"Ye"},
                {'ѳ', @"fh"},
                {'Ѳ', @"Fh"},
                {'ѵ', @"yh"},
                {'Ѵ', @"Yh"},
                {'№', @"#"},
            },
        };
    }

    internal partial struct LatinToCyrillicConverter
    {
        private readonly string _src;
        private readonly ConvertRule[] _ruleSet;

        /// <summary>
        /// Create an instance of algorithm.
        /// </summary>
        public LatinToCyrillicConverter(string source, Language lang)
        {
            Debug.Assert(Language.Unknown == 0);
            Debug.Assert((int)Language.Russian == 1);
            /*Debug.Assert((int)Language.Belorussian == 2);
            Debug.Assert((int)Language.Ukrainian == 3);
            Debug.Assert((int)Language.Bulgarian == 4);
            Debug.Assert((int)Language.Macedonian == 5);*/

            _ruleSet = Rules[(int)lang];
            _src = source;
        }

        /// <summary>
        /// Detransliterate source. Should be invoked only once.
        /// </summary>
        /// <returns>Detransliterated cyrillic string.</returns>
        public string Convert()
        {
            Debug.Assert(_ruleSet != null);

            if (string.IsNullOrEmpty(_src))
                return _src;

            var result = _src;
            for (var i = 0; i < _ruleSet.Length; i++)
            {
                result = result.Replace(_ruleSet[i].Latin, _ruleSet[i].Cyrillic);
            }
            return result;
        }
        private struct ConvertRule
        {
            public readonly string Latin;
            public readonly string Cyrillic;

            public ConvertRule(string cyrillic, string latin)
            {
                Cyrillic = cyrillic;
                Latin = latin;
            }
        }

        private static readonly ConvertRule[][] Rules = new[]
        {
            new[] // Unknown
            {
                new ConvertRule("щ", @"shh"),
                new ConvertRule("Щ", @"Shh"),
                new ConvertRule("ѓ", @"g`"),
                new ConvertRule("Ѓ", @"G`"),
                new ConvertRule("ґ", @"g`"),
                new ConvertRule("Ґ", @"G`"),
                new ConvertRule("ё", @"yo"),
                new ConvertRule("Ё", @"Yo"),
                new ConvertRule("є", @"ye"),
                new ConvertRule("Є", @"Ye"),
                new ConvertRule("ж", @"zh"),
                new ConvertRule("Ж", @"Zh"),
                new ConvertRule("s", @"z`"),
                new ConvertRule("S", @"Z`"),
                new ConvertRule("ї", @"yi"),
                new ConvertRule("Ї", @"Yi"),
                new ConvertRule("ќ", @"k`"),
                new ConvertRule("Ќ", @"K`"),
                new ConvertRule("љ", @"l`"),
                new ConvertRule("Љ", @"L`"),
                new ConvertRule("њ", @"n`"),
                new ConvertRule("Њ", @"N`"),
                new ConvertRule("ў", @"u`"),
                new ConvertRule("Ў", @"U`"),
                new ConvertRule("ц", @"cz"),
                new ConvertRule("Ц", @"Cz"),
                new ConvertRule("ч", @"ch"),
                new ConvertRule("Ч", @"Ch"),
                new ConvertRule("џ", @"dh"),
                new ConvertRule("Џ", @"Dh"),
                new ConvertRule("ш", @"sh"),
                new ConvertRule("Ш", @"Sh"),
                new ConvertRule("ъ", @"``"),
                new ConvertRule("Ъ", @"``"),
                new ConvertRule("ы", @"y`"),
                new ConvertRule("Ы", @"Y`"),
                new ConvertRule("э", @"e`"),
                new ConvertRule("Э", @"E`"),
                new ConvertRule("ю", @"yu"),
                new ConvertRule("Ю", @"Yu"),
                new ConvertRule("я", @"ya"),
                new ConvertRule("Я", @"Ya"),
                new ConvertRule("ѣ", @"ye"),
                new ConvertRule("Ѣ", @"Ye"),
                new ConvertRule("ѳ", @"fh"),
                new ConvertRule("Ѳ", @"Fh"),
                new ConvertRule("ѵ", @"yh"),
                new ConvertRule("Ѵ", @"Yh"),
                new ConvertRule("ѫ", @"о`"),
                new ConvertRule("Ѫ", @"О`"),
                new ConvertRule("а", @"a"),
                new ConvertRule("А", @"A"),
                new ConvertRule("б", @"b"),
                new ConvertRule("Б", @"B"),
                new ConvertRule("в", @"v"),
                new ConvertRule("В", @"V"),
                new ConvertRule("г", @"g"),
                new ConvertRule("Г", @"G"),
                new ConvertRule("д", @"d"),
                new ConvertRule("Д", @"D"),
                new ConvertRule("е", @"e"),
                new ConvertRule("Е", @"E"),
                new ConvertRule("з", @"z"),
                new ConvertRule("З", @"Z"),
                new ConvertRule("и", @"i"),
                new ConvertRule("И", @"I"),
                new ConvertRule("й", @"j"),
                new ConvertRule("Й", @"J"),
                new ConvertRule("j", @"j"),
                new ConvertRule("J", @"J"),
                new ConvertRule("i", @"i"),
                new ConvertRule("I", @"I"),
                new ConvertRule("к", @"k"),
                new ConvertRule("К", @"K"),
                new ConvertRule("л", @"l"),
                new ConvertRule("Л", @"L"),
                new ConvertRule("м", @"m"),
                new ConvertRule("М", @"M"),
                new ConvertRule("н", @"n"),
                new ConvertRule("Н", @"N"),
                new ConvertRule("о", @"o"),
                new ConvertRule("О", @"O"),
                new ConvertRule("п", @"p"),
                new ConvertRule("П", @"P"),
                new ConvertRule("р", @"r"),
                new ConvertRule("Р", @"R"),
                new ConvertRule("с", @"s"),
                new ConvertRule("С", @"S"),
                new ConvertRule("т", @"t"),
                new ConvertRule("Т", @"T"),
                new ConvertRule("у", @"u"),
                new ConvertRule("У", @"U"),
                new ConvertRule("ф", @"f"),
                new ConvertRule("Ф", @"F"),
                new ConvertRule("х", @"x"),
                new ConvertRule("Х", @"X"),
                new ConvertRule("ц", @"c"),
                new ConvertRule("Ц", @"C"),
                new ConvertRule("ь", @"`"),
                new ConvertRule("Ь", @"`"),
                new ConvertRule("’", @"'"),
                new ConvertRule("№", @"#"),
            },
            new [] // ru-RU
			{
                new ConvertRule("щ", @"shh"),
                new ConvertRule("Щ", @"Shh"),
                new ConvertRule("ё", @"yo"),
                new ConvertRule("Ё", @"Yo"),
                new ConvertRule("ж", @"zh"),
                new ConvertRule("Ж", @"Zh"),
                new ConvertRule("ц", @"cz"),
                new ConvertRule("Ц", @"Cz"),
                new ConvertRule("ч", @"ch"),
                new ConvertRule("Ч", @"Ch"),
                new ConvertRule("ш", @"sh"),
                new ConvertRule("Ш", @"Sh"),
                new ConvertRule("ъ", @"``"),
                new ConvertRule("Ъ", @"``"),
                new ConvertRule("ы", @"y`"),
                new ConvertRule("Ы", @"Y`"),
                new ConvertRule("э", @"e`"),
                new ConvertRule("Э", @"E`"),
                new ConvertRule("ю", @"yu"),
                new ConvertRule("Ю", @"Yu"),
                new ConvertRule("я", @"ya"),
                new ConvertRule("Я", @"Ya"),
                new ConvertRule("ѣ", @"ye"),
                new ConvertRule("Ѣ", @"Ye"),
                new ConvertRule("ѳ", @"fh"),
                new ConvertRule("Ѳ", @"Fh"),
                new ConvertRule("ѵ", @"yh"),
                new ConvertRule("Ѵ", @"Yh"),
                new ConvertRule("а", @"a"),
                new ConvertRule("А", @"A"),
                new ConvertRule("б", @"b"),
                new ConvertRule("Б", @"B"),
                new ConvertRule("в", @"v"),
                new ConvertRule("В", @"V"),
                new ConvertRule("г", @"g"),
                new ConvertRule("Г", @"G"),
                new ConvertRule("д", @"d"),
                new ConvertRule("Д", @"D"),
                new ConvertRule("е", @"e"),
                new ConvertRule("Е", @"E"),
                new ConvertRule("з", @"z"),
                new ConvertRule("З", @"Z"),
                new ConvertRule("и", @"i"),
                new ConvertRule("И", @"I"),
                new ConvertRule("й", @"j"),
                new ConvertRule("Й", @"J"),
                new ConvertRule("i", @"i"),
                new ConvertRule("I", @"I"),
                new ConvertRule("к", @"k"),
                new ConvertRule("К", @"K"),
                new ConvertRule("л", @"l"),
                new ConvertRule("Л", @"L"),
                new ConvertRule("м", @"m"),
                new ConvertRule("М", @"M"),
                new ConvertRule("н", @"n"),
                new ConvertRule("Н", @"N"),
                new ConvertRule("о", @"o"),
                new ConvertRule("О", @"O"),
                new ConvertRule("п", @"p"),
                new ConvertRule("П", @"P"),
                new ConvertRule("р", @"r"),
                new ConvertRule("Р", @"R"),
                new ConvertRule("с", @"s"),
                new ConvertRule("С", @"S"),
                new ConvertRule("т", @"t"),
                new ConvertRule("Т", @"T"),
                new ConvertRule("у", @"u"),
                new ConvertRule("У", @"U"),
                new ConvertRule("ф", @"f"),
                new ConvertRule("Ф", @"F"),
                new ConvertRule("х", @"x"),
                new ConvertRule("Х", @"X"),
                new ConvertRule("ц", @"c"),
                new ConvertRule("Ц", @"C"),
                new ConvertRule("ь", @"`"),
                new ConvertRule("Ь", @"`"),
                new ConvertRule("’", @"'"),
                new ConvertRule("№", @"#"),
            },
        };
    }

    public class Translit
    {

    }
}