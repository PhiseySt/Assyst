using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;

namespace Assyst.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

      }
}
