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
        Task<IEnumerable<WordDto>> GetSortedBy<T>(Expression<Func<Word, T>> keySelector, bool isDesc = true);
        Task<IEnumerable<WordDto>> SortBy(Expression<Func<Word, bool>> predicate);
        Task<IEnumerable<WordContextFile>> GetContextFiles(Guid id);
    }
}