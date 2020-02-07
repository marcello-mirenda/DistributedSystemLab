using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoutingSlipWebApp.Models;
using RoutingSlipWebApp.Services;

namespace RoutingSlipWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CommandResolver _commandResolver;

        public HomeController(ILogger<HomeController> logger, CommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string cmd = null)
        {
            await _commandResolver.Lookup(cmd).ExecuteAsync();
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
