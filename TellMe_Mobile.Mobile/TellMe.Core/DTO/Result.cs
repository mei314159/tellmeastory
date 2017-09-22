using System.Collections.Generic;

namespace TellMe.Core.DTO
{
    public class Result
    {
        public string ErrorMessage { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
        public bool IsSuccess { get; set; }
		public bool IsNetworkIssue { get; set; }
    }

    public class Result<T>:Result
    {
        public T Data { get; set; }
    }

	public class Result<T, TError> : Result<T>
	{
        public TError Error { get; set; }
	}
}