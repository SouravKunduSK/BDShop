using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Owner
{
    public class DashboardController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            if(Session["sellername"]!=null)
            {
                ViewBag.latestOrders = db.Ordereds.OrderByDescending(x => x.OrderedID).Take(10).ToList();
                ViewBag.NewOrders = db.Ordereds.Where(a => /*a.Dispatched == false &&*/(a.Shipped == false || a.Shipped == null) && (a.Deliver == false || a.Deliver == null) && (a.CancelOrder == false || a.CancelOrder == null)).Count();
                //ViewBag.DispatchedOrders = db.Ordereds.Where(a => a.Dispatched == true && a.Shipped == false && a.Deliver == false).Count();
                ViewBag.ShippedOrders = db.Ordereds.Where(a => /*a.Dispatched == true &&*/ a.Shipped == true && (a.Deliver == false || a.Deliver == null)).Count();
                ViewBag.DeliveredOrders = db.Ordereds.Where(a => /*a.Dispatched == true &&*/ a.Shipped == true && a.Deliver == true).Count();
                ViewBag.CancleOrders = db.Ordereds.Where(a => /*a.Dispatched == true &&*/ /*a.Shipped == true &&*/ a.CancelOrder == true).Count();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "SAccount");
            }
           
        }

        public ActionResult MyProfile()
        {
            if (Session["sellername"] != null)
            {
                var usr = db.Sellers.Find(Session["sid"]);
                return View(usr);
            }
            //this.GetDefaultData();
            else
            {
                return RedirectToAction("Login", "SAccount");
            }

        }

       

    }
}