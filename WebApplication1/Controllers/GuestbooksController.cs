using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class GuestbooksController : Controller
    {
        private readonly GuestbooksDBService GuestbookService = new GuestbooksDBService();
        // GET: Guestbooks
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetDataList(string Search,int Page = 1)
        {
            GuestbooksViewsModel Data = new GuestbooksViewsModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.DataList = GuestbookService.GetDataList(Data.Paging,Data.Search);
            return PartialView(Data);
        }
        [HttpPost]
        public ActionResult GetDataList([Bind(Include ="Search")]GuestbooksViewsModel Data)
        {
            return RedirectToAction("GetDataList", new { Search = Data.Search });
            //注意這裡
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create([Bind(Include ="Name,Content")]Guestbooks Data)
        {
            Data.Account = User.Identity.Name;
            GuestbookService.InsertGuestbooks(Data);
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult Edit(int Id)
        {
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int Id,[Bind(Include = "Name,Content")] Guestbooks Data)
        {
            if (GuestbookService.CheckUpdate(Id))
            {
                Data.Id = Id;
                Data.Account = User.Identity.Name;
                GuestbookService.UpdateGuestbooks(Data);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles ="Admin")]
        public ActionResult Reply(int Id)
        {
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Reply(int Id, [Bind(Include = "Reply,ReplyTime")] Guestbooks Data)
        {
            if (GuestbookService.CheckUpdate(Id))
            {
                Data.Id = Id;
                GuestbookService.ReplyGuestbooks(Data);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int Id)
        {
            GuestbookService.DeleteGuestbooks(Id);
            return RedirectToAction("Index");
        }
    }
}