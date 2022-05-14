using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Guestbooks
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("會員帳號:")]
        public string Account { get; set; }
        [DisplayName("姓名:")]
        [Required(ErrorMessage ="請輸入姓名")]
        [StringLength(20,ErrorMessage ="請勿輸入超過20字")]
        public string Name { get; set; }
        [DisplayName("留言:")]
        [Required(ErrorMessage = "請輸入留言")]
        [StringLength(100, ErrorMessage = "請勿輸入超過100字")]
        public string Content { get; set; }
        [DisplayName("留言時間:")]
        public DateTime CreateTime { get; set; }
        [DisplayName("回覆:")]
        [Required(ErrorMessage = "請輸入回覆")]
        [StringLength(100, ErrorMessage = "請勿輸入超過100字")]
        public string Reply { get; set; }
        [DisplayName("回覆時間:")]
        public DateTime? ReplyTime { get; set; }
        public Members Member { get; set; } = new Members();
    }
}