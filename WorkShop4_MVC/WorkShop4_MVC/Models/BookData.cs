using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookManagement.Models
{
    public class BookData    //***這裡的結構再想一下
    {
        [DisplayName("PK流水號")]
        public string BookId { get; set; }  //not null  
        [DisplayName("書名")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookName { get; set; }
        [DisplayName("類別代號")]
        public string BookClassId { get; set; }  //not null
        [DisplayName("圖書類別")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookClassName { get; set; }
        [DisplayName("作者")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookAuthor { get; set; } //allow null
        [DisplayName("購書日期")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookBoughtDate { get; set; }
        [DisplayName("出版商")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookPublisher { get; set; }  //allow null
        [DisplayName("內容簡介")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookNote { get; set; }   //allow null
        [DisplayName("書籍保管人")]
        public string BookKeeper { get; set; }  //allow null, 保管人代碼
        [DisplayName("書籍狀態代號")]
        public string BookStatusId { get; set; }  //not null
        [DisplayName("書籍狀態代名稱")]
        public string BookStatusName { get; set; }  //not null
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