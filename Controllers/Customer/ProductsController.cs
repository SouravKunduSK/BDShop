using BdShop.Database;
using BdShop.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public class ProductsController : Controller
    {
        
        BdShopEntities db = new BdShopEntities();
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.Select(x => x.CategoryName).ToList();

            ViewBag.TopRatedProducts = TopSoldProducts();
            ViewBag.RecentViewsProducts = RecentViewProducts().Take(3);

            return View("Products");
        }

        //TOP SOLD PRODUCTS
        public List<TopSoldProduct> TopSoldProducts()
        {
            var prodList = (from prod in db.OrderDetails
                            select new { prod.ProductID, prod.Quantity } into p
                            group p by p.ProductID into g
                            select new
                            {
                                pID = g.Key,
                                sold = g.Sum(x => x.Quantity)
                            }).OrderByDescending(y => y.sold).Take(3).ToList();



            List<TopSoldProduct> topSoldProds = new List<TopSoldProduct>();
            if(topSoldProds.Count>0)
            {
                for (int i = 0; i < 3; i++)
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
        //RECENT VIEWS PRODUCTS
        public IEnumerable<Product> RecentViewProducts()
        {
            if (TempShpData.UserID > 0)
            {
                var top3Products = (from recent in db.RecentlyViews
                                    where recent.UserID == TempShpData.UserID
                                    orderby recent.ViewDate descending
                                    select recent.ProductID).ToList().Take(8);

                var recentViewProd = db.Products.Where(x => top3Products.Contains(x.ProductID));
                return recentViewProd;
            }
            else
            {
                var prod = (from p in db.Products
                            select p).OrderByDescending(x => x.UnitPrice).Take(3).ToList();
                return prod;
            }
        }

        
        //ADD TO CART
        public ActionResult AddToCart(int id)
        {
           
            OrderDetail OD = new OrderDetail();
            OD.ProductID = id;
            int Qty = 1;
            int price = (int)db.Products.Find(id).UnitPrice;
            OD.Quantity = Convert.ToInt32(Qty);
            OD.UnitPrice = price;
            OD.TotalAmount = OD.UnitPrice * OD.Quantity;
            OD.Product = db.Products.Find(id);

            if (TempShpData.items == null)
            {
                TempShpData.items = new List<OrderDetail>();
            }
            TempShpData.items.Add(OD);
            AddRecentViewProduct(id);
            return Redirect(TempData["returnURL"].ToString());

        }

        //VIEW DETAILS
        public ActionResult ViewDetails(int id)
        {
            var prod = db.Products.Find(id);
            var reviews = db.Reviews.Where(x => x.ProductID == id).ToList();
            ViewBag.Reviews = reviews;
            ViewBag.TotalReviews = reviews.Count();
            ViewBag.RelatedProducts = db.Products.Where(y => y.CategoryID == prod.CategoryID).ToList();
            AddRecentViewProduct(id);

            var ratedProd = db.Reviews.Where(x => x.ProductID == id).ToList();
            int count = ratedProd.Count();
            int TotalRate = ratedProd.Sum(x => x.Rate).GetValueOrDefault();
            ViewBag.AvgRate = TotalRate > 0 ? TotalRate / count : 0;

            this.GetDefaultData();
            return View(prod);
        }

        //WISHLIST
        public ActionResult WishList(int id)
        {

            Wishlist wl = new Wishlist();
            wl.ProductID = id;
            wl.UserID = TempShpData.UserID;

            db.Wishlists.Add(wl);
            db.SaveChanges();
            AddRecentViewProduct(id);
            ViewBag.WlItemsNo = db.Wishlists.Where(x => x.UserID == TempShpData.UserID).ToList().Count();
            if (TempData["returnURL"].ToString() == "/")
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(TempData["returnURL"].ToString());
        }

        //ADD RECENT VIEWS PRODUCT IN DB
        public void AddRecentViewProduct(int? pid)
        {
            if (TempShpData.UserID > 0)
            {
                RecentlyView Rv = new RecentlyView();
                Rv.UserID = TempShpData.UserID;
                Rv.ProductID = pid;
                Rv.ViewDate = DateTime.Now;
                db.RecentlyViews.Add(Rv);
                db.SaveChanges();
            }
        }

        //ADD REVIEWS ABOUT PRODUCT
        public ActionResult AddReview(int productID, FormCollection getReview)
        {

            Review r = new Review();
            r.UserID = TempShpData.UserID;
            r.ProductID = productID;
            r.Username = getReview["name"];
            r.Email = getReview["email"];
            r.Review1 = getReview["message"];
            r.Rate = Convert.ToInt32(getReview["rate"]);
            r.DateTime = DateTime.Now;

            db.Reviews.Add(r);
            db.SaveChanges();
            return RedirectToAction("ViewDetails/" + productID + "");

        }


        public ActionResult Products(int? subCatID)
        {
            ViewBag.Categories = db.Categories.Select(x => x.CategoryName).ToList();
            var prods = db.Products.Where(y => y.CategoryID == subCatID).ToList();
            return View(prods);
        }

        //GET PRODUCTS BY CATEGORY
        public ActionResult GetProductsByCategory(string categoryName, int? page)
        {
            ViewBag.Categories = db.Categories.Select(x => x.CategoryName).ToList();
            ViewBag.TopRatedProducts = TopSoldProducts();

            ViewBag.RecentViewsProducts = RecentViewProducts();
            ViewBag.GetProductsByCategory = true;

            var prods = db.Products.Where(x => x.Category.CategoryName == categoryName).ToList();
            return View("Products", prods.ToPagedList(page ?? 1, 9));
        }
        

        //SEARCH BAR
        public ActionResult Search(string product, int? page)
        {
            ViewBag.Categories = db.Categories.Select(x => x.CategoryName).ToList();
            ViewBag.TopRatedProducts = TopSoldProducts();

            ViewBag.RecentViewsProducts = RecentViewProducts();

            List<Product> products;
            if (!string.IsNullOrEmpty(product))
            {
                products = db.Products.Where(x => x.ProductName.StartsWith(product)).ToList();
            }
            else
            {
                products = db.Products.ToList();
            }
            return View("Products", products.ToPagedList(page ?? 1, 6));
        }

        public JsonResult GetProducts(string term)
        {
            List<string> prodNames = db.Products.Where(x => x.ProductName.StartsWith(term)).Select(y => y.ProductName).ToList();
            return Json(prodNames, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FilterByPrice(int minPrice, int maxPrice, int? page)
        {
            ViewBag.Categories = db.Categories.Select(x => x.CategoryName).ToList();
            ViewBag.TopRatedProducts = TopSoldProducts();

            ViewBag.RecentViewsProducts = RecentViewProducts();
            ViewBag.filterByPrice = true;
            var filterProducts = db.Products.Where(x => x.UnitPrice >= minPrice && x.UnitPrice <= maxPrice).ToList();
            return View("Products", filterProducts.ToPagedList(page ?? 1, 9));
        }


    }
}