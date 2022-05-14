using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Services
{
    public class ArticleDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        
        #region 用Id取得一筆資料
        public Article GetArticleDataById(int A_Id)
        {
            Article Data = new Article();
            string sql = $@"Select m.*,d.Name,d.Image From Article m inner join Members d on m.Account = d.Account where m.A_Id = '{A_Id}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Title = dr["Title"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Watch = Convert.ToInt32(dr["Watch"]);
                Data.Member.Name = dr["Name"].ToString();
                Data.Member.Image = dr["Image"].ToString();
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

        #region 判斷是否有搜尋值
        public List<Article> GetDataList(ForPaging Paging,string Search,string Account)
        {
            List<Article> DataList = new List<Article>();
            if (!string.IsNullOrWhiteSpace(Search))
            {
                string sql = $@"Select * from Article where (Title like '%{Search}%' or Content like '%{Search}%') AND Account = '{Account}';";
                SetMaxPaging(Paging, sql,Account);
                sql = $@"Select m.*,d.Name from (Select row_number() OVER(order by A_Id) AS sort,* from Article where (Title like '%{Search}%' or Content like '%{Search}%') AND Account = '{Account}') 
                m inner join Members d on m.Account = d.Account where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
                DataList = GetAllDataList(Paging, sql,Account);
            }
            else
            {
                string sql = $@"Select * from Article where Account = '{Account}';";
                SetMaxPaging(Paging, sql,Account);
                sql = $@"Select m.*,d.Name from (Select row_number() OVER(order by A_Id) AS sort,* from Article where Account = '{Account}') m inner join Members d on m.Account = d.Account where m.sort 
                Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
                DataList = GetAllDataList(Paging, sql,Account);
            }
            return DataList;
        }
        #endregion

        #region 設定最大頁數
        public void SetMaxPaging(ForPaging Paging,string sql,string Account)
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

        #region 拿到所有資料
        public List<Article> GetAllDataList(ForPaging Paging,string sql,string Account)
        {
            List<Article> DataList = new List<Article>();
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Article Data = new Article();
                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Title = dr["Title"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Member.Name = dr["Name"].ToString();
                Data.Watch = Convert.ToInt32(dr["Watch"]);
                DataList.Add(Data);
            }
            conn.Close();
            return DataList;
        }
        #endregion

        #region 新增文章
        public void InsertArticle(Article newData)
        {
            newData.A_Id = LastArticleFinder();
            string sql = $@"Insert into Article(A_Id,Title,Content,Account,CreateTime,Watch) 
            Values({newData.A_Id},'{newData.Title}','{newData.Content}','{newData.Account}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}',0);";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 計算目前文章最新一筆的A_Id
        public int LastArticleFinder()
        {
            int Id;
            string sql = $@"Select TOP 1 * From Article order by A_Id desc;";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["A_Id"]);
            }
            catch (Exception e)
            {
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion

        #region 修改文章
        public void UpdateArticle(Article UpdateData)
        {
            string sql = $@"Update Article Set Content = '{UpdateData.Content}' where A_Id = {UpdateData.A_Id};";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 刪除文章
        public void DeleteArticle(int A_Id)
        {
            string DeleteMessage = $@"Delete from Message where A_Id = {A_Id};";
            string DeleteArticle = $@"Delete from Article where A_Id = {A_Id};";
            string CombineSql = DeleteMessage + DeleteArticle;
            conn.Open();
            SqlCommand cmd = new SqlCommand(CombineSql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 修改檢查
        public bool CheckUpdate(int A_Id)
        {
            Article Data = GetArticleDataById(A_Id);
            int MessageCount = 0;
            string sql = $@"Select * from Message where A_Id = {A_Id};";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                MessageCount++;
            }
            conn.Close();
            return (Data != null && MessageCount == 0);
        }
        #endregion

        #region 人氣查詢
        public List<Article> GetPopularList(string Account)
        {
            List<Article> popularList = new List<Article>();
            string sql = $@"Select TOP 5 * from Article m inner join Members d on m.Account = d.Account where m.Account = '{Account}' order by Watch desc;";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Article Data = new Article();
                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Title = dr["Title"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Watch = Convert.ToInt32(dr["Watch"]);
                Data.Member.Name = dr["Name"].ToString();
                popularList.Add(Data);
            }
            conn.Close();
            return popularList;
        }
        #endregion

        #region 增加觀看人數
        public void AddWatch(int A_Id)
        {
            string sql = $@"Update Article set Watch = Watch + 1 where A_Id = '{A_Id}';";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion
    }
}