namespace TellMe.Core.Contracts.UI.Views
{
    public interface IView
    {
		void ShowErrorMessage(string title, string message = null);
    }
}