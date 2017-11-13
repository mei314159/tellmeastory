using System;
using System.Collections.Generic;

namespace TellMe.DAL.Contracts.DTO
{
    public class BulkDTO<T>
    {
        public int TotalCount { get; set; }

        public ICollection<T> Items { get; set; }

        public DateTime? OlderThanUtc { get; set; }
    }
}
