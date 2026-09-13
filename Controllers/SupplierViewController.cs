using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("suppliers")]
    public class SupplierViewController : Controller
    {
        private readonly ERPDbContext _context;

        public SupplierViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /suppliers
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Suppliers.OrderBy(x => x.name_of_supplier).ToListAsync();
            return View(items);
        }

        // GET: /suppliers/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            return View(new Supplier());
        }

        // POST: /suppliers/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.created_at = DateTime.UtcNow;
            _context.Suppliers.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Supplier created.";
            return RedirectToAction("Index");
        }

        // GET: /suppliers/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Suppliers.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /suppliers/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier model)
        {
            if (id != model.supplier_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existing = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(x => x.supplier_id == id);
            if (existing == null)
            {
                return NotFound();
            }
            model.created_at = existing.created_at;
            _context.Entry(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Suppliers.Any(x => x.supplier_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Supplier updated.";
            return RedirectToAction("Index");
        }

        // GET: /suppliers/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Suppliers.FirstOrDefaultAsync(x => x.supplier_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: /suppliers/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Suppliers.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /suppliers/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Suppliers.FindAsync(id);
            if (model != null)
            {
                _context.Suppliers.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Supplier deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
