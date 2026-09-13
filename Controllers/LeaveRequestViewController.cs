using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("leave-requests")]
    public class LeaveRequestViewController : Controller
    {
        private readonly ERPDbContext _context;

        public LeaveRequestViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /leave-requests
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.LeaveRequests.OrderByDescending(x => x.start_date).ToListAsync();
            ViewBag.EmployeeNames = await _context.Employees.ToDictionaryAsync(x => x.employee_id, x => x.first_name + " " + x.last_name);
            return View(items);
        }

        // GET: /leave-requests/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
            return View(new LeaveRequest());
        }

        // POST: /leave-requests/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
                return View(model);
            }
            model.created_at = DateTime.UtcNow;
            model.updated_at = DateTime.UtcNow;
            _context.LeaveRequests.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Leave request created.";
            return RedirectToAction("Index");
        }

        // GET: /leave-requests/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.LeaveRequests.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
            return View(model);
        }

        // POST: /leave-requests/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveRequest model)
        {
            if (id != model.leave_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.OrderBy(x => x.first_name).ThenBy(x => x.last_name).ToListAsync();
                return View(model);
            }
            var existing = await _context.LeaveRequests.AsNoTracking().FirstOrDefaultAsync(x => x.leave_id == id);
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
                if (!_context.LeaveRequests.Any(x => x.leave_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Leave request updated.";
            return RedirectToAction("Index");
        }

        // GET: /leave-requests/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.LeaveRequests.FirstOrDefaultAsync(x => x.leave_id == id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.EmployeeNames = await _context.Employees.ToDictionaryAsync(x => x.employee_id, x => x.first_name + " " + x.last_name);
            return View(model);
        }

        // GET: /leave-requests/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.LeaveRequests.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /leave-requests/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.LeaveRequests.FindAsync(id);
            if (model != null)
            {
                _context.LeaveRequests.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
