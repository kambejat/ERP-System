using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("payroll")]
    public class PayrollViewController : Controller
    {
        private readonly ERPDbContext _context;

        public PayrollViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /payroll
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Payrolls.Include(x => x.Employee).OrderByDescending(x => x.pay_period_start).ToListAsync();
            return View(items);
        }

        // GET: /payroll/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
            return View(new Payroll());
        }

        // POST: /payroll/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payroll model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
                return View(model);
            }
            _context.Payrolls.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Payroll record created.";
            return RedirectToAction("Index");
        }

        // GET: /payroll/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Payrolls.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
            return View(model);
        }

        // POST: /payroll/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payroll model)
        {
            if (id != model.payroll_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
                return View(model);
            }
            _context.Entry(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Payrolls.Any(x => x.payroll_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Payroll record updated.";
            return RedirectToAction("Index");
        }

        // GET: /payroll/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Payrolls.Include(x => x.Employee).FirstOrDefaultAsync(x => x.payroll_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: /payroll/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Payrolls.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /payroll/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Payrolls.FindAsync(id);
            if (model != null)
            {
                _context.Payrolls.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Payroll record deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
