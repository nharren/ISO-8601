﻿using System.ISO8601.Abstract;

namespace System.ISO8601
{
    public static class ISO8601Calculator
    {
        private const int DaysPerMonth = 30;
        private const int HoursPerDay = 24;
        private const int MinutesPerDay = 1440;
        private const int MinutesPerHour = 60;
        private const int MonthsPerYear = 12;
        private const int SecondsPerDay = 86400;
        private const int SecondsPerMinute = 60;
        private static readonly int[] DayOfWeekCenturyKeys = { 6, 4, 2, 0 };
        private static readonly int[] DayOfWeekMonthKeys365 = { 0, 0, 3, 3, 6, 1, 4, 6, 2, 5, 0, 3, 5 };
        private static readonly int[] DayOfWeekMonthKeys366 = { 0, 6, 2, 3, 6, 1, 4, 6, 2, 5, 0, 3, 5 };
        private static readonly int[] DaysInMonthArray = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int[] DaysToMonth365 = { 0, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };
        private static readonly int[] DaysToMonth366 = { 0, 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335 };

        public static CalendarDateTime Add(CalendarDateTime x, Duration y)
        {
            if (y is CalendarDateTimeDuration)
            {
                return Add(x, (CalendarDateTimeDuration)y);
            }

            if (y is OrdinalDateTimeDuration)
            {
                return Add(x, ((OrdinalDateTimeDuration)y).ToCalendarDateTimeDuration());
            }

            if (y is CalendarDateDuration)
            {
                return Add(x, (CalendarDateDuration)y);
            }

            if (y is OrdinalDateDuration)
            {
                return Add(x, ((OrdinalDateDuration)y).ToCalendarDateDuration());
            }

            if (y is TimeDuration)
            {
                return Add(x, (TimeDuration)y);
            }

            if (y is DesignatedDuration)
            {
                return Add(x, (DesignatedDuration)y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be added to a {x.GetType()}.");
        }

        public static CalendarDate Add(CalendarDate x, Duration y)
        {
            if (y is CalendarDateDuration)
            {
                return Add(x, (CalendarDateDuration)y);
            }

            if (y is OrdinalDateDuration)
            {
                return Add(x, ((OrdinalDateDuration)y).ToCalendarDateDuration());
            }

            if (y is DesignatedDuration)
            {
                return Add(x, (DesignatedDuration)y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be added to a {x.GetType()}.");
        }

        public static TimePoint Add(TimePoint x, Duration y)
        {
            if (x is CalendarDateTime)
            {
                return Add((CalendarDateTime)x, y);
            }

            if (x is OrdinalDateTime)
            {
                return Add(((OrdinalDateTime)x).ToCalendarDateTime(), y);
            }

            if (x is WeekDateTime)
            {
                return Add(((WeekDateTime)x).ToCalendarDateTime(), y);
            }

            if (x is CalendarDate)
            {
                return Add((CalendarDate)x, y);
            }

            if (x is OrdinalDate)
            {
                return Add(((OrdinalDate)x).ToCalendarDate(), y);
            }

            if (x is WeekDate)
            {
                return Add(((WeekDate)x).ToCalendarDate(), y);
            }

            if (x is Time)
            {
                return Add((Time)x, y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be added to a {x.GetType()}.");
        }

        public static Time Add(Time x, Duration y)
        {
            if (y is TimeDuration)
            {
                return Add(x, (TimeDuration)y);
            }

            if (y is DesignatedDuration)
            {
                return Add(x, (DesignatedDuration)y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be added to a {x.GetType()}.");
        }

        public static CalendarDateTime Add(CalendarDateTime x, CalendarDateTimeDuration y)
        {
            double day = x.Day + y.Days;
            double month = x.Month + y.Months;
            double year = x.Year + y.Years;
            var second = x.Second;
            var minute = x.Minute;
            var hour = x.Hour + y.Hours;
            var precision = x.Precision;

            if (y.Minutes != null)
            {
                minute += y.Minutes.Value;

                if (hour != (int)hour)
                {
                    minute += (hour - (int)hour) * MinutesPerHour;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second += y.Seconds.Value;

                if (minute != (int)minute)
                {
                    second += (minute - (int)minute) * SecondsPerMinute;
                }

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            while (!(second < SecondsPerMinute))
            {
                second -= SecondsPerMinute;
                minute++;
            }

            while ((int)minute >= MinutesPerHour)
            {
                minute -= MinutesPerHour;
                hour++;
            }

            while ((int)hour >= HoursPerDay)
            {
                hour -= HoursPerDay;
                day++;
            }

            while ((int)month > MonthsPerYear)
            {
                month -= MonthsPerYear;
                year++;
            }

            int daysInMonth = DaysInMonth((long)year, (int)month);

            while (day > daysInMonth)
            {
                day -= daysInMonth;
                month++;

                daysInMonth = DaysInMonth((long)year, (int)month);
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDateTime Add(CalendarDateTime x, CalendarDateDuration y)
        {
            double day = x.Day;
            double month = x.Month;
            double year = x.Year;
            var second = x.Second;
            var minute = x.Minute;
            var hour = x.Hour;
            var precision = x.Precision;

            year += y.Years;

            if (y.Months != null)
            {
                month += y.Months.Value;
            }

            if (y.Days != null)
            {
                day += y.Days.Value;
            }

            while ((int)month > MonthsPerYear)
            {
                month -= MonthsPerYear;
                year++;
            }

            int daysInMonth = DaysInMonth((long)year, (int)month);

            while (day > daysInMonth)
            {
                day -= daysInMonth;
                month++;

                daysInMonth = DaysInMonth((long)year, (int)month);
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDateTime Add(CalendarDateTime x, TimeDuration y)
        {
            double day = x.Day;
            double month = x.Month;
            long year = x.Year;
            double second = x.Second;
            double minute = x.Minute;
            double hour = x.Hour + y.Hours;
            TimePrecision precision = x.Precision;

            if (y.Minutes != null)
            {
                minute += y.Minutes.Value;

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second += y.Seconds.Value;

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            if (hour != (int)hour && precision < TimePrecision.Minute)
            {
                minute += (hour - (int)hour) * MinutesPerHour;
                hour = (int)hour;
            }

            if (minute != (int)minute && precision < TimePrecision.Second)
            {
                second += (minute - (int)minute) * SecondsPerMinute;
                second = (int)second;
            }

            while (second >= SecondsPerMinute)
            {
                second -= SecondsPerMinute;
                minute++;
            }

            while (minute >= MinutesPerHour)
            {
                minute -= MinutesPerHour;
                hour++;
            }

            while (hour >= HoursPerDay)
            {
                hour -= HoursPerDay;
                day++;
            }

            while (month > MonthsPerYear)
            {
                month -= MonthsPerYear;
                year++;
            }

            int daysInMonth = DaysInMonth(year, (int)month);

            while (day > daysInMonth)
            {
                day -= daysInMonth;
                month++;

                daysInMonth = DaysInMonth(year, (int)month);
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDateTime Add(CalendarDateTime x, DesignatedDuration y)
        {
            double day = x.Day;
            double month = x.Month;
            double year = x.Year;
            var second = x.Second;
            var minute = x.Minute;
            var hour = x.Hour;
            var precision = x.Precision;

            if (y.Years != null)
            {
                year += y.Years.Value;
            }

            if (y.Months != null)
            {
                month += y.Months.Value;

                if (year != (int)year)
                {
                    month += (year - (int)year) * 60;
                }
            }

            if (y.Days != null)
            {
                day += y.Days.Value;

                if (month != (int)month)
                {
                    day += (month - (int)month) * 60;
                }
            }

            if (y.Hours != null)
            {
                hour += y.Hours.Value;

                if (day != (int)day)
                {
                    hour += (day - (int)day) * 24;
                }
            }

            if (y.Minutes != null)
            {
                minute += y.Minutes.Value;

                if (hour != (int)hour)
                {
                    minute += (hour - (int)hour) * 60;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second += y.Seconds.Value;

                if (minute != (int)minute)
                {
                    second += (minute - (int)minute) * 60;
                }

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            while (!(second < 60))
            {
                second -= 60;
                minute++;
            }

            while ((int)minute > 59 || (precision == TimePrecision.Minute && !(minute < 60d)))
            {
                minute -= 60;
                hour++;
            }

            while ((int)hour > 23 || (precision == TimePrecision.Hour && hour >= 24d))
            {
                hour -= 24;
                day++;
            }

            while ((int)month > 12)
            {
                month -= 12;
                year++;
            }

            int daysInMonth = DaysInMonth((long)year, (int)month);

            while (day > daysInMonth)
            {
                day -= daysInMonth;
                month++;

                daysInMonth = DaysInMonth((long)year, (int)month);
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate((long)year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDate Add(CalendarDate x, CalendarDateDuration y)
        {
            var day = x.Day;
            var month = x.Month;
            var year = x.Year;
            var century = x.Century;
            var precision = x.Precision;

            if (y.Centuries != null)
            {
                century += y.Centuries.Value;
            }

            year += y.Years;

            if (y.Months != null)
            {
                month += y.Months.Value;

                if (precision < CalendarDatePrecision.Month)
                {
                    precision = CalendarDatePrecision.Month;
                }
            }

            if (y.Days != null)
            {
                day += y.Days.Value;

                if (precision < CalendarDatePrecision.Day)
                {
                    precision = CalendarDatePrecision.Day;
                }
            }

            while (month > 12)
            {
                month -= 12;
                year++;
            }

            var daysInMonth = DaysInMonth(year, month);

            while (day > daysInMonth)
            {
                day -= daysInMonth;
                month++;

                daysInMonth = DaysInMonth(year, month);
            }

            switch (precision)
            {
                case CalendarDatePrecision.Century:
                    return CalendarDate.FromCentury(century);

                case CalendarDatePrecision.Year:
                    return new CalendarDate(year);

                case CalendarDatePrecision.Month:
                    return new CalendarDate(year, month);

                case CalendarDatePrecision.Day:
                    return new CalendarDate(year, month, day);

                default:
                    return null;
            }
        }

        public static CalendarDate Add(CalendarDate x, DesignatedDuration y)
        {
            if (y.Hours != null)
            {
                throw new InvalidOperationException("The Hours component of a DesignatedDuration cannot be added to a CalendarDate.");
            }

            if (y.Minutes != null)
            {
                throw new InvalidOperationException("The Minutes component of a DesignatedDuration cannot be added to a CalendarDate.");
            }

            if (y.Seconds != null)
            {
                throw new InvalidOperationException("The Seconds component of a DesignatedDuration cannot be added to a CalendarDate.");
            }

            double day = x.Day;
            double month = x.Month;
            double year = x.Year;
            long century = x.Century;
            var precision = x.Precision;

            if (y.Years != null)
            {
                century += (long)y.Years.Value / 100;
                year += y.Years.Value;
            }

            if (y.Months != null)
            {
                month += (int)y.Months.Value;

                if (year != (int)year)
                {
                    month += (year - (int)year) * 60;
                }

                if (precision < CalendarDatePrecision.Month)
                {
                    precision = CalendarDatePrecision.Month;
                }
            }
            else if (precision == CalendarDatePrecision.Year && year != (int)year)
            {
                throw new InvalidOperationException("The Year value of a CalendarDate cannot be fractional.");
            }

            if (y.Days != null)
            {
                day += y.Days.Value;

                if (month != (int)month)
                {
                    day += (month - (int)month) * 60;
                }

                if (y.Days != (int)y.Days)
                {
                    throw new InvalidOperationException("The Day value of a CalendarDate cannot be fractional.");
                }

                if (precision < CalendarDatePrecision.Day)
                {
                    precision = CalendarDatePrecision.Day;
                }
            }
            else if (precision == CalendarDatePrecision.Month && month != (int)month)
            {
                throw new InvalidOperationException("The Month value of a CalendarDate cannot be fractional.");
            }

            while ((int)month > 12)
            {
                month -= 12;
                year++;
            }

            int daysInMonth = DaysInMonth((long)year, (int)month);

            while (day > daysInMonth)
            {
                day -= daysInMonth;
                month++;

                daysInMonth = DaysInMonth((long)year, (int)month);
            }

            switch (precision)
            {
                case CalendarDatePrecision.Century:
                    return CalendarDate.FromCentury(century);

                case CalendarDatePrecision.Year:
                    return new CalendarDate((int)year);

                case CalendarDatePrecision.Month:
                    return new CalendarDate((long)year, (int)month);

                case CalendarDatePrecision.Day:
                    return new CalendarDate((long)year, (int)month, (int)day);

                default:
                    return null;
            }
        }

        public static Time Add(Time x, TimeDuration y)
        {
            var second = x.Second;
            var minute = x.Minute;
            var hour = x.Hour;
            var precision = x.Precision;

            hour += y.Hours;

            if (y.Minutes != null)
            {
                minute += y.Minutes.Value;

                if (hour != (int)hour)
                {
                    minute += (hour - (int)hour) * 60;
                }

                while (minute > 59)
                {
                    minute -= 60;
                    hour++;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second += y.Seconds.Value;

                if (minute != (int)minute)
                {
                    second += (minute - (int)minute) * 60;
                }

                while (second > 59)
                {
                    second -= 60;
                    minute++;
                }

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new Time(hour);

                case TimePrecision.Minute:
                    return new Time((int)hour, minute);

                case TimePrecision.Second:
                    return new Time((int)hour, (int)minute, second);

                default:
                    return null;
            }
        }

        public static Time Add(Time x, DesignatedDuration y)
        {
            if (y.Years != null)
            {
                throw new InvalidOperationException("The Years component of the DesignatedDuration cannot be added to a Time.");
            }

            if (y.Months != null)
            {
                throw new InvalidOperationException("The Months component of the DesignatedDuration cannot be added to a Time.");
            }

            if (y.Days != null)
            {
                throw new InvalidOperationException("The Days component of the DesignatedDuration cannot be added to a Time.");
            }

            var second = x.Second;
            var minute = x.Minute;
            var hour = x.Hour;
            var precision = x.Precision;

            if (y.Seconds != null)
            {
                second += y.Seconds.Value;

                if (hour != (int)hour)
                {
                    minute += (hour - (int)hour) * 60;
                }

                if (minute != (int)minute)
                {
                    second += (minute - (int)minute) * 60;
                }

                while (second > 59)
                {
                    second -= 60;
                    minute++;
                }

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            if (y.Minutes != null)
            {
                precision = TimePrecision.Minute;
                minute += y.Minutes.Value;

                if (hour != (int)hour)
                {
                    minute += (hour - (int)hour) * 60;
                }

                while (minute > 59)
                {
                    minute -= 60;
                    hour++;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Hours != null)
            {
                hour += y.Hours.Value;
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new Time(hour);

                case TimePrecision.Minute:
                    return new Time((int)hour, minute);

                case TimePrecision.Second:
                    return new Time((int)hour, (int)minute, second);

                default:
                    return null;
            }
        }

        public static long CenturyOfYear(long year)
        {
            return year / 100;
        }

        public static DayOfWeek DayOfWeek(CalendarDate calendarDate)                                                                  // http://www.stoimen.com/blog/2012/04/24/computer-algorithms-how-to-determine-the-day-of-the-week/
        {
            var yearOfCentury = YearOfCentury(calendarDate.Year);

            return (DayOfWeek)(
                         (calendarDate.Day
                       + (IsLeapYear(calendarDate.Year) ? DayOfWeekMonthKeys366[calendarDate.Month] : DayOfWeekMonthKeys365[calendarDate.Month])
                       + yearOfCentury + yearOfCentury / 4
                       + DayOfWeekCenturyKeys[calendarDate.Century % 4 + calendarDate.Century < 0 ? 4 : 0])
                     % 7);
        }

        public static int DayOfYear(CalendarDate calendarDate)
        {
            return (IsLeapYear(calendarDate.Year) ? DaysToMonth366[calendarDate.Month] : DaysToMonth365[calendarDate.Month]) + calendarDate.Day;
        }

        public static int DayOfYear(WeekDate weekDate)
        {
            return (weekDate.Week - 1) * 7
                 + (weekDate.Day - 1)
                 + (int)DayOfWeek(new CalendarDate(weekDate.Year, 1, 1));
        }

        public static int DaysInMonth(long year, int month)
        {
            return month == 2 && IsLeapYear(year) ? 29 : DaysInMonthArray[month];
        }

        public static int DaysInYear(long year)
        {
            return IsLeapYear(year) ? 366 : 365;
        }

        public static int DaysToMonth(long year, int month)
        {
            return IsLeapYear(year) ? DaysToMonth366[month] : DaysToMonth365[month];
        }

        public static bool IsLeapYear(long year)                                         // http://www.timeanddate.com/date/leapyear.html
        {
            return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
        }

        public static TimePoint Subtract(TimePoint x, Duration y)
        {
            if (x is CalendarDateTime)
            {
                return Subtract((CalendarDateTime)x, y);
            }

            if (x is OrdinalDateTime)
            {
                return Subtract(((OrdinalDateTime)x).ToCalendarDateTime(), y);
            }

            if (x is WeekDateTime)
            {
                return Subtract(((WeekDateTime)x).ToCalendarDateTime(), y);
            }

            if (x is CalendarDate)
            {
                return Subtract((CalendarDate)x, y);
            }

            if (x is OrdinalDate)
            {
                return Subtract(((OrdinalDate)x).ToCalendarDate(), y);
            }

            if (x is WeekDate)
            {
                return Subtract(((WeekDate)x).ToCalendarDate(), y);
            }

            if (x is Time)
            {
                return Subtract((Time)x, y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be subtracted from a {x.GetType()}.");
        }

        public static CalendarDate Subtract(CalendarDate x, Duration y)
        {
            if (y is CalendarDateDuration)
            {
                return Subtract(x, (CalendarDateDuration)y);
            }

            if (y is OrdinalDateDuration)
            {
                return Subtract(x, ((OrdinalDateDuration)y).ToCalendarDateDuration());
            }

            if (y is DesignatedDuration)
            {
                return Subtract(x, (DesignatedDuration)y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be subtracted from a {x.GetType()}.");
        }

        public static CalendarDateTime Subtract(CalendarDateTime x, Duration y)
        {
            if (y is CalendarDateTimeDuration)
            {
                return Subtract(x, (CalendarDateTimeDuration)y);
            }

            if (y is OrdinalDateTimeDuration)
            {
                return Subtract(x, ((OrdinalDateTimeDuration)y).ToCalendarDateTimeDuration());
            }

            if (y is CalendarDateDuration)
            {
                return Subtract(x, (CalendarDateDuration)y);
            }

            if (y is OrdinalDateDuration)
            {
                return Subtract(x, ((OrdinalDateDuration)y).ToCalendarDateDuration());
            }

            if (y is TimeDuration)
            {
                return Subtract(x, (TimeDuration)y);
            }

            if (y is DesignatedDuration)
            {
                return Subtract(x, (DesignatedDuration)y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be subtracted from a {x.GetType()}.");
        }

        public static Time Subtract(Time x, Duration y)
        {
            if (y is TimeDuration)
            {
                return Subtract(x, (TimeDuration)y);
            }

            if (y is DesignatedDuration)
            {
                return Subtract(x, (DesignatedDuration)y);
            }

            throw new InvalidOperationException($"A {y.GetType()} cannot be subtracted from a {x.GetType()}.");
        }

        public static CalendarDateTime Subtract(CalendarDateTime x, CalendarDateTimeDuration y)
        {
            long year = x.Year - y.Years;
            double month = x.Month - y.Months;
            double day = x.Day - y.Days;
            double hour = x.Hour - y.Hours;
            double minute = x.Minute;
            double second = x.Second;
            TimePrecision precision = x.Precision;

            if (y.Minutes != null)
            {
                minute -= y.Minutes.Value;

                if (minute != (int)minute && precision > TimePrecision.Minute)
                {
                    second += (minute - (int)minute) * SecondsPerMinute;
                    minute = (int)minute;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second -= y.Seconds.Value;

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            while (second <= -SecondsPerMinute)
            {
                second += SecondsPerMinute;
                minute--;
            }

            while (minute <= -MinutesPerHour)
            {
                minute += MinutesPerHour;
                hour--;
            }

            while (hour <= -HoursPerDay)
            {
                hour += HoursPerDay;
                day--;
            }

            while (month < 1)
            {
                month += MonthsPerYear;
                year--;
            }

            while (day < 1)
            {
                day += DaysInMonth(year, (int)month);
                month--;
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDateTime Subtract(CalendarDateTime x, CalendarDateDuration y)
        {
            long year = x.Year - y.Years;
            double month = x.Month;
            double day = x.Day;
            double hour = x.Hour;
            double minute = x.Minute;
            double second = x.Second;
            TimePrecision precision = x.Precision;

            if (y.Months != null)
            {
                month -= y.Months.Value;

                if (month != (int)month)
                {
                    day += (month - (int)month) * DaysInMonth(year, (int)month);
                    month = (int)month;
                }
            }

            if (y.Days != null)
            {
                day -= (int)y.Days.Value;

                if (day != (int)day)
                {
                    hour += (day - (int)day) * HoursPerDay;
                    day = (int)day;
                }
            }

            while (second <= -SecondsPerMinute)
            {
                second += SecondsPerMinute;
                minute--;
            }

            while (minute <= -MinutesPerHour)
            {
                minute += MinutesPerHour;
                hour--;
            }

            while (hour <= -HoursPerDay)
            {
                hour += HoursPerDay;
                day--;
            }

            while (month < 1)
            {
                month += MonthsPerYear;
                year--;
            }

            while (day < 1)
            {
                day += DaysInMonth(year, (int)month);
                month--;
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDateTime Subtract(CalendarDateTime x, TimeDuration y)
        {
            long year = x.Year;
            double month = x.Month;
            double day = x.Day;
            double hour = x.Hour - y.Hours;
            double minute = x.Minute;
            double second = x.Second;
            TimePrecision precision = x.Precision;

            if (y.Minutes != null)
            {
                minute -= y.Minutes.Value;

                if (minute != (int)minute && precision > TimePrecision.Minute)
                {
                    second += (minute - (int)minute) * SecondsPerMinute;
                    minute = (int)minute;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second -= y.Seconds.Value;

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            while (second <= -SecondsPerMinute)
            {
                second += SecondsPerMinute;
                minute--;
            }

            while (minute <= -MinutesPerHour)
            {
                minute += MinutesPerHour;
                hour--;
            }

            while (hour <= -HoursPerDay)
            {
                hour += HoursPerDay;
                day--;
            }

            while (month < 1)
            {
                month += MonthsPerYear;
                year--;
            }

            while (day < 1)
            {
                day += DaysInMonth(year, (int)month);
                month--;
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDateTime Subtract(CalendarDateTime x, DesignatedDuration y)
        {
            long year = x.Year;
            double month = x.Month;
            double day = x.Day;
            double hour = x.Hour;
            double minute = x.Minute;
            double second = x.Second;
            TimePrecision precision = x.Precision;

            if (y.Years != null)
            {
                year -= (long)y.Years.Value;

                if (y.Years != (int)y.Years)
                {
                    month += (y.Years.Value - (long)y.Years.Value) * MonthsPerYear;
                }
            }

            if (y.Months != null)
            {
                month -= y.Months.Value;

                if (month != (int)month)
                {
                    day += (month - (int)month) * DaysInMonth(year, (int)month);
                    month = (int)month;
                }
            }

            if (y.Days != null)
            {
                day -= (int)y.Days.Value;

                if (day != (int)day)
                {
                    hour += (day - (int)day) * HoursPerDay;
                    day = (int)day;
                }
            }

            if (y.Hours != null)
            {
                hour -= y.Hours.Value;

                if (hour != (int)hour && precision > TimePrecision.Hour)
                {
                    minute += (hour - (int)hour) * MinutesPerHour;
                    hour = (int)hour;
                }
            }

            if (y.Minutes != null)
            {
                minute -= y.Minutes.Value;

                if (minute != (int)minute && precision > TimePrecision.Minute)
                {
                    second += (minute - (int)minute) * SecondsPerMinute;
                    minute = (int)minute;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Seconds != null)
            {
                second -= y.Seconds.Value;

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            while (second <= -SecondsPerMinute)
            {
                second += SecondsPerMinute;
                minute--;
            }

            while (minute <= -MinutesPerHour)
            {
                minute += MinutesPerHour;
                hour--;
            }

            while (hour <= -HoursPerDay)
            {
                hour += HoursPerDay;
                day--;
            }

            while (month < 1)
            {
                month += MonthsPerYear;
                year--;
            }

            while (day < 1)
            {
                day += DaysInMonth(year, (int)month);
                month--;
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time(hour));

                case TimePrecision.Minute:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, minute));

                case TimePrecision.Second:
                    return new CalendarDateTime(new CalendarDate(year, (int)month, (int)day), new Time((int)hour, (int)minute, second));

                default:
                    return null;
            }
        }

        public static CalendarDate Subtract(CalendarDate x, CalendarDateDuration y)
        {
            long century = x.Century;
            long year = x.Year - y.Years;
            double month = x.Month;
            double day = x.Day;
            CalendarDatePrecision precision = x.Precision;

            if (y.Days != null)
            {
                day -= y.Days.Value;

                if (precision < CalendarDatePrecision.Day)
                {
                    precision = CalendarDatePrecision.Day;
                }
            }

            if (y.Months != null)
            {
                month -= y.Months.Value;

                if (precision < CalendarDatePrecision.Month)
                {
                    precision = CalendarDatePrecision.Month;
                }
            }

            while (month < 1)
            {
                month += MonthsPerYear;
                year--;
            }

            while (day < 1)
            {
                day += DaysInMonth(year, (int)month);
                month--;
            }

            switch (precision)
            {
                case CalendarDatePrecision.Century:
                    return CalendarDate.FromCentury(century);

                case CalendarDatePrecision.Year:
                    return new CalendarDate(year);

                case CalendarDatePrecision.Month:
                    return new CalendarDate(year, (int)month);

                case CalendarDatePrecision.Day:
                    return new CalendarDate(year, (int)month, (int)day);

                default:
                    return null;
            }
        }

        public static CalendarDate Subtract(CalendarDate x, DesignatedDuration y)
        {
            long century = x.Century;
            long year = x.Year;
            double month = x.Month;
            double day = x.Day;
            CalendarDatePrecision precision = x.Precision;

            if (y.Seconds != null)
            {
                day += y.Seconds.Value / SecondsPerDay;
            }

            if (y.Minutes != null)
            {
                day += y.Minutes.Value / MinutesPerDay;
            }

            if (y.Hours != null)
            {
                day += y.Hours.Value / HoursPerDay;
            }

            if (y.Days != null)
            {
                day -= y.Days.Value;

                if (precision < CalendarDatePrecision.Day)
                {
                    precision = CalendarDatePrecision.Day;
                }
            }

            if (y.Months != null)
            {
                month -= y.Months.Value;

                if (precision < CalendarDatePrecision.Month)
                {
                    precision = CalendarDatePrecision.Month;
                }
            }

            if (y.Years != null)
            {
                if (y.Years != (int)y.Years)
                {
                    month += (y.Years.Value - (int)y.Years.Value) * MonthsPerYear;
                }

                year -= (long)y.Years.Value;

                if (precision < CalendarDatePrecision.Year)
                {
                    precision = CalendarDatePrecision.Year;
                }
            }

            while (month < 1)
            {
                month += MonthsPerYear;
                year--;
            }

            if (month != (int)month && precision > CalendarDatePrecision.Month)
            {
                day -= (month - (int)month) * DaysInMonth(year, (int)month);
                month = (int)month;
            }

            while (day < 1)
            {
                day += DaysInMonth(year, (int)month);
                month--;
            }

            if (day != (int)day)
            {
                throw new InvalidOperationException("A CalendarDate cannot have a fractional day.");
            }

            if (month != (int)month)
            {
                throw new InvalidOperationException("A CalendarDate cannot have a fractional month.");
            }

            switch (precision)
            {
                case CalendarDatePrecision.Century:
                    return CalendarDate.FromCentury(century);

                case CalendarDatePrecision.Year:
                    return new CalendarDate(year);

                case CalendarDatePrecision.Month:
                    return new CalendarDate(year, (int)month);

                case CalendarDatePrecision.Day:
                    return new CalendarDate(year, (int)month, (int)day);

                default:
                    return null;
            }
        }

        public static Time Subtract(Time x, TimeDuration y)
        {
            var hour = x.Hour;
            var minute = x.Minute;
            var second = x.Second;
            var precision = x.Precision;

            if (y.Seconds != null)
            {
                second -= y.Seconds.Value;

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            if (y.Minutes != null)
            {
                if (y.Minutes != (int)y.Minutes && precision > TimePrecision.Minute)
                {
                    second -= (y.Minutes.Value - (int)y.Minutes.Value) * SecondsPerMinute;
                    minute -= (int)y.Minutes.Value;
                }
                else
                {
                    minute -= y.Minutes.Value;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Hours != (int)y.Hours && precision > TimePrecision.Hour)
            {
                minute -= (y.Hours - (int)y.Hours) * MinutesPerHour;
                hour -= (int)y.Hours;
            }
            else
            {
                hour -= y.Hours;
            }

            while (second < -59d)
            {
                second += SecondsPerMinute;
                minute--;
            }

            while (minute < -59d)
            {
                minute += MinutesPerHour;
                hour--;
            }

            if (hour < 0d)
            {
                throw new InvalidOperationException("The resulting Time is is in the previous day.");
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new Time(hour);

                case TimePrecision.Minute:
                    return new Time((int)hour, minute);

                case TimePrecision.Second:
                    return new Time((int)hour, (int)minute, second);

                default:
                    return null;
            }
        }

        public static Time Subtract(Time x, DesignatedDuration y)
        {
            if (y.Years != null)
            {
                throw new InvalidOperationException("The Years component of the DesignatedDuration cannot be subtracted from a Time");
            }

            if (y.Months != null)
            {
                throw new InvalidOperationException("The Months component of the DesignatedDuration cannot be subtracted from a Time");
            }

            if (y.Days != null)
            {
                throw new InvalidOperationException("The Days component of the DesignatedDuration cannot be subtracted from a Time");
            }

            var hour = x.Hour;
            var minute = x.Minute;
            var second = x.Second;
            var precision = x.Precision;

            if (y.Seconds != null)
            {
                second -= y.Seconds.Value;

                if (precision < TimePrecision.Second)
                {
                    precision = TimePrecision.Second;
                }
            }

            if (y.Minutes != null)
            {
                if (y.Minutes != (int)y.Minutes && precision > TimePrecision.Minute)
                {
                    second -= (y.Minutes.Value - (int)y.Minutes.Value) * 60d;
                    minute -= (int)y.Minutes.Value;
                }
                else
                {
                    minute -= y.Minutes.Value;
                }

                if (precision < TimePrecision.Minute)
                {
                    precision = TimePrecision.Minute;
                }
            }

            if (y.Hours != null)
            {
                if (y.Hours != (int)y.Hours && precision > TimePrecision.Hour)
                {
                    minute -= (y.Hours.Value - (int)y.Hours.Value) * 60d;
                    hour -= (int)y.Hours.Value;
                }
                else
                {
                    hour -= y.Hours.Value;
                }
            }

            while (second < -59d)
            {
                second += 60d;
                minute--;
            }

            while (minute < -59d)
            {
                minute += 60d;
                hour--;
            }

            if (hour < 0d)
            {
                throw new InvalidOperationException("The resulting Time is is in the previous day.");
            }

            switch (precision)
            {
                case TimePrecision.Hour:
                    return new Time(hour);

                case TimePrecision.Minute:
                    return new Time((int)hour, minute);

                case TimePrecision.Second:
                    return new Time((int)hour, (int)minute, second);

                default:
                    return null;
            }
        }

        public static TimeSpan Subtract(CalendarDate later, CalendarDate earlier)
        {
            return TimeSpan.FromDays(
                         (later.Year - earlier.Year) * 365
                       + (later.Year - 1) / 4 - (earlier.Year - 1) / 4
                       - (later.Year - 1) / 100 + (earlier.Year - 1) / 100
                       + (later.Year - 1) / 400 - (earlier.Year - 1) / 400
                       + DaysToMonth(later.Year, later.Month)
                       - DaysToMonth(earlier.Year, earlier.Month)
                       + later.Day
                       - earlier.Day);
        }

        public static TimeSpan Subtract(CalendarDateTime later, CalendarDateTime earlier)
        {
            return TimeSpan.FromDays(
                         (later.Year - earlier.Year) * 365
                       + (later.Year - 1) / 4 - (earlier.Year - 1) / 4
                       - (later.Year - 1) / 100 + (earlier.Year - 1) / 100
                       + (later.Year - 1) / 400 - (earlier.Year - 1) / 400
                       + DaysToMonth(later.Year, later.Month)
                       - DaysToMonth(earlier.Year, earlier.Month)
                       + later.Day
                       - earlier.Day)
                 + TimeSpan.FromHours(
                         later.Hour - earlier.Hour
                       + later.Time.UtcOffset.Hours - earlier.Time.UtcOffset.Hours)
                 + TimeSpan.FromMinutes(
                         later.Minute - earlier.Minute
                       + later.Time.UtcOffset.Minutes - earlier.Time.UtcOffset.Minutes)
                 + TimeSpan.FromSeconds(later.Second - earlier.Second);
        }

        public static TimeSpan Subtract(CalendarDateTime later, CalendarDate earlier)
        {
            return TimeSpan.FromDays(
                         (later.Year - earlier.Year) * 365
                       + (later.Year - 1) / 4 - (earlier.Year - 1) / 4
                       - (later.Year - 1) / 100 + (earlier.Year - 1) / 100
                       + (later.Year - 1) / 400 - (earlier.Year - 1) / 400
                       + DaysToMonth(later.Year, later.Month)
                       - DaysToMonth(earlier.Year, earlier.Month)
                       + later.Day
                       - earlier.Day)
                 + TimeSpan.FromHours(later.Hour)
                 + TimeSpan.FromMinutes(later.Minute)
                 + TimeSpan.FromSeconds(later.Second);
        }

        public static TimeSpan Subtract(CalendarDateTime later, Time earlier)
        {
            return TimeSpan.FromHours(
                         later.Hour - earlier.Hour
                       + later.Time.UtcOffset.Hours - earlier.UtcOffset.Hours)
                 + TimeSpan.FromMinutes(
                         later.Minute - earlier.Minute
                       + later.Time.UtcOffset.Minutes - earlier.UtcOffset.Minutes)
                 + TimeSpan.FromSeconds(later.Second - earlier.Second);
        }

        public static TimeSpan Subtract(Time later, Time earlier)
        {
            return TimeSpan.FromHours(
                         later.Hour - earlier.Hour
                       + later.UtcOffset.Hours - earlier.UtcOffset.Hours)
                 + TimeSpan.FromMinutes(
                         later.Minute - earlier.Minute
                       + later.UtcOffset.Minutes - earlier.UtcOffset.Minutes)
                 + TimeSpan.FromSeconds(later.Second - earlier.Second);
        }

        public static TimeSpan Subtract(TimePoint later, TimePoint earlier)
        {
            if (later is CalendarDateTime)
            {
                return Subtract((CalendarDateTime)later, earlier);
            }

            if (later is OrdinalDateTime)
            {
                return Subtract(((OrdinalDateTime)later).ToCalendarDateTime(), earlier);
            }

            if (later is WeekDateTime)
            {
                return Subtract(((WeekDateTime)later).ToCalendarDateTime(), earlier);
            }

            if (later is CalendarDate)
            {
                return Subtract((CalendarDate)later, earlier);
            }

            if (later is OrdinalDate)
            {
                return Subtract(((OrdinalDate)later).ToCalendarDate(), earlier);
            }

            if (later is WeekDate)
            {
                return Subtract(((WeekDate)later).ToCalendarDate(), earlier);
            }

            if (later is Time)
            {
                return Subtract((Time)later, earlier);
            }

            throw new InvalidOperationException($"A {earlier.GetType()} cannot be subtracted from a {later.GetType()}");
        }

        public static int WeeksInYear(long year)
        {
            return WeekOfYear(new CalendarDate(year, 12, 28));
        }

        public static long YearOfCentury(long year)
        {
            return year % 100;
        }

        internal static TimeSpan Subtract(CalendarDate later, TimePoint earlier)
        {
            if (earlier is CalendarDate)
            {
                return later - (CalendarDate)earlier;
            }

            if (earlier is OrdinalDate)
            {
                return later - ((OrdinalDate)earlier).ToCalendarDate();
            }

            if (earlier is WeekDate)
            {
                return later - ((WeekDate)earlier).ToCalendarDate();
            }

            throw new InvalidOperationException($"A {earlier.GetType()} cannot be subtracted from a {later.GetType()}");
        }

        internal static TimeSpan Subtract(CalendarDateTime later, TimePoint earlier)
        {
            if (earlier is CalendarDateTime)
            {
                return later - (CalendarDateTime)earlier;
            }

            if (earlier is OrdinalDateTime)
            {
                return later - ((OrdinalDateTime)earlier).ToCalendarDateTime();
            }

            if (earlier is WeekDateTime)
            {
                return later - ((WeekDateTime)earlier).ToCalendarDateTime();
            }

            if (earlier is CalendarDate)
            {
                return later - (CalendarDate)earlier;
            }

            if (earlier is OrdinalDate)
            {
                return later - ((OrdinalDate)earlier).ToCalendarDate();
            }

            if (earlier is WeekDate)
            {
                return later - ((WeekDate)earlier).ToCalendarDate();
            }

            if (earlier is Time)
            {
                return later - (Time)earlier;
            }

            throw new InvalidOperationException($"A {earlier.GetType()} cannot be subtracted from a {later.GetType()}");
        }

        internal static TimeSpan Subtract(Time later, TimePoint earlier)
        {
            if (earlier is Time)
            {
                return later - (Time)earlier;
            }

            throw new InvalidOperationException($"A {earlier.GetType()} cannot be subtracted from a {later.GetType()}");
        }

        internal static int WeekOfYear(CalendarDate calendarDate)
        {
            return (DayOfYear(calendarDate) - (int)DayOfWeek(calendarDate) + 10) / 7;
        }
    }
}