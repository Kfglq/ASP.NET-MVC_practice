using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using System.Text;

namespace WebApplication1.Services
{
    public class ItemService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 藉由Id拿一筆資料
        public Item GetDataById(int Id)
        {
            Item Data = new Item();
            string sql = $@"Select * from Item Where Id = '{Id}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Price = Convert.ToInt32(dr["Price"]);
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

        #region 拿整筆資料的陣列
        public List<int> GetDataList(ForPaging Paging)
        {
            SetMaxPaging(Paging);
            List<int> IdList = new List<int>();
            string sql = $@"Select Id From (Select row_number() OVER(order by Id desc) AS sort,* From Item) m Where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                IdList.Add(Convert.ToInt32(dr["Id"]));
            }
            conn.Close();
            return IdList;
        }
        #endregion

        #region 設定最大頁數
        public void SetMaxPaging(ForPaging Paging)
        {
            int row = 0;
            string sql = $@"Select * from Item;";
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

        #region 取得最新一筆Id
        public int LastItemFinder()
        {
            int Id;
            string sql = $@"Select top 1 * from Item order by Id desc;";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Id"]);
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

        #region 新增商品
        public void Insert(Item Data)
        {
            Data.Id = LastItemFinder();
            string sql = $@"Insert into Item(Id,Name,Price,Image) values('{Data.Id}','{Data.Name}','{Data.Price}','{Data.Image}');";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 刪除商品
        public void Delete(int Id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($@"Delete from Cart Where Item_Id = '{Id}';");
            sql.AppendLine($@"Delete from Item Where Id = '{Id}';");
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

    }
}