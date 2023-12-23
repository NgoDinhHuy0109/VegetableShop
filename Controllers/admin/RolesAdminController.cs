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
    public class RolesAdminController : Controller
    {
        private readonly emmahopContext _context;
        CheckPermission check = new CheckPermission();
        public RolesAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: RolesAdmin
        public async Task<IActionResult> Index()
        {

            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "VIEW_ROLES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                return View(await _context.Roles.ToListAsync());
            }

        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var role1 = HttpContext.Session.GetString("role");
            if (check.HasCredential(role1, "DETAILS_ROLES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {

            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "CREATE_ROLES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                return View();
            }

        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var role1 = HttpContext.Session.GetString("role");
            if (check.HasCredential(role1, "EDIT_ROLES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title")] Role role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.Id))
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
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var role1 = HttpContext.Session.GetString("role");
            if (check.HasCredential(role1, "DELETE_ROLES", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(string id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
