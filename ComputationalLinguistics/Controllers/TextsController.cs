using System;
using AutoMapper;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;

namespace ComputationalLinguistics.Controllers
{
    public class TextsController : Controller
    {
        private readonly ITextService _textService;
        private readonly IWordService _wordService;
        private readonly IMapper _mapper;

        public TextsController(ITextService textService, IMapper mapper, IWordService wordService)
        {
            _textService = textService;
            _mapper = mapper;
            _wordService = wordService;
        }

        public async Task<IActionResult> Index()
        {
            var texts = await _textService.GetAll();
            var model = _mapper.Map<List<TextFileViewModel>>(texts);
            return View(model);
        }

        public async Task<IActionResult> Add(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                if (!Directory.Exists("TextFiles"))
                {
                    Directory.CreateDirectory("TextFiles");
                }

                var path = Path.Combine("TextFiles", uploadedFile.FileName);
                if (System.IO.File.Exists(path))
                {
                    return View("UserError", new UserErrorViewModel { Message = "Such file already exists"});
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                try
                {
                    await _textService.ParseText(path);
                }
                catch (Exception ex)
                {
                    return View("UserError", new UserErrorViewModel { Message = "Error while text processing", InnerMessages = new List<string>(ex.HResult)});
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id, Guid? wordId)
        {
            var textFile = await _textService.GetById(id);
            var text = await System.IO.File.ReadAllTextAsync(textFile.FilePath);

            var model = new TextFileInfoViewModel
            {
                Text = text,
                FileName = Path.GetFileName(textFile.FilePath),
            };

            if (wordId.HasValue)
            {
                var seeks = await _wordService.GetUsages(wordId.Value, id);
                model.Seeks = seeks;
            }

            return View(model);
        }
    }
}
