using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.DAL;
using Microsoft.EntityFrameworkCore;
using ComputationalLinguistics.Core.Services.Interfaces;
using System.Threading.Tasks;
using ComputationalLinguistics.Models;
using ComputationalLinguistics.Tools;

namespace ComputationalLinguistics.Controllers
{
    public class TagsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITagsInfoService _tagsInfoService;
        private readonly ComputationalLinguisticsContext _context;

        public TagsController(IMapper mapper, ITagsInfoService tagsInfoService, ComputationalLinguisticsContext context)
        {
            _mapper = mapper;
            _tagsInfoService = tagsInfoService;
            _context = context;
        }

        public async Task<ActionResult> Index(string sortBy, string pattern)
        {
            var model = await GetTagViewModelsAsync(sortBy, pattern);

            return View(new TagsListViewModel
            {
                Tags = model, 
                Pattern = pattern, 
                SortBy = sortBy,
            });
        }

        public async Task<ActionResult> PairsInfo()
        {
            var pairs = await _context.WordsInText
                .Where(wit => wit.NextWordInTextId.HasValue)
                .GroupBy(wit => new
                {
                    FirstTag = wit.Word.TagInfo.TagName,
                    SecondTag = wit.NextWordInText.Word.TagInfo.TagName,
                })
                .Select(group => new TagPairViewModel
                {
                    FirstTag = group.Key.FirstTag, 
                    SecondTag = group.Key.SecondTag,
                    Frequency = group.Count()
                })
                .ToListAsync();

            return View(pairs);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(CreateTagModel createTagModel)
        {
            try
            {
                var model = new TagInfoDto()
                {
                    Id = Guid.NewGuid(),
                    TagName = createTagModel.TagName,
                    Info = createTagModel.Info, 
                    IsGeneric = false,
                };

                await _tagsInfoService.Add(model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var model = await _tagsInfoService.GetById(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, TagInfoDto tagInfoDto)
        {
            try
            {
                var model = new TagInfoDto()
                {
                    Id = id,
                    TagName = tagInfoDto.TagName,
                    Info = tagInfoDto.Info,
                };

                await _tagsInfoService.Update(model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var model = await _tagsInfoService.GetById(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(Guid id, IFormCollection collection)
        {
            try
            {
                var model = await _tagsInfoService.GetById(id);

                await _tagsInfoService.Remove(model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<List<TagInfoViewModel>> GetTagViewModelsAsync(string sortBy, string pattern)
        {
            IEnumerable<TagInfoWithFrequencyDto> tags = new List<TagInfoWithFrequencyDto>();

            switch (sortBy)
            {
                case Variables.OnFrequencyPattern:
                    tags = await _tagsInfoService.GetSortedByFrequency();
                    break;
                case Variables.OnFrequencyBackPattern:
                    tags = await _tagsInfoService.GetSortedByFrequency(false);
                    break;
                case Variables.OnPatternPattern:
                    if (!string.IsNullOrWhiteSpace(pattern))
                        tags = await _tagsInfoService.SortBy(t => t.TagName.Substring(0, pattern.Length) == pattern, w => w.TagName);
                    break;
                case Variables.OnAlphabetBackPattern:
                    tags = await _tagsInfoService.GetSortedBy(t => t.TagName);
                    break;
                default:
                    tags = await _tagsInfoService.GetSortedBy(t => t.TagName, false);
                    break;
            }

            return _mapper.Map<List<TagInfoViewModel>>(tags);
        }
    }
}
