using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using PagedList;
using Root.Models;

namespace Root.Data.Infrastructure
{
	public abstract class RepositoryBase<T> where T : class
	{
		private SGTSVNDBContext _dataContext;
		private readonly IDbSet<T> _dbset;
		protected RepositoryBase(IDatabaseFactory databaseFactory)
		{
			DatabaseFactory = databaseFactory;
			_dbset = DataContext.Set<T>();
		}

		protected IDatabaseFactory DatabaseFactory
		{
			get;
			private set;
		}

		protected SGTSVNDBContext DataContext
		{
			get { return _dataContext ?? (_dataContext = DatabaseFactory.Get()); }
		}
		public virtual void Add(T entity)
		{
			try
			{
				_dbset.Add(entity);
			}

			catch (Exception ex)
			{
				return;
			}
		}
		public virtual void Update(T entity)
		{
			_dbset.Attach(entity);
			_dataContext.Entry(entity).State = EntityState.Modified;
		}
		public virtual void AddOrUpdate(T entity)
		{
			try
			{
				_dbset.AddOrUpdate(entity);
			}

			catch (Exception ex)
			{
				return;
			}
		}
		public virtual void Detach(T entity)
		{
			((IObjectContextAdapter)_dataContext).ObjectContext.Detach(entity);
		}
		public virtual void Delete(T entity)
		{
			_dbset.Remove(entity);
		}
		public virtual void Delete(Expression<Func<T, bool>> where)
		{
			IEnumerable<T> objects = _dbset.Where<T>(where).AsEnumerable();
			foreach (T obj in objects)
				_dbset.Remove(obj);
		}
		public virtual T GetById(long id)
		{
			return _dbset.Find(id);
		}
		public virtual T GetById(string id)
		{
			return _dbset.Find(id);
		}
		public virtual IEnumerable<T> GetAll()
		{
			return _dbset.ToList();
		}

		public virtual IQueryable<T> GetAllQueryable()
		{
			return _dbset.AsQueryable();
		}

		public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
		{
			return _dbset.Where(where).ToList();
		}

		public IEnumerable<T> GetLocal()
		{
			return _dbset.Local;
		}

		/// <summary>
		/// Return a paged list of entities
		/// </summary>
		/// <typeparam name="TOrder"></typeparam>
		/// <param name="page">Which page to retrieve</param>
		/// <param name="where">Where clause to apply</param>
		/// <param name="order">Order by to apply</param>
		/// <returns></returns>
		public virtual IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
		{
			var results = _dbset.OrderBy(order).Where(where).GetPage(page).ToList();
			var total = _dbset.Count(where);
			return new StaticPagedList<T>(results, page.PageNumber, page.PageSize, total);
		}

		public T Get(Expression<Func<T, bool>> where)
		{
			return _dbset.Where(where).FirstOrDefault<T>();
		}

		public virtual IQueryable<T> Query(Expression<Func<T, bool>> where)
		{
			return _dbset.Where(where);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		/// IEnumerable<Products> products =
		///		 _unitOfWork.ProductRepository.ExecWithStoreProcedure(
		///		 "spGetProducts @bigCategoryId",
		///		 new SqlParameter("bigCategoryId", SqlDbType.BigInt) { Value = categoryId }
		///);
		public DbRawSqlQuery<object> ExecWithStoreProcedure(string query, params object[] parameters)
		{
			return _dataContext.Database.SqlQuery<object>(query, parameters);
		}

		public IEnumerable<T> ExecWithStoreProcedureWithType(string query, params object[] parameters)
		{
			return _dataContext.Database.SqlQuery<T>(query, parameters);
		}
	}
}
