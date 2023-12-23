using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Command;
using Shop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.admin
{
    public class CredentialsAdminController : Controller
    {
        private readonly emmahopContext _context;
        CheckPermission check = new CheckPermission();
        public CredentialsAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: CredentialsAdmin
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "Credentials", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                var emmahopContext = _context.Credentials.Include(c => c.Permission).Include(c => c.Role);
                return View(await emmahopContext.ToListAsync());
            }

        }

        // GET: Credentials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credentials
                .Include(c => c.Permission)
                .Include(c => c.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            return View(credential);
        }

        // GET: Credentials/Create
        public IActionResult Create()
        {
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "Id", "Id");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: Credentials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleId,PermissionId")] Credential credential)
        {
            if (ModelState.IsValid)
            {
                _context.Add(credential);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "Id", "Id", credential.PermissionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", credential.RoleId);
            return View(credential);
        }

        // GET: Credentials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credentials.FindAsync(id);
            if (credential == null)
            {
                return NotFound();
            }
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "Id", "Id", credential.PermissionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", credential.RoleId);
            return View(credential);
        }

        // POST: Credentials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleId,PermissionId")] Credential credential)
        {
            if (id != credential.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credential);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialExists(credential.Id))
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
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "Id", "Id", credential.PermissionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", credential.RoleId);
            return View(credential);
        }

        // GET: Credentials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credentials
                .Include(c => c.Permission)
                .Include(c => c.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            return View(credential);
        }

        // POST: Credentials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var credential = await _context.Credentials.FindAsync(id);
            _context.Credentials.Remove(credential);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CredentialExists(int id)
        {
            return _context.Credentials.Any(e => e.Id == id);
        }
    }
}
