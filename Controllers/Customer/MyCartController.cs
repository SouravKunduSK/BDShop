using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public class MyCartController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: MyCart
        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                var data = this.GetDefaultData();

                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            

        }

        public ActionResult Remove(int id)
        {
            TempShpData.items.RemoveAll(x => x.ProductID == id);
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult ProcedToCheckout(FormCollection formcoll)
        {
            var a = TempShpData.items.ToList();
            for (int i = 0; i < formcoll.Count / 2; i++)
            {

                int pID = Convert.ToInt32(formcoll["shcartID-" + i + ""]);
                var ODetails = TempShpData.items.FirstOrDefault(x => x.ProductID == pID);


                int qty = Convert.ToInt32(formcoll["Qty-" + i + ""]);
                ODetails.Quantity = qty;
                ODetails.UnitPrice = ODetails.UnitPrice;
                ODetails.TotalAmount = qty * ODetails.UnitPrice;
                TempShpData.items.RemoveAll(x => x.ProductID == pID);

                if (TempShpData.items == null)
                {
                    TempShpData.items = new List<OrderDetail>();
                }
                TempShpData.items.Add(ODetails);

            }

            return RedirectToAction("Index", "CheckOut");
        }
    }
}