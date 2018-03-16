using Persistence.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Persistence
{
    public class ReleaseRepository<T> where T : DomainEntity
    {
        internal ReleaseContext context;
        internal DbSet<T> dbSet;

        public ReleaseRepository(ReleaseContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                var s = query.ToString();
                return query.ToList();
            }
        }

        public virtual T GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual T GetLastEntry()
        {
            if(dbSet.Count() > 0)
                return dbSet.OrderByDescending(t => t.Id).FirstOrDefault();
            return null;
        }

        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (context.GetState(entityToDelete) == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.SetModified(entityToUpdate);
        }

        public Type GetEntityType()
        {
            return typeof(T);
        }
    }
}