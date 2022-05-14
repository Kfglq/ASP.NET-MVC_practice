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
    public class HomeController : Controller
    {
        private readonly MembersDBService membersDBService = new MembersDBService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList(string Search,int Page = 1)
        {
            HomeViewModel Data = new HomeViewModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.DataList = membersDBService.GetDataList(Data.Paging, Data.Search);
            return PartialView(Data);
        }
        [HttpPost]
        public ActionResult GetDataList([Bind(Include ="Search")]HomeViewModel Data)
        {
            return RedirectToAction("GetDataList", new { Search = Data.Search });
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