using AutoMapper;

using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.Models;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ComputationalLinguistics.Controllers
{
    public class StatsController : Controller
    {
        private readonly IWordService _wordService;
        private readonly IMapper _mapper;

        public StatsController(IWordService wordService, IMapper mapper)
        {
            _wordService = wordService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var wordsCount = await _wordService.GetWordsCount();
            var allWordsCount = await _wordService.GetWordsInTextsCount();

            return View(new StatisticsViewModel
            {
                AllWordsCount = allWordsCount,
                WordsCount = wordsCount
            });
        }
    }
}
