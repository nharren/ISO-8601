using System.ExtendedDateTimeFormat.Internal.Serializers;

namespace System.ExtendedDateTimeFormat
{
    public class ExtendedDateTimeRange : IExtendedDateTimeSetType
    {
        public ISingleExtendedDateTimeType Start { get; set; }

        public ISingleExtendedDateTimeType End { get; set; }

        public override string ToString()
        {
            return ExtendedDateTimeRangeSerializer.Serialize(this);
        }
    }
}