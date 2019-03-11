using System;
using System.Collections.Generic;
using System.Linq;

namespace Aster.Common.Models
{
    public class Pagination<T> : List<T>
    {
        public Pagination(int totalCount, IEnumerable<T> data, Pager page) : this(totalCount, page.Page, page.Limit, data)
        {

        }

        public Pagination(int totalCount, int page, int limit, IEnumerable<T> data) : base(data)
        {
            TotalCount = totalCount;
            Page = page;
            Limit = limit;
        }

        public int Page { get; set; }

        public int Limit { get; set; }

        public int TotalCount { get; set; }

        public Pagination<T1> To<T1>(Func<T, T1> func)
        {
            return new Pagination<T1>(TotalCount, Page, Limit, this.Select(x => func(x)).ToList());
        }
    }
}
