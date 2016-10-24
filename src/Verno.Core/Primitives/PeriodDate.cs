using System;
using System.Globalization;

namespace Verno
{
    public class PeriodDate : Period
    {
        private DateTime _date1;
        private DateTime _date2;

        public PeriodDate()
        {
        }

        public PeriodDate(DateTime date1, DateTime date2)
        {
            _date1 = date1.Date;
            _date2 = date2.Date;
        }

        public override DateTime Date1
        {
            get { return _date1; }
            set { _date1 = value.Date; }
        }

        public override DateTime Date2
        {
            get { return _date2; }
            set { _date2 = value.Date; }
        }

        protected bool Equals(PeriodDate other)
        {
            return _date1.Equals(other._date1) && _date2.Equals(other._date2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PeriodDate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_date1.GetHashCode()*397) ^ _date2.GetHashCode();
            }
        }

        public static bool operator ==(PeriodDate a, PeriodDate b)
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

        public static bool operator !=(PeriodDate a, PeriodDate b)
        {
            return !(a == b);
        }

        public static implicit operator string(PeriodDate period)
        {
            return period.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", Date1.ToString("yyyy-MM-dd"), Date2.ToString("yyyy-MM-dd"));
        }

        public static implicit operator PeriodDate(string str)
        {
            return Parse(str);
        }

        public static PeriodDate Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string[] dates = str.Split('|');
            if (dates.Length!=2)
                throw new FormatException("Expected string in 'date|date' format.");

            var d1 = dates[0].AsDateTime(DateTime.MinValue, CultureInfo.InvariantCulture);
            var d2 = dates[1].AsDateTime(DateTime.MinValue, CultureInfo.InvariantCulture);
            return new PeriodDate(d1, d2);
        }
    }
}