using System;
using System.Diagnostics.CodeAnalysis;

namespace TellMe.Mobile.Core.Contracts.DTO
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class BulkDTO<T>
    {
        public int TotalCount { get; set; }

        public T[] Items { get; set; }

        public DateTime? OlderThanUtc { get; set; }
    }
}