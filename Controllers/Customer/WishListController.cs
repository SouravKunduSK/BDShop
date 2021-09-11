using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public class WishListController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: WishList
        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                this.GetDefaultData();

                var wishlistProducts = db.Wishlists.Where(x => x.UserID == TempShpData.UserID).ToList();
                return View(wishlistProducts);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

           
        }

        //REMOVE ITEM FROM WISHLIST
        public ActionResult Remove(int id)
        {
            db.Wishlists.Remove(db.Wishlists.Find(id));
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        //ADD TO CART WISHLIST
        public ActionResult AddToCart(int id)
        {
            OrderDetail OD = new OrderDetail();

            int pid = (int)db.Wishlists.Find(id).ProductID;
            OD.ProductID = pid;
            int Qty = 1;
            decimal price = (decimal)db.Products.Find(pid).UnitPrice;
            OD.Quantity = Qty;
            OD.UnitPrice = price;
            OD.TotalAmount = Qty * price;
            OD.Product = db.Products.Find(pid);

            if (TempShpData.items == null)
            {
                TempShpData.items = new List<OrderDetail>();
            }
            TempShpData.items.Add(OD);

            db.Wishlists.Remove(db.Wishlists.Find(id));
            db.SaveChanges();

            return Redirect(TempData["returnURL"].ToString());

        }
    }
}