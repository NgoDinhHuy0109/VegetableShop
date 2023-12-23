using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Command;
using Shop.Models;

namespace Shop.Controllers.admin
{
    public class BillsAdminController : Controller
    {
        private readonly emmahopContext _context;
        CheckPermission check = new CheckPermission();
        public BillsAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: BillsAdmin
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "VIEW_ORDERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status >= 2);
                return View(await emmahopContext.ToListAsync());
            }

        }
        public  ActionResult IndexReport(string? month)
        {
            var items = new List<string> { "1", "2", "3","4","5","6","7","8","9","10","11","12"};
            if (String.IsNullOrEmpty(month))
            {
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status == 2 || x.Status == 3);
                ViewData["month"] = items;
                ViewData["monthselect"] = month;
                return View(emmahopContext.ToList());
            }
            else if (!String.IsNullOrEmpty(month))
            {
                var dateTime = DateTime.Now;
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status == 2 || x.Status == 3).Where(y => y.CreateAt.Month.ToString().Equals(month));
                ViewData["month"] = items;
                ViewData["monthselect"] = month;
                return View( emmahopContext.ToList());
            }
            else
            {
                var dateTime = DateTime.Now;
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status == 2 || x.Status == 3 && x.CreateAt.Month.ToString().Equals(dateTime.Month.ToString()));
                ViewData["month"] = items;
                ViewData["monthselect"] = month;
                return View(emmahopContext.ToList());
            }
            

        }
        public IActionResult ExprotToCSV()
        {  
           
                var builder = new StringBuilder();
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status == 2 || x.Status == 3 );
                builder.AppendLine("BillID,Total,Date");
                foreach (var item in emmahopContext)
                {
                    builder.AppendLine($"{item.OrderId},{item.TotalPrice},{item.CreateAt.ToString()}");
                }
                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "report.csv");
            
           

        }
        public IActionResult ExprotToExcel(string? month)
        {
            if (String.IsNullOrEmpty(month))
            {
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status == 2 || x.Status == 3);
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Report");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Bill ID";
                    worksheet.Cell(currentRow, 2).Value = "Total Price";
                    worksheet.Cell(currentRow, 3).Value = "Date";
                    worksheet.Cell(currentRow, 4).Value = "Customer";
                    foreach (var item in emmahopContext)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.OrderId;
                        worksheet.Cell(currentRow, 2).Value = item.TotalPrice;
                        worksheet.Cell(currentRow, 3).Value = item.CreateAt.ToString();
                        worksheet.Cell(currentRow, 4).Value = item.User.Fullname;
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
                    }
                }
            }
            else
            {
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher).Where(x => x.Status == 2 || x.Status == 3 && x.CreateAt.Month.ToString().Equals(month));
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Report");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Bill ID";
                    worksheet.Cell(currentRow, 2).Value = "Total Price";
                    worksheet.Cell(currentRow, 3).Value = "Date";
                    worksheet.Cell(currentRow, 4).Value = "Customer";
                    foreach (var item in emmahopContext)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.OrderId;
                        worksheet.Cell(currentRow, 2).Value = item.TotalPrice;
                        worksheet.Cell(currentRow, 3).Value = item.CreateAt.ToString();
                        worksheet.Cell(currentRow, 4).Value = item.User.Fullname;
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
                    }
                }
            }
               

        }
      
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DETAILS_ORDERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var emmahopContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == id);
            return View(await emmahopContext.ToListAsync());

        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "CREATE_ORDERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
                ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id");
                return View();
            }

        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,UserId,TotalPrice,CreateAt,VoucherId,Status")] Order order)
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
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "EDIT_ORDERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if(order.Status == 2)
            {
                order.Status = 3;
            }
            else if(order.Status == 3)
            {
                order.Status = 4;
            }
            else if(order.Status == 4)
            {
                order.Status = 5;
            }
          
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,UserId,TotalPrice,CreateAt,VoucherId,Status")] Order order)
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
            var role = HttpContext.Session.GetString("role");
            if (check.HasCredential(role, "DELETE_ORDERS", _context) == false)
            {
                return Redirect("/Home/Error");
            }
            else
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Status = 6;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
