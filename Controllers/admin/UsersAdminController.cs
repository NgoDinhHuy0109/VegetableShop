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
using Shop.SendMail;

namespace Shop.Controllers.admin
{
    public class UsersAdminController : Controller
    {
        private readonly emmahopContext _context;
        CheckPermission check = new CheckPermission();
        public UsersAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: UsersAdmin
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "VIEW_USERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                var emmahopContext = _context.Users.Include(u => u.Role);
                return View(await emmahopContext.ToListAsync());
            }

        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DETAILS_USERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
           if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "CREATE_USERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                ViewData["Password"] = GeneratePassword(8);
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
                return View();
            }

        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,Fullname,Phone,Address,Status,RoleId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                MailUtils.SendMailGoogleSmtp("nganb1706840@student.ctu.edu.vn", user.Email, "Password Dành Cho Nhân Viên ", "Password:: " + user.Password + " Vui Lòng không chia sẽ mật khẩu cho bất kì ai",
                                            "nganb1706840@student.ctu.edu.vn", "Legiabao08102008").Wait();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "EDIT_USERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
           if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,Fullname,Phone,Address,Status,RoleId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DELETE_USERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
           if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user.Status == 1)
            {
                user.Status = 0;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                user.Status = 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            return View();


        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public static string GeneratePassword(int passLength)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, passLength)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
