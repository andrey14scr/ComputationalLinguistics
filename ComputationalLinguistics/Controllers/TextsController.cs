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

                try
                {
                    using (var fileStream = new FileStream(path, FileMode.CreateNew))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    await _textService.ParseText(path);
                }
                catch (Exception ex)
                {
                    return View("UserError", new UserErrorViewModel { Message = "Error while text processing", InnerMessages = new List<string>(){ ex.Message } });
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id, Guid? wordId)
        {
            var textFile = await _textService.GetById(id);

            if (textFile is null)
            {
                return View("UserError", new UserErrorViewModel { Message = "This text file doesn't exist"});
            }
            
            var text = await System.IO.File.ReadAllTextAsync(textFile.FilePath);
            var l = text.Length;
            var model = new TextFileInfoViewModel
            {
                Id = id,
                Text = text,
                FileName = Path.GetFileName(textFile.FilePath),
            };

            if (wordId.HasValue)
            {
                var seeks = await _wordService.GetUsages(wordId.Value, id);
                model.Seeks = seeks;
                model.Word = _wordService.GetById(wordId.Value).Result.Content;
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var textFileDto = await _textService.GetById(id.Value);
            
            if (textFileDto == null)
            {
                return NotFound();
            }

            return View(new TextFileViewModel
            {
                Id = textFileDto.Id, 
                FilePath = textFileDto.FilePath,
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(TextFileDto textFileDto)
        {
            if (textFileDto == null)
            {
                return NotFound();
            }
            
            await _textService.Remove(textFileDto);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
