using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkShop4_MVC.Controllers
{
    public class BookManagementController : Controller
    {
        // GET: BookManagement
        [HttpGet]
        public ActionResult Index()
        {
            Models.BookMangementService bookMangement = new Models.BookMangementService();
            ViewBag.BookStatus = bookMangement.GetAllBookStatus();   //**命名應該不用加All，除非有其他會用到取個別的BookStatus
            ViewBag.BookClass = bookMangement.GetAllBookClassName();
            ViewBag.BookKeeper = bookMangement.GetAllMemberName();  //**CName代表中文名字，但是要求是要英文+中文，並且搜尋結果中的只要顯示英文名字，所以這樣別人在看的時候會混淆
            return View();
        }
        [HttpPost]
        public ActionResult Index(Models.SearchBookStatusArg arg)
        {
            Models.BookMangementService bookMangement = new Models.BookMangementService();
            ViewBag.BookStatus = bookMangement.GetAllBookStatus();
            ViewBag.BookClass = bookMangement.GetAllBookClassName();
            ViewBag.BookKeeper = bookMangement.GetAllMemberName();
            ViewBag.SearchResult = bookMangement.GetSearchResultByCondition(arg);
            return View();
        }
        [HttpGet]
        public ActionResult AddBook()    //**action 的名字會發現Index是大寫而addBook是駱駝命名 所以不好，visual studio預設是pascal命名，因此會有小灰點在addBook
        {
            Models.BookMangementService bookMangement = new Models.BookMangementService();
            ViewBag.BookClass = bookMangement.GetAllBookClassName();
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(Models.AddBookArg arg)
        {
            Models.BookMangementService bookMangement = new Models.BookMangementService();
            ViewBag.BookClass = bookMangement.GetAllBookClassName();
            ViewBag.BookStatus = bookMangement.GetAllBookStatus();
            ViewBag.BookClass = bookMangement.GetAllBookClassName();
            ViewBag.BookKeeper = bookMangement.GetAllMemberName();
            bookMangement.InsertBookData(arg);
            return View("Index");
        }
        [HttpPost]
        public JsonResult DeleteBook(string BookId)
        {
            try
            {
                Models.BookMangementService bookMangementService = new Models.BookMangementService();
                bool deleteSuccess = bookMangementService.DeleteBookDataById(BookId);
                if (deleteSuccess)
                {
                    return this.Json(true);
                }
                else
                {
                    return this.Json(false);
                }
            }
            catch(Exception ex)
            {
                return this.Json(false);
            }
        }
        [HttpGet]
        public ActionResult Detail(string BookId)       //**lookDetail -> Detail 因為lookDetail命名怪怪的
        {                                                   //**查看明細的時候將網址列的 ?bookId=A，會跳出錯誤，
                                                            //**因為資料庫中的 BOOK_ID 屬性是 int，GetBookDataById()裏面對資料庫的存取會出錯
                                                            //**所以用try catch包起來ex就會有錯誤訊息，網頁就不會當掉

            try
            {
                Models.BookMangementService bookMangementService = new Models.BookMangementService();
            Models.BookData bookData = new Models.BookData();
            bookData = bookMangementService.GetBookDataById(BookId);
            return View("Detail", bookData);
            }
            catch (Exception ex)
            {
                return View("Detail");
            }
        }
    }
}