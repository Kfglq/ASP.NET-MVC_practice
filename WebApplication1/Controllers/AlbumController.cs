using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModel;
using System.IO;

namespace WebApplication1.Controllers
{
    public class AlbumController : Controller
    {
        private readonly AlbumDBService albumDBService = new AlbumDBService();

        #region 首頁
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 相片列表
        [Authorize(Roles ="Admin")]
        public ActionResult List(int Page = 1)
        {
            AlbumViewModel Data = new AlbumViewModel();
            Data.Paging = new ForPaging(Page);
            Data.FileList = albumDBService.GetDataList(Data.Paging);
            return PartialView(Data);
        }
        #endregion

        #region 上傳檔案
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Upload([Bind(Include ="upload")]AlbumViewModel File)
        {
            if(File.upload != null)
            {
                int Alb_Id = albumDBService.LastAlbumFinder();
                string Url = Path.Combine(Server.MapPath("~/Upload/"), Alb_Id.ToString() + "_" + File.upload.FileName);
                File.upload.SaveAs(Url);
                albumDBService.UploadFile(Alb_Id, Alb_Id.ToString() + "_" + File.upload.FileName, Url, 
                File.upload.ContentLength, File.upload.ContentType, User.Identity.Name); 
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 顯示圖片
        [Authorize(Roles ="Admin")]
        public ActionResult Show(int Alb_Id)
        {
            AlbumViewModel ToShow = new AlbumViewModel();
            ToShow.File = albumDBService.GetDataById(Alb_Id);
            if (ToShow.File != null)
            {
                UrlHelper urlHelper = new UrlHelper(Request.RequestContext);
                urlHelper.Content("~/Upload/" + ToShow.File.FileName);
                return Content(urlHelper.Content("~/Upload/" + ToShow.File.FileName));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 下載檔案
        [Authorize(Roles ="Admin")]
        public ActionResult DownloadFile(int Alb_Id)
        {
            AlbumViewModel Download = new AlbumViewModel();
            Download.File = albumDBService.GetDataById(Alb_Id);
            if(Download != null)
            {
                Stream iStream = new FileStream(Download.File.Url, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, Download.File.Type, Download.File.FileName);
            }
            else
            {
                return JavaScript("alert(\"無此檔案\")");
            }
        }
        #endregion

        #region 刪除文章
        [Authorize]
        public ActionResult DeleteFile(int Alb_Id)
        {
            albumDBService.Delete(Alb_Id);
            return RedirectToAction("Index");
        }
        #endregion

        #region 相片輪播
        public ActionResult Carousel()
        {
            AlbumViewModel Data = new AlbumViewModel();
            Data.Paging = new ForPaging(1);
            Data.FileList = albumDBService.GetDataList(Data.Paging);
            return View(Data);
        }
        #endregion

    }
}