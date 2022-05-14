using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Services;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using WebApplication1.Security;
using System.Web.Configuration;
using System.IO;

namespace WebApplication1.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members
        private readonly MembersDBService membersService = new MembersDBService();
        private readonly MailService mailService = new MailService();
        private readonly CartService cartService = new CartService();
        public ActionResult Index()
        {
            return View();
        }

        #region 註冊
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            if (ModelState.IsValid)
            {
                if(RegisterMember.MembersImage != null)
                {
                    if (membersService.CheckImage(RegisterMember.MembersImage.ContentType))
                    {
                        string fileName = Path.GetFileName(RegisterMember.MembersImage.FileName);
                        string path = Path.Combine(Server.MapPath($@"~/Upload/Members/"), fileName);
                        RegisterMember.MembersImage.SaveAs(path);
                        RegisterMember.newMember.Image = fileName;
                        RegisterMember.newMember.Password = RegisterMember.Password;
                        string AuthCode = mailService.GetValidateCode();
                        RegisterMember.newMember.AuthCode = AuthCode;
                        membersService.Register(RegisterMember.newMember);
                        string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                        UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                        { Path = Url.Action("EmailValidate", "Members", new { Account = RegisterMember.newMember.Account, AuthCode = AuthCode }) };
                        string MailBody = mailService.GetRegisterMailBody(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));
                        mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);
                        TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                        return RedirectToAction("RegisterResult");
                    }
                    else
                    {
                        ModelState.AddModelError("MembersImage", "所上傳檔案不是圖片");
                    }
                }
                else
                {
                    ModelState.AddModelError("MembersImage", "請選擇上傳檔案");
                    return View(RegisterMember);
                }
            }
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            return View(RegisterMember);
        }
        #endregion

        #region 註冊結果
        public ActionResult RegisterResult()
        {
            return View();
        }
        #endregion

        #region 帳號確認
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 信箱驗證
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }
        #endregion

        #region 登入
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            if (String.IsNullOrEmpty(ValidateStr))
            {
                HttpContext.Session.Clear();
                string Cart = cartService.GetCartSave(LoginMember.Account);
                if (Cart != null)
                {
                    HttpContext.Session["Cart"] = Cart;
                }
                string RoleData = membersService.GetRole(LoginMember.Account);
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                JwtService jwtService = new JwtService();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Value = Server.UrlEncode(Token);
                Response.Cookies.Add(cookie);
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", ValidateStr);
                return View(LoginMember);
            }
        }
        #endregion

        #region 登出
        public ActionResult Logout()
        {
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            return RedirectToAction("Login");
        }
        #endregion

        #region 更換密碼
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(MembersChangePasswordViewModel ChangeData)
        {
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion
    }
}