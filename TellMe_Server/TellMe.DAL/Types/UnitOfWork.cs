using System.Threading.Tasks;
using TellMe.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace TellMe.DAL.Types
{
	public class UnitOfWork : IUnitOfWork
	{
		protected IDbContextTransaction Transaction { get; set; }

		private AppDbContext _dbContext;
		public virtual DbContext Context => _dbContext;

		public UnitOfWork(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public virtual void BeginTransaction()
		{
			if (null == Transaction)
			{
				Transaction = Context.Database.BeginTransaction();
			}
		}

		public virtual void PreCommitSave()
		{
			_dbContext?.SaveChanges();
		}

		public virtual async Task PreCommitSaveAsync()
		{
			if (_dbContext != null)
				await _dbContext.SaveChangesAsync().ConfigureAwait(false);
		}

		public virtual void Commit()
		{
			_dbContext?.SaveChanges();
			if (Transaction != null)
			{
				Transaction.Commit();
				Transaction.Dispose();
				Transaction = null;
			}
			if (_dbContext != null)
			{
				_dbContext.Dispose();
				_dbContext = null;
			}
		}

		public virtual void Dispose()
		{
			// _dbContext.Dispose();
		}

		public virtual void Rollback()
		{
			if (Transaction != null)
			{
				Transaction.Rollback();
				Transaction.Dispose();
				Transaction = null;
			}
			if (_dbContext != null)
			{
				_dbContext.Dispose();
				_dbContext = null;
			}
		}

		public virtual void SaveChanges()
		{
			_dbContext?.SaveChanges();
			if (Transaction != null)
			{
				Transaction.Commit();
				Transaction.Dispose();
				Transaction = null;
			}
		}
	}
}