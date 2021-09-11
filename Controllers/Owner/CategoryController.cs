using BdShop.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Owner
{
    public class CategoryController : Controller
    {
        BdShopEntities db = new BdShopEntities();

        // GET: Category
        public ActionResult Index()
        {
            if (Session["sellername"] != null)
            {
                var q = db.Categories.ToList();
                return View(q);
            }
            else
            {
                return RedirectToAction("Login", "SAccount");
            }

        }

        #region Create
        public ActionResult Create()
        {
            if (Session["sellername"] != null)
            {
                return View();
            }
                
            else
            {
                return RedirectToAction("Login", "SAccount");
            }

        }

        [HttpPost]
        public ActionResult Create(Category c)
        {

            var sd = db.Categories.Where(x => x.CategoryName == c.CategoryName).SingleOrDefault();
            if (sd != null)
            {
                TempData["msg"] = "Category Name has already been added! Try another...";
                return RedirectToAction("Create", "Category");
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    Category cat = new Category();
                    cat.CategoryName = c.CategoryName;
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Category");
                }
                else
                {
                    TempData["msg"] = "Failed Attempt!";
                    return RedirectToAction("Create", "Category");
                }

            }
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {

            if (Session["sellername"] != null)
            {
                var query = db.Categories.Where(m => m.CategoryID == id).ToList().FirstOrDefault();
                return View(query);
            }
              
            else
            {
                return RedirectToAction("Login", "SAccount");
            }

        }

        [HttpPost]
        public ActionResult Edit(Category c)
        {
            try
            {

                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Category");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex;
            }
            return RedirectToAction("Index", "Category");
        }

        public ActionResult Delete(int? id)
        {
            var query = db.Categories.SingleOrDefault(m => m.CategoryID == id);
            db.Categories.Remove(query);
            db.SaveChanges();
            return RedirectToAction("Index", "Category");
        }

        //Category Ends

        #endregion
    }
}