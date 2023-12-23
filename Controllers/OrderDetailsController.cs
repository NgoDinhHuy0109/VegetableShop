using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly emmahopContext _context;

        public OrderDetailsController(emmahopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.GetString("user");
            int? checkOrderID = 0;
            if (user != null)
            {
                ViewData["Voucher"] = _context.Vouchers;
                var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1)).FirstOrDefault()?.Id;
                if (checkOrderID == 0)
                {
                    var emmahopContext2 = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                    return View(await emmahopContext2.ToListAsync());
                }
                var emmahopContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == checkOrderID);
                return View(await emmahopContext.ToListAsync());

            }
            else
            {
                return Redirect("/Login");
            }
           
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

            }
        public async Task<IActionResult> Wishlist()
        {
            var user = HttpContext.Session.GetString("user");
            int? checkOrderID = 0;
            if (user != null)
            {

                var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(6)).FirstOrDefault()?.Id;
                if (checkOrderID == 0)
                {
                    var emmahopContext2 = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                    return View(await emmahopContext2.ToListAsync());
                }
                var emmahopContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == checkOrderID);
                return View(await emmahopContext.ToListAsync());

            }
            else
            {
                return Redirect("/Login");
            }
        }
        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderId");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,OrderId,Quantity,Price")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode", orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,OrderId,Quantity,Price")] OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductCode", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = HttpContext.Session.GetString("user");
            var order = await _context.Orders.FindAsync(id);
            order.Status = 6;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return Redirect("/Users/Edit/" + user);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}

