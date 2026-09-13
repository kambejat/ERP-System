using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers

{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ERPDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ERPDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            ViewBag.EmployeeCount = await _context.Employees.CountAsync();
            ViewBag.PendingLeaveCount = await _context.LeaveRequests.CountAsync(l => l.status == "Pending");
            ViewBag.OpenOrdersCount = await _context.Orders.CountAsync(o => o.status == OrderStatus.pending);
            ViewBag.LowStockCount = await _context.Products.CountAsync(p => p.quantity_in_stock <= p.reorder_level);
            ViewBag.SupplierCount = await _context.Suppliers.CountAsync();

            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            ViewBag.MonthRevenue = await _context.Transactions
                .Where(t => t.status == TransactionStatus.completed && t.transaction_date >= monthStart)
                .SumAsync(t => (decimal?)t.amount) ?? 0m;

            ViewBag.RecentLogs = await _context.Logs
                .Include(l => l.User)
                .OrderByDescending(l => l.timestamp)
                .Take(6)
                .ToListAsync();

            return View();
        }

        // You can add more actions as needed, like widgets, analytics, etc.
    }
}
