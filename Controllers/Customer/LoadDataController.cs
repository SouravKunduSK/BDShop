using BdShop.Database;
using BdShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BdShop.Controllers.Customer
{
    public static class LoadDataController
    {
        static BdShopEntities db = new BdShopEntities();
        // GET: LoadData
        public static List<OrderDetail> GetDefaultData(this ControllerBase controller)
        {
            if (TempShpData.items == null)
            {
                TempShpData.items = new List<OrderDetail>();
            }
            var data = TempShpData.items.ToList();

            controller.ViewBag.cartBox = data.Count == 0 ? null : data;
            controller.ViewBag.NoOfItem = data.Count();
            int? SubTotal = Convert.ToInt32(data.Sum(x => x.TotalAmount));
            controller.ViewBag.Total = SubTotal;

            int Discount = 0;
            int ShippingCost = 50;
            controller.ViewBag.SubTotal = SubTotal;
            controller.ViewBag.Discount = Discount;
            controller.ViewBag.ShippingCost = ShippingCost;
            controller.ViewBag.TotalAmount = SubTotal - Discount + ShippingCost;

            controller.ViewBag.WlItemsNo = db.Wishlists.Where(x => x.UserID == TempShpData.UserID).ToList().Count();
            return data;
        }
    }
}