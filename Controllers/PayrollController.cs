using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly ERPDbContext _context;

        public PayrollController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: api/Payroll
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payroll>>> GetPayrolls()
        {
            return await _context.Payrolls.Include(p => p.Employee).ToListAsync();
        }

        // GET: api/Payroll/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payroll>> GetPayroll(int id)
        {
            var payroll = await _context.Payrolls.Include(p => p.Employee).FirstOrDefaultAsync(p => p.payroll_id == id);

            if (payroll == null)
            {
                return NotFound();
            }

            return payroll;
        }

        // POST: api/Payroll
        [HttpPost]
        public async Task<ActionResult<Payroll>> PostPayroll(Payroll payroll)
        {
            if (!_context.Employees.Any(e => e.employee_id == payroll.employee_id))
            {
                return BadRequest("Invalid Employee ID");
            }

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayroll), new { id = payroll.payroll_id }, payroll);
        }

        // PUT: api/Payroll/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayroll(int id, Payroll payroll)
        {
            if (id != payroll.payroll_id)
            {
                return BadRequest("Payroll ID mismatch");
            }

            _context.Entry(payroll).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayrollExists(id))
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

        // DELETE: api/Payroll/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayroll(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            _context.Payrolls.Remove(payroll);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(p => p.payroll_id == id);
        }
    }
}
