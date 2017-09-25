namespace TellMe.Core.Contracts.UI.Views
{
    public interface IImportContactsView
    {
        void ShowErrorMessage(string title, string message = null);
    }
}