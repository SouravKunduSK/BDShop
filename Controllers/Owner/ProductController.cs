using BdShop.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Owner
{
    public class ProductController : Controller
    {
        // GET: Product
        BdShopEntities db = new BdShopEntities();
        // GET: Product
        public ActionResult Index()
        {
            if (Session["sellername"] != null)
            {
                var query = db.Products.ToList();
                return View(query);
            }
            
              else
            {
                return RedirectToAction("Login", "SAccount");
            }
        }


        public ActionResult Create()
        {
            if (Session["sellername"] != null)
            {
                List<Category> CategoryList = db.Categories.ToList();
                ViewBag.CategoryList = new SelectList(CategoryList, "CategoryID", "CategoryName");
                //GetViewBagData();
                return View();
            }
                
              else
            {
                return RedirectToAction("Login", "SAccount");
            }
        }

        [HttpPost]
        public ActionResult Create(Product prod, HttpPostedFileBase Image)
        {
            List<Category> CategoryList = db.Categories.ToList();
            ViewBag.CategoryList = new SelectList(CategoryList, "CategoryID", "CategoryName");
            if (!ModelState.IsValid)
            {
                prod.CreatedDate = DateTime.Now;
                prod.Picture1 = Image.FileName.ToString();

                var folder = Server.MapPath("~/Uploads/");
                Image.SaveAs(Path.Combine(folder, Image.FileName.ToString()));
                db.Products.Add(prod);
                db.SaveChanges();
                return RedirectToAction("Index", "Product");
            }
            //GetViewBagData();
            return View();
        }

        //Get Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["sellername"] != null)
            {
                var query = db.Products.Where(m => m.ProductID == id).ToList().SingleOrDefault();

                if (query == null)
                {
                    return HttpNotFound();
                }
                List<Category> CategoryList = db.Categories.ToList();
                ViewBag.CategoryList = new SelectList(CategoryList, "CategoryID", "CategoryName");
                //GetViewBagData();
                return View(query);
            }
               
              else
            {
                return RedirectToAction("Login", "SAccount");
            }

        }

        //Post Edit
        [HttpPost]
        public ActionResult Edit(Product prod, HttpPostedFileBase Image)
        {
            //GetViewBagData();
            var p = new Product();
            List<Category> CategoryList = db.Categories.ToList();
            ViewBag.CategoryList = new SelectList(CategoryList, "CategoryID", "CategoryName");
            try
            {
                
                prod.ModifiedDate = DateTime.Now;
                prod.Picture1 = Image.FileName.ToString();
                var folder = Server.MapPath("~/Uploads/");
                Image.SaveAs(Path.Combine(folder, Image.FileName.ToString()));
                db.Entry(prod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Product");
            }
            catch
            {
                TempData["msg"] = "Product isn't updated!" +
                    "You must update the product Image..";
                return RedirectToAction("Edit", "Product");
            }
        }

        public ActionResult Delete(int id)
        {
            var query = db.Products.SingleOrDefault(m => m.ProductID == id);
            db.Products.Remove(query);
            db.SaveChanges();
            return RedirectToAction("Index", "Product");
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
