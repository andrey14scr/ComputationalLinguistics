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

        public async Task UpdateFrequencyAsync(Word word)
        {
            var w = await _table.AsNoTracking().FirstAsync(w => w.Id == word.Id);
            w.Frequency += word.Frequency;
            _table.Update(w);
        }
    }
}