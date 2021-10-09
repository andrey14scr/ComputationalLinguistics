using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputationalLinguistics.DAL.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();
        IQueryable<T> GetNoTracking();
        IQueryable<T> GetNoTrackingWhere(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetTrackingWhere(Expression<Func<T, bool>> predicate);

        Task AddAsync(T obj);
        Task AddRangeAsync(IEnumerable<T> objs);

        void Update(T obj);
        void UpdateRange(IEnumerable<T> objs);

        void Remove(T obj);
        void RemoveRange(IEnumerable<T> objs);
    }
}