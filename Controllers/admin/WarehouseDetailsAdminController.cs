using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Controllers.admin
{
    public class WarehouseDetailsAdminController : Controller
    {
        private readonly emmahopContext _context;

        public WarehouseDetailsAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: WarehouseDetailsAdmin
        public async Task<IActionResult> Index()
        {
            var emmahopContext = _context.WarehouseDetails.Include(w => w.Product).Include(w => w.User);
            return View(await emmahopContext.ToListAsync());
        }

        // GET: WarehouseDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouseDetail = await _context.WarehouseDetails
                .Include(w => w.Product)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouseDetail == null)
            {
                return NotFound();
            }

            return View(warehouseDetail);
        }

        // GET: WarehouseDetails/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: WarehouseDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,InPrice,Quantity,UserId")] WarehouseDetail warehouseDetail)
        {
            if (ModelState.IsValid)
            {
                var products = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductColorNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(warehouseDetail.ProductId)).FirstOrDefault();
                DateTime dateTime = DateTime.Now;
                warehouseDetail.DateIn = dateTime;
                warehouseDetail.TotalPrice = Convert.ToDouble(warehouseDetail.InPrice * warehouseDetail.Quantity);
                _context.Add(warehouseDetail);
                await _context.SaveChangesAsync();
                products.OutPrice =Convert.ToDouble(warehouseDetail.InPrice + (warehouseDetail.InPrice * 0.3));
                products.ProductQuantity = products.ProductQuantity + warehouseDetail.Quantity;
                _context.Update(products);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode", warehouseDetail.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", warehouseDetail.UserId);
            return View(warehouseDetail);
        }

        // GET: WarehouseDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouseDetail = await _context.WarehouseDetails.FindAsync(id);
            if (warehouseDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode", warehouseDetail.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", warehouseDetail.UserId);
            return View(warehouseDetail);
        }

        // POST: WarehouseDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,InPrice,Quantity,UserId")] WarehouseDetail warehouseDetail)
        {
            if (id != warehouseDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    warehouseDetail.TotalPrice = Convert.ToDouble(warehouseDetail.InPrice * warehouseDetail.Quantity);
                    _context.Update(warehouseDetail);
                    await _context.SaveChangesAsync();
                    var products = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductColorNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(warehouseDetail.ProductId)).FirstOrDefault();
                    products.ProductQuantity = products.ProductQuantity + warehouseDetail.Quantity;
                    _context.Update(products);
                    _context.SaveChanges();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseDetailExists(warehouseDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode", warehouseDetail.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", warehouseDetail.UserId);
            return View(warehouseDetail);
        }

        // GET: WarehouseDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouseDetail = await _context.WarehouseDetails
                .Include(w => w.Product)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouseDetail == null)
            {
                return NotFound();
            }

            return View(warehouseDetail);
        }

        // POST: WarehouseDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouseDetail = await _context.WarehouseDetails.FindAsync(id);
            _context.WarehouseDetails.Remove(warehouseDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseDetailExists(int id)
        {
            return _context.WarehouseDetails.Any(e => e.Id == id);
        }
    }
}
