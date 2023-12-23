using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly emmahopContext _context;

        public ProductsController(emmahopContext context)
        {
            _context = context;
        }

        // GET: Products
       
        public async Task<IActionResult> Index()
        {
            var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Status == 1);
            ViewData["ProductBrand"] = _context.Brands;
            ViewData["ProductSize"] = _context.Sizes;
            ViewData["ProductType"] = _context.Types;
            ViewData["ProductColor"] = _context.Colors;
            ViewData["min"] = emmahopContext.Select(x => x.OutPrice)?.Min();
            ViewData["max"] = emmahopContext.Select(x => x.OutPrice)?.Max();
            int? checkOrderID = 0;
            var user = HttpContext.Session.GetString("user");
            if (user == null)
            {
                ViewData["orderdeatail"] = null;
             
            }
            else
            {
              
                try
                {
                    var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                     checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1)).FirstOrDefault()?.Id;
                    if (checkOrderID == 0)
                    {
                        ViewData["orderdeatail"] = null;
                        
                    }
                    else
                    {
                        ViewData["orderdeatail"] = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == checkOrderID); ;
                       
                    }

                }
                catch (Exception e)
                {
                    ViewData["orderdeatail"] = null;
                   
                }

            }
            return View(await emmahopContext.ToListAsync());
        }

        public async Task<IActionResult> PriceTable()
        {
            var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Status == 1);
            return View("~/Views/Products/PriceTable.cshtml", emmahopContext);

        }

        // GET: Products/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =  _context.Products
                .Include(p => p.ProductBrandNavigation)
                .Include(p => p.ProductSizeNavigation)
                .Include(p => p.ProductTypeNavigation)
                .FirstOrDefault(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var user = HttpContext.Session.GetString("user");
            if (user == null)
            {
                ViewData["orderdeatail"] = null;

            }
            else
            {

                try
                {
                     int? checkOrderID = 0;
                    var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                    checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1)).FirstOrDefault()?.Id;
                    if (checkOrderID == 0)
                    {
                        ViewData["orderdeatail"] = null;

                    }
                    else
                    {
                        ViewData["orderdeatail"] = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == checkOrderID); ;

                    }

                }
                catch (Exception e)
                {
                    ViewData["orderdeatail"] = null;

                }

            }
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Tilte");
            ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Title");
            ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Tilte");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductCode,ProductName,ProductType,ProductColor,ProductQuantity,ProductBrand,ProductImage,ProductSize,ProductDescription,OutPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Id", product.ProductBrand);
            ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Id", product.ProductSize);
            ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Id", product.ProductType);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Id", product.ProductBrand);
            ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Id", product.ProductSize);
            ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Id", product.ProductType);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.ProductBrandNavigation)
                .Include(p => p.ProductSizeNavigation)
                .Include(p => p.ProductTypeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
       
    } 
}
