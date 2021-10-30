using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class AnnotationHelpRepository : Repository<Word>
    {
        public AnnotationHelpRepository(ComputationalLinguisticsContext context) : base(context)
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
