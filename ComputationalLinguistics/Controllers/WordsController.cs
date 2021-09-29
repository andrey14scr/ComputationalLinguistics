using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;
using ComputationalLinguistics.Tools;

namespace ComputationalLinguistics.Controllers
{
    public class WordsController : Controller
    {
        private readonly IWordService _wordService;
        private readonly IMapper _mapper;

        public WordsController(IWordService wordService, IMapper mapper)
        {
            _wordService = wordService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(string sortBy, string pattern, int wordsBlockSize)
        {
            if (wordsBlockSize <= 0)
            {
                wordsBlockSize = Variables.WordsBlockSize;
            }

            IEnumerable<WordDto> words = new List<WordDto>();

            switch (sortBy)
            {
                case Variables.OnFrequencyPattern:
                    words = await _wordService.GetSortedBy(w => w.Frequency, 0, wordsBlockSize);
                    break;
                case Variables.OnFrequencyBackPattern:
                    words = await _wordService.GetSortedBy(w => w.Frequency, 0, wordsBlockSize, false);
                    break;
                case Variables.OnPatternPattern:
                    if(!string.IsNullOrWhiteSpace(pattern))
                        words = await _wordService.SortBy(w => w.Content.Substring(0, pattern.Length) == pattern, w => w.Content, 0, wordsBlockSize);
                    break;
                case Variables.OnAlphabetBackPattern:
                    words = await _wordService.GetSortedBy(w => w.Content, 0, wordsBlockSize);
                    break;
                default:
                    words = await _wordService.GetSortedBy(w => w.Content, 0, wordsBlockSize, false);
                    pattern = string.Empty;
                    break;
            }

            var model = _mapper.Map<List<WordViewModel>>(words);

            return View(new WordsListViewModel
            {
                Words = model, 
                SortBy = sortBy, 
                Pattern = pattern, 
                WordsBlockSize = wordsBlockSize,
            });
        }

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var word = await _wordService.GetById(id.Value);
            var contextFiles = await _wordService.GetContextFiles(id.Value);

            if (word == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<WordViewModel>(word);
            model.WordContextFiles = contextFiles;

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Content")] CreateWordModel createWordModel)
        {
            WordDto wordDto = null;
            if (ModelState.IsValid)
            {
                try
                {
                    wordDto = new WordDto
                    {
                        Id = Guid.NewGuid(),
                        Content = createWordModel.Content,
                        Frequency = 0,
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

            var word = await _wordService.GetById(id.Value);
            var model = _mapper.Map<WordViewModel>(word);

            return View(new WordViewModelFrom { PreviousPage = previousPage, WordViewModel = model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WordViewModelFrom wvm)
        {
            if (ModelState.IsValid)
            {
                await _wordService.Update(new WordDto
                {
                    Id = wvm.WordViewModel.Id, Content = wvm.WordViewModel.Content, Frequency = wvm.WordViewModel.Frequency
                });

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(wvm.PreviousPage);
        }

        public async Task<ActionResult> Delete(Guid id, string previousPage)
        {
            var word = await _wordService.GetById(id);
            var model = _mapper.Map<WordViewModel>(word);

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
            IEnumerable<WordDto> words = new List<WordDto>();

            switch (sortBy)
            {
                case Variables.OnFrequencyPattern:
                    words = await _wordService.GetSortedBy(w => w.Frequency, skip, next);
                    break;
                case Variables.OnFrequencyBackPattern:
                    words = await _wordService.GetSortedBy(w => w.Frequency, skip, next, false);
                    break;
                case Variables.OnPatternPattern:
                    if(!string.IsNullOrWhiteSpace(pattern))
                        words = await _wordService.SortBy(w => w.Content.Substring(0, pattern.Length) == pattern, w => w.Content, skip, next);
                    break;
                case Variables.OnAlphabetBackPattern:
                    words = await _wordService.GetSortedBy(w => w.Content, skip, next);
                    break;
                default:
                    words = await _wordService.GetSortedBy(w => w.Content, skip, next, false);
                    break;
            }
            
            return View(words);
        }
    }
}
