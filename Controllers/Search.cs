using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Controllers
{
    public class Search : Controller
    {
        private readonly emmahopContext _context;

        public Search(emmahopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SearchByName(string? id)
        {
            var user = HttpContext.Session.GetString("user");
            Double? min = 0;
            Double? max = 0;
            int? checkOrderID = 0;
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
            if (id == null)
            {
               
                String search = "";
                try
                {
                    search = HttpContext.Request.Form["searchname"];
                    if (search != null)
                    {
                        ViewData["ProductBrand"] = _context.Brands;
                        ViewData["ProductSize"] = _context.Sizes;
                        ViewData["ProductType"] = _context.Types;
                        ViewData["ProductColor"] = _context.Colors;
                        var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.ProductName.Contains(search));
                        min = emmahopContext.Select(x => x.OutPrice)?.Min();
                        max = emmahopContext.Select(x => x.OutPrice)?.Max();
                        ViewData["min"] = min;
                        ViewData["max"] = max;
                        return View(await emmahopContext.ToListAsync());
                    }
                    else
                    {
                        String price = HttpContext.Request.Form["pricesearch"];
                        Debug.WriteLine("long" + price.Trim());
                        string[] a = price.Trim().Split('$');
                        String[] b = a[1].Trim().Split('-');
                        Debug.WriteLine(b[0].Trim() + a[2]);
                        ViewData["ProductBrand"] = _context.Brands;
                        ViewData["ProductSize"] = _context.Sizes;
                        ViewData["ProductType"] = _context.Types;
                        ViewData["ProductColor"] = _context.Colors;
                        var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.OutPrice >= Convert.ToDouble(b[0].Trim()) && x.OutPrice <= Convert.ToDouble(a[2].Trim()));
                        min = emmahopContext.Select(x => x.OutPrice)?.Min();
                        max = emmahopContext.Select(x => x.OutPrice)?.Max();
                        ViewData["min"] = min;
                        ViewData["max"] = max;
                        return View(await emmahopContext.ToListAsync());
                    }
                }

                catch (Exception e)
                {

                }

                return Redirect("/Products");

            }
            else
            {
                string[] arrListStr = id.Split(' ');
                if (arrListStr[1].Equals("type"))
                {
                    ViewData["ProductBrand"] = _context.Brands;
                    ViewData["ProductSize"] = _context.Sizes;
                    ViewData["ProductType"] = _context.Types;
                    ViewData["ProductColor"] = _context.Colors;
                    var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.ProductTypeNavigation.Id.Equals(Int32.Parse(arrListStr[0])));
                    min = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Min();
                    max = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Max();
                    ViewData["min"] = min;
                    ViewData["max"] = max;
                    return View(await emmahopContext.ToListAsync());
                }
                else if (arrListStr[1].Equals("brand"))
                {
                    ViewData["ProductBrand"] = _context.Brands;
                    ViewData["ProductSize"] = _context.Sizes;
                    ViewData["ProductType"] = _context.Types;
                    ViewData["ProductColor"] = _context.Colors;
                    var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.ProductBrandNavigation.Id.Equals(Int32.Parse(arrListStr[0])));
                    min = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Min();
                    max = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Max();
                    ViewData["min"] = min;
                    ViewData["max"] = max;
                    return View(await emmahopContext.ToListAsync());
                }
                else if (arrListStr[1].Equals("size"))
                {
                    ViewData["ProductBrand"] = _context.Brands;
                    ViewData["ProductSize"] = _context.Sizes;
                    ViewData["ProductType"] = _context.Types;
                    ViewData["ProductColor"] = _context.Colors;
                    var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.ProductSizeNavigation.Id.Equals(Int32.Parse(arrListStr[0])));
                    min = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Min();
                    max = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Max();
                    ViewData["min"] = min;
                    ViewData["max"] = max;
                    return View(await emmahopContext.ToListAsync());
                }
                else
                {
                    ViewData["ProductBrand"] = _context.Brands;
                    ViewData["ProductSize"] = _context.Sizes;
                    ViewData["ProductType"] = _context.Types;
                    ViewData["ProductColor"] = _context.Colors;
                    var emmahopContext = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.ProductColorNavigation.Id.Equals(Int32.Parse(arrListStr[0])));
                    min = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Min();
                    max = emmahopContext.Select(x => x.OutPrice).DefaultIfEmpty().Max();
                    ViewData["min"] = min;
                    ViewData["max"] = max;
                    return View(await emmahopContext.ToListAsync());
                }

            }

        }
    }
}
