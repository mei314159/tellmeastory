namespace TellMe.Web.DAL.Contracts.Domain
{
    public interface IEntityBase
    {
    }

    public interface IEntityBase<T> : IEntityBase
    {
        T Id { get; set; }
    }
}