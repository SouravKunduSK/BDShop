using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public class CheckOutController : Controller
    {
        BdShopEntities db = new BdShopEntities();
        // GET: CheckOut
        public ActionResult Index()
        {
            if(Session["username"]!=null)
            {
                ViewBag.PayMethod = new SelectList(db.PaymentTypes, "PaymentTypeID", "PaymentTypeName");


                var data = this.GetDefaultData();

                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }


        //PLACE ORDER--LAST STEP
        public ActionResult PlaceOrder(FormCollection getCheckoutDetails)
        {

            int shpID = 1;
            if (db.ShippingDetails.Count() > 0)
            {
                shpID = db.ShippingDetails.Max(x => x.ShippingID) + 1;
            }
            int payID = 1;
            if (db.Payments.Count() > 0)
            {
                payID = db.Payments.Max(x => x.PaymentID) + 1;
            }
            int orderID = 1;
            if (db.Ordereds.Count() > 0)
            {
                orderID = db.Ordereds.Max(x => x.OrderedID) + 1;
            }


            var user = new User();
            ShippingDetail shpDetails = new ShippingDetail();
            shpDetails.ShippingID = shpID;
            shpDetails.FirstName = getCheckoutDetails["FirstName"];
            shpDetails.LastName = getCheckoutDetails["LastName"];
            shpDetails.Email = getCheckoutDetails["Email"];
            shpDetails.Mobile = getCheckoutDetails["Mobile"];
            shpDetails.Address = getCheckoutDetails["Address"];
            //shpDetails.Province = getCheckoutDetails["Province"];
            //shpDetails.City = getCheckoutDetails["City"];
            //shpDetails.PostCode = getCheckoutDetails["PostCode"];
            shpDetails.PostalCode = getCheckoutDetails["PostCode"];
            db.ShippingDetails.Add(shpDetails);
            db.SaveChanges();

            Payment pay = new Payment();
            pay.PaymentID = payID;
            pay.PaymentType = Convert.ToInt32(getCheckoutDetails["PayMethod"]);
            db.Payments.Add(pay);
            db.SaveChanges();

            Ordered o = new Ordered();
            o.OrderedID = orderID;
            o.UserID = TempShpData.UserID;
            o.PaymentID = payID;
            o.ShippingID = shpID;
            o.Discount = Convert.ToInt32(getCheckoutDetails["discount"]);
            o.TotalAmount = Convert.ToInt32(getCheckoutDetails["totalAmount"]);
            o.isCompleted = true;
            o.OrderDate = DateTime.Now;
            db.Ordereds.Add(o);
            db.SaveChanges();

            foreach (var OD in TempShpData.items)
            {
                OD.OrderedID = orderID;
                OD.Ordered = db.Ordereds.Find(orderID);
                OD.Product = db.Products.Find(OD.ProductID);
                db.OrderDetails.Add(OD);
                db.SaveChanges();
            }


            return RedirectToAction("Index", "ThankYou");

        }
    }
}