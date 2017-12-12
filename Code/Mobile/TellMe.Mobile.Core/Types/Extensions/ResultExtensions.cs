using System;
using System.Linq;
using FluentValidation.Results;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Types.Extensions
{
    public static class ResultExtensions
    {
        public static void ShowResultError(this Result result, IView view)
        {
            if (result.IsSuccess)
                return;
            else if (result.IsNetworkIssue)
            {
                view.ShowErrorMessage("Error", result.ErrorMessage);
                return;
            }

            if (result.ModelState != null)
            {
                view.ShowErrorMessage("Error",
                    string.Join(Environment.NewLine, result.ModelState.SelectMany(x => x.Value)));
                return;
            }

            var authResult = result as Result<AuthenticationInfoDTO, AuthenticationErrorDto>;
            if (authResult?.Error != null)
            {
                view.ShowErrorMessage("Error", authResult.Error.ErrorMessage);
            }
            else
            {
                view.ShowErrorMessage("Error", result.ErrorMessage);
            }
        }

        public static void ShowValidationResult(this ValidationResult validationResult, IView view)
        {
            string message = string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));
            if (!string.IsNullOrWhiteSpace(message))
            {
                view.ShowErrorMessage("Validation error", message);
            }
            else
            {
                view.ShowErrorMessage("Error");
            }
        }
    }
}