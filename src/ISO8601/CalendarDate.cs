﻿using System.ISO8601.Abstract;
using System.ISO8601.Internal.Comparers;
using System.ISO8601.Internal.Converters;
using System.ISO8601.Internal.Parsers;
using System.ISO8601.Internal.Serializers;

namespace System.ISO8601
{
    public class CalendarDate : Date, IComparable, IComparable<Date>, IEquatable<Date>
    {
        private static DateComparer _comparer;
        private readonly int _century;
        private readonly int _day;
        private readonly int _month;
        private readonly CalendarDatePrecision _precision;
        private readonly long _year;
        private int _yearLength;
        private bool _isExpanded;

        public CalendarDate(long year, int month, int day) : this(year, month)
        {
            if (day < 1 || day > DateCalculator.DaysInMonth(year, month))
            {
                throw new ArgumentOutOfRangeException("day", string.Format("The day must be a value from 1 to {0}.", DateCalculator.DaysInMonth(year, month)));
            }

            _day = day;
            _precision = CalendarDatePrecision.Day;
        }

        public CalendarDate(long year, int month) : this(year)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month", "The month must be a value from 1 to 12.");
            }

            _month = month;
            _precision = CalendarDatePrecision.Month;
        }

        public CalendarDate(long year)
        {
            if (year < 0 || year > 9999)
            {
                _isExpanded = true;
            }

            _year = year;
            _century = DateCalculator.CenturyOfYear(year);
            _precision = CalendarDatePrecision.Year;
        }

        private CalendarDate(int century)
        {
            _century = century;
            _precision = CalendarDatePrecision.Century;
        }

        public static DateComparer Comparer
        {
            get
            {
                if (_comparer == null)
                {
                    _comparer = new DateComparer();
                }

                return _comparer;
            }
        }

        public int Century
        {
            get
            {
                return _century;
            }
        }

        public int Day
        {
            get
            {
                return _day;
            }
        }

        public int Month
        {
            get
            {
                return _month;
            }
        }

        public CalendarDatePrecision Precision
        {
            get
            {
                return _precision;
            }
        }

        public long Year
        {
            get
            {
                return _year;
            }
        }

        public int YearLength
        {
            get
            {
                return _yearLength;
            }
            set
            {
                _yearLength = value;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                if ((_year < 0 || _year > 9999) && !value)
                {
                    _isExpanded = true;
                }

                _isExpanded = value;
            }
        }

        public static CalendarDate FromCentury(int century)
        {
            return new CalendarDate(century);
        }

        public static TimeSpan operator -(CalendarDate x, CalendarDate y)
        {
            return DateCalculator.Subtract(x, y);
        }

        public static bool operator !=(CalendarDate x, Date y)
        {
            return Comparer.Compare(x, y) != 0;
        }

        public static bool operator <(CalendarDate x, Date y)
        {
            return Comparer.Compare(x, y) < 0;
        }

        public static bool operator <=(CalendarDate x, Date y)
        {
            return Comparer.Compare(x, y) <= 0;
        }

        public static bool operator ==(CalendarDate x, Date y)
        {
            return Comparer.Compare(x, y) == 0;
        }

        public static bool operator >(CalendarDate x, Date y)
        {
            return Comparer.Compare(x, y) > 0;
        }

        public static bool operator >=(CalendarDate x, Date y)
        {
            return Comparer.Compare(x, y) >= 0;
        }

        public static CalendarDate Parse(string input, int yearLength = 4)
        {
            if (yearLength < 4)
            {
                throw new ArgumentOutOfRangeException(nameof(yearLength), "The year length must be four or greater.");
            }

            return CalendarDateParser.Parse(input, yearLength);
        }

        public int CompareTo(Date other)
        {
            return Comparer.Compare(this, other);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is Date))
            {
                throw new ArgumentException("A calendar date can only be compared with other dates.");
            }

            return Comparer.Compare(this, (Date)obj);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Date))
            {
                return false;
            }

            return Comparer.Compare(this, (Date)obj) == 0;
        }

        public bool Equals(Date other)
        {
            return Comparer.Compare(this, other) == 0;
        }

        public override int GetHashCode()
        {
            return unchecked((int)Year) ^ (Month << 28) ^ (Day << 22);
        }

        public OrdinalDate ToOrdinalDate()
        {
            return CalendarDateConverter.ToOrdinalDate(this);
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public virtual string ToString(bool hyphenate)
        {
            return CalendarDateSerializer.Serialize(this, hyphenate);
        }

        public WeekDate ToWeekDate(WeekDatePrecision precision)
        {
            return CalendarDateConverter.ToWeekDate(this, precision);
        }

        public WeekDate ToWeekDate()
        {
            return CalendarDateConverter.ToWeekDate(this, WeekDatePrecision.Day);
        }
    }
}