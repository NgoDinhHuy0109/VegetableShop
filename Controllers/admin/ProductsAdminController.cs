using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Command;
using Shop.Models;

namespace Shop.Controllers.admin
{
    public class ProductsAdminController : Controller
    {
        private readonly emmahopContext _context;
        CheckPermission check = new CheckPermission();
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductsAdminController(emmahopContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: ProductsAdmin
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "VIEW_PRODUCT", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductColorNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation);
                return View(await emmahopContext.ToListAsync());
            }

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DETAILS_PRODUCT", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductBrandNavigation)
                .Include(p => p.ProductColorNavigation)
                .Include(p => p.ProductSizeNavigation)
                .Include(p => p.ProductTypeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "CREATE_PRODUCT", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Tilte");
                ViewData["ProductColor"] = new SelectList(_context.Colors, "Id", "Title");
                ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Title");
                ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Tilte");
                return View();
            }

        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductCode,ProductName,ProductType,ProductColor,ProductQuantity,ProductBrand,ProductImage,ProductSize,ProductDescription,OutPrice,ProfileImage")] Product product)
        {
           
                string uniqueFileName = UploadedFile(product);
                product.ProductImage = uniqueFileName;
                product.Status = 1;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Tilte", product.ProductBrand);
            ViewData["ProductColor"] = new SelectList(_context.Colors, "Id", "Title", product.ProductColor);
            ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Title", product.ProductSize);
            ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Tilte", product.ProductType);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "EDIT_PRODUCT", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Tilte", product.ProductBrand);
            ViewData["ProductColor"] = new SelectList(_context.Colors, "Id", "Title", product.ProductColor);
            ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Title", product.ProductSize);
            ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Tilte", product.ProductType);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductCode,ProductName,ProductType,ProductColor,ProductQuantity,ProductBrand,ProductImage,ProductSize,ProductDescription,OutPrice,ProfileImage")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product1 = await _context.Products.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
                    product.Status = product1.Status;
                    if (product.ProfileImage == null)
                    {
                        product.ProductImage = product1.ProductImage;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        string uniqueFileName = UploadedFile(product);
                        product.ProductImage = uniqueFileName;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["ProductBrand"] = new SelectList(_context.Brands, "Id", "Tilte", product.ProductBrand);
            ViewData["ProductColor"] = new SelectList(_context.Colors, "Id", "Title", product.ProductColor);
            ViewData["ProductSize"] = new SelectList(_context.Sizes, "Id", "Title", product.ProductSize);
            ViewData["ProductType"] = new SelectList(_context.Types, "Id", "Tilte", product.ProductType);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DELETE_PRODUCT", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
          if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product.Status == 1)
            {
                product.Status = 0;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                product.Status = 1;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            return View();

        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        private string UploadedFile(Product model)
        {
            string uniqueFileName = "";
            var files = model.ProfileImage;
            if (files == null || files.Count == 0)
                return uniqueFileName;
            long size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    filePaths.Add(filePath);
                    String filesname = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    uniqueFileName += filesname + " ";
                    var fileNameWithPath = Path.Combine(filePath, filesname);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
                else
                {
                    Debug.WriteLine("Null");
                }
            }

            return uniqueFileName;
        }
    }
}
