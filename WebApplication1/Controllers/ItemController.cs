using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Services;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using System.IO;

namespace WebApplication1.Controllers
{
    public class ItemController : Controller
    {
        private CartService cartService = new CartService();
        private ItemService itemService = new ItemService();
        
        #region 商品主頁
        public ActionResult Index(int Page = 1)
            {
            ItemViewModel Data = new ItemViewModel();
            Data.Paging = new ForPaging(Page);
            Data.IdList = itemService.GetDataList(Data.Paging);
            Data.ItemBlock = new List<ItemDetailViewModel>();
            foreach(var Id in Data.IdList)
            {
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                newBlock.Data = itemService.GetDataById(Id);
                string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
                newBlock.InCart = cartService.CheckInCart(Cart, Id);
                Data.ItemBlock.Add(newBlock);
            }
            return View(Data);
        }
        #endregion

        #region 單一商品頁面
        public ActionResult Item(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return View(ViewData);
        }
        #endregion

        #region 商品主頁(部分檢視商品)
        public ActionResult ItemBlock(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return PartialView(ViewData);
        }
        #endregion

        #region 新增商品
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult Create(ItemCreateViewModel Data)
        {
            if (Data.ItemImage != null)
            {
                string fileName = Path.GetFileName(Data.ItemImage.FileName);//只傳檔案名稱和副檔名
                string Url = Path.Combine(Server.MapPath("~/Upload/"), fileName);
                Data.ItemImage.SaveAs(Url);
                Data.NewData.Image = fileName;
                itemService.Insert(Data.NewData);
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("", "請選擇上傳檔案");//
                return View();
            }
        }
        #endregion

        #region 刪除商品
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int Id,string fileName)
        {
            itemService.Delete(Id);
            string path = Path.Combine(Server.MapPath("~/Upload/"), fileName);//在記憶體的路徑
            FileInfo file = new FileInfo(path);
            file.Delete();
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}