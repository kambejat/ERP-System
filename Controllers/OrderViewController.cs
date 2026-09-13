using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;
using System.Text.Json;

namespace Erp.Controllers
{
    [Route("orders")]
    public class OrderViewController : Controller
    {
        private readonly ERPDbContext _context;

        public OrderViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /orders
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Orders.Include(o => o.order_items).OrderByDescending(o => o.order_date).ToListAsync();
            ViewBag.UserNames = await _context.Users.ToDictionaryAsync(u => u.user_id, u => u.username);
            return View(items);
        }

        // GET: /orders/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await PopulateLookupsAsync();
            var model = new Order { order_date = DateTime.UtcNow };
            return View(model);
        }

        // POST: /orders/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order model, string? order_items_json)
        {
            var items = ParseLineItems(order_items_json);
            if (items.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one line item before saving the order.");
            }
            if (!ModelState.IsValid)
            {
                await PopulateLookupsAsync();
                return View(model);
            }

            model.order_items = items;
            model.total_amount = items.Sum(i => i.quantity * i.price);
            model.created_at = DateTime.UtcNow;
            model.updated_at = DateTime.UtcNow;

            _context.Orders.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Order created.";
            return RedirectToAction("Index");
        }

        // GET: /orders/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Orders.Include(o => o.order_items).FirstOrDefaultAsync(o => o.order_id == id);
            if (model == null)
            {
                return NotFound();
            }
            await PopulateLookupsAsync();
            return View(model);
        }

        // POST: /orders/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model, string? order_items_json)
        {
            if (id != model.order_id)
            {
                return BadRequest();
            }

            var items = ParseLineItems(order_items_json);
            if (items.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one line item before saving the order.");
            }
            if (!ModelState.IsValid)
            {
                await PopulateLookupsAsync();
                return View(model);
            }

            var existing = await _context.Orders.Include(o => o.order_items).AsNoTracking().FirstOrDefaultAsync(o => o.order_id == id);
            if (existing == null)
            {
                return NotFound();
            }

            model.created_at = existing.created_at;
            model.updated_at = DateTime.UtcNow;
            model.total_amount = items.Sum(i => i.quantity * i.price);
            _context.Entry(model).State = EntityState.Modified;

            var existingItemIds = existing.order_items.Select(i => i.order_item_id).ToHashSet();
            var incomingItemIds = items.Where(i => i.order_item_id != 0).Select(i => i.order_item_id).ToHashSet();

            // Remove line items that were dropped from the editor.
            foreach (var oldItem in existing.order_items)
            {
                if (!incomingItemIds.Contains(oldItem.order_item_id))
                {
                    _context.Entry(new OrderItem { order_item_id = oldItem.order_item_id }).State = EntityState.Deleted;
                }
            }

            // Add new rows and update rows that already existed.
            foreach (var item in items)
            {
                item.order_id = id;
                if (item.order_item_id == 0 || !existingItemIds.Contains(item.order_item_id))
                {
                    item.order_item_id = 0;
                    _context.Entry(item).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(item).State = EntityState.Modified;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(x => x.order_id == id))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Order updated.";
            return RedirectToAction("Index");
        }

        // GET: /orders/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Orders.Include(o => o.order_items).FirstOrDefaultAsync(o => o.order_id == id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.UserNames = await _context.Users.ToDictionaryAsync(u => u.user_id, u => u.username);
            ViewBag.ProductNames = await _context.Products.ToDictionaryAsync(p => p.product_id, p => p.name_of_product);
            return View(model);
        }

        // GET: /orders/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Orders.Include(o => o.order_items).FirstOrDefaultAsync(o => o.order_id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /orders/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Orders.Include(o => o.order_items).FirstOrDefaultAsync(o => o.order_id == id);
            if (model != null)
            {
                _context.OrderItems.RemoveRange(model.order_items);
                _context.Orders.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order deleted.";
            }
            return RedirectToAction("Index");
        }

        private async Task PopulateLookupsAsync()
        {
            ViewBag.Users = await _context.Users.OrderBy(u => u.username).ToListAsync();
            ViewBag.Products = await _context.Products.OrderBy(p => p.name_of_product).ToListAsync();
        }

        // The line-item editor posts its rows as a JSON array in a hidden field instead of
        // indexed form fields, since indexes get fragile once rows can be added/removed freely.
        private List<OrderItem> ParseLineItems(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<OrderItem>();
            }
            try
            {
                var raw = JsonSerializer.Deserialize<List<LineItemInput>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (raw == null)
                {
                    return new List<OrderItem>();
                }
                return raw
                    .Where(r => r.product_id > 0 && r.quantity > 0)
                    .Select(r => new OrderItem
                    {
                        order_item_id = r.order_item_id,
                        product_id = r.product_id,
                        quantity = r.quantity,
                        price = r.price
                    })
                    .ToList();
            }
            catch (JsonException)
            {
                return new List<OrderItem>();
            }
        }

        private class LineItemInput
        {
            public int order_item_id { get; set; }
            public int product_id { get; set; }
            public int quantity { get; set; }
            public decimal price { get; set; }
        }
    }
}
