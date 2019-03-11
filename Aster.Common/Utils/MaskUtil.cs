using System.Collections.Generic;

namespace Aster.Common.Utils
{
    public class MaskUtil
    {
        public static string IdNoMask(string idNo)
        {
            //320***********3414

            return Mask(idNo, 3, 4);
        }

        public static string BankNoMask(string bankNo)
        {
            ////**** **** **** 8664

            //string s = Mask(bankNo, 0, 4);

            //List<char> cs = new List<char>();
            //for (int i = 1; i <= s.Length; i++)
            //{
            //    cs.Add(s[i - 1]);
            //    if (i % 4 == 0) cs.Add(' ');
            //}

            //return new string(cs.ToArray()).TrimEnd();
            //(8664)

            if (bankNo.Length <= 4) return $"({bankNo})";
            return $"({bankNo.Substring(bankNo.Length-4)})";
        }

        public static string Mask(string s, int leftLen, int rightLen, char padChar = '*')
        {
            if (leftLen + rightLen > s.Length)
            {
                return new string(padChar, s.Length - 1);
            }

            var left = s.Substring(0, leftLen);
            var right = s.Substring(s.Length - rightLen);

            return $"{left}{new string(padChar, s.Length - leftLen - rightLen)}{right}";
        }
    }
}
