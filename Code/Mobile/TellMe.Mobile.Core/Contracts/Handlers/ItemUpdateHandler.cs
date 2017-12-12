namespace TellMe.Mobile.Core.Contracts.Handlers
{
    public delegate void ItemUpdateHandler<in T>(T item, ItemState state);

    public enum ItemState
    {
        Created,
        Updated,
        Deleted
    }
}