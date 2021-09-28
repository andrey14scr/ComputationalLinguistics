using ComputationalLinguistics.DAL.Core.Entities;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class TextFileRepository : Repository<TextFile>
    {
        public TextFileRepository(ComputationalLinguisticsContext context) : base(context)
        { }

        public async Task<TextFile> GetByIdAsync(Guid id)
        {
            return await _table.AsNoTracking().FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            _table.Remove(await GetByIdAsync(id));
        }
    }
}