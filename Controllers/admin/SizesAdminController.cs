using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Command;
using Shop.Models;

namespace Shop.Controllers.admin
{
    public class SizesAdminController : Controller
    {
        private readonly emmahopContext _context;
        CheckPermission check = new CheckPermission();
        public SizesAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: SizesAdmin
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "VIEW_SIZES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                return View(await _context.Sizes.ToListAsync());
            }

        }

        // GET: Sizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DETAILS_SIZES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (size == null)
            {
                return NotFound();
            }

            return View(size);
        }

        // GET: Sizes/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "CREATE_SIZES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                return View();
            }

        }

        // POST: Sizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] Size size)
        {
            if (ModelState.IsValid)
            {
                _context.Add(size);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(size);
        }

        // GET: Sizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "EDIT_SIZES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
            {
                return NotFound();
            }
            return View(size);
        }

        // POST: Sizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] Size size)
        {
            if (id != size.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(size);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SizeExists(size.Id))
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
            return View(size);
        }

        // GET: Sizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DELETE_SIZES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (size == null)
            {
                return NotFound();
            }

            return View(size);
        }

        // POST: Sizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SizeExists(int id)
        {
            return _context.Sizes.Any(e => e.Id == id);
        }
    }
}
