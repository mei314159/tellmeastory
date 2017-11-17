using System;
using System.Collections;

namespace TellMe.Core.Contracts.DataServices
{
    public class DataResult
    {
        public DataResult(DateTime updateDateUtc)
        {
            UpdateDateUtc = updateDateUtc;
        }

        public DateTime UpdateDateUtc { get; }

        public virtual bool Expired => (DateTime.UtcNow - this.UpdateDateUtc).TotalMinutes > 5;
    }

    public class DataResult<T> : DataResult
    {
        public DataResult(DateTime updateDateUtc, T data) : base(updateDateUtc)
        {
            Data = data;
        }

        public T Data { get; }

        public override bool Expired => base.Expired || (Data as ICollection)?.Count == 0;
    }
}