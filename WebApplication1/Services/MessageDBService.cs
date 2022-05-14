using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class MessageDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        
        #region 取得留言陣列
        public List<Message> GetDataList(ForPaging Paging,int A_Id)
        {
            List<Message> DataList = new List<Message>();
            SetMaxPaging(Paging, A_Id);
            DataList = GetAllDataList(Paging, A_Id);
            return DataList;
        }
        #endregion

        #region 設定最大頁數
        public void SetMaxPaging(ForPaging Paging,int A_Id)
        {
            int num = 0;
            string sql = $@"Select * from Message where A_Id = '{A_Id}';";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                num++;
            }
            conn.Close();
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(num) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion

        #region 取得Message資料
        public List<Message> GetAllDataList(ForPaging Paging,int A_Id)
        {
            List<Message> DataList = new List<Message>();
            string sql = $@"Select m.*,d.Name from (Select row_number() OVER(order by M_Id) AS sort,* from Message where A_Id = {A_Id}) m inner join Members d on m.Account = d.Account where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Message Data = new Message();
                    Data.M_Id = Convert.ToInt32(dr["M_Id"]);
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        #endregion

        #region 新增文章留言
        public void InsertMessage(Message newData)
        {
            newData.M_Id = LastMessageFinder(newData.A_Id);
            string sql = $@"Insert into Message(A_Id,M_Id,Account,Content,CreateTime) 
            Values('{newData.A_Id}','{newData.M_Id}','{newData.Account}','{newData.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}');";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 計算目前最新一筆的M_Id
        public int LastMessageFinder(int A_Id)
        {
            int Id = 0;
            string sql = $@"Select top 1 * from Message where A_Id = '{A_Id}' order by M_Id desc;";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["M_Id"]);
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

        #region 修改留言
        public void UpdateMessage(Message UpdateData)
        {
            string sql = $@"Update Message set Content = '{UpdateData.Content}' where A_Id = {UpdateData.A_Id} AND M_Id = {UpdateData.M_Id};";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 刪除留言
        public void DeleteMessage(int A_Id,int M_Id)
        {
            string DeleteMessage = $@"Delete from Message where A_Id = {A_Id} AND M_Id = {M_Id};";
            conn.Open();
            SqlCommand cmd = new SqlCommand(DeleteMessage, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion
    }
}