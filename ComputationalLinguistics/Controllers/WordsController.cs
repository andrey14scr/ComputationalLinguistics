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


        // GET: WordsController
        public async Task<ActionResult> Index()
        {
            var words = await _wordService.GetAll();
            var model = _mapper.Map<IEnumerable<WordViewModel>>(words);

            return View(model);
        }

        // GET: WordsController/Details/5
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

        // GET: WordsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WordsController/Create
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

        // GET: WordsController/Edit/5
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

        // POST: WordsController/Edit/5
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

        // GET: WordsController/Delete/5
        public async Task<ActionResult> Delete(Guid id, string from)
        {
            var word = await _wordService.GetById(id);
            var model = _mapper.Map<WordViewModel>(word);

            return View(new WordViewModelWithPath(){PreviousPage = from, WordViewModel = model});
        }

        // POST: WordsController/Delete/5
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
