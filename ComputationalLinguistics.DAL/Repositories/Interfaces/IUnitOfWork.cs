using System.Threading.Tasks;
using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Word> Words { get; }
        IRepository<TextFile> TextFiles { get; }
        IRepository<WordInText> WordsInText { get; }
        IRepository<TagInfo> TagsInfo { get; }

        Task<int> SaveChangesAsync();
    }
}