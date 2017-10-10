using System.Linq;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts.Domain;
using System.Collections.Generic;

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
    }
}