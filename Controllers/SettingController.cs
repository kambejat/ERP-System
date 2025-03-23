using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ERPDbContext _context;

        public SettingsController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: api/Settings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Setting>>> GetSettings()
        {
            return await _context.Settings.ToListAsync();
        }

        // GET: api/Settings/key
        [HttpGet("{key}")]
        public async Task<ActionResult<Setting>> GetSetting(string key)
        {
            var setting = await _context.Settings.FindAsync(key);

            if (setting == null)
            {
                return NotFound();
            }

            return setting;
        }

        // POST: api/Settings
        [HttpPost]
        public async Task<ActionResult<Setting>> PostSetting(Setting setting)
        {
            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSetting), new { key = setting.setting_key }, setting);
        }

        // PUT: api/Settings/key
        [HttpPut("{key}")]
        public async Task<IActionResult> PutSetting(string key, Setting setting)
        {
            if (key != setting.setting_key)
            {
                return BadRequest("Setting key mismatch");
            }

            _context.Entry(setting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Settings/key
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteSetting(string key)
        {
            var setting = await _context.Settings.FindAsync(key);
            if (setting == null)
            {
                return NotFound();
            }

            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SettingExists(string key)
        {
            return _context.Settings.Any(e => e.setting_key == key);
        }
    }
}