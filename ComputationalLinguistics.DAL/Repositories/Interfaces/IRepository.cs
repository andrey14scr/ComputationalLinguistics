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
        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);
        IQueryable<T> GetTracking(Expression<Func<T, bool>> predicate);

        Task AddAsync(T obj);
        Task AddRangeAsync(IEnumerable<T> objs);

        void Update(T obj);
        void UpdateRange(IEnumerable<T> objs);

        void Remove(T obj);
        void RemoveRange(IEnumerable<T> objs);
    }
}