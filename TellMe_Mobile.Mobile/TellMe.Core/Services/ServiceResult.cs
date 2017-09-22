using System;
using System.Linq;
using FluentValidation.Results;
using TellMe.Core.DTO;
using TellMe.Core.Validation;

namespace TellMe.Core.Services
{
    public class ServiceResult
    {
        public ServiceResult()
        {
        }

        public ServiceResult(ValidationResult validationResult, Result result = null)
        {
            ValidationResult = validationResult;
            Result = result;
        }

        public Result Result { get; set; }

        public ValidationResult ValidationResult { get; set; }


        public bool IsValid => this.ValidationResult?.IsValid != false
                                   && this.Result?.IsSuccess == true;


        public string ErrorsString
        {
            get
            {
                string message = null;

                if (ValidationResult?.IsValid == false)
                {
                    message = string.Join(Environment.NewLine, ValidationResult.Errors.Select(x => x.ErrorMessage));
                }
                else if (Result?.IsSuccess == false)
                {
                    if (Result.ModelState != null)
                    {
                        message = string.Join(Environment.NewLine, Result.ModelState.SelectMany(x => x.Value));
                    }
                    else
                    {
                        message = Result.ErrorMessage;
                    }
                }

                return message;
            }
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public ServiceResult()
        {
        }

        public ServiceResult(ValidationResult validationResult, Result<T> result = null) : base(validationResult, result)
        {
        }

        public new Result<T> Result
        {
            get
            {
                return (Result<T>)base.Result;
            }
            set
            {
                base.Result = value;
            }
        }
    }

	public class ServiceResult<T, TError> : ServiceResult
	{
		public ServiceResult()
		{
		}

		public ServiceResult(ValidationResult validationResult, Result<T, TError> result = null) : base(validationResult, result)
		{
		}

		public new Result<T, TError> Result
		{
			get
			{
				return (Result<T, TError>)base.Result;
			}
			set
			{
				base.Result = value;
			}
		}
	}
}
