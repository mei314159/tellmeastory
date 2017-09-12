using System.Linq;
using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Contracts.Repositories
{
	public interface IRepository<TEntity, in TKey> where TEntity: class, IEntityBase<TKey>
	{
		IQueryable<TEntity> GetQueryable(bool asNoTracking = false);
		
		void Remove(TEntity entity, bool commit = false);
		
		void Save(TEntity entity, bool commit = false);

		void PreCommitSave();

		void Commit();
	}
}