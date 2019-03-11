namespace Aster.Common.Mvc.Models
{
    public class Pager
    {
        public Pager() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">页大小</param>
        public Pager(int page, int limit)
        {
            Page = page;
            Limit = limit;
        }

        public int Page { get; set; }

        public int Limit { get; set; }

        public int Offset
        {
            get { return (Page - 1) * Limit; }
            set { Page = (int)(value / Limit) + 1; }
        }
    }
}
