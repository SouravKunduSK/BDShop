using BdShop.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Owner
{
    public class OrderController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        public ActionResult Index()
        {
            if (Session["sellername"] != null)
            {
                return View(db.Ordereds.OrderByDescending(x => x.OrderedID).ToList());
            }
               
            else
            {
                return RedirectToAction("Login", "SAccount");
            }
        }
        public ActionResult Details(int? id)
        {
            if (Session["sellername"] != null)
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
                return RedirectToAction("Login", "SAccount");
            }
        
        }


        public ActionResult Edit(int? id)
        {

            if (Session["sellername"] != null)
            {
                var query = db.Ordereds.Where(m => m.OrderedID == id).ToList().FirstOrDefault();
                return View(query);
            }

            else
            {
                return RedirectToAction("Login", "SAccount");
            }
            
           

        }

        [HttpPost]
        public ActionResult Edit(Ordered o)
        {
            try
            {
                if (o.CancelOrder == true)
                {
                    o.CancelDate = DateTime.Now;
                }
                else if (o.Deliver == true)
                {
                    o.DeliveryDate = DateTime.Now;
                }
                else if (o.Shipped == true)
                {
                    o.ShippedDate = DateTime.Now;
                }
                else if (o.CancelOrder == false && o.Deliver == false && o.Shipped == false)
                {
                    o.CancelDate = null;
                    o.ShippedDate = null;
                    o.DeliveryDate = null;
                }




                db.Entry(o).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Order");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex;
            }
            return RedirectToAction("Edit", "Order");
        }
    }
}