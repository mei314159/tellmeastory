namespace TellMe.DAL.Contracts.Domain
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
    }
}
