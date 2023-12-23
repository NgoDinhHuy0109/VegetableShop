using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class AddOrderController : Controller
    {
        private readonly emmahopContext _context;
        public AddOrderController(emmahopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddtoCart(int? id)
        {
            var user = HttpContext.Session.GetString("user");
            var product = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(id));
           
            if (user == null)
            {
                return Redirect("/Login");
            }
            else
            {
                if (product.FirstOrDefault().ProductQuantity <= 0)
                {
                    TempData["AlertType"] = "alert-success";
                    TempData["AlertMessage"] = "Product is sold out";
                    return Redirect("/Products");
                }
                else
                {
                    var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                    var checkOrderExist = emmahopContext.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1));
                    if (checkOrderExist.Count() > 0)
                    {
                        var data = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                        var checkProductExist = data.Where(s => s.ProductId.Equals(id) && s.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
                        var orderder = data.Where(x => x.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
                        if (checkProductExist.Count() > 0)
                        {  if(product.FirstOrDefault().ProductQuantity - (checkProductExist.FirstOrDefault().Quantity + 1) < 0)
                            {
                                TempData["AlertType"] = "alert-success";
                                TempData["AlertMessage"] = "Product is sold out";
                                return Redirect("/Products");
                            }
                            var orderDetalis = checkProductExist.FirstOrDefault();
                            orderDetalis.Quantity = orderDetalis.Quantity + 1;
                            _context.OrderDetails.Update(orderDetalis);
                            _context.SaveChanges();
                            double totlaprice = 0;
                            foreach (OrderDetail ordetail in orderder)
                            {
                                totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                                Debug.WriteLine(totlaprice + "gia");
                            }

                            checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
                            _context.Orders.Update(checkOrderExist.FirstOrDefault());
                            _context.SaveChanges();

                        }
                        else
                        {
                            double price = _context.Products.Where(x => x.Id.Equals(id)).FirstOrDefault().OutPrice;
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderId = checkOrderExist.FirstOrDefault().Id;
                            orderDetail.ProductId = id;
                            orderDetail.Price = price;
                            orderDetail.Quantity = 1;
                            _context.OrderDetails.Add(orderDetail);
                            _context.SaveChanges();
                            double totlaprice = 0;
                            foreach (OrderDetail ordetail in orderder)
                            {
                                totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                                Debug.WriteLine(totlaprice);
                            }
                            checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
                            _context.Orders.Update(checkOrderExist.FirstOrDefault());
                            _context.SaveChanges();


                        }
                        TempData["AlertType"] = "alert-success";
                        TempData["AlertMessage"] = "Add To Cart successful";
                        return Redirect("/Products");
                    }
                    else
                    {
                        double price = _context.Products.Where(x => x.Id.Equals(id)).FirstOrDefault().OutPrice;
                        Order order = new Order();
                        order.CreateAt = DateTime.Now;
                        order.OrderId = GenerateOrderID(5);
                        order.TotalPrice = price;
                        order.Status = 1;
                        order.UserId = Int32.Parse(user);
                        _context.Orders.Add(order);
                        _context.SaveChanges();
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = order.Id;
                        orderDetail.ProductId = id;
                        orderDetail.Price = price;
                        orderDetail.Quantity = 1;
                        _context.OrderDetails.Add(orderDetail);
                        _context.SaveChanges();

                        TempData["AlertType"] = "alert-success";
                        TempData["AlertMessage"] = "Add To Cart successful";
                        return Redirect("/Products");
                    }
                }


            }
        }
        public IActionResult Indexcheckout()
        {

            var user = HttpContext.Session.GetString("user");
            int? checkOrderID = 0;
            if (user != null)
            {
                ViewData["User"] = _context.Users.Include(u => u.Role).Where(x => x.Id.Equals(Int32.Parse(user)));
                ViewData["Product"] = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation);
                ViewData["Voucher"] = _context.Vouchers;
                var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1)).FirstOrDefault()?.Id;
                if (checkOrderID == 0)
                {
                    var emmahopContext2 = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                    return View(emmahopContext2.ToList());
                }
                var emmahopContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == checkOrderID);
                return View(emmahopContext.ToList());
            }
            else
            {
                return Redirect("/Login");
            }
        }
        [HttpPost]
        public IActionResult Checkout()
        {
            String note = "";
            note =  HttpContext.Request.Form["ordernote"];
            var user = HttpContext.Session.GetString("user");
            var emmahopContext1 = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
            var checkOrderID = emmahopContext1.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1)).FirstOrDefault();
            checkOrderID.Status = 2;
            checkOrderID.Note = note;
            _context.Orders.Update(checkOrderID);
            _context.SaveChanges();
            var ordetail = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).Where(s => s.OrderId == checkOrderID.Id).ToArray();
            foreach(OrderDetail orderDetail in ordetail)
            {
                var product = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(orderDetail.ProductId));
                product.FirstOrDefault().ProductQuantity = Convert.ToInt32(product.FirstOrDefault().ProductQuantity - orderDetail.Quantity);
                _context.Products.Update(product.FirstOrDefault());
                _context.SaveChanges();
            }
            try
            {
                var quanlity_voucher = _context.Vouchers.Where(x => x.Id.Equals(checkOrderID.VoucherId)).FirstOrDefault();
                if(quanlity_voucher != null)
                {
                    quanlity_voucher.Quantity = quanlity_voucher.Quantity - 1;
                    _context.Vouchers.Update(quanlity_voucher);
                    _context.SaveChanges();
                }
              
            }
            catch (Exception)
            {

                throw;
            }
            
            TempData["AlertType"] = "alert-success";
            TempData["AlertMessage"] = "Checkout successful";
            return Redirect("/Home");
        }

        public ActionResult Payment()
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string serectkey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string orderInfo = "Thanh Toán Mua Hàng Emma Shop";
            string redirectUrl = "https://localhost:44398/AddOrder/Indexcheckout";
            string ipnUrl = "https://localhost:44398/AddOrder/Indexcheckout";
            string requestType = "captureWallet";
            string amount = "1000";
            string orderId = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            string rawHash = "accessKey=" + accessKey +
                  "&amount=" + amount +
                  "&extraData=" + extraData +
                  "&ipnUrl=" + ipnUrl +
                  "&orderId=" + orderId +
                  "&orderInfo=" + orderInfo +
                  "&partnerCode=" + partnerCode +
                  "&redirectUrl=" + redirectUrl +
                  "&requestId=" + requestId +
                  "&requestType=" + requestType
                  ;
            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient()
        {
            //hiển thị thông báo cho người dùng
            return View();
        }

        public ActionResult SavePayment(float total)
        {
            //string url = Request.Url.PathAndQuery;
            string url = HttpContext.Request.Host.ToString();
            if (!url.Contains("message=Success"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                Checkout();
            }
            return RedirectToAction("Index");

        }

        public IActionResult Delete(int? id)
        {
            var orderDetail = _context.OrderDetails.Where(x => x.Id == id).FirstOrDefault();
            var ProductID = orderDetail.ProductId;
            _context.OrderDetails.Remove(orderDetail);
            _context.SaveChanges();
            var product = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(ProductID));
            var user = HttpContext.Session.GetString("user");
            var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
            var checkOrderExist = emmahopContext.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1));
            var data = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            var orderder = data.Where(x => x.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
            double totlaprice = 0;
            foreach (OrderDetail ordetail in orderder)
            {
                totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                Debug.WriteLine(totlaprice + "gia");
            }

            checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
            _context.Orders.Update(checkOrderExist.FirstOrDefault());
            _context.SaveChanges();

            return Redirect("/OrderDetails");
        }
        public IActionResult AddQuantity(int? id)
        {    
            
            var orderDetail = _context.OrderDetails.Where(x => x.Id == id).FirstOrDefault();
            orderDetail.Quantity = orderDetail.Quantity + 1;
            var ProductID = orderDetail.ProductId;
            var product = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(ProductID)).FirstOrDefault();
            if(product.ProductQuantity - orderDetail.Quantity  > 0)
            {
                _context.OrderDetails.Update(orderDetail);
                _context.SaveChanges();
                var user = HttpContext.Session.GetString("user");
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                var checkOrderExist = emmahopContext.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1));
                var data = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                var orderder = data.Where(x => x.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
                double totlaprice = 0;
                foreach (OrderDetail ordetail in orderder)
                {
                    totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                    Debug.WriteLine(totlaprice + "gia");
                }

                checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
                _context.Orders.Update(checkOrderExist.FirstOrDefault());
                _context.SaveChanges();

                return Redirect("/OrderDetails");
            }
            else
            {
                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Product is sold out";
                return Redirect("/Products");
            }
            
           
           
        }
        public IActionResult SubQuantity(int? id)
        {
            var orderDetail = _context.OrderDetails.Where(x => x.Id == id).FirstOrDefault();
            if (orderDetail.Quantity <= 1)
            {
                return Redirect("/OrderDetails");
            }
            else
            {
                orderDetail.Quantity = orderDetail.Quantity - 1;
                var ProductID = orderDetail.ProductId;
                _context.OrderDetails.Update(orderDetail);
                _context.SaveChanges();
                var product = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(ProductID));
                var user = HttpContext.Session.GetString("user");
                var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                var checkOrderExist = emmahopContext.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1));
                var data = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                var orderder = data.Where(x => x.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
                double totlaprice = 0;
                foreach (OrderDetail ordetail in orderder)
                {
                    totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                    Debug.WriteLine(totlaprice + "gia");
                }

                checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
                _context.Orders.Update(checkOrderExist.FirstOrDefault());
                _context.SaveChanges();

                return Redirect("/OrderDetails");
            }
        }
        public IActionResult Voucher()
        {
            double? percent = 0;
         
            String voucher = HttpContext.Request.Form["vouchercode"];
            var user = HttpContext.Session.GetString("user");
            var date=  _context.Vouchers.Where(x => x.VoucherCode.Equals(voucher)).FirstOrDefault().DeleteAt;
            var quanlity = _context.Vouchers.Where(x => x.VoucherCode.Equals(voucher)).FirstOrDefault().Quantity;
            Debug.WriteLine(date);
            if(!date.ToString().Equals("") || quanlity == 0)
            {
                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Expired Voucher ";
                return Redirect("/OrderDetails");
            }
            percent = _context.Vouchers.Where(x => x.VoucherCode.Equals(voucher)).FirstOrDefault()?.VoucherDiscount;
            var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
            var checkOrderExist = emmahopContext.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(1));
            checkOrderExist.FirstOrDefault().TotalPrice = (checkOrderExist.FirstOrDefault().TotalPrice) - Convert.ToDouble(checkOrderExist.FirstOrDefault().TotalPrice * (percent / 100));
            checkOrderExist.FirstOrDefault().VoucherId = _context.Vouchers.Where(x => x.VoucherCode.EndsWith(voucher)).FirstOrDefault().Id;
            _context.Orders.Update(checkOrderExist.FirstOrDefault());
            _context.SaveChanges();
         
            return Redirect("/OrderDetails");
        }
        public IActionResult AddtoWishlist(int? id)
        {
            var user = HttpContext.Session.GetString("user");
            var product = _context.Products.Include(p => p.ProductBrandNavigation).Include(p => p.ProductSizeNavigation).Include(p => p.ProductTypeNavigation).Where(x => x.Id.Equals(id));
            if (user == null)
            {
                return Redirect("/Login");
            }
            else
            {
                if (product.FirstOrDefault().Id == 0)
                {
                    TempData["AlertType"] = "alert-success";
                    TempData["AlertMessage"] = "Product is sold out";
                    return Redirect("/Products");
                }
                else
                {
                    var emmahopContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
                    var checkOrderExist = emmahopContext.Where(s => s.UserId.Equals(Int32.Parse(user)) && s.Status.Equals(6));
                    if (checkOrderExist.Count() > 0)
                    {
                        var data = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
                        var checkProductExist = data.Where(s => s.ProductId.Equals(id) && s.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
                        var orderder = data.Where(x => x.OrderId.Equals(checkOrderExist.FirstOrDefault().Id));
                        if (checkProductExist.Count() > 0)
                        {
                            var orderDetalis = checkProductExist.FirstOrDefault();
                            orderDetalis.Quantity = orderDetalis.Quantity + 1;
                            _context.OrderDetails.Update(orderDetalis);
                            _context.SaveChanges();
                            double totlaprice = 0;
                            foreach (OrderDetail ordetail in orderder)
                            {
                                totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                                Debug.WriteLine(totlaprice + "gia");
                            }

                            checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
                            _context.Orders.Update(checkOrderExist.FirstOrDefault());
                            _context.SaveChanges();
                            product.FirstOrDefault().ProductQuantity = product.FirstOrDefault().ProductQuantity - 1;
                            _context.Products.Update(product.FirstOrDefault());
                            _context.SaveChanges();
                        }
                        else
                        {
                            double price = _context.Products.Where(x => x.Id.Equals(id)).FirstOrDefault().OutPrice;
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderId = checkOrderExist.FirstOrDefault().Id;
                            orderDetail.ProductId = id;
                            orderDetail.Price = price;
                            orderDetail.Quantity = 1;
                            _context.OrderDetails.Add(orderDetail);
                            _context.SaveChanges();
                            double totlaprice = 0;
                            foreach (OrderDetail ordetail in orderder)
                            {
                                totlaprice += Convert.ToDouble(ordetail.Quantity * ordetail.Price);
                                Debug.WriteLine(totlaprice);
                            }
                            checkOrderExist.FirstOrDefault().TotalPrice = totlaprice;
                            _context.Orders.Update(checkOrderExist.FirstOrDefault());
                            _context.SaveChanges();
                            product.FirstOrDefault().ProductQuantity = product.FirstOrDefault().ProductQuantity - 1;
                            _context.Products.Update(product.FirstOrDefault());
                            _context.SaveChanges();

                        }
                        TempData["AlertType"] = "alert-success";
                        TempData["AlertMessage"] = "Add To Wish List successful";
                        return Redirect("/Products");
                    }
                    else
                    {
                        double price = _context.Products.Where(x => x.Id.Equals(id)).FirstOrDefault().OutPrice;
                        Order order = new Order();
                        order.CreateAt = DateTime.Now;
                        order.OrderId = 1 + "";
                        order.TotalPrice = price;
                        order.Status = 6;
                        order.UserId = Int32.Parse(user);
                        _context.Orders.Add(order);
                        _context.SaveChanges();
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = order.Id;
                        orderDetail.ProductId = id;
                        orderDetail.Price = price;
                        orderDetail.Quantity = 1;
                        _context.OrderDetails.Add(orderDetail);
                        _context.SaveChanges();
                        product.FirstOrDefault().ProductQuantity = product.FirstOrDefault().ProductQuantity - 1;
                        _context.Products.Update(product.FirstOrDefault());
                        _context.SaveChanges();
                        TempData["AlertType"] = "alert-success";
                        TempData["AlertMessage"] = "Add To Wish List successful";
                        return Redirect("/Products");
                    }
                }


            }
        }
        public static string GenerateOrderID(int Length)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, Length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
