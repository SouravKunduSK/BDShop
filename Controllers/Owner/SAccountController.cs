using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Owner
{
    public class SAccountController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: SAccount

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(SellerViewMoel user)
        {
            Seller seller = new Seller();
            var s = db.Sellers.Count();
            if (s>0)
            {


                ViewBag.Message = "Seller Register is Not Possible!!";


            }
            else
            {
                seller.FirstName = user.FirstName;
                seller.LastName = user.LastName;
                seller.Email = user.Email;
                seller.Password = user.Password;
                seller.PassCode = Crypto.Hash(user.Password);

                seller.RoleType = 1;

                db.Sellers.Add(seller);
                db.SaveChanges();

                return RedirectToAction("Login", "SAccount");
                
            }
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Seller user)
        {
            var cust = db.Sellers.SingleOrDefault(x => x.Email == user.Email);
            if (cust != null && cust.RoleType == 1)
            {
                if (string.Compare(Crypto.Hash(user.Password), cust.PassCode) == 0)
                {
                    Session["sid"] = cust.SellerID;
                    Session["sellername"] = cust.FirstName + " " + cust.LastName;
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.Message = "Email or Password Error!";
                }

            }
            else
            {
                ViewBag.Message = "Email or Password Error!";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["sellername"] = null;
            Session["sid"] = 0;
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Index()
        {

            var usr = db.Sellers.Find(Session["sid"]);
            return View(usr);
        }
    }
}