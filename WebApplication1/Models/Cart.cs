using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Cart
    {
        public string Account { get; set; }
        public string Cart_Id { get; set; }
        public int Item_Id { get; set; }
        public Item Item { get; set; } = new Item();
        public Members Member { get; set; } = new Members();
    }
}