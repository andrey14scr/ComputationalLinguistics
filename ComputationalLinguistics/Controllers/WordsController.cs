using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;
using Microsoft.EntityFrameworkCore;
using ComputationalLinguistics.DAL.Core.Entities;

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

        public async Task<ActionResult> Index([Bind("SortBy")] string sortBy)
        {
            IEnumerable<WordDto> words = new List<WordDto>();

            switch (sortBy)
            {
                case "abc":
                    words = await _wordService.GetSortedBy(w => w.Content, false);
                    break;
                case "freq":
                    words = await _wordService.GetSortedBy(w => w.Frequency);
                    break;
                default:
                    words = await _wordService.GetAll();
                    break;
            }

            var model = _mapper.Map<List<WordViewModel>>(words);

            return View(new WordsList{Words = model});
        }

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var word = await _wordService.GetById(id.Value);

            if (word == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<WordViewModel>(word);

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Content,Frequency")] WordDto wordDto)
        {
            if (ModelState.IsValid)
            {
                await _wordService.Add(wordDto);
                return RedirectToAction(nameof(Index));
            }

            return View(_mapper.Map<WordViewModel>(wordDto));
        }

        public async Task<ActionResult> Edit(Guid? id, string from)
        {
            if (id == null)
            {
                return NotFound();
            }

            var word = await _wordService.GetById(id.Value);
            var model = _mapper.Map<WordViewModel>(word);

            return View(new WordViewModelWithPath { PreviousPage = from, WordViewModel = model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Content,Frequency")] WordDto wordDto, string from)
        {
            if (ModelState.IsValid)
            {
                await _wordService.Update(wordDto);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(from);
        }

        public async Task<ActionResult> Delete(Guid id, string from)
        {
            var word = await _wordService.GetById(id);
            var model = _mapper.Map<WordViewModel>(word);

            return View(new WordViewModelWithPath() { PreviousPage = from, WordViewModel = model });
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
