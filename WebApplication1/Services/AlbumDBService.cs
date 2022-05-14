using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;
using System.IO;

namespace WebApplication1.Services
{
    public class AlbumDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        
        #region 查詢一筆相片
        public Album GetDataById(int Alb_Id)
        {
            Album Data = new Album();
            string sql = $@"Select m.*,d.Name from Album m inner join Members d on m.Account = d.Account where m.Alb_Id = {Alb_Id};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                Data.FileName = dr["FileName"].ToString();
                Data.Size = Convert.ToInt32(dr["Size"]);
                Data.Url = dr["Url"].ToString();
                Data.Type = dr["Type"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Member.Name = dr["Name"].ToString();
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

        #region 查詢相片陣列資料
        public List<Album> GetDataList(ForPaging Paging)
        {
            SetMaxPaging(Paging);
            List<Album> DataList = new List<Album>();
            string sql = $@"Select m.*,d.Name from (Select row_number() over(order by CreateTime desc) as sort,* from Album )
                         m inner join Members d on m.Account = d.Account where m.sort between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum};";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Album Data = new Album();
                Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                Data.FileName = dr["FileName"].ToString();
                Data.Size = Convert.ToInt32(dr["Size"]);
                Data.Account = dr["Account"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Member.Name = dr["Name"].ToString();
                DataList.Add(Data);
            }
            conn.Close();
            return DataList;
        }

        #region 設定最大頁數
        public void SetMaxPaging(ForPaging Paging)
        {
            int row = 0;
            string sql = $@"Select * from Album;";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql,conn);
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

        #endregion

        #region 上傳檔案
        public void UploadFile(int Alb_Id,string FileName,string Url,int Size,string Type,string Account)
        {
            string sql = $@"Insert into Album(Alb_Id,FileName,Url,Size,Type,Account,CreateTime)
                         values('{Alb_Id}','{FileName}','{Url}','{Size}','{Type}','{Account}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}');";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 計算目前最新一筆Alb_Id
        public int LastAlbumFinder()
        {
            int Id;
            string sql = $@"Select top 1 * from Album order by Alb_Id desc;";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Alb_Id"]);
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

        #region 刪除檔案
        public void Delete(int Alb_Id)
        {
            string sql = $@"Delete from Album where Alb_Id = '{Alb_Id}';";
            Album Data = GetDataById(Alb_Id);
            File.Delete(Data.Url);
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion
    }
}