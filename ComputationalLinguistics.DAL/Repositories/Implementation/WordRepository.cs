using System;
using System.Threading.Tasks;
using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class WordRepository : Repository<Word>
    {
        public WordRepository(ComputationalLinguisticsContext context) : base(context)
        {
        }

        public async Task<Word> GetByIdAsync(Guid id)
        {
            return await _table.AsNoTracking().FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            _table.Remove(await GetByIdAsync(id));
        }
    }
}