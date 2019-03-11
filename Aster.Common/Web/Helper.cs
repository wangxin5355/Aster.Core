using System;

namespace Aster.Common.Web
{
    public static class Helper
    {
        /// <summary>
        /// 环境
        /// </summary>
        public static int Env = 0;

        /// <summary>
        /// 对price向下取整
        /// </summary>
        /// <param name="minPrice"></param>
        /// <param name="minPricePrecision"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static decimal FloorPrice(decimal minPrice, int minPricePrecision, decimal price)
        {
            long f = (long)Math.Pow(10, minPricePrecision);
            long minPriceL = (long)(minPrice * f);

            long priceL = (long)(price * f);
            priceL = priceL / minPriceL;
            priceL = priceL * minPriceL;

            return (decimal)priceL / f;
        }

        /// <summary>
        /// 对price向上取整
        /// </summary>
        /// <param name="minPrice"></param>
        /// <param name="minPricePrecision"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static decimal CeilPrice(decimal minPrice, int minPricePrecision, decimal price)
        {
            long f = (long)Math.Pow(10, minPricePrecision);
            long minPriceL = (long)(minPrice * f);

            long priceL = (long)(price * f);
            priceL = (long)Math.Ceiling((double)priceL / minPriceL);
            priceL = priceL * minPriceL;

            return (decimal)priceL / f;
        }

        /// <summary>
        /// 获取币种的排序
        /// </summary>
        /// <param name="quotationCurrency"></param>
        /// <returns></returns>
        public static int GetCurrencySeq(string quotationCurrency)
        {
            switch (quotationCurrency)
            {
                case "BTC": return 0;
                case "ETH": return 1;
                case "ETC": return 2;
                case "XRP": return 3;
                case "EOS": return 4;
                case "LTC": return 5;
                case "ADA": return 6;
                case "BCH": return 7;
                case "BSV": return 8;
                default: return 9;
            }
        }
    }
}
