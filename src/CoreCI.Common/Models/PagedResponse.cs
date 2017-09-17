namespace CoreCI.Common.Models
{
    using System.Collections.Generic;

    public class PagedResponse<T>
    {
//        public int PageLength { get; set; }
        public List<T> Values { get; set; }
        public int Page { get; set; }
//        public int Size { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
    }
}
