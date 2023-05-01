using System;
namespace PropertyBase.DTOs
{
    public class PagingMetadata<T>
    {
        public PagingMetadata()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; set; }
        public int Count { get; set; }
    }
}

