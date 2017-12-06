using TellMe.Web.DAL.Contracts.Domain;

namespace TellMe.Web.DAL.Types.Domain
{
    public abstract class EntityBase<T> : IEntityBase<T>
    {
        public virtual T Id { get; set; }
    }
}
