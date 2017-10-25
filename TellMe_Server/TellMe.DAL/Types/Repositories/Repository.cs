using System.Linq;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;

namespace TellMe.DAL.Types.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntityBase<TKey>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected DbSet<TEntity> Set => _unitOfWork.Context.Set<TEntity>();

        public void PreCommitSave()
        {
            _unitOfWork.PreCommitSave();
        }

        public void Commit()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<TEntity> GetQueryable(bool asNoTracking = false)
        {
            return asNoTracking ? Set.AsNoTracking() : Set;
        }

        public void Detach(TEntity entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Detached;
        }

        public void Save(TEntity entity, bool commit = false)
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
                _unitOfWork.PreCommitSave();
            }
        }

        public async Task SaveAsync(TEntity entity, bool commit = false)
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
                await _unitOfWork.PreCommitSaveAsync().ConfigureAwait(false);
            }
        }

        public void AddRange(IEnumerable<TEntity> entity, bool commit = false)
        {
            Set.AddRange(entity);
            if (commit)
            {
                _unitOfWork.PreCommitSave();
            }
        }

        public void Remove(TEntity entity, bool commit = false)
        {
            Set.Remove(entity);
            if (commit)
            {
                _unitOfWork.PreCommitSave();
            }
        }

        public void LoadProperty<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
        where TProperty : class
        {
            _unitOfWork.Context.Entry(entity).Reference(property);
        }

        public void LoadProperty<TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> property)
        where TProperty : class
        {
            foreach (var entity in entities)
            {
                LoadProperty(entity, property);
            }
        }

        public void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
        where TProperty : class
        {
            _unitOfWork.Context.Entry(entity).Collection(propertyExpression);
        }

        public void LoadCollection<TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
        where TProperty : class
        {
            foreach (var entity in entities)
            {
                LoadCollection(entity, propertyExpression);
            }
        }
    }
}