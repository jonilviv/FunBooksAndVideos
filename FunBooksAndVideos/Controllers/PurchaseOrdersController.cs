using FunBooksAndVideos.Controllers.Dto;
using FunBooksAndVideos.Data;
using FunBooksAndVideos.Entities;
using FunBooksAndVideos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunBooksAndVideos.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    public sealed class PurchaseOrdersController : ControllerBase
    {
        private readonly IFunDbContext _context;
        private readonly IPurchaseOrderProcessor _processor;

        public PurchaseOrdersController(IFunDbContext context, IPurchaseOrderProcessor processor)
        {
            _context = context;
            _processor = processor;
        }

        // GET: api/purchase-orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders()
        {
            PurchaseOrder[] purchaseOrders = await _context.PurchaseOrders
                .Include(p => p.Customer)
                .Include(p => p.Products)
                .Include(p => p.ShippingSlip)
                .ToArrayAsync();

            return purchaseOrders;
        }

        // GET: api/purchase-orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
        {
            PurchaseOrder purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Customer)
                .Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return purchaseOrder;
        }

        // POST: api/purchase-orders
        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> PostPurchaseOrder(PurchaseOrderDto purchaseOrderDto)
        {
            Customer customer = await _context.Customers.FindAsync(purchaseOrderDto.CustomerId);

            if (customer == null)
            {
                return BadRequest($"Customer Id is wrong. Customer with id={purchaseOrderDto.CustomerId} does not exists.");
            }

            var products = new List<Product>();

            foreach (int productId in purchaseOrderDto.ProductIds)
            {
                Product product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    return BadRequest($"Product Id is wrong. Product with id={productId} does not exists.");
                }

                products.Add(product);
            }

            var purchaseOrder = new PurchaseOrder
            {
                CreatedAt = DateTime.UtcNow,
                TotalPrice = products.Sum(p => p.Price),
                Customer = customer,
                Products = products
            };

            _context.PurchaseOrders.Add(purchaseOrder);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = purchaseOrder.Id }, purchaseOrder);
        }

        // PUT: api/purchase-orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrder(int id, PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.PurchaseOrders.Any(p => p.Id == id))
                {
                    throw;
                }

                return NotFound();

            }

            return NoContent();
        }

        // DELETE: api/purchase-orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            PurchaseOrder purchaseOrder = await _context.PurchaseOrders.FindAsync(id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            _context.PurchaseOrders.Remove(purchaseOrder);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/purchase-orders/process
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPurchaseOrder(int id)
        {
            PurchaseOrder purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Customer)
                .Include(po => po.Products)
                .Include(po => po.ShippingSlip)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            ProcessResult result = _processor.Process(purchaseOrder);

            if (result.Successful)
            {
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}