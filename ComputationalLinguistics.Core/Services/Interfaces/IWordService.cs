using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Models;
using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface IWordService : IService<WordDto>
    {
        Task<IEnumerable<WordWithFrequencyDto>> GetSortedBy<T>(Expression<Func<Word, T>> keySelector, int skip, int take, bool isDesc = true);
        Task<IEnumerable<WordWithFrequencyDto>> GetSortedByFrequency(int skip, int take, bool isDesc = true);

        Task<List<WordWithFrequencyDto>> SortBy<T>(Expression<Func<Word, bool>> predicate,
            Expression<Func<Word, T>> keySelector, int skip, int take);
        Task<IEnumerable<WordContextFile>> GetContextFiles(Guid id);
        Task<IEnumerable<int>> GetUsages(Guid wordId, Guid textFileId);
        Task<int> GetFrequency(Guid wordId);
        Task<int> GetWordsCount();
        Task<int> GetWordsInTextsCount();
        Task<WordForms> GetForms(string word);
        Task<int> GetAbsoluteFrequency(string word);

        Task AddNewWords(List<WordDto> toAdd, List<WordInTextDto> wordsInTextToAdd);
    }
}