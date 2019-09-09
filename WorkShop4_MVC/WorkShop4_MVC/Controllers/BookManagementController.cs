using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookManagement.Controllers
{
    public class BookManagementController : Controller
    {
        // GET: BookManagement
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetBookClassToDrop()
        {
            Models.BookMangementService bookManagement = new Models.BookMangementService();
            List<SelectListItem> bookClass = bookManagement.GetAllBookClassName();
            return Json(bookClass);
        }
        [HttpPost]
        public JsonResult GetBookStatusToDrop()  //**命名應該不用加All，除非有其他會用到取個別的BookStatus
        {
            Models.BookMangementService bookManagement = new Models.BookMangementService();
            List<SelectListItem> bookKeeper = bookManagement.GetAllBookStatus();
            return Json(bookKeeper);
        }
        [HttpPost]
        public JsonResult GetMemberNameToDrop()
        {
            Models.BookMangementService bookManagement = new Models.BookMangementService();
            List<SelectListItem> bookKeeper = bookManagement.GetAllMemberName(); //**CName代表中文名字，但是要求是要英文+中文，並且搜尋結果中的只要顯示英文名字，所以這樣別人在看的時候會混淆
            return Json(bookKeeper);
        }
        [HttpPost]
        public JsonResult GetGridData(Models.SearchBookStatusArg arg)
        {
            Models.BookMangementService bookMangement = new Models.BookMangementService();
            List<Models.SearchBookStatusResult> GridData = bookMangement.GetSearchResultByCondition(arg);
            return Json(GridData);
        }

        [HttpGet]
        public ActionResult AddBook()    //**action 的名字會發現Index是大寫而addBook是駱駝命名 所以不好，visual studio預設是pascal命名，因此會有小灰點在addBook
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(Models.AddBookArg arg)  //回傳Json到前端在導向其他頁面
        {
            Models.BookMangementService bookMangement = new Models.BookMangementService();
            {
                if (ModelState.IsValid)
                {
                    string BookID = bookMangement.InsertBookData(arg).ToString();    //**BookID用來作為導向明細頁面的索引值
                    return RedirectToAction("Detail", new { BookID });
                }
            }
            return View(arg);
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
            catch (Exception ex)
            {
                return this.Json(false);
            }
        }
        [HttpGet]     //Detail的參數用string 是不是比較好
        public ActionResult Detail(string BookId)       //**lookDetail -> Detail 因為lookDetail命名怪怪的
        {                                                   //**查看明細的時候將網址列的 ?bookId=A，會跳出錯誤，
                                                            //**因為資料庫中的 BOOK_ID 屬性是 int，GetBookDetails()裏面對資料庫的存取會出錯
                                                            //**所以用try catch包起來ex就會有錯誤訊息，網頁就不會當掉
                   //BookId 甚麼時候轉型比較好，Detail的參數是int，那參數是string的時候不會進來
            try
            {
                Models.BookMangementService bookMangementService = new Models.BookMangementService();
                Models.BookData bookData = bookMangementService.GetBookDetails(BookId).FirstOrDefault();
                return View("Detail", bookData);
            }
            catch (Exception ex)
            {
                return View("Index");
            }
        }
        /// <summary>
        /// 修改圖書的畫面
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateBook(string BookId)
        {
            Models.BookMangementService bookMangementService = new Models.BookMangementService();
            Models.BookData bookData = bookMangementService.GetBookData(BookId).FirstOrDefault();
            return View(bookData);
        }
        /// <summary>
        /// 修改圖書存檔
        /// </summary>
        /// <param name="bookData"></param>
        /// <returns></returns>
        [HttpPost]    //**會和使用者直接接觸的Action參數型別用string是不是比較好?
        public ActionResult UpdateBook(Models.BookData bookData)
        {
            Models.BookMangementService bookMangementService = new Models.BookMangementService();
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime.Parse(bookData.BookBoughtDate);
                    bookMangementService.UpdateBookData(bookData);
                    return RedirectToAction("Detail", new { bookData.BookId });

                }catch(Exception ex)
                {
                    Response.Write("<script language=javascript>alert('日期格式錯誤')</script>");
                }
            }
            return View(bookData);
        }
    }
}