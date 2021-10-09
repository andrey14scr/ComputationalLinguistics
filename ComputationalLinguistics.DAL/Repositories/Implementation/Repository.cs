using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ComputationalLinguisticsContext _context;
        protected readonly DbSet<T> _table;

        public Repository(ComputationalLinguisticsContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _table.AsNoTracking().ToListAsync();
        }

        public IQueryable<T> GetNoTracking()
        {
            return _table.AsNoTracking();
        }

        public IQueryable<T> GetNoTrackingWhere(Expression<Func<T, bool>> predicate)
        {
            return predicate is null ? _table.AsNoTracking() : _table.AsNoTracking().Where(predicate);
        }

        public IQueryable<T> GetTrackingWhere(Expression<Func<T, bool>> predicate)
        {
            return predicate is null ? _table : _table.Where(predicate);
        }

        public async Task AddAsync(T obj)
        {
            await _table.AddAsync(obj);
        }

        public async Task AddRangeAsync(IEnumerable<T> objs)
        {
            await _table.AddRangeAsync(objs);
        }

        public void Update(T obj)
        {
            _table.Update(obj);
        }

        public void UpdateRange(IEnumerable<T> objs)
        {
            _table.UpdateRange(objs);
        }

        public void Remove(T obj)
        {
            _table.Remove(obj);
        }

        public void RemoveRange(IEnumerable<T> objs)
        {
            _table.RemoveRange(objs);
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}