﻿using System.Text;

namespace System.ISO8601.Internal.Serialization
{
    internal static class TimeSerializer
    {
        internal static string Serialize(Time time, ISO8601FormatInfo formatInfo)
        {
            if (formatInfo == null)
            {
                formatInfo = ISO8601FormatInfo.Default;
            }
            var output = new StringBuilder();

            if (formatInfo.UseTimeDesignator)
            {
                output.Append('T');
            }

            var hourParts = time.Hour.ToString().Split('.', ',');

            if (time.Precision == TimePrecision.Hour && hourParts.Length > 1)
            {
                output.AppendFormat("{0}{1}{2}", int.Parse(hourParts[0]).ToString("D2"), formatInfo.DecimalSeparator == DecimalSeparator.Comma ? "," : ".", int.Parse(hourParts[1]).ToString("D" + time.FractionLength));
            }
            else
            {
                output.AppendFormat("{0}", int.Parse(hourParts[0]).ToString("D2"));
            }

            if (time.Precision != TimePrecision.Hour)
            {
                if (formatInfo.UseComponentSeparators)
                {
                    output.Append(":");
                }

                var minuteParts = time.Minute.ToString().Split('.', ',');

                if (time.Precision == TimePrecision.Minute && minuteParts.Length > 1)
                {
                    output.AppendFormat("{0}{1}{2}", int.Parse(minuteParts[0]).ToString("D2"), formatInfo.DecimalSeparator == DecimalSeparator.Comma ? "," : ".", int.Parse(minuteParts[1]).ToString("D" + time.FractionLength));
                }
                else
                {
                    output.AppendFormat("{0}", int.Parse(minuteParts[0]).ToString("D2"));
                }
            }

            if (time.Precision == TimePrecision.Second)
            {
                if (formatInfo.UseComponentSeparators)
                {
                    output.Append(":");
                }

                var secondParts = time.Second.ToString().Split('.', ',');

                if (secondParts.Length > 1)
                {
                    output.AppendFormat("{0}{1}{2}", int.Parse(secondParts[0]).ToString("D2"), formatInfo.DecimalSeparator == DecimalSeparator.Comma ? "," : ".", int.Parse(secondParts[1]).ToString("D" + time.FractionLength));
                }
                else
                {
                    output.AppendFormat("{0}", int.Parse(secondParts[0]).ToString("D2"));
                }
            }

            if (formatInfo.UseUtcOffset)
            {
                if (time.UtcOffset.Hours == 0 && time.UtcOffset.Minutes == 0)
                {
                    output.Append('Z');
                }
                else
                {
                    if (time.UtcOffset.Hours >= 0)
                    {
                        output.Append('+');
                    }

                    output.AppendFormat("{0}", time.UtcOffset.Hours.ToString("D2"));

                    if (time.UtcOffset.Minutes != 0)
                    {
                        if (formatInfo.UseComponentSeparators)
                        {
                            output.Append(':');
                        }

                        output.AppendFormat("{0}", time.UtcOffset.Minutes.ToString("D2"));
                    }
                }
            }

            return output.ToString();
        }
    }
}