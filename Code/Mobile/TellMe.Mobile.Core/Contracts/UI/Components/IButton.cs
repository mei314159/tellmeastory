namespace TellMe.Mobile.Core.Contracts.UI.Components
{
    public interface IButton
    {
        string TitleString { get; set; }
        bool Enabled { get; set; }
        bool Hidden { get; set; }
    }
}