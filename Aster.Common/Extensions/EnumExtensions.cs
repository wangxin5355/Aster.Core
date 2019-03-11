using System;
using System.ComponentModel;
using System.Reflection;

namespace Aster.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum @enum)
        {
            FieldInfo f = @enum.GetType().GetField(@enum.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])f.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return @enum.ToString();
            }
        }
    }
}
