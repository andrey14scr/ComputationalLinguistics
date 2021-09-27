using System.Diagnostics;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComputationalLinguistics.Models;
using ComputationalLinguistics.Core.Services.Interfaces;

namespace ComputationalLinguistics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWordService _wordService;
        private readonly ITextService _textService;

        public HomeController(ILogger<HomeController> logger, IWordService wordService, ITextService textService)
        {
            _logger = logger;
            _wordService = wordService;
            _textService = textService;
        }

        public async Task<IActionResult> Index()
        {
            await _textService.ParseText(@"D:\fff2.txt");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}