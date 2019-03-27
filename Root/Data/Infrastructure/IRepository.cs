using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using PagedList;

namespace Root.Data.Infrastructure
{
	public interface IRepository<T> where T : class
	{
		void Add(T entity);
		void Update(T entity);
		void AddOrUpdate(T entity);
		void Detach(T entity);
		void Delete(T entity);
		void Delete(Expression<Func<T, bool>> where);
		T GetById(long id);
		T GetById(string id);
		T Get(Expression<Func<T, bool>> where);
		IEnumerable<T> GetAll();
		IQueryable<T> GetAllQueryable();
		IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
		IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);
		IQueryable<T> Query(Expression<Func<T, bool>> where);
		DbRawSqlQuery<object> ExecWithStoreProcedure(string query, params object[] parameters);
		IEnumerable<T> ExecWithStoreProcedureWithType(string query, params object[] parameters);
		IEnumerable<T> GetLocal();
	}
}
