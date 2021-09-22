using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.DAL.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class, IBaseEntity
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);

        Task AddAsync(T obj);
        Task AddRangeAsync(IEnumerable<T> objs);

        void Update(T obj);
        void UpdateRange(IEnumerable<T> objs);

        Task RemoveByIdAsync(Guid id);
        void Remove(T obj);
        void RemoveRange(IEnumerable<T> objs);
    }
}