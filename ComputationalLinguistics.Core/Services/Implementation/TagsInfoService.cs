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
    }
}
