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
    public class CartService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 讀取購物車中的商品
        public List<Cart> GetItemFromCart(string Cart)
        {
            List<Cart> DataList = new List<Cart>();
            string sql = $@"Select * From Cart m inner join Item d on m.Item_Id = d.Id Where Cart_Id = '{Cart}';";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Cart Data = new Cart();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                Data.Item.Id = Convert.ToInt32(dr["Id"]);
                Data.Item.Image = dr["Image"].ToString();
                Data.Item.Name = dr["Name"].ToString();
                Data.Item.Price = Convert.ToInt32(dr["Price"]);
                DataList.Add(Data);
            }
            conn.Close();
            return DataList;
        }
        #endregion

        #region 確認商品是否在購物車中
        public bool CheckInCart(string Cart,int Item_Id)
        {
            Cart Data = new Cart();
            string sql = $@"Select * from Cart m inner join Item d on m.Item_Id = d.Id Where Cart_Id = '{Cart}' AND Item_Id = '{Item_Id}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                Data.Item.Id = Convert.ToInt32(dr["Id"]);
                Data.Item.Image = dr["Image"].ToString();
                Data.Item.Name = dr["Name"].ToString();
                Data.Item.Price = Convert.ToInt32(dr["Price"]);
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data != null);
        }
        #endregion

        #region 加入購物車中
        public void AddtoCart(string Account,string Cart,int Item_Id)
        {
            string sql = $@"Insert into Cart(Account,Cart_Id,Item_Id) Values('{Account}','{Cart}','{Item_Id}');";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 從購物車中移除
        public void RemoveForCart(string Account,string Cart,int Item_Id)
        {
            string sql = $@"Delete From Cart Where Account = '{Account}' AND Cart_Id = '{Cart}' AND Item_Id = '{Item_Id}';";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion

        #region 讀取存起來的購物車
        public string GetCartSave(string Account)
        {
            Cart Data = new Cart();
            string sql = $@"Select * From Cart m inner join Members d on m.Account = d.Account Where m.Account = '{Account}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Member.Name = dr["Name"].ToString();
            }
            catch(Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            if(Data != null)
            {
                return Data.Cart_Id;
            }
            else
            {
                return null;
            }
        }
        #endregion

    }
}