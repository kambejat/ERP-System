using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("employees")]
    public class EmployeeViewController : Controller
    {
        private readonly ERPDbContext _context;

        public EmployeeViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /employees
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Employees.OrderBy(x => x.last_name).ToListAsync();
            return View(items);
        }

        // GET: /employees/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users.OrderBy(u => u.username).ToListAsync();
            return View(new Employee());
        }

        // POST: /employees/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = await _context.Users.OrderBy(u => u.username).ToListAsync();
                return View(model);
            }
            model.created_at = DateTime.UtcNow;
            model.updated_at = DateTime.UtcNow;
            _context.Employees.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Employee created.";
            return RedirectToAction("Index");
        }

        // GET: /employees/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Employees.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Users = await _context.Users.OrderBy(u => u.username).ToListAsync();
            return View(model);
        }

        // POST: /employees/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee model)
        {
            if (id != model.employee_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Users = await _context.Users.OrderBy(u => u.username).ToListAsync();
                return View(model);
            }
            var existing = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.employee_id == id);
            if (existing == null)
            {
                return NotFound();
            }
            model.created_at = existing.created_at;
            model.updated_at = DateTime.UtcNow;
            _context.Entry(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employees.Any(x => x.employee_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Employee updated.";
            return RedirectToAction("Index");
        }

        // GET: /employees/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Employees.FirstOrDefaultAsync(x => x.employee_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: /employees/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Employees.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /employees/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Employees.FindAsync(id);
            if (model != null)
            {
                _context.Employees.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Employee deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
