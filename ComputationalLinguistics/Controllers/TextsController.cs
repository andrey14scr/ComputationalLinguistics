using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputationalLinguistics.Controllers
{
    public class TextsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
