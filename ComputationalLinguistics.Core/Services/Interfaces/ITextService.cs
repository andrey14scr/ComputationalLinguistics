using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITextService : IService<TextFileDto>
    {
        Task<TextFileDto> GetById(Guid id);

        Task ParseText(string connectionString, string fileName, ConcurrentBag<WordDto> toUpdate, ConcurrentBag<WordInTextDto> wordsInTextToUpdate);

        Task<bool> Exists(string path);
    }
}