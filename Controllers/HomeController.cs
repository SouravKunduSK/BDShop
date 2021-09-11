using BdShop.Controllers.Customer;
using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers
{
    public class HomeController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        public ActionResult Index()
        {
            ViewBag.FruitsProduct = db.Products.Where(x => x.Category.CategoryName.Equals("Fruits")).ToList();
            ViewBag.VegetablesProduct = db.Products.Where(x => x.Category.CategoryName.Equals("Vegetables")).ToList();
            ViewBag.SpicesProduct = db.Products.Where(x => x.Category.CategoryName.Equals("Spices")).ToList();
            ViewBag.CropsProduct = db.Products.Where(x => x.Category.CategoryName.Equals("Crops")).ToList();
            ViewBag.Slider = db.genMainSliders.ToList();
            ViewBag.PromoRight = db.genPromoRights.ToList();
            
            ViewBag.LatestProducts = db.Products.OrderByDescending(x => x.ProductID).ToList();
            ViewBag.AllProducts = db.Products.ToList();
            ViewBag.TopRate = db.Reviews.Where(x => x.Rate >= 4).ToList();
            ViewBag.PopularProducts = TopSoldProduct();


            this.GetDefaultData();
            return View();
        }


        //TOP SOLD PRODUCTS
        public List<TopSoldProduct> TopSoldProduct()
        {
            var prodList = (from prod in db.OrderDetails
                            select new { prod.ProductID, prod.Quantity } into p
                            group p by p.ProductID into g
                            select new
                            {
                                pID = g.Key,
                                sold = g.Sum(x => x.Quantity)
                            }).OrderByDescending(y => y.sold).Take(8).ToList();



            List<TopSoldProduct> topSoldProds = new List<TopSoldProduct>();
            if (topSoldProds.Count > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    topSoldProds.Add(new TopSoldProduct()
                    {
                        product = db.Products.Find(prodList[i].pID),
                        CountSold = Convert.ToInt32(prodList[i].sold)
                    });
                }

            }

            return topSoldProds;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}