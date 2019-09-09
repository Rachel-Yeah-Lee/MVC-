using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BookManagement.Models
{
    public class UpdateArg
    {
        [DisplayName("書名")]
        public string BookName { get; set; }
        [DisplayName("作者")]
        public string BookAuthor { get; set; }
        [DisplayName("出版商")]
        public string BookPublisher { get; set; }
        [DisplayName("內容簡介")]
        public string BookNote { get; set; }
        [DisplayName("購書日期")]
        public string BookBoughtDate { get; set; }
        [DisplayName("圖書類別")]
        public string BookClass { get; set; }
    }
}