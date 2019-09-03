using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WorkShop4_MVC.Models
{
    public class BookData
    {
        [DisplayName("PK流水號")]
        public string BookId { get; set; }  //not null  
        [DisplayName("書籍名稱")]
        public string BookName { get; set;} //not null
        [DisplayName("類別代號")]
        public string BookClassId { get; set; }  //not null
        [DisplayName("書籍作者")]
        public string BookAuthor { get; set; } //allow null
        [DisplayName("購書日期")]
        public string BookBoughtDate { get; set; }  //allow null
        [DisplayName("出版商")]
        public string BookPublisher { get; set; }  //allow null
        [DisplayName("內容簡介")]
        public string BookNote { get; set; }   //allow null
        [DisplayName("書籍保管人")]
        public string BookKeeper { get; set; }  //allow null
        [DisplayName("狀態")]
        public string BookStatus { get; set; }  //not null
        [DisplayName("書籍購買金額")]
        public string BookAmount { get; set; }  //allow null
        [DisplayName("建立時間")]
        public string CreateDate { get; set; }  //allow null
        [DisplayName("建立使用者")]
        public string CreateUser { get; set; }  //allow null
        [DisplayName("修改時間")]
        public string ModifyDate { get; set; }  //allow null
        [DisplayName("修改使用者")]
        public string ModifyUser { get; set; }  //allow null
    }
}