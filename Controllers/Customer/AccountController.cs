using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BdShop.Controllers.Customer
{
    public class AccountController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: Account


        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserViewModel user)
        {
            

            if(ModelState.IsValid)
            {
                var isExist = IsEmailExist(user.Email);
                if (!isExist)
                {
                    User u = new User();
                    u.FirstName = user.FirstName;
                    u.LastName = user.LastName;
                    u.Email = user.Email;
                    u.Password = user.Password;
                    u.PassCode = Crypto.Hash(user.Password);
                    u.RoleType = 2;
                    u.Created = DateTime.Now;

                    db.Users.Add(u);
                    db.SaveChanges();

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.Message = "Email has already been Register!!";
                }



            }
            else
            {
                ViewBag.Message = "Not Register!!";
            }
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            var v = db.Users.Where(x => x.Email == email).FirstOrDefault();
            return v != null;
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            var cust = db.Users.SingleOrDefault(x => x.Email == user.Email);
            if(cust!=null && cust.RoleType == 2)
            {
                if(string.Compare(Crypto.Hash(user.Password), cust.PassCode) == 0)
                {
                    Session["uid"] = cust.UserID;
                    Session["username"] = cust.FirstName + " " + cust.LastName;
                    TempShpData.UserID = cust.UserID;
                    //user.UserID = user.UserID;
                    cust.RoleType = 2;
                    cust.LastLogin = DateTime.Now;
                    db.Entry(cust).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
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
            Session["uid"] = null;
            Session["username"] = null;
            TempShpData.UserID = 0;
            TempShpData.items = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                this.GetDefaultData();

                var usr = db.Users.Find(Session["uid"]);
                return View(usr);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
        public ActionResult Edit(int? id)
        {
            if (Session["username"] != null)
            {
                this.GetDefaultData();

                var usr = db.Users.Find(id);
                Session["image"] = usr.Picture;
                return View(usr);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
          
        }
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase Image, User user)
        {
            try
            {
                if(Image!=null)
                {
                    user.Modified = DateTime.Now;
                    user.Picture = Image.FileName.ToString();
                    var folder = Server.MapPath("~/Uploads/");
                    Image.SaveAs(Path.Combine(folder, Image.FileName.ToString()));
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Account");
                }
                else
                { 
                    
                    user.Modified = DateTime.Now;
                    user.Picture = Session["image"].ToString();
                    //var folder = Server.MapPath("~/Uploads/");
                    //Image.SaveAs(Path.Combine(folder, Session["image"].ToString()));
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Account");
                }

                
            }
            catch
            {
                TempData["msg"] = "Product isn't updated!" +
                    "You must update the product Image..";
                return RedirectToAction("Edit", "Account");
            }


        }


    }
}