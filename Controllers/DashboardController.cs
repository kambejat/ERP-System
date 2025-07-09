using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Erp.Controllers

{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        // GET: /Dashboard
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

        // You can add more actions as needed, like widgets, analytics, etc.
    }
}
