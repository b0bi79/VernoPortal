using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Verno
{
    /// <summary>
    /// ����� ��������� �� ������ ��������� ��������� ������������� ����.
    /// n            - ������
    /// t            - �������
    /// wb           - ������ ������� ������
    /// we           - ����� ������� ������
    /// mb           - ������ �������� ������
    /// me           - ����� �������� ������
    /// qb           - ������ �������� ��������
    /// qe           - ����� �������� ��������
    /// yb           - 1 ������ �������� ����
    /// ye           - 31 ������� �������� ����
    /// t-2d         - ���������
    /// n-2h         - 2 ���� �����
    /// t-1m         - ����� �����
    /// mb-1w        - �� ������� ��� ������ ������� ������
    /// wb-1w        - ������� �����������
    /// t 09:00      - ������� 9 ����
    /// t-1m 21:00   - 9 ���� ����� �����
    /// </summary>
    public class DateTimeExpression
    {
        private readonly CultureInfo _culture;
        private static readonly Regex Regex = new Regex(@"([wmqy][be])?(n|t)?(?:([+-])(\d{1,2})([hdwmqy]))? ?([012]?\d:[0-5]\d(?::[0-5]\d)?)?");

        private static readonly Dictionary<string, Func<DateTime, DateTime>> DateFunctions = new Dictionary<string, Func<DateTime, DateTime>>
        {
            {"", d => d},
            {"wb", d => d.GetFirstDayOfWeek()},
            {"we", d => d.GetFirstDayOfWeek().AddDays(6)},
            {"mb", d => d.GetFirstDayOfMonth()},
            {"me", d => d.GetFirstDayOfMonth().AddMonths(1).AddDays(-1)},
            {"qb", d => d.GetFirstDateOfQuarter()},
            {"qe", d => d.GetFirstDateOfQuarter().AddMonths(3).AddDays(-1)},
            {"yb", d => new DateTime(d.Year, 1, 1)},
            {"ye", d => new DateTime(d.Year, 12, 31)},
        };

        private static readonly Dictionary<string, Func<DateTime, int, DateTime>> DatechangeFunctions = new Dictionary<string, Func<DateTime, int, DateTime>>
        {
            {"", (d, m) => d},
            {"h", (d, m) => d.AddHours(m)},
            {"d", (d, m) => d.AddDays(m)},
            {"w", (d, m) => d.AddDays(m*7)},
            {"m", (d, m) => d.AddMonths(m)},
            {"q", (d, m) => d.AddMonths(m*3)},
            {"y", (d, m) => d.AddYears(m)},
        };

        public DateTimeExpression()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public DateTimeExpression(CultureInfo culture)
        {
            _culture = culture;
        }

        public DateTime Evaluate(string exprString)
        {
            DateTime result;
            if (new DateTimeExpression().TryEvaluate(exprString, out result))
                return result;
            throw new FormatException("exprString �� �������� ���������� ��������� ������������� ��������� DateTimeExpression.");
        }

        public bool TryEvaluate(string exprString, out DateTime result)
        {
            Match m = Regex.Match(exprString);
            while (m.Success)
            {
                var dateFunc = m.Groups[1].Length > 0 ? DateFunctions[m.Groups[1].Value.ToLower()] : null;
                DateTime seed = DateTime.Today;
                if (m.Groups[2].Length > 0 && m.Groups[2].Value.ToLower() == "n")
                    seed = DateTime.Now;
                var chngSign = m.Groups[3].Value;
                var chngMul = m.Groups[4].Length > 0 ? int.Parse(m.Groups[4].Value) : 0;
                if (chngSign == "-")
                    chngMul *= -1;
                var chgFunc = m.Groups[5].Length > 0 ? DatechangeFunctions[m.Groups[5].Value.ToLower()] : null;
                var time = m.Groups[6].Length > 0 ? TimeSpan.Parse(m.Groups[6].Value) : TimeSpan.Zero;
                result = CalculateDate(dateFunc, seed, chngMul, chgFunc, time);
                return true;
            }
            result = DateTime.MinValue;
            return false;
        }

        private DateTime CalculateDate(Func<DateTime, DateTime> dateFunc, DateTime seed, int chngMul, Func<DateTime, int, DateTime> chgFunc, TimeSpan time)
        {
            var result = seed;
            if (chgFunc != null) result = chgFunc(seed, chngMul);
            if (dateFunc != null) result = dateFunc(result);
            result = result.Add(time);
            return result;
        }
    }
}