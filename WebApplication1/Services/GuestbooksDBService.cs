using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class GuestbooksDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 判斷有無Search
        public List<Guestbooks> GetDataList(ForPaging Paging,string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            string sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                sql = $@"SELECT * FROM Guestbooks WHERE Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%';";
                SetMaxPaging(Paging, sql);
                sql = $@"SELECT m.*,d.Name,d.IsAdmin FROM (SELECT row_number() OVER(order by Id) AS sort,* FROM Guestbooks WHERE Name like '%{Search}%' or Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%') m inner join Members d on
                        m.Account = d.Account Where m.sort BETWEEN {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
            }
            else
            {
                sql = @"SELECT * FROM Guestbooks;";
                SetMaxPaging(Paging, sql);
                sql = $@"SELECT m.*,d.Name,d.IsAdmin FROM (SELECT row_number() OVER(order by Id) AS sort,* FROM Guestbooks )m 
                       inner join Members d on m.Account = d.Account WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
            }
            DataList = GetAllDataList(Paging, sql);
            return DataList;
        }
        #endregion

        #region 設定頁面的最大頁數
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

        #region 取得主頁所有資料
        public List<Guestbooks> GetAllDataList(ForPaging Paging,string sql)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Guestbooks Data = new Guestbooks();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                if (!dr["ReplyTime"].Equals(DBNull.Value))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }

                Data.Member.Name = dr["Name"].ToString();
                DataList.Add(Data);
            }
            return DataList;
        }
        #endregion

        #region 新增留言
        public void InsertGuestbooks(Guestbooks Data)
        {
            string sql = $@"INSERT INTO Guestbooks(Account,Content,CreateTime) VALUES('{Data.Account}','{Data.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}');";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 用Id找到單筆資料
        public Guestbooks GetDataById(int Id)
        {
            Guestbooks Data = new Guestbooks();
            string sql = $@"SELECT * FROM Guestbooks m inner join Members d on m.Account = d.Account WHERE Id = {Id};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                if (!dr["ReplyTime"].Equals(DBNull.Value))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }
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

        #region 修改留言
        public void UpdateGuestbooks(Guestbooks Data)
        {
            string sql = $@"UPDATE Guestbooks SET Account = '{Data.Account}',Content = '{Data.Content}' WHERE Id = '{Data.Id}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 檢查留言是否已被回覆
        public bool CheckUpdate(int Id)
        {
            Guestbooks Data = GetDataById(Id);
            return (Data != null && Data.Reply == null);
        }
        #endregion

        #region 回覆留言
        public void ReplyGuestbooks(Guestbooks Data)
        {
            string sql = $@"UPDATE Guestbooks SET Reply = '{Data.Reply}',ReplyTime = '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE Id = '{Data.Id}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 刪除留言
        public void DeleteGuestbooks(int Id)
        {
            string sql = $@"Delete FROM Guestbooks WHERE Id = {Id};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

    }
}