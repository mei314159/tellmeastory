using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public abstract class EntityBase<T> : IEntityBase<T>
    {
        public T Id { get; set; }
    }
}
