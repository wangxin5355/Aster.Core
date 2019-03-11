using System;

namespace Aster.Common.Utils
{
    public static class DisplayUtil
    {
        public static string ToFmtString(this decimal d)
        {
            string s = d.ToString("G");
            if (s.Contains('.'))
            {
                return s.TrimEnd('0').TrimEnd('.');
            }
            return s;
        }

        private static string EnsureSign(decimal d, string dStr)
        {
            if (d < 0 && dStr[0] != '-')
            {
                return $"-{dStr}";
            }
            return dStr;
        }

        public static decimal Abs(this decimal d)
        {
            return Math.Abs(d);
        }

        public static string ToFixed(this decimal d, int precision)
        {
            return string.Format($"{{0:F{precision}}}", d);
        }

        public static string TruncFixed(this decimal d, int precision)
        {
            return d.Truncate(precision: precision).ToFixed(precision);
        }

        public static string ToFmtString(this decimal d, int precision, char split = ',')
        {
            return string.Format($"{{0:N{precision}}}", d);
        }

        public static decimal Round(this decimal d, int precision)
        {
            return Math.Round(d, precision, MidpointRounding.AwayFromZero);
        }

        public static decimal Truncate(this decimal d, int precision)
        {
            var f = (decimal)Math.Pow(10, precision);

            return Math.Truncate(d * f) / f;
        }

        public static string ToFmtString(this int d, int precision, char split = ',')
        {
            return EnsureSign(d, string.Format($"{{0:N{precision}}}", d));
        }
    }
}
