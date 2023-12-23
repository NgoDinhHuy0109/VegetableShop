using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.admin
{
    public class HomeAdminController : Controller
    {
        private readonly emmahopContext _context;

        public HomeAdminController(emmahopContext context)
        {
            _context = context;
        }

        // GET: HomeAdminController
        public ActionResult Index()
        {
            double SumBill = 0;
            int i = 0, j = 0;
            DateTime fromdate = DateTime.Parse(DateTime.Now.Year + "-01-01");
            DateTime todate = DateTime.Parse(DateTime.Now.Year + "-12-12");
            List<double> listMN = new List<double>();
            for (int k = 0; k < 12; k++)
            {
                listMN.Add(0);
            }
            List<Order> listTP = _context.Orders.Where(x => x.CreateAt >= fromdate && x.CreateAt <= todate).OrderBy(x => x.CreateAt).ToList();
            foreach (Order itemTP in listTP)
            {
                for (int k = 1; k <= 12; k++)
                {
                    if (k + 1 == DateTime.Parse(itemTP.CreateAt.ToString()).Month)
                    {
                        listMN[k] += itemTP.TotalPrice;
                    }
                }
            }
            List<Order> listtotal = _context.Orders.ToList();
            foreach (var item in listtotal)
            {
                SumBill += item.TotalPrice;
            }
           
            string MNString = "";
            for (int k = 0; k < listMN.Count(); k++)
            {
                if (k == listMN.Count() - 1)
                {
                    MNString += listMN[k];
                }
                else
                {
                    MNString += listMN[k] + ",";
                }
            }
            TempData["MNChartArea"] = MNString;
            TempData["Bill"] = SumBill;
            TempData["totalProduct"] = _context.Products.Count();
            TempData["sellProduct"] = _context.OrderDetails.Count();
            TempData["totalbill"] = _context.Orders.Where(x => x.Status == 2).Count();
            return View();
        }

        // GET: HomeAdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeAdminController/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: HomeAdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeAdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeAdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeAdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private void SetData()
        {
          
         
        }
    }
}
