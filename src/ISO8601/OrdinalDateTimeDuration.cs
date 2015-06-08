﻿using System.ISO8601.Abstract;
using System.ISO8601.Internal.Parsing;
using System.ISO8601.Internal.Serialization;
using System.Globalization;

namespace System.ISO8601
{
    public class OrdinalDateTimeDuration : DateTimeDuration
    {
        private readonly OrdinalDateDuration _dateDuration;
        private readonly TimeDuration _timeDuration;

        public OrdinalDateTimeDuration(int years, int days, int hours, int minutes, double seconds)
        {
            if (years < 0 || years >= 10000)
            {
                throw new ArgumentOutOfRangeException(nameof(years), "Years must be a number between 0 and 9999.");
            }

            if (days < 0 || days > 999)
            {
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be a number between 0 and 999.");
            }

            if (hours < 0 || hours > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be a number between 0 and 24.");
            }

            if (minutes < 0 || minutes > 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be a number between 0 and 60.");
            }

            if (seconds < 0 || seconds > 60)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds must be a number between 0 and 60.");
            }

            _dateDuration = new OrdinalDateDuration(years, days);
            _timeDuration = new TimeDuration(hours, minutes, seconds);
        }

        public OrdinalDateTimeDuration(int years, int days, int hours, double minutes)
        {
            if (years < 0 || years >= 10000)
            {
                throw new ArgumentOutOfRangeException(nameof(years), "Years must be a number between 0 and 9999.");
            }

            if (days < 0 || days > 999)
            {
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be a number between 0 and 999.");
            }

            if (hours < 0 || hours > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be a number between 0 and 24.");
            }

            if (minutes < 0 || minutes > 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be a number between 0 and 60.");
            }

            _dateDuration = new OrdinalDateDuration(years, days);
            _timeDuration = new TimeDuration(hours, minutes);
        }

        public OrdinalDateTimeDuration(int years, int days, double hours)
        {
            if (years < 0 || years >= 10000)
            {
                throw new ArgumentOutOfRangeException(nameof(years), "Years must be a number between 0 and 9999.");
            }

            if (days < 0 || days > 30)
            {
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be a number between 0 and 30.");
            }

            if (hours < 0 || hours > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be a number between 0 and 24.");
            }

            _dateDuration = new OrdinalDateDuration(years, days);
            _timeDuration = new TimeDuration(hours);
        }

        internal OrdinalDateTimeDuration(OrdinalDateDuration dateDuration, TimeDuration timeDuration)
        {
            _dateDuration = dateDuration;
            _timeDuration = timeDuration;
        }

        public double Days
        {
            get
            {
                return _dateDuration.Days;
            }
        }

        public double Hours
        {
            get
            {
                return _timeDuration.Hours;
            }
        }

        public double? Minutes
        {
            get
            {
                return _timeDuration.Minutes;
            }
        }

        public double? Seconds
        {
            get
            {
                return _timeDuration.Seconds;
            }
        }

        public double Years
        {
            get
            {
                return _dateDuration.Years;
            }
        }

        internal OrdinalDateDuration DateDuration
        {
            get
            {
                return _dateDuration;
            }
        }

        internal TimeDuration TimeDuration
        {
            get
            {
                return _timeDuration;
            }
        }

        public static OrdinalDateTimeDuration Parse(string input, int yearLength = 4)
        {
            return OrdinalDateTimeDurationParser.Parse(input, yearLength);
        }

        public override string ToString()
        {
            return ToString();
        }

        public virtual string ToString(bool withComponentSeparators = true, bool isExpanded = false, int yearLength = 4, int fractionLength = 0, DecimalSeparator decimalSeparator = DecimalSeparator.Comma)
        {
            return OrdinalDateTimeDurationSerializer.Serialize(this, withComponentSeparators, isExpanded, yearLength, fractionLength, decimalSeparator);
        }
    }
}