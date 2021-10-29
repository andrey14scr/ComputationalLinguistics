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
using System.Text;

namespace ComputationalLinguistics.Controllers
{
    public class TextsController : Controller
    {
        private readonly ITextService _textService;
        private readonly IWordService _wordService;
        private readonly IMapper _mapper;
        private const string TextFilesFolder = "TextFiles";

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

        public async Task<IActionResult> Add(IFormFileCollection uploadedFiles)
        {
            var exceptions = new List<Exception>();

            if (!Directory.Exists(TextFilesFolder))
            {
                Directory.CreateDirectory(TextFilesFolder);
            }

            foreach (var uploadedFile in uploadedFiles)
            {
                if (uploadedFile is not null)
                {
                    var path = Path.Combine(TextFilesFolder, uploadedFile.FileName);

                    if (System.IO.File.Exists(path))
                    {
                        path = Path.Combine(TextFilesFolder, $"Copy_{DateTime.Now:MM/dd/yyyy_HH/mm/ss}_", uploadedFile.FileName);
                    }

                    try
                    {
                        await using (var fileStream = new FileStream(path, FileMode.CreateNew))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        await _textService.ParseText(path);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task ReParse([FromBody]TextReparseJsonModel reparseInfo)
        {
            var textFileDto = await _textService.GetById(reparseInfo.Id);

            await using (var fileStream = new FileStream(textFileDto.FilePath, FileMode.Create))
            {
                var bytes = Encoding.UTF8.GetBytes(reparseInfo.Txt);
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
            }

            await _textService.ParseText(textFileDto.FilePath);
        }

        public async Task Parse(IFormFile uploadedFile)
        {
            var exceptions = new List<Exception>();

            if (!Directory.Exists(TextFilesFolder))
            {
                Directory.CreateDirectory(TextFilesFolder);
            }

            if (uploadedFile is not null)
            {
                var path = Path.Combine(TextFilesFolder, uploadedFile.FileName);

                if (System.IO.File.Exists(path))
                {
                    path = Path.Combine(TextFilesFolder, $"Copy_{DateTime.Now:MM/dd/yyyy_HH/mm/ss}_", uploadedFile.FileName);
                }

                try
                {
                    await using (var fileStream = new FileStream(path, FileMode.CreateNew))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    //await _textService.ParseText(path);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
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
