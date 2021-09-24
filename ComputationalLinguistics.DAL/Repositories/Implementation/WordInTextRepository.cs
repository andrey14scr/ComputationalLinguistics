using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class WordInTextRepository : Repository<WordInText>
    {
        public WordInTextRepository(ComputationalLinguisticsContext context) : base(context)
        { }
    }
}