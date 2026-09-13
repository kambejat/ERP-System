using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("attendance")]
    public class AttendanceViewController : Controller
    {
        private readonly ERPDbContext _context;

        public AttendanceViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /attendance
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Attendances.OrderByDescending(x => x.date).ToListAsync();
            ViewBag.EmployeeNames = await _context.Employees.ToDictionaryAsync(x => x.employee_id, x => x.first_name + " " + x.last_name);
            return View(items);
        }

        // GET: /attendance/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
            return View(new Attendance());
        }

        // POST: /attendance/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
                return View(model);
            }
            model.created_at = DateTime.UtcNow;
            model.updated_at = DateTime.UtcNow;
            _context.Attendances.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Attendance record created.";
            return RedirectToAction("Index");
        }

        // GET: /attendance/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Attendances.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
            return View(model);
        }

        // POST: /attendance/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance model)
        {
            if (id != model.attendance_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
                return View(model);
            }
            var existing = await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(x => x.attendance_id == id);
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
                if (!_context.Attendances.Any(x => x.attendance_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Attendance record updated.";
            return RedirectToAction("Index");
        }

        // GET: /attendance/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Attendances.FirstOrDefaultAsync(x => x.attendance_id == id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.EmployeeNames = await _context.Employees.ToDictionaryAsync(x => x.employee_id, x => x.first_name + " " + x.last_name);
            return View(model);
        }

        // GET: /attendance/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Attendances.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /attendance/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Attendances.FindAsync(id);
            if (model != null)
            {
                _context.Attendances.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Attendance record deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
