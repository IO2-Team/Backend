using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace Org.OpenAPITools.Converters
{
    /// <summary>
    /// Custom string to enum converter
    /// </summary>
    public class CustomEnumConverter<T> : TypeConverter
    {
        /// <summary>
        /// Determine if we can convert a type to an enum
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Convert from a type value to an enum
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
            var s = value as string;
            if (string.IsNullOrEmpty(s))
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

#pragma warning disable CS8603 // Possible null reference return.
            return JsonConvert.DeserializeObject<T>(@"""" + value.ToString() + @"""");
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
