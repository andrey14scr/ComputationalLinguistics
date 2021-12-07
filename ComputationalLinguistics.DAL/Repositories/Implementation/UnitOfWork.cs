using System;
using System.Threading.Tasks;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Interfaces;

namespace ComputationalLinguistics.DAL.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ComputationalLinguisticsContext _context;
        
        public IRepository<Word> Words { get; }
        public IRepository<TextFile> TextFiles { get; }
        public IRepository<WordInText> WordsInText { get; }
        public IRepository<TagInfo> TagsInfo { get; }

        public UnitOfWork(ComputationalLinguisticsContext context, IRepository<Word> words, IRepository<TextFile> textFiles, IRepository<WordInText> wordsInText, IRepository<TagInfo> tagsInfo)
        {
            _context = context;
            Words = words;
            TextFiles = textFiles;
            WordsInText = wordsInText;
            TagsInfo = tagsInfo;
        }

        public async Task<int> SaveChangesAsync()
        {
            var res = await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return res;
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}