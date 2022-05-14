using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private CartService cartService = new CartService();

        #region 購物車主頁
        [Authorize]
        public ActionResult Index()
        {
            CartViewModel Data = new CartViewModel();
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            Data.DataList = cartService.GetItemFromCart(Cart);
            return View(Data);
        }
        #endregion

        #region 加入購物車中
        [Authorize]
        public ActionResult Put(int Id,string toPage)
        {
            if(HttpContext.Session["Cart"] == null)
            {
                HttpContext.Session["Cart"] = DateTime.Now.ToString() + User.Identity.Name;
            }
            cartService.AddtoCart(User.Identity.Name,HttpContext.Session["Cart"].ToString(), Id);
            if(toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if(toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 移出購物車中
        [Authorize]
        public ActionResult Pop(int Id,string toPage)
        {
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            cartService.RemoveForCart(User.Identity.Name,Cart, Id);
            if (toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

    }
}