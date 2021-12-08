using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;
using ComputationalLinguistics.Tools;
using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.Controllers
{
    public class WordsController : Controller
    {
        private readonly IWordService _wordService;
        private readonly ITagsInfoService _tagsInfoService;
        private readonly IMapper _mapper;

        public WordsController(IWordService wordService, ITagsInfoService tagsInfoService, IMapper mapper)
        {
            _wordService = wordService;
            _tagsInfoService = tagsInfoService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(string sortBy, string pattern, int wordsBlockSize)
        {
            if (wordsBlockSize <= 0)
            {
                wordsBlockSize = Variables.WordsBlockSize;
            }

            var model = await GetWordViewModelsAsync(sortBy, pattern, 0, wordsBlockSize);

            return View(new WordsListViewModel
            {
                Words = model, 
                SortBy = sortBy, 
                Pattern = pattern, 
                WordsBlockSize = wordsBlockSize,
            });
        }

        public async Task<ActionResult> Details(Guid? id, int frequency = 0)
        {
            if (id is null)
            {
                return NotFound();
            }

            var wordDto = await _wordService.GetById(id.Value);
            if (wordDto == null)
            {
                return NotFound();
            }

            var contextFiles = await _wordService.GetContextFiles(id.Value);
            var wordForms = await _wordService.GetForms(wordDto.Content);
            var tagInfo = await _tagsInfoService.GetById(wordDto.TagInfoId);

            if (string.IsNullOrWhiteSpace(wordDto.Initial))
            {
                wordDto.Initial = wordForms.Initial;
                await _wordService.Update(wordDto);
            }

            var model = _mapper.Map<WordViewModel>(wordDto);
            model.WordContextFiles = contextFiles;
            model.Forms = wordForms;
            model.Frequency = frequency;
            model.AbsoluteFrequency = await _wordService.GetAbsoluteFrequency(wordDto.Content);
            model.TagName = tagInfo.TagName;

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateWordModel createWordModel)
        {
            WordDto wordDto = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var tagInfo = await _tagsInfoService.GetByName(createWordModel.TagName);

                    if (tagInfo is null)
                    {
                        throw new Exception($"Нет такого тэга \"{createWordModel.TagName}\".");
                    }

                    wordDto = new WordDto
                    {
                        Id = Guid.NewGuid(),
                        Content = createWordModel.Content, 
                        TagInfoId = tagInfo.Id, 
                        Initial = createWordModel.Initial,
                    };
                    await _wordService.Add(wordDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var msg = ExceptionBuilder.GetExceptionMessages(ex);

                    return View("UserError", new UserErrorViewModel { Message = "Error while word creating", InnerMessages = msg });
                }
            }

            return View(new CreateWordModel{Content = createWordModel.Content});
        }

        public async Task<ActionResult> Edit(Guid? id, string previousPage)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wordDto = await _wordService.GetById(id.Value);
            var wordForms = await _wordService.GetForms(wordDto.Content);

            if (string.IsNullOrWhiteSpace(wordDto.Initial))
            {
                wordDto.Initial = wordForms.Initial;
                await _wordService.Update(wordDto);
            }

            var tagInfo = await _tagsInfoService.GetById(wordDto.TagInfoId);
            var model = _mapper.Map<WordViewModel>(wordDto);
            model.TagName = tagInfo.TagName;

            return View(new WordViewModelFrom { PreviousPage = previousPage, WordViewModel = model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WordViewModelFrom wvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tagInfo = await _tagsInfoService.GetByName(wvm.WordViewModel.TagName);

                    if (tagInfo is null)
                    {
                        throw new Exception($"Нет такого тэга \"{wvm.WordViewModel.TagName}\".");
                    }

                    await _wordService.Update(new WordDto
                    {
                        Id = wvm.WordViewModel.Id,
                        Content = wvm.WordViewModel.Content,
                        TagInfoId = tagInfo.Id, 
                        Initial = wvm.WordViewModel.Initial,
                    });
                }
                catch (Exception ex)
                {
                    var msg = ExceptionBuilder.GetExceptionMessages(ex);

                    return View("UserError", new UserErrorViewModel { Message = "Error while word updating", InnerMessages = msg });
                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(wvm.PreviousPage);
        }

        public async Task<ActionResult> Delete(Guid id, string previousPage)
        {
            var wordDto = await _wordService.GetById(id);
            var tagInfo = await _tagsInfoService.GetById(wordDto.TagInfoId);
            var model = _mapper.Map<WordViewModel>(wordDto);
            model.TagName = tagInfo.TagName;
            model.Frequency = await _wordService.GetFrequency(id);
            model.AbsoluteFrequency = await _wordService.GetAbsoluteFrequency(wordDto.Content);

            return View(new WordViewModelFrom() { PreviousPage = previousPage, WordViewModel = model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(WordDto wordDto)
        {
            await _wordService.Remove(wordDto);

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<ActionResult> List(string sortBy, string pattern, int skip, int next)
        {
            var model = await GetWordViewModelsAsync(sortBy, pattern, skip, next);

            return View(model);
        }

        private async Task<List<WordViewModel>> GetWordViewModelsAsync(string sortBy, string pattern, int skip, int next)
        {
            IEnumerable<WordWithFrequencyDto> words = new List<WordWithFrequencyDto>();

            switch (sortBy)
            {
                case Variables.OnFrequencyPattern:
                    words = await _wordService.GetSortedByFrequency(skip, next);
                    break;
                case Variables.OnFrequencyBackPattern:
                    words = await _wordService.GetSortedByFrequency(skip, next, false);
                    break;
                case Variables.OnPatternPattern:
                    if(!string.IsNullOrWhiteSpace(pattern))
                        words = await _wordService.SortBy(w => w.Content.Substring(0, pattern.Length) == pattern, w => w.Content, skip, next);
                    break;
                case Variables.OnAnnotationPattern:
                    if (!string.IsNullOrWhiteSpace(pattern))
                        words = await _wordService.SortBy(w => w.TagInfo.TagName.Substring(0, pattern.Length) == pattern.ToUpper(), w => w.TagInfo.TagName, skip, next);
                    break;
                case Variables.OnAlphabetBackPattern:
                    words = await _wordService.GetSortedBy(w => w.Content, skip, next);
                    break;
                default:
                    words = await _wordService.GetSortedBy(w => w.Content, skip, next, false);
                    break;
            }

            return _mapper.Map<List<WordViewModel>>(words);
        }
    }
}
