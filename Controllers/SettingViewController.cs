using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("settings")]
    public class SettingViewController : Controller
    {
        private readonly ERPDbContext _context;

        public SettingViewController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: /settings
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Settings.OrderBy(x => x.setting_key).ToListAsync();
            return View(items);
        }

        // GET: /settings/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new Setting());
        }

        // POST: /settings/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Setting model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var exists = await _context.Settings.FindAsync(model.setting_key);
            if (exists != null)
            {
                ModelState.AddModelError("setting_key", "A setting with this key already exists.");
                return View(model);
            }
            _context.Settings.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Setting created.";
            return RedirectToAction("Index");
        }

        // GET: /settings/edit/{key}
        [HttpGet("edit/{key}")]
        public async Task<IActionResult> Edit(string key)
        {
            var model = await _context.Settings.FindAsync(key);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /settings/edit/{key}
        [HttpPost("edit/{key}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string key, Setting model)
        {
            if (key != model.setting_key)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _context.Entry(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Settings.Any(x => x.setting_key == key))
                {
                    return NotFound();
                }
                throw;
            }
            TempData["SuccessMessage"] = "Setting updated.";
            return RedirectToAction("Index");
        }

        // GET: /settings/delete/{key}
        [HttpGet("delete/{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var model = await _context.Settings.FindAsync(key);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /settings/delete/{key}
        [HttpPost("delete/{key}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string key)
        {
            var model = await _context.Settings.FindAsync(key);
            if (model != null)
            {
                _context.Settings.Remove(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Setting deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}
