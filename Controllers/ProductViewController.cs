using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("products")]
    public class ProductViewController : Controller
    {
        private readonly ERPDbContext _context;

        public ProductViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /products
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Products.OrderBy(x => x.name_of_product).ToListAsync();
            return View(items);
        }

        // GET: /products/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            return View(new Product());
        }

        // POST: /products/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.created_at = DateTime.UtcNow;
            model.updated_at = DateTime.UtcNow;
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product created.";
            return RedirectToAction("Index");
        }

        // GET: /products/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Products.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /products/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model)
        {
            if (id != model.product_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existing = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.product_id == id);
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
                if (!_context.Products.Any(x => x.product_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Product updated.";
            return RedirectToAction("Index");
        }

        // GET: /products/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Products.FirstOrDefaultAsync(x => x.product_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: /products/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Products.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /products/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Products.FindAsync(id);
            if (model != null)
            {
                _context.Products.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
