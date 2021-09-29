using System.Diagnostics;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComputationalLinguistics.Models;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWordService _wordService;
        private readonly ITextService _textService;

        private readonly ComputationalLinguisticsContext _context;

        public HomeController(ILogger<HomeController> logger, IWordService wordService, ITextService textService, ComputationalLinguisticsContext context)
        {
            _logger = logger;
            _wordService = wordService;
            _textService = textService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lol = await _context.Words.AsNoTracking().FirstOrDefaultAsync(w => w.Content == "lol");
            lol.Content = "asd";
            _context.Words.Update(lol);
            await _context.SaveChangesAsync();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}