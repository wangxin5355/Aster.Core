using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aster.Common.Utils
{
    public static class CommonValidator
    {
        public static bool BankNoIsValid(string card_number)
        {
            if (!new Regex(@"^\d+$").IsMatch(card_number))
                return false;

            int[] cardNoArr = card_number.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();

            for (int i = cardNoArr.Length - 2; i >= 0; i -= 2)
            {
                cardNoArr[i] <<= 1;
                cardNoArr[i] = cardNoArr[i] / 10 + cardNoArr[i] % 10;
            }
            int sum = 0;
            for (int i = 0; i < cardNoArr.Length; i++)
            {
                sum += cardNoArr[i];
            }
            return sum % 10 == 0;
        }

        public static bool IdCardIsValid(string idcard)
        {
            var powers = new String[] { "7", "9", "10", "5", "8", "4", "2", "1", "6", "3", "7", "9", "10", "5", "8", "4", "2" };
            var parityBit = new String[] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };

            if (string.IsNullOrEmpty(idcard)) return false;
            if (idcard.Length == 15)
            {
                string _id = idcard;

                //15位的，全部都数字表示
                Match matchNumber = Regex.Match(idcard, @"^\d{15}$");
                if (!matchNumber.Success) return false;

                var year = _id.Substring(6, 2);
                var month = _id.Substring(8, 2);
                var day = _id.Substring(10, 2);
                var sexBit = _id.Substring(14);
                //校验年份位
                // ^\d{2}$
                Match matchYear = Regex.Match(year, @"^\d{2}$");
                if (!matchYear.Success) return false;
                //校验月份
                Match matchMonth = Regex.Match(month, @"^0[1-9]|1[0-2]$");
                if (!matchMonth.Success) return false;
                //校验日
                Match matchDay = Regex.Match(day, @"^[0-2][1-9]|3[0-1]|10|20$");
                if (!matchDay.Success) return false;
                //设置性别


                string strDate = string.Format("19{0}-{1}-{2}", year, month, day);
                DateTime birthDate = DateTime.Now;
                if (DateTime.TryParse(strDate, out birthDate))
                {
                    if (int.Parse("19" + year) != birthDate.Year || int.Parse(month) != birthDate.Month ||
                        int.Parse(day) != birthDate.Day)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            else if (idcard.Length == 18)
            {
                //_valid = validId18(_id);
                string _id = idcard;
                var _num = _id.Substring(0, 17);

                //18位的，前17位是全部都数字表示
                Match matchNumber = Regex.Match(_num, @"^\d{17}$");
                if (!matchNumber.Success) return false;

                var _parityBit = _id.Substring(17);
                var _power = 0;

                for (var i = 0; i < 17; i++)
                {
                    //校验每一位的合法性
                    //加权
                    _power += int.Parse(_num[i].ToString()) * int.Parse(powers[i]);
                }

                var year = _id.Substring(6, 4);
                var month = _id.Substring(10, 2);
                var day = _id.Substring(12, 2);

                //校验年份位
                Match matchYear = Regex.Match(year, @"^\d{4}$");
                if (!matchYear.Success) return false;
                //校验月份
                Match matchMonth = Regex.Match(month, @"^0[1-9]|1[0-2]$");
                if (!matchMonth.Success) return false;
                //校验日
                Match matchDay = Regex.Match(day, @"^[0-2][0-9]|3[0-1]$");
                if (!matchDay.Success) return false;
                //获取日期
                string strDate = string.Format("{0}-{1}-{2}", year, month, day);
                DateTime birthDate = DateTime.Now;
                if (DateTime.TryParse(strDate, out birthDate))
                {
                    if (int.Parse(year) != birthDate.Year || int.Parse(month) != birthDate.Month ||
                        int.Parse(day) != birthDate.Day)
                    {
                        return false;
                    }
                }
                //取模
                var mod = _power % 11;
                if (parityBit[mod] == _parityBit)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// 特殊化身份证号
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="startLen">前保留长度</param>
        /// <param name="endLen">尾保留长度</param>
        /// <param name="replaceChar">特殊字符</param>
        /// <returns>被特殊字符替换的字符串</returns>
        public static string ReplaceWithSpecialChar(string type, string strNo, int startLen, int endLen, char specialChar)
        {
            switch (type)
            {
                case "idCard":
                    if (string.IsNullOrEmpty(strNo) || strNo == null)
                        return "";
                    if (strNo.Length < 7)
                        return strNo;
                    break;
                case "bankNo":
                    if (string.IsNullOrEmpty(strNo) || strNo == null)
                        return "";
                    if (strNo.Length < 5)
                        return strNo;
                    break;
            }

            try
            {
                int lenth = strNo.Length - startLen - endLen;
                string replaceStr = strNo.Substring(startLen, lenth);
                string specialStr = string.Empty;
                for (int i = 0; i < replaceStr.Length; i++)
                {
                    specialStr += specialChar;
                }
                strNo = strNo.Replace(replaceStr, specialStr);
            }
            catch (Exception)
            {
                throw;
            }
            return strNo;
        }
    }
}
