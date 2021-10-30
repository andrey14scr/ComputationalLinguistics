using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class TagInfoRepository : Repository<TagInfo>
    {
        public TagInfoRepository(ComputationalLinguisticsContext context) : base(context)
        {
        }

        public async Task<TagInfo> GetByIdAsync(Guid id)
        {
            return await _table.AsNoTracking().FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            _table.Remove(await GetByIdAsync(id));
        }
    }
}
