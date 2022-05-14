using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Services
{
    public class ForPaging
    {
        public int NowPage { get; set; }
        public int MaxPage { get; set; }
        public int ItemNum 
        { 
            get
            {
                return 5;
            }
        }
        public ForPaging()
        {
            NowPage = 1;
        }
        public ForPaging(int Page)
        {
            NowPage = Page;
        }
        public void SetRightPage()
        {
            if (NowPage < 1)
            {
                NowPage = 1;
            }
            else if (NowPage > MaxPage)
            {
                NowPage = MaxPage;
            }
            if (MaxPage.Equals(0))
            {
                NowPage = 1;
            }
        }
    }
}