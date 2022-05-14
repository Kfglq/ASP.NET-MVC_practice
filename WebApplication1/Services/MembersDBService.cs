using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services
{
    public class MembersDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        
        #region 新增帳號
        public void Register(Members newMembers) 
        { 
            newMembers.Password = HashPassword(newMembers.Password);
            string sql = $@"INSERT INTO Members(Account,Password,Name,Image,Email,AuthCode,IsAdmin) 
                VALUES('{newMembers.Account}','{newMembers.Password}','{newMembers.Name}','{newMembers.Image}','{newMembers.Email}','{newMembers.AuthCode}','1');";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region Hash密碼
        public string HashPassword(string Password)
        {
            string saltkey = "1q2w3e4r5t6y7u8i9o0po7tyy";
            string saltAndPassword = string.Concat(Password, saltkey);
            //這段不懂 注意
            SHA256CryptoServiceProvider sha256Hasher= new SHA256CryptoServiceProvider();
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            byte[] HashData = sha256Hasher.ComputeHash(PasswordData);
            string HashResult = Convert.ToBase64String(HashData);
            return HashResult;
        }
        #endregion

        #region 查詢陣列資料
        public List<Members> GetDataList(ForPaging Paging,string Search)
        {
            List<Members> DataList = new List<Members>();
            if (!string.IsNullOrWhiteSpace(Search))
            {
                string sql = $@"Select * from Members where Account like '%{Search}%' or Name like '%{Search}%'";
                SetMaxPaging(Paging, sql);
                sql = $@"Select * from (Select row_number() over(order by Account) as sort,* from Members where Account like '%{Search}%' or Name like '%{Search}%') m 
                      where m.sort between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum}";
                DataList = GetAllDataList(Paging, sql);
            }
            else
            {
                string sql = $@"Select * from Members";
                SetMaxPaging(Paging, sql);
                sql = $@"Select * from (Select row_number() over(order by Account) as sort,* from Members) m 
                      where m.sort between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum}";
                DataList = GetAllDataList(Paging, sql);
            }
            return DataList;
        }

        #region 計算頁數
        public void SetMaxPaging(ForPaging Paging,string sql)
        {
            int row = 0;
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                row++;
            }
            conn.Close();
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion

        #region 取得陣列資料
        public List<Members> GetAllDataList(ForPaging Paging, string sql)
        {
            List<Members> DataList = new List<Members>();
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Members Data = new Members();
                Data.Account = dr["Account"].ToString();
                Data.Name = dr["Name"].ToString();
                DataList.Add(Data);
            }
            conn.Close();
            return DataList;
        }
        #endregion

        #endregion

        #region 藉由帳號找到單筆資料(私人資料)
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"SELECT * FROM Members WHERE Account = '{Account}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
            }
            catch(Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 確認帳號是否重複註冊
        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            bool result = (Data == null);
            return result;
        }
        #endregion

        #region 藉由帳號找到單筆資料(公開性資料)
        public Members GetPublicDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"SELECT * FROM Members WHERE Account = '{Account}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Account = dr["Account"].ToString();
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 信箱驗證
        public string EmailValidate(string Account,string AuthCode)
        {
            Members ValidateMembers = GetDataByAccount(Account);
            string ValidateStr = string.Empty;
            if(ValidateMembers != null)
            {
                if(ValidateMembers.AuthCode == AuthCode)
                {
                    string sql = $@"UPDATE Members SET AuthCode = '{string.Empty}' WHERE Account = '{Account}';";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    ValidateStr = "帳號驗證成功,現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤,請再重新確認或再註冊";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤,請再重新確認或再註冊";
            }
            return ValidateStr;
        }
        #endregion

        #region 登入確認
        public string LoginCheck(string Account,string Password)
        {
            Members LoginMember = GetDataByAccount(Account);
            if(LoginMember != null)
            {
                if (String.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過Email驗證，請取收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }
        #endregion

        #region 密碼確認
        public bool PasswordCheck(Members CheckMember,string Password)
        {
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            return result;
        }
        #endregion

        #region 取得腳色
        public string GetRole(string Account)
        {
            string Role = "User";
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";
            }
            return Role;
        }
        #endregion

        #region 變更密碼
        public string ChangePassword(string Account,string Password,string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);
            if (PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);
                string sql = $@"UPDATE Members set Password = '{LoginMember.Password}' WHERE Account = '{Account}'";
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return "密碼修改成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion

        #region 檢查圖片類型
        public bool CheckImage(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
            }
            return false;
        }
        #endregion

    }
}