using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly emmahopContext _context;
        public HomeController(ILogger<HomeController> logger, emmahopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Status == 1);
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

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult GioiThieu()
        {
            return View("~/Views/Home/GioiThieu.cshtml");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
