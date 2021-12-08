using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.DAL.Core.Entities;

using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITagsInfoService : IService<TagInfoDto>
    {
        Task<TagInfoDto> GetByName(string name);
        Task<int> GetCountByTagsName(string tagName);
        Task<IEnumerable<TagPairDto>> GetPairs<T>(Func<TagPairDto, bool> predicate, Func<TagPairDto, T> keySelector, bool isDesc = true);

        Task<IEnumerable<TagInfoWithFrequencyDto>> GetSortedBy<T>(Expression<Func<TagInfo, T>> keySelector, bool isDesc = true);
        Task<IEnumerable<TagInfoWithFrequencyDto>> GetSortedByFrequency(bool isDesc = true);
        Task<List<TagInfoWithFrequencyDto>> SortBy<T>(Expression<Func<TagInfo, bool>> predicate,
            Expression<Func<TagInfo, T>> keySelector);
    }
}
