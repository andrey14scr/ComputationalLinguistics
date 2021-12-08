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

        public TagsController(IMapper mapper, ITagsInfoService tagsInfoService)
        {
            _mapper = mapper;
            _tagsInfoService = tagsInfoService;
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

        public async Task<ActionResult> PairsInfo(string sortBy, string pattern)
        {
            IEnumerable<TagPairDto> pairs = new List<TagPairDto>();

            switch (sortBy)
            {
                case Variables.OnFrequencyPattern:
                    pairs = await _tagsInfoService.GetPairs(p => true, p => p.Frequency);
                    break;
                case Variables.OnFrequencyBackPattern:
                    pairs = await _tagsInfoService.GetPairs(p => true, p => p.Frequency, false);
                    break;
                case Variables.OnPatternPattern:
                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        while (pattern.Contains("  "))
                        {
                            pattern = pattern.Replace("  ", " ");
                        }

                        var patterns = pattern.Trim().Split(' ');
                        if (patterns.Length == 2 && !string.IsNullOrWhiteSpace(patterns[0]) && !string.IsNullOrWhiteSpace(patterns[1]))
                        {
                            pairs = await _tagsInfoService.GetPairs(p => 
                                p.FirstTag.Substring(0, patterns[0].Length) == patterns[0] &&
                                p.SecondTag.Substring(0, patterns[1].Length) == patterns[1], 
                                p => string.Concat(p.FirstTag, p.SecondTag));
                        }
                    }
                    break;
                case Variables.OnAlphabetBackPattern:
                    pairs = await _tagsInfoService.GetPairs(p => true, p => string.Concat(p.FirstTag, p.SecondTag));
                    break;
                default:
                    pairs = await _tagsInfoService.GetPairs(p => true, p => string.Concat(p.FirstTag, p.SecondTag), false);
                    break;
            }

            return View(new TagPairsListViewModel
            {
                TagPairs = _mapper.Map<List<TagPairModel>>(pairs), 
                Pattern = pattern, 
                SortBy = sortBy,
            });
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
                    tags = await _tagsInfoService.GetSortedBy(t => t.TagName, false);
                    break;
                default:
                    tags = await _tagsInfoService.GetSortedBy(t => t.TagName);
                    break;
            }

            return _mapper.Map<List<TagInfoViewModel>>(tags);
        }
    }
}
