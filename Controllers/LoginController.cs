using Shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdminWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly emmahopContext _context;
        public LoginController(emmahopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("user");
            if(user != null)
            {
                return Redirect("/Home");
            }
            else
            {
                return View();
            }
           
        }
        public IActionResult IndexRes()
        {
            var user = HttpContext.Session.GetString("user");
            if (user != null)
            {
                return Redirect("/Home");
            }
            else
            {
                return View();
            }
        }
        [TempData]
        public string Message { get; set; }
        [HttpPost]
        public ActionResult LoginAdmin()
        {
            String email = HttpContext.Request.Form["Email"];
            String Pass = HttpContext.Request.Form["pass"];
            String btnLogin = HttpContext.Request.Form["login"];
            if(btnLogin != null)
            {
                var emmahopContext = _context.Users.Include(u => u.Role);
                var data = emmahopContext.Where(s => s.Email.Equals(email) && s.Password.Equals(Pass) && s.Status == 1 && s.RoleId.Equals("Customer")).ToList();
                if(data.Count > 0)
                {
                    HttpContext.Session.SetString("user", data.FirstOrDefault().Id.ToString());
                    
                    TempData["AlertType"] = "alert-success";
                    TempData["AlertMessage"] = "Login Success";
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                  
                    var dataAdmin = emmahopContext.Where(s => s.Email.Equals(email) && s.Password.Equals(Pass) && s.Status == 1 && s.RoleId != "Customer").ToList();
                    if (dataAdmin.Count > 0)
                    {
                        HttpContext.Session.SetString("user", dataAdmin.FirstOrDefault().Id.ToString());
                        HttpContext.Session.SetString("role", dataAdmin.FirstOrDefault().RoleId.ToString());
                        Message = dataAdmin.FirstOrDefault().Fullname;
                        ViewBag.Message = "Success";
                        return Redirect("/HomeAdmin");
                    }
                    TempData["AlertType"] = "alert-warning";
                    TempData["AlertMessage"] = "Wrong pass or Email";
                    return Redirect("/Login");

                }
            }
            TempData["AlertType"] = "alert-warning";
            TempData["AlertMessage"] = "Wrong pass or Email";
            return Redirect("/Login");
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            ViewBag.Message = "Success";
            return Redirect("/Login");
        }
        [HttpPost]
        public ActionResult Res()
        {
          //  Id,Email,Password,Fullname,Phone,Address,Status,RoleId
               String email = HttpContext.Request.Form["Email"];
            String Pass = HttpContext.Request.Form["Password"];
            String Fullname = HttpContext.Request.Form["Fullname"];
            String Address = HttpContext.Request.Form["Address"];
            String Phone = HttpContext.Request.Form["Phone"];
            String btnRes= HttpContext.Request.Form["res"];
            if(btnRes != null)
            {
                if (checkEmailExist(email) == false)
                {
                    TempData["AlertType"] = "alert-warning";
                    TempData["AlertMessage"] = "Email is Exist";
                    return Redirect("/Login");
                }
                else
                {
                    User user = new User();
                    user.Email = email;
                    user.Password = Pass;
                    user.Address = Address;
                    user.Phone = Phone;
                    user.Fullname = Fullname;
                    user.Status = 1;
                    user.RoleId = "Customer";
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    TempData["AlertType"] = "alert-success";
                    TempData["AlertMessage"] = "Success";
                    return Redirect("/Login");
                }
            
            }
            TempData["AlertType"] = "alert-warning";
            TempData["AlertMessage"] = "Fail";
            return Redirect("/Login");
        }
     
        public ActionResult ChangeInfo()
        {
            String email = HttpContext.Request.Form["Email"];
            String Pass = HttpContext.Request.Form["Password"];
            String Fullname = HttpContext.Request.Form["Fullname"];
            String Address = HttpContext.Request.Form["Address"];
            String Phone = HttpContext.Request.Form["Phone"];
            String btnRes = HttpContext.Request.Form["res"];
           
             if (btnRes != null)
            {
                if (checkEmailExist(email) == false)
                {
                    ViewBag.Message = "Email is Exist";
                    return Redirect("/Login");
                }
                else
                {
                    User user = new User();
                    user.Email = email;
                    user.Password = Pass;
                    user.Address = Address;
                    user.Phone = Phone;
                    user.Fullname = Fullname;
                    user.Status = 1;
                    user.RoleId = "Customer";
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    ViewBag.Message = "Success";
                    return Redirect("/Login");
                }
               
            }
            return Redirect("");
        }
        public bool checkEmailExist(string Email)
        {
            var emmahopContext = _context.Users.Include(u => u.Role);
            var data = emmahopContext.Where(s => s.Email.Equals(Email)).ToList();
            if(data.Count() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
          
        }
          
    }
}
