using AutoMapper;
using ComputationalLinguistics.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.DAL;
using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.Controllers
{
    public class TagsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ComputationalLinguisticsContext _context;

        public TagsController(IMapper mapper, ComputationalLinguisticsContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: TagsController
        public ActionResult Index()
        {
            var model = _mapper.Map<List<TagInfoDto>>(_context.TagsInfo.ToList());

            return View(model);
        }

        // GET: TagsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TagsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TagInfoDto tagInfoDto)
        {
            try
            {
                var model = new TagInfo()
                {
                    Id = Guid.NewGuid(),
                    TagName = tagInfoDto.TagName,
                    Info = tagInfoDto.Info,
                };

                _context.TagsInfo.Add(model);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TagsController/Edit/5
        public ActionResult Edit(Guid id)
        {
            var model = _mapper.Map<TagInfoDto>(_context.TagsInfo.FirstOrDefault(t => t.Id == id));
            return View(model);
        }

        // POST: TagsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, TagInfoDto tagInfoDto)
        {
            try
            {
                var model = new TagInfo()
                {
                    Id = id,
                    TagName = tagInfoDto.TagName,
                    Info = tagInfoDto.Info,
                };

                _context.TagsInfo.Update(model);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TagsController/Delete/5
        public ActionResult Delete(Guid id)
        {
            var model = _mapper.Map<TagInfoDto>(_context.TagsInfo.FirstOrDefault(t => t.Id == id));
            return View(model);
        }

        // POST: TagsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var model = _context.TagsInfo.AsNoTracking().FirstOrDefault(t => t.Id == id);

                _context.TagsInfo.Remove(model);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
