using AutoMapper;
using ComputationalLinguistics.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputationalLinguistics.Models;

namespace ComputationalLinguistics.Controllers
{
    public class HelpController : Controller
    {
        private readonly ITagsInfoService _tagsInfoService;
        private readonly IMapper _mapper;

        public HelpController(ITagsInfoService tagHelpService, IMapper mapper)
        {
            _tagsInfoService = tagHelpService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TagHelp()
        {
            var allTags = await _tagsInfoService.GetAll();
            var model = _mapper.Map<IEnumerable<TagInfoModel>>(allTags);
            return View(model.OrderBy(m => m.TagName));
        }

        public async Task<IActionResult> GetTagByName(string name)
        {
            var tagDto = await _tagsInfoService.GetByName(name);

            if (tagDto is null)
            {
                return NotFound();
            }

            return Ok(tagDto.Info);
        }
    }
}
