using System;
using AutoMapper;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Implementation;

namespace ComputationalLinguistics.Controllers
{
    public class TextsController : Controller
    {
        private readonly ITextService _textService;
        private readonly IMapper _mapper;

        public TextsController(ITextService textService, IMapper mapper)
        {
            _textService = textService;
            _mapper = mapper;
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

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                var fileDto = new TextFileDto
                {
                    Id = Guid.NewGuid(), 
                    FilePath = path,
                };

                try
                {
                    await _textService.ParseText(path);
                    await _textService.Add(fileDto);
                }
                catch (Exception ex)
                {
                    return View("UserError", new UserErrorViewModel { Message = "Error while text processing", InnerMessages = new List<string>(ex.HResult)});
                }
            }

            return RedirectToAction("Index");
        }
    }
}
