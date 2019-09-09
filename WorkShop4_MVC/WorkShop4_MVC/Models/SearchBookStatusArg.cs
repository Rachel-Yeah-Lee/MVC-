using System.ComponentModel;
namespace BookManagement.Models
{
    public class SearchBookStatusArg
    {
        [DisplayName("書名")]
        public string BookName { get; set; }
        [DisplayName("圖書類別")]
        public string BookClass { get; set; }
        [DisplayName("借閱人")]
        public string BookKeeperID { get; set; }    //這裡是大寫的ID，Index裏ajax也要同樣的名稱才能對上給值
        [DisplayName("借閱狀態")]
        public string BookStatus { get; set; }

    }
}