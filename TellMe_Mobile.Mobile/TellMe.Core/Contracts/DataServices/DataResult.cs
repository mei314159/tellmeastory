using System;
namespace TellMe.Core.Contracts.DataServices
{
    public class DataResult
    {
        public DataResult(DateTime updateDateUtc)
        {
            UpdateDateUtc = updateDateUtc;
        }

        public DateTime UpdateDateUtc { get; set; }

        public bool Expired
        {
            get
            {
                return (DateTime.UtcNow - this.UpdateDateUtc).TotalMinutes > 5; //Should be editable
            }
        }
    }

    public class DataResult<T> : DataResult
    {
        public DataResult(DateTime updateDateUtc, T data) : base(updateDateUtc)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
