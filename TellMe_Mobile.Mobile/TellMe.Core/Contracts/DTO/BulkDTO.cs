using System;
using System.Collections.Generic;

namespace TellMe.Core.Contracts.DTO
{
    public class BulkDTO<T>
    {
        public int TotalCount { get; set; }

        public T[] Items { get; set; }

        public DateTime? OlderThanUtc { get; set; }
    }
}