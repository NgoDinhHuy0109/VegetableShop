using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Models;
using Shop.SendMail;

namespace Shop.Controllers
{
    public class UsersController : Controller
    {
        private readonly emmahopContext _context;

        public UsersController(emmahopContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var emmahopContext = _context.Users.Include(u => u.Role);
            return View(await emmahopContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit()
        {
           
            var User = HttpContext.Session.GetString("user");
            int? checkOrderID = 0;
            if (User == null)
            {
                ViewData["orderdeatail"] = null;

            }
            else
            {
                try
                {
                    var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                     checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(User)) && s.Status.Equals(1)).FirstOrDefault()?.Id;
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
            if (User == null) 
            {
                return Redirect("/Login");
            }
            int id = Int32.Parse(User);
            if (id == null)
            {
                return Redirect("/Login");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["ViewOrders"] = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(s => s.UserId.Equals(Int32.Parse(User)) && s.Status >= 2 );
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Fullname,Phone,Address,Status,RoleId,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { 
                    var User = HttpContext.Session.GetString("user");
                    user.Id = Int32.Parse(User);
                    user.Status = 1;
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
                return RedirectToAction(nameof(Edit));
            }
            return RedirectToAction(nameof(Edit));
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        [HttpPost]
        public IActionResult Updatepassword()
        {
            var user = HttpContext.Session.GetString("user");
            String curentpassword = HttpContext.Request.Form["current-password"];
            String newpassword = HttpContext.Request.Form["newpassword"];
            String confirmpwd = HttpContext.Request.Form["confirm-pwd"];
            var emmahopContext = _context.Users.Include(u => u.Role).Where(x => x.Id == Int32.Parse(user));
            if (confirmpwd != newpassword)
            {
                return RedirectToAction(nameof(Edit));
            }
            else if (curentpassword != emmahopContext.FirstOrDefault().Password)
            {
                return RedirectToAction(nameof(Edit));
            }
            else
            {
                emmahopContext.FirstOrDefault().Password = newpassword;
                _context.Update(emmahopContext.FirstOrDefault());
                _context.SaveChanges();
                return RedirectToAction(nameof(Edit));
            }
            
           
        }
        public IActionResult ContactUs()
        {
          
            return View();
        }
        public IActionResult IndexForgotPassWord() {
            return View();
            }
        [HttpPost]
        public IActionResult Forgotpassword()
        {
            String email = HttpContext.Request.Form["Email"];
            String btnLogin = HttpContext.Request.Form["login"];
            if(btnLogin != null)
            {
                var checkUser = _context.Users.Include(u => u.Role).Where(x => x.Email.Equals(email));
                if(checkUser.ToList().Count > 0)
                {
                    checkUser.FirstOrDefault().Password = GeneratePassword(8);
                    _context.Update(checkUser.FirstOrDefault());
                    _context.SaveChanges();
                    MailUtils.SendMailGoogleSmtp("nganb1706840@student.ctu.edu.vn", email, "Password Mới của bạn là ", "Password:: " + checkUser.FirstOrDefault().Password + "  + Vui Lòng không chia sẽ mật khẩu cho bất kì ai",
                                      "nganb1706840@student.ctu.edu.vn", "Legiabao08102008").Wait();
                
                }
            }
          
            return Redirect("/Login");
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
