using BdShop.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public class MyOrderController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: MyOrder
        public ActionResult Index(int? id)
        {
            if (Session["username"] != null)
            {
                var query = db.getallorderusers.Where(m => m.UserID == id).ToList();
                return View(query);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
           

        }

        public ActionResult Details(int? id)
        {
            if (Session["username"] != null)
            {
                Ordered ord = db.Ordereds.Find(id);
                var Ord_details = db.OrderDetails.Where(x => x.OrderedID == id).ToList();
                var tuple = new Tuple<Ordered, IEnumerable<OrderDetail>>(ord, Ord_details);

                double SumAmount = Convert.ToDouble(Ord_details.Sum(x => x.TotalAmount));
                ViewBag.TotalItems = Ord_details.Sum(x => x.Quantity);
                ViewBag.Discount = 0;
                ViewBag.ShippingCost = 50;
                ViewBag.TAmount = SumAmount - 0 + 50;
                ViewBag.Amount = SumAmount;
                return View(tuple);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
    }
}