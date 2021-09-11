using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public class ThankYouController : Controller
    {
        // GET: ThankYou
        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                ViewBag.cartBox = null;
                ViewBag.Total = null;
                ViewBag.NoOfItem = null;
                TempShpData.items = null;
                return View("Index");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
    }
}