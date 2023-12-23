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
    public class OrdersAdminController : Controller
    {
        private readonly emmahopContext _context;

        public OrdersAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.GetString("user");
            var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(s => s.UserId.Equals(Int32.Parse(user)));

            return View(await emmahopContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = HttpContext.Session.GetString("user");
            if (id == null)
            {
                return NotFound();
            }
            int? checkOrderID = 0;
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

            var emmahopContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == id);
            return View(await emmahopContext.ToListAsync());
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,UserId,TotalPrice,CreateAt,VoucherId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id", order.VoucherId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id", order.VoucherId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,UserId,TotalPrice,CreateAt,VoucherId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id", order.VoucherId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = HttpContext.Session.GetString("user");
            var order = await _context.Orders.FindAsync(id);
            order.Status = 6;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return Redirect("Users/Edit/" + user);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = HttpContext.Session.GetString("user");
            var order = await _context.Orders.FindAsync(id); 
            order.Status = 6;
            var orderDetail =  _context.OrderDetails.Where(x => x.OrderId == id);
            foreach (var item in orderDetail)
            {
                var product = _context.Products.Where(x => x.Id.Equals(item.ProductId)).FirstOrDefault();
                product.ProductQuantity = Convert.ToInt32(product.ProductQuantity + item.Quantity);
                _context.Update(product);
            }
            _context.Update(order);
            await _context.SaveChangesAsync();
            return Redirect("Users/Edit/"+user);
        }
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
