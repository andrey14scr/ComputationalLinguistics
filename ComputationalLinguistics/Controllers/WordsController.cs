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

        public async Task<ActionResult> Index(string sortBy, string pattern)
        {
            var r = await _wordService.GetContextFiles(new Guid("d7b7283a-e1f7-46c0-82c4-222d80f84530"));
            IEnumerable<WordDto> words = new List<WordDto>();

            switch (sortBy)
            {
                case WordsList.OnFrequency:
                    words = await _wordService.GetSortedBy(w => w.Frequency);
                    break;
                case WordsList.OnPattern:
                    if(!string.IsNullOrWhiteSpace(pattern))
                        words = await _wordService.SortBy(w => w.Content.Substring(0, pattern.Length) == pattern);
                    break;
                default:
                    words = await _wordService.GetSortedBy(w => w.Content, false);
                    pattern = string.Empty;
                    break;
            }

            var model = _mapper.Map<List<WordViewModel>>(words);

            return View(new WordsList{Words = model, SortBy = sortBy, Pattern = pattern});
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

        private async Task<bool> PersonExists(Guid id)
        {
            return (await _wordService.GetById(id)) != null;
        }
    }
}
