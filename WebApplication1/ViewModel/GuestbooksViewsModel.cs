using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.ViewModel
{
    public class GuestbooksViewsModel
    {
        [DisplayName("搜尋:")]
        public string Search { get; set; }
        public List<Guestbooks> DataList { get; set; }
        public ForPaging Paging { get; set; }
    }
}