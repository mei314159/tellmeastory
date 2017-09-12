using System;
using Microsoft.EntityFrameworkCore;

namespace TellMe.DAL.Contracts
{
	public interface IUnitOfWork : IDisposable
	{
		DbContext Context { get; }
		void Commit();
		void PreCommitSave();
		void BeginTransaction();
		void Rollback();
		void SaveChanges();
	}
}