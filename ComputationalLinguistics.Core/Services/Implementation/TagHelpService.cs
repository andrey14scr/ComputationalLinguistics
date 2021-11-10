using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ComputationalLinguistics.Core.Services.Implementation
{
    public class TagHelpService : ITagHelpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public TagHelpService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<TagInfoDto>> GetAll()
        {
            var tagInfos = await _unitOfWork.TagsInfo.GetAllAsync();
            var tagInfoDtos = _mapper.Map<IEnumerable<TagInfoDto>>(tagInfos);
            return tagInfoDtos;
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
    }
}