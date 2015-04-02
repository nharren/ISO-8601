using System.ComponentModel;
using System.ExtendedDateTimeFormat.Internal.Converters;
using System.ExtendedDateTimeFormat.Internal.Serializers;

namespace System.ExtendedDateTimeFormat
{
    [TypeConverter(typeof(UnspecifiedExtendedDateTimeConverter))]
    public class UnspecifiedExtendedDateTime : ISingleExtendedDateTimeType
    {
        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public override string ToString()
        {
            return UnspecifiedExtendedDateTimeSerializer.Serialize(this);
        }

        public ExtendedDateTimePossibilityCollection ToPossibilityCollection()
        {
            return UnspecifiedExtendedDateTimeConverter.ToPossibilityCollection(this);
        }

        public ExtendedDateTime Earliest()
        {
            return ToPossibilityCollection().Earliest();
        }

        public ExtendedDateTime Latest()
        {
            return ToPossibilityCollection().Latest();
        }
    }
}