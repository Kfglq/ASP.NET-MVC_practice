using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
    public class ItemCreateViewModel
    {
        [DisplayName("商品圖片 ")]
        [FileExtensions(ErrorMessage ="上傳的檔案不是圖片")]
        public HttpPostedFileBase ItemImage { get; set; }
        public Item NewData { get; set; }
    }
}