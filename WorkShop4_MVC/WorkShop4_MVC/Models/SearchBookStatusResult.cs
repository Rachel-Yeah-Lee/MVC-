using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookManagement.Models
{
    public class SearchBookStatusResult
    {   /// <summary>
        /// 書籍代碼
        /// </summary>
        public string BookId { get; set; }
        /// <summary>
        /// 書籍類別
        /// </summary>
        public string BookClass { get; set; }
        /// <summary>
        /// 書籍名稱
        /// </summary>
        public string BookName { get; set; }
        /// <summary>
        /// 購買日期
        /// </summary>
        public string BoughtDate { get; set; }
        /// <summary>
        /// 書本狀態
        /// </summary>
        public string BookStatus { get; set; }
        /// <summary>
        /// 借閱人Id
        /// </summary>
        public string BookKeeper { get; set; }
    }
}