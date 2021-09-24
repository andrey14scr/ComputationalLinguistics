using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class TextFileRepository : Repository<TextFile>
    {
        public TextFileRepository(ComputationalLinguisticsContext context) : base(context)
        { }
    }
}