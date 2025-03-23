using Microsoft.AspNetCore.Mvc;
using Erp.Models;
using System.Collections.Generic;
using System.Linq;
using Erp.Data;

namespace Erp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ERPDbContext _context;

        public SupplierController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: api/supplier
        [HttpGet]
        public ActionResult<IEnumerable<Supplier>> GetSuppliers()
        {
            return _context.Suppliers.ToList();
        }

        // GET: api/supplier/{id}
        [HttpGet("{id}")]
        public ActionResult<Supplier> GetSupplier(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return supplier;
        }

        // POST: api/supplier
        [HttpPost]
        public ActionResult<Supplier> CreateSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.supplier_id }, supplier);
        }

        // PUT: api/supplier/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateSupplier(int id, Supplier supplier)
        {
            if (id != supplier.supplier_id)
            {
                return BadRequest();
            }

            _context.Entry(supplier).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/supplier/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            return NoContent();
        }
    }
}