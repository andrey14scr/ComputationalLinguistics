using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.DAL;
using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Core.Services.Implementation;
using System.Threading.Tasks;
using ComputationalLinguistics.Models;

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

        public async Task<ActionResult> Index()
        {
            var tagDtos = await _tagsInfoService.GetAll();
            var model = _mapper.Map<List<TagInfoViewModel>>(tagDtos);

            foreach (var ti in model)
            {
                ti.Frequency = await _tagsInfoService.GetCountByTagsName(ti.TagName);
            }

            return View(model);
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
    }
}
