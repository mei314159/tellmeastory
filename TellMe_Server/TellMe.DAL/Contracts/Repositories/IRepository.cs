using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Domain;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Types.Repositories;

namespace TellMe.DAL.Contracts.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, IEntityBase
    {
        IQueryable<TEntity> GetQueryable(bool asNoTracking = false);

        void Remove(TEntity entity, bool commit = false);
        void RemoveAll(List<TEntity> deletedMembers, bool commit);
        void Detach(TEntity entity);
        void Save(TEntity entity, bool commit = false);

        Task SaveAsync(TEntity entity, bool commit = false);

        void AddRange(IEnumerable<TEntity> entity, bool commit = false);

        void LoadProperty<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
            where TProperty : class;

        void LoadProperty<TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> property)
            where TProperty : class;

        void LoadCollection<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TProperty : class;

        void LoadCollection<TProperty>(IEnumerable<TEntity> entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TProperty : class;

        void PreCommitSave();

        void Commit();
    }

    public interface IRepository<TEntity, in TKey> : IRepository<TEntity>
        where TEntity : class, IEntityBase<TKey>
    {
    }
}