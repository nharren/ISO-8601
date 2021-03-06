﻿using System.ISO8601.Abstract;

namespace System.ISO8601.Internal.Parsers
{
    internal static class TimeIntervalParser
    {
        internal static TimeInterval Parse(string input, int startYearLength, int endYearLength)
        {
            if (input.StartsWith("P"))
            {
                return DurationEndTimeInterval.Parse(input, startYearLength, endYearLength);
            }

            if (input.ContainsAfter('P', '/'))
            {
                return StartDurationTimeInterval.Parse(input, startYearLength, endYearLength);
            }

            return StartEndTimeInterval.Parse(input, startYearLength, endYearLength);
        }
    }
}