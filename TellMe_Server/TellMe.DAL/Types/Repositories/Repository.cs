using System.Linq;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Types.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntityBase
    {
        protected readonly IUnitOfWork UnitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected DbSet<TEntity> Set => UnitOfWork.Context.Set<TEntity>();

        public void PreCommitSave()
        {
            UnitOfWork.PreCommitSave();
        }

        public void Commit()
        {
            UnitOfWork.Commit();
        }

        public IQueryable<TEntity> GetQueryable(bool asNoTracking = false)
        {
            return asNoTracking ? Set.AsNoTracking() : Set;
        }

        public void Detach(TEntity entity)
        {
            UnitOfWork.Context.Entry(entity).State = EntityState.Detached;
        }

        public virtual void Save(TEntity entity, bool commit = false)
        {
            Set.Add(entity);
            if (commit)
            {
                UnitOfWork.PreCommitSave();
            }
        }

        public virtual async Task SaveAsync(TEntity entity, bool commit = false)
        {
            await Set.AddAsync(entity).ConfigureAwait(false);
            if (commit)
            {
                await UnitOfWork.PreCommitSaveAsync().ConfigureAwait(false);
            }
        }

        public void AddRange(IEnumerable<TEntity> entity, bool commit = false)
        {
            Set.AddRange(entity);
            if (commit)
            {
                UnitOfWork.PreCommitSave();
            }
        }

        public void Remove(TEntity entity, bool commit = false)
        {
            Set.Remove(entity);
            if (commit)
            {
                UnitOfWork.PreCommitSave();
            }
        }

        public void RemoveAll(List<TEntity> deletedMembers, bool commit)
        {
            Set.RemoveRange(deletedMembers);
            if (commit)
            {
                UnitOfWork.PreCommitSave();
            }
        }

        public void LoadProperty<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
            where TProperty : class
        {
            UnitOfWork.Context.Entry(entity).Reference(property);
        }

        public void LoadProperty<TProperty>(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, TProperty>> property)
            where TProperty : class
        {
            foreach (var entity in entities)
            {
                LoadProperty(entity, property);
            }
        }

        public void LoadCollection<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TProperty : class
        {
            UnitOfWork.Context.Entry(entity).Collection(propertyExpression);
        }

        public void LoadCollection<TProperty>(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TProperty : class
        {
            foreach (var entity in entities)
            {
                LoadCollection(entity, propertyExpression);
            }
        }
    }

    public class Repository<TEntity, TKey> : Repository<TEntity>, IRepository<TEntity, TKey> where TEntity : class, IEntityBase<TKey>
    {
        public Repository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override void Save(TEntity entity, bool commit = false)
        {
            if (Equals(entity.Id, default(TKey)))
            {
                Set.Add(entity);
            }
            else
            {
                Set.Attach(entity);
            }

            if (commit)
            {
                UnitOfWork.PreCommitSave();
            }
        }

        public override async Task SaveAsync(TEntity entity, bool commit = false)
        {
            if (Equals(entity.Id, default(TKey)))
            {
                await Set.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                Set.Attach(entity);
            }

            if (commit)
            {
                await UnitOfWork.PreCommitSaveAsync().ConfigureAwait(false);
            }
        }
    }
}