using Microsoft.AspNetCore.Mvc;
using Erp.Models;
using Erp.Data;

namespace Erp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ERPDbContext _context;

        public LeaveRequestsController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: api/LeaveRequests
        [HttpGet]
        public IActionResult GetLeaveRequests()
        {
            var leaveRequests = _context.LeaveRequests.ToList();
            return Ok(leaveRequests);
        }

        // GET: api/LeaveRequests/5
        [HttpGet("{id}")]
        public IActionResult GetLeaveRequest(int id)
        {
            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest == null)
                return NotFound();

            return Ok(leaveRequest);
        }

        // POST: api/LeaveRequests
        [HttpPost]
        public IActionResult CreateLeaveRequest([FromBody] LeaveRequest leaveRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.LeaveRequests.Add(leaveRequest);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.leave_id }, leaveRequest);
        }

        // PUT: api/LeaveRequests/5
        [HttpPut("{id}")]
        public IActionResult UpdateLeaveRequest(int id, [FromBody] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.leave_id)
                return BadRequest("Leave ID mismatch");

            if (!_context.LeaveRequests.Any(lr => lr.leave_id == id))
                return NotFound();

            _context.LeaveRequests.Update(leaveRequest);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLeaveRequest(int id)
        {
            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest == null)
                return NotFound();

            _context.LeaveRequests.Remove(leaveRequest);
            _context.SaveChanges();

            return NoContent();
        }
    }
}