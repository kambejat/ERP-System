using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("logs")]
    public class LogViewController : Controller
    {
        private readonly ERPDbContext _context;

        public LogViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /logs
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Logs.Include(x => x.User).OrderByDescending(x => x.timestamp).ToListAsync();
            return View(items);
        }

        // GET: /logs/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Logs.Include(x => x.User).FirstOrDefaultAsync(x => x.log_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: /logs/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Logs.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /logs/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Logs.FindAsync(id);
            if (model != null)
            {
                _context.Logs.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Log entry deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
