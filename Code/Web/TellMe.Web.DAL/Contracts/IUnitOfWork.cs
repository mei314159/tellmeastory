using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TellMe.DAL.Contracts
{
	public interface IUnitOfWork : IDisposable
	{
		DbContext Context { get; }
		void Commit();
		void PreCommitSave();
		Task PreCommitSaveAsync();
		void BeginTransaction();
		void Rollback();
		void SaveChanges();
	}
}