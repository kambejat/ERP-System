using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("transactions")]
    public class TransactionViewController : Controller
    {
        private readonly ERPDbContext _context;

        public TransactionViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /transactions
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Transactions.Include(x => x.Order).OrderByDescending(x => x.transaction_date).ToListAsync();
            return View(items);
        }

        // GET: /transactions/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Orders = await _context.Orders.OrderByDescending(o => o.order_id).ToListAsync();
            return View(new Transaction());
        }

        // POST: /transactions/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Orders = await _context.Orders.OrderByDescending(o => o.order_id).ToListAsync();
                return View(model);
            }
            _context.Transactions.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Transaction created.";
            return RedirectToAction("Index");
        }

        // GET: /transactions/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Transactions.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Orders = await _context.Orders.OrderByDescending(o => o.order_id).ToListAsync();
            return View(model);
        }

        // POST: /transactions/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transaction model)
        {
            if (id != model.transaction_id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Orders = await _context.Orders.OrderByDescending(o => o.order_id).ToListAsync();
                return View(model);
            }
            _context.Entry(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(x => x.transaction_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Transaction updated.";
            return RedirectToAction("Index");
        }

        // GET: /transactions/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Transactions.Include(x => x.Order).FirstOrDefaultAsync(x => x.transaction_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: /transactions/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Transactions.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /transactions/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Transactions.FindAsync(id);
            if (model != null)
            {
                _context.Transactions.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction deleted.";
            }
            return RedirectToAction("Index");
        }

    }
}
