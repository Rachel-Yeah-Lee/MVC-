using System.ComponentModel;
namespace WorkShop4_MVC.Models
{
    public class SearchBookStatusArg
    {
        [DisplayName("書名")]
        public string BookName { get; set; }
        [DisplayName("圖書類別")]
        public string BookClass { get; set; }
        [DisplayName("借閱人")]
        public string BookKeeper { get; set; }
        [DisplayName("借閱狀態")]
        public string BookStatus { get; set; }

    }
}