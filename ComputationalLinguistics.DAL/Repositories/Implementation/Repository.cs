using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ComputationalLinguistics.DAL.Core;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ComputationalLinguisticsContext _context;
        private readonly DbSet<T> _table;

        public Repository(ComputationalLinguisticsContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _table.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _table.AsNoTracking().FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return predicate is null ? _table.AsNoTracking() : _table.Where(predicate).AsNoTracking();
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

        public async Task RemoveByIdAsync(Guid id)
        {
            _table.Remove(await GetByIdAsync(id));
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