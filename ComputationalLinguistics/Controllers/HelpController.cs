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
        private readonly ITagHelpService _tagHelpService;
        private readonly IMapper _mapper;

        public HelpController(ITagHelpService tagHelpService, IMapper mapper)
        {
            _tagHelpService = tagHelpService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TagHelp()
        {
            var allTags = await _tagHelpService.GetAll();
            var model = _mapper.Map<IEnumerable<TagInfoModel>>(allTags);
            return View(model.OrderBy(m => m.TagName));
        }

        public async Task<IActionResult> GetTagByName(string name)
        {
            var tagDto = await _tagHelpService.GetByName(name);

            if (tagDto is null)
            {
                return NotFound();
            }

            return Ok(tagDto.Info);
        }
    }
}
