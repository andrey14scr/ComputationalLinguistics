using ComputationalLinguistics.DAL.Core;
using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class WordRepository: Repository<Word>
    {
        public WordRepository(ComputationalLinguisticsContext context) : base(context)
        { }
    }
}