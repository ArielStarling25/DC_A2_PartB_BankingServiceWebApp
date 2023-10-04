using BankServiceBTier.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankServiceBTier.Controllers
{
    public class BusinessHomeController : Controller
    {
        private readonly ILogger<BusinessHomeController> _logger;

        public BusinessHomeController(ILogger<BusinessHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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