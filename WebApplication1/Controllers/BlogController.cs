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
    public class BlogController : Controller
    {
        private readonly MembersDBService membersDBService = new MembersDBService();
        private readonly ArticleDBService articleDBService = new ArticleDBService();
        private readonly MessageDBService messageDBService = new MessageDBService();
        
        #region 部落格頁面
        public ActionResult Index(string Account)
        {
            BlogViewModel Data = new BlogViewModel();
            Data.Member = membersDBService.GetPublicDataByAccount(Account);
            return View(Data);
        }
        #endregion

        #region 文章列表
        public ActionResult ArticleList(string Search, string Account, int Page = 1)
        {
            ArticleIndexViewModel Data = new ArticleIndexViewModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.Account = Account;
            Data.DataList = articleDBService.GetDataList(Data.Paging, Data.Search, Data.Account);
            return PartialView(Data);
        }
        #endregion

        #region 文章頁面
        public ActionResult Article(int A_Id)
        {
            ArticleViewModel Data = new ArticleViewModel();
            articleDBService.AddWatch(A_Id);
            Data.article = articleDBService.GetArticleDataById(A_Id);
            ForPaging paging = new ForPaging(0);
            Data.DataList = messageDBService.GetDataList(paging, A_Id);
            return View(Data);
        }
        #endregion

        #region 新增文章
        [Authorize]
        public ActionResult CreateArticle()
        {
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult CreateArticle([Bind(Include = "Title,Content")] Article Data)
        {
            Data.Account = User.Identity.Name;
            articleDBService.InsertArticle(Data);
            return RedirectToAction("Index", "Blog", new { Account = User.Identity.Name });
        }
        #endregion

        #region 修改文章
        [Authorize]
        public ActionResult EditArticle(int A_Id)
        {
            Article Data = new Article();
            Data = articleDBService.GetArticleDataById(A_Id);
            return PartialView(Data);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditArticle(int A_Id, Article Data)
        {
            if (articleDBService.CheckUpdate(A_Id))
            {
                articleDBService.UpdateArticle(Data);
            }
            return RedirectToAction("Article", new { A_Id = A_Id });
        }
        #endregion

        #region  刪除文章
        [Authorize]
        public ActionResult DeleteArticle(int A_Id)
        {
            articleDBService.DeleteArticle(A_Id);
            return RedirectToAction("Index", "Blog", new { Account = User.Identity.Name });
        }
        #endregion

        #region 顯示人氣
        public ActionResult ShowPopularity(string Account)
        {
            ArticleIndexViewModel Data = new ArticleIndexViewModel();
            Data.DataList = articleDBService.GetPopularList(Account);
            return View(Data);
        }
        #endregion

        #region 留言頁面
        public ActionResult Message(int A_Id = 1)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        #endregion

        #region 留言陣列
        public ActionResult MessageList(int A_Id, int Page = 1)
        {
            MessageViewModel Data = new MessageViewModel();
            Data.Paging = new ForPaging(Page);
            Data.A_Id = A_Id;
            Data.DataList = messageDBService.GetDataList(Data.Paging, Data.A_Id);
            return PartialView(Data);
        }
        #endregion

        #region 新增留言
        [Authorize]
        public ActionResult CreateMessage(int A_Id)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult CreateMessage(int A_Id, [Bind(Include = "Content")] Message Data)
        {
            Data.A_Id = A_Id;
            Data.Account = User.Identity.Name;
            messageDBService.InsertMessage(Data);
            return RedirectToAction("MessageList", new { A_Id = A_Id });
        }
        #endregion

        #region 修改留言
        [Authorize]
        public ActionResult UpdateMessage(int A_Id, int M_Id, string Content)
        {
            Message message = new Message();
            message.A_Id = A_Id;
            message.M_Id = M_Id;
            message.Content = Content;
            messageDBService.UpdateMessage(message);
            return RedirectToAction("Article", "Blog", new { A_Id = A_Id });
        }
        #endregion

        #region 刪除留言
        [Authorize]
        public ActionResult DeleteMessage(int A_Id, int M_Id)
        {
            messageDBService.DeleteMessage(A_Id, M_Id);
            return RedirectToAction("Article", "Blog", new { A_Id = A_Id });
        }
        #endregion

    }
}