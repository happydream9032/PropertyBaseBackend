using System;
namespace PropertyBase.DTOs
{
    public class PagedRequest
    {
        public PagedRequest()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

