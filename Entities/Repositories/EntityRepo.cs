using Entities.Data;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Entities.Repositories
{
    public interface IEntityRepo<T>
    {
        ICollection<T> GetAll(Func<IQueryable<T>, IQueryable<T>>? include = null);
        IQueryable<T> Query(Expression<Func<T, bool>>? Cond = null);
        ICollection<T> FindAll(Expression<Func<T, bool>> Cond);
        T GetById(int id, Func<IQueryable<T>, IQueryable<T>>? include = null);

        void Add(T Entity);
        void Update(T Entity);
        void Delete(int id);
        int SaveChanges();
    }



    public class EntityRepo<T> : IEntityRepo<T> where T : class
    {
        AppDbContext dbContext;
        DbSet<T> Set;

        public EntityRepo(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
            Set = dbContext.Set<T>();
        }

        //===============================================
        public void Add(T Entity)
        {
            Set.Add(Entity);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> Cond)
        {
            return Set.Where(Cond).ToList();
        }

        public ICollection<T> GetAll(Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = Set;

            if (include != null)
                query = include(query);

            return query.ToList();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>>? Cond = null)
        {
            IQueryable<T> query = Set;

            if (Cond != null)
                query = query.Where(Cond);

            return query;
        }

        public T? GetById(int id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = Set;

            if (include != null)
                query = include(query);

            return query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public void Update(T Entity)
        {
            Set.Update(Entity);
        }

        public void Delete(int id)
        {
            Set.Remove(GetById(id));
        }
    }
}
