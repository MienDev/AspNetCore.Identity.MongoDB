using System;
using System.ComponentModel;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    public class UserEmailConverter : TypeConverter
    {

        /// <summary>
        /// Overrides the CanConvertFrom method of TypeConverter.
        /// The ITypeDescriptorContext interface provides the context for the
        /// conversion. Typically, this interface is used at design time to 
        /// provide information about the design-time container.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// convert string to email type
        /// did not validate.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            UserEmail result = null;
            if (value is string)
            {
                result = new UserEmail(value.ToString());
            }
            return result ?? base.ConvertFrom(context, culture, value);
        }
    }
}