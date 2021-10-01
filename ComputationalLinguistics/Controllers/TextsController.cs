using System;
using System.Collections.Concurrent;
using AutoMapper;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;
using Microsoft.Extensions.Configuration;

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

        public async Task<IActionResult> Add(IFormFileCollection uploadedFiles)
        {
            var errors = new ConcurrentBag<string>();
            var exceptions = new ConcurrentBag<Exception>();
            var toUpdate = new ConcurrentBag<WordDto>();
            var wordsInTexts = new ConcurrentBag<WordInTextDto>();

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            var connectionString = config.GetConnectionString("DefaultConnection");

            var sw = new Stopwatch();
            sw.Start();
            
            var tasks = uploadedFiles.Select(async uploadedFile =>
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
                        path = Path.Combine("TextFiles", $"Copy_{DateTime.Now:MM/dd/yyyy_HH/mm/ss}_",
                            uploadedFile.FileName);
                    }

                    try
                    {
                        await using (var fileStream = new FileStream(path, FileMode.CreateNew))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        await _textService.ParseText(connectionString, path, toUpdate, wordsInTexts);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
            await Task.WhenAll(tasks);
            
            sw.Stop();
            var r = sw.ElapsedMilliseconds;
            await System.IO.File.WriteAllTextAsync($"Time_{DateTime.Now:MM/dd/yyyy_HH/mm/ss}.txt", r.ToString());

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
