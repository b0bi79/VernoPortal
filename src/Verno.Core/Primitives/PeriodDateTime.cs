using System;
using System.Globalization;

namespace Verno
{
    public abstract class Period
    {
        public abstract DateTime Date1 { get; set; }
        public abstract DateTime Date2 { get; set; }
    }

    public class PeriodDateTime : Period
    {
        private DateTimeOffset _date1;
        private DateTimeOffset _date2;

        public PeriodDateTime()
        {
        }

        public PeriodDateTime(DateTimeOffset date1, DateTimeOffset date2)
        {
            _date1 = date1;
            _date2 = date2;
        }

        public override DateTime Date1
        {
            get { return _date1.LocalDateTime; }
            set { _date1 = value; }
        }

        public override DateTime Date2
        {
            get { return _date2.LocalDateTime; }
            set { _date2 = value; }
        }

        protected bool Equals(PeriodDateTime other)
        {
            return _date1.Equals(other._date1) && _date2.Equals(other._date2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PeriodDateTime)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_date1.GetHashCode() * 397) ^ _date2.GetHashCode();
            }
        }

        public static bool operator ==(PeriodDateTime a, PeriodDateTime b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((a == null) || (b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(PeriodDateTime a, PeriodDateTime b)
        {
            return !(a == b);
        }

        public static implicit operator string(PeriodDateTime period)
        {
            return period.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", _date1.UtcDateTime.ToString("u"), _date2.UtcDateTime.ToString("u"));
        }

        public static implicit operator PeriodDateTime(string str)
        {
            return Parse(str);
        }

        public static PeriodDateTime Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string[] dates = str.Split('|');
            if (dates.Length!=2)
                throw new FormatException("Expected string in 'date|date' format.");

            var d1 = dates[0].AsDateTime(DateTime.Now, CultureInfo.InvariantCulture);
            var d2 = dates[1].AsDateTime(DateTime.Now, CultureInfo.InvariantCulture);
            return new PeriodDateTime(d1, d2);
        }
    }
}