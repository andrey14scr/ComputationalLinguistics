using AutoMapper;

using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Implementation;
using ComputationalLinguistics.DAL.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputationalLinguistics.Core.Services.Implementation
{
    public class TagsInfoService : ITagsInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public TagsInfoService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task Add(TagInfoDto tagInfoDto)
        {
            var tagInfo = _mapper.Map<TagInfo>(tagInfoDto);
            await _unitOfWork.TagsInfo.AddAsync(tagInfo);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddRange(IEnumerable<TagInfoDto> tagInfoDtos)
        {
            var tagInfos = _mapper.Map<List<TagInfo>>(tagInfoDtos);
            await _unitOfWork.TagsInfo.AddRangeAsync(tagInfos);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<TagInfoDto>> GetAll()
        {
            var tagInfos = await _unitOfWork.TagsInfo.GetAllAsync();
            var tagInfoDtos = _mapper.Map<List<TagInfoDto>>(tagInfos);

            return tagInfoDtos;
        }

        public async Task<TagInfoDto> GetById(Guid id)
        {
            var tagInfo = await(_unitOfWork.TagsInfo as TagInfoRepository).GetByIdAsync(id);
            var tagInfoDto = _mapper.Map<TagInfoDto>(tagInfo);

            return tagInfoDto;
        }

        public async Task<TagInfoDto> GetByName(string name)
        {
            if (!_cache.TryGetValue(name, out var tagInfoDto))
            {
                var tagInfo = await _unitOfWork.TagsInfo.GetNoTrackingWhere(t => t.TagName == name)
                    .FirstOrDefaultAsync();

                tagInfoDto = _mapper.Map<TagInfoDto>(tagInfo);

                if (tagInfoDto != null)
                {
                    _cache.Set(name, tagInfoDto, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
                }
            }

            return (TagInfoDto)tagInfoDto;
        }

        public async Task Remove(TagInfoDto tagInfoDto)
        {
            var tagInfo = _mapper.Map<TagInfo>(tagInfoDto);
            _unitOfWork.TagsInfo.Remove(tagInfo);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<TagInfoDto> tagInfoDtos)
        {
            var tagInfos = _mapper.Map<List<TagInfo>>(tagInfoDtos);
            _unitOfWork.TagsInfo.RemoveRange(tagInfos);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(TagInfoDto tagInfoDto)
        {
            var tagInfo = _mapper.Map<TagInfo>(tagInfoDto);
            _unitOfWork.TagsInfo.Update(tagInfo);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetCountByTagsName(string tagName)
        {
            var tagInfo = await _unitOfWork.TagsInfo.GetNoTrackingWhere(ti => ti.TagName == tagName).FirstOrDefaultAsync();

            return await _unitOfWork.Words.GetNoTrackingWhere(w => w.TagInfoId == tagInfo.Id).CountAsync();
        }

        public async Task<IEnumerable<TagInfoWithFrequencyDto>> GetSortedBy<T>(Expression<Func<TagInfo, T>> keySelector, bool isDesc = true)
        {
            var query = _unitOfWork.TagsInfo.GetNoTracking();

            query = isDesc ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

            var tags = await query
                .Select(ti => new TagInfoWithFrequencyDto
                {
                    Id = ti.Id, 
                    Info = ti.Info, 
                    IsGeneric = ti.IsGeneric,
                    TagName = ti.TagName,
                    Frequency = _unitOfWork.Words.GetNoTracking().Count(w => w.TagInfoId == ti.Id),
                })
                .ToListAsync();

            return tags;
        }

        public async Task<IEnumerable<TagInfoWithFrequencyDto>> GetSortedByFrequency(bool isDesc = true)
        {
            var query = _unitOfWork.TagsInfo.GetNoTracking()
                .Select(ti => new TagInfoWithFrequencyDto
                {
                    Id = ti.Id, 
                    Info = ti.Info, 
                    IsGeneric = ti.IsGeneric,
                    TagName = ti.TagName,
                    Frequency = _unitOfWork.Words.GetNoTracking().Count(w => w.TagInfoId == ti.Id),
                });

            var tags = isDesc
                ? await query.OrderByDescending(ti => ti.Frequency).ToListAsync()
                : await query.OrderBy(ti => ti.Frequency).ToListAsync();
            ;

            return tags;
        }

        public async Task<List<TagInfoWithFrequencyDto>> SortBy<T>(Expression<Func<TagInfo, bool>> predicate, Expression<Func<TagInfo, T>> keySelector)
        {
            var tags = await _unitOfWork.TagsInfo.GetNoTrackingWhere(predicate)
                .OrderBy(keySelector)
                .Select(ti => new TagInfoWithFrequencyDto
                {
                    Id = ti.Id,  
                    TagName = ti.TagName,
                    Info = ti.Info, 
                    IsGeneric = ti.IsGeneric,
                    Frequency = _unitOfWork.Words.GetNoTracking().Count(w => w.TagInfoId == ti.Id),
                })
                .ToListAsync();

            return tags;
        }

        public async Task<IEnumerable<TagPairDto>> GetPairs<T>(Func<TagPairDto, bool> predicate, Func<TagPairDto, T> keySelector, bool isDesc = true)
        {
            var pairs = _unitOfWork.WordsInText
                .GetNoTrackingWhere(wit => wit.NextWordInTextId.HasValue)
                .GroupBy(wit => new
                {
                    FirstTag = wit.Word.TagInfo.TagName,
                    SecondTag = wit.NextWordInText.Word.TagInfo.TagName,
                })
                .Select(group => new TagPairDto
                {
                    FirstTag = group.Key.FirstTag,
                    SecondTag = group.Key.SecondTag,
                    Frequency = group.Count()
                })
                .Where(predicate)
                .ToList();

            var all = await GetAll();

            foreach (var first in all)
            {
                foreach (var second in all)
                {
                    if (!pairs.Exists(p => p.FirstTag == first.TagName && p.SecondTag == second.TagName))
                    {
                        var pair = new TagPairDto
                        {
                            FirstTag = first.TagName,
                            SecondTag = second.TagName,
                            Frequency = 0,
                        };

                        if (predicate(pair))
                        {
                            pairs.Add(pair);
                        }
                    }
                }
            }

            if (!isDesc)
            {
                pairs = pairs.OrderBy(keySelector).ToList();
            }
            else
            {
                pairs = pairs.OrderByDescending(keySelector).ToList();
            }

            return pairs;
        }
    }
}
