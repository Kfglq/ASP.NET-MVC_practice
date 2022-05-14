using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
    public class ArticleViewModel
    {
        public Article article { get; set; }
        public List<Message> DataList { get; set; }
    }
}