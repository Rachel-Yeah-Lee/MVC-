using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkShop4_MVC.Models
{
    public class BookMangementService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }
        /// <summary>
        /// 依照條件從資料庫取得書籍狀態資料
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public List<Models.SearchBookStatusResult> GetSearchResultByCondition(Models.SearchBookStatusArg arg)
        {   //**主體資料表BOOK_DATA, BOOK_CLASS, BOOK_CODE，用INNER JOIN 而 MEMBER_M 是依附的資料表，所以用LEFT JOIN，MEMBER_M是依附的資料表的原因是，可以看到如果書籍狀態是'可以借出'，借閱人的欄位是空的，That's，就算沒有借閱人資料還是要顯示出來
            DataTable dt = new DataTable();
            string sql = @"SELECT bd.BOOK_ID AS BookId,bc.BOOK_CLASS_NAME AS 圖書類別,bd.BOOK_NAME AS 書名,bd.BOOK_BOUGHT_DATE AS 購書日期,
	                       bc1.CODE_NAME AS 借閱狀態,ISNULL(mm.USER_ENAME,'')AS 借閱人
                           FROM BOOK_DATA bd
	                       INNER JOIN BOOK_CLASS bc ON bd.BOOK_CLASS_ID = bc.BOOK_CLASS_ID
	                       LEFT JOIN MEMBER_M mm ON mm.USER_ID = bd.BOOK_KEEPER
	                       INNER JOIN BOOK_CODE bc1 ON bc1.CODE_ID=bd.BOOK_STATUS AND bc1.CODE_TYPE='BOOK_STATUS'
                           WHERE (bd.BOOK_NAME LIKE '%' + @BOOK_NAME + '%' OR @BOOK_NAME='') AND
                                 (bc.BOOK_CLASS_NAME = @BOOK_CLASS_NAME OR @BOOK_CLASS_NAME ='') AND
                                 (mm.USER_ENAME = @USER_ENAME OR @USER_ENAME = '') AND
                                 (bc1.CODE_NAME = @CODE_NAME OR @CODE_NAME = '')
                           ORDER BY 購書日期";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BOOK_NAME", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BOOK_CLASS_NAME", arg.BookClass == null ? string.Empty : arg.BookClass));
                cmd.Parameters.Add(new SqlParameter("@USER_ENAME", arg.BookKeeper == null ? string.Empty : arg.BookKeeper));
                cmd.Parameters.Add(new SqlParameter("@CODE_NAME", arg.BookStatus == null ? string.Empty : arg.BookStatus));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapSearchBookStatusToList(dt);
        }
        /// <summary>
        /// Map查詢資料進List
        /// </summary>
        /// <param name="BookStatusData"></param>
        /// <returns></returns>
        private List<SearchBookStatusResult> MapSearchBookStatusToList(DataTable BookStatusData)
        {
            List<SearchBookStatusResult> result = new List<SearchBookStatusResult>();
            foreach (DataRow row in BookStatusData.Rows)
            {
                result.Add(new SearchBookStatusResult()
                {
                    BookId = row["BookId"].ToString(),
                    BookName = row["書名"].ToString(),
                    BookClass = row["圖書類別"].ToString(),
                    BookKeeper = row["借閱人"].ToString(),
                    BookStatus = row["借閱狀態"].ToString(),
                    BoughtDate = Convert.ToDateTime(row["購書日期"]).ToString("yyyy/MM/dd")  //**或許可以在sql查詢語法做資料轉型在邏輯的部份的時候會比較單純，單純做邏輯
                });
            }
            return result;
        }
        /// <summary>
        /// 從資料庫取得書籍類別(BOOK_CLASS_NAME)
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllBookClassName()
        {
            DataTable dt = new DataTable();      // instantiate DataTable
            string sql = @"SELECT BOOK_CLASS_NAME
                           FROM BOOK_CLASS
                           ORDER BY BOOK_CLASS_NAME";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            { //create the connection
                conn.Open();   //open the connection
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);   //fill the datatable
                conn.Close();
            }
            return this.MapBookClassNameToList(dt);
        }
        /// <summary>
        /// 將書籍資料從資料表換成清單的型式(map table to list)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SelectListItem> MapBookClassNameToList(DataTable dt)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)    //derive the data form table by row
            {
                items.Add(new SelectListItem()
                { //add item into <SelectListItem> list
                    Text = row["BOOK_CLASS_NAME"].ToString(),
                    Value = row["BOOK_CLASS_NAME"].ToString()
                });
            }
            return items;
        }
        /// <summary>
        /// 取得所有可能的書籍狀態資料
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllBookStatus()
        {
            DataTable dt = new DataTable();    //**因為這邊要做下拉選單的ViewBag，會有Text和Value，所以可以把'狀態名稱'放在Text，'狀態Id'放在Value
            string cmdstr = @"SELECT CODE_NAME
                              FROM BOOK_CODE
                              WHERE CODE_TYPE='BOOK_STATUS'";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))  //**可以把這邊弄成一個方法
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
            }
            return this.MapAllBookStatusToList(dt);

        }
        /// <summary>
        /// 將書籍狀態從資料表轉換成清單(map table to list)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SelectListItem> MapAllBookStatusToList(DataTable dt)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new SelectListItem()
                {
                    Text = row["CODE_NAME"].ToString(),
                    Value = row["CODE_NAME"].ToString()
                });
            }
            return items;
        }
        /// <summary>
        /// 取得借閱人中文姓名資料
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllMemberName()
        {
            DataTable dt = new DataTable();
            string cmdstr = @"SELECT USER_ENAME, USER_CNAME
                              FROM MEMBER_M";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return this.MapMemberCNameToList(dt);
        }
        /// <summary>
        /// 將借閱人資料轉換成清單(map table to list)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SelectListItem> MapMemberCNameToList(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["USER_CNAME"].ToString() + "-" + row["USER_ENAME"].ToString(),  //Text是給user看的
                    Value = row["USER_ENAME"].ToString()    //Value是給電腦讀的
                });
            }
            return result;
        }
        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <param name="arg">asss</param>
        /// <returns></returns>
        public void InsertBookData(Models.AddBookArg arg)
        {
            string cmdstr = @"INSERT INTO BOOK_DATA
                              ([BOOK_NAME],[BOOK_CLASS_ID],[BOOK_AUTHOR],[BOOK_BOUGHT_DATE]
                               ,[BOOK_PUBLISHER],[BOOK_NOTE],[BOOK_STATUS],[BOOK_KEEPER]
                               ,[BOOK_AMOUNT],[CREATE_DATE],[CREATE_USER],[MODIFY_DATE]
                               ,[MODIFY_USER])
                              VALUES(
                               @BookName,@BookClassId,@BookAuthor,@BookBoughtDate
                               ,@BookPublisher,@BookNote,@BookStatus,@BookKeeper
                               ,@BookAmount,@CreateDate,@CreateUser,@ModifyDate
                               ,@ModifyUser)";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                DateTime SysTime = DateTime.Now;
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", GetClassIdByName(arg.BookClass)));
                cmd.Parameters.Add(new SqlParameter("@BookAuthor", arg.BookAuthor == null ? (Object)DBNull.Value : arg.BookAuthor));
                cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", arg.BookBoughtDate == null ? (Object)DBNull.Value : arg.BookBoughtDate));
                cmd.Parameters.Add(new SqlParameter("@BookPublisher", arg.BookPublisher == null ? (Object)DBNull.Value : arg.BookPublisher));
                cmd.Parameters.Add(new SqlParameter("@BookNote", arg.BookNote == null ? (Object)DBNull.Value : arg.BookNote));
                cmd.Parameters.Add(new SqlParameter("@BookStatus", GetBookStatusIdByName("可以借出"))); //
                cmd.Parameters.Add(new SqlParameter("@BookKeeper", (Object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@BookAmount", (Object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@CreateDate", SysTime));
                cmd.Parameters.Add(new SqlParameter("@CreateUser", "admin"));
                cmd.Parameters.Add(new SqlParameter("@ModifyDate", SysTime));
                cmd.Parameters.Add(new SqlParameter("@ModifyUser", "admin"));
                cmd.ExecuteNonQuery();     //**如果書本簡介超過資料庫中設定的長度 會跳錯誤所以要注意長度限制
                conn.Close();
            }

        }
        /// <summary>
        /// 由書籍狀態名稱找書籍狀態代號
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private string GetBookStatusIdByName(string status)
        {
            DataTable dt = new DataTable();
            string cmdstr = @"SELECT CODE_ID
                             FROM BOOK_CODE
                             WHERE CODE_NAME = @CodeName  ";  
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                cmd.Parameters.Add(new SqlParameter("@CodeName", status));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
            }
            return dt.Rows[0]["CODE_ID"].ToString();        //**在做陣列值第零列的時候要先'判斷'Rows.Count!=0，否則程式可能會整個當掉
        }                                                   //**情況會發生在和資料庫取資料時可能突然斷線或沒取到資料時，Rows[0]會找不到索引
        /// <summary>
        /// 由圖書類別名稱找類別代號
        /// </summary>
        /// <param name="BookClassName"></param>
        /// <returns></returns>
        private string GetClassIdByName(string BookClassName)
        {
            DataTable dt = new DataTable();
            string cmdstr = @"SELECT BOOK_CLASS_ID,BOOK_CLASS_NAME
                             FROM BOOK_CLASS 
                             WHERE BOOK_CLASS_NAME=@BookClassName";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                cmd.Parameters.Add(new SqlParameter("@BookClassName", BookClassName));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
            }
            return dt.Rows[0]["BOOK_CLASS_ID"].ToString();    //
        }
        /// <summary>
        /// 刪除書籍
        /// </summary>
        /// <param name="BookId"></param>
        public bool DeleteBookDataById(string BookId)

        {
            DataTable dt = new DataTable();
            string selectStr = @"SELECT * FROM BOOK_DATA WHERE BOOK_ID=@BookId AND BOOK_STATUS='B' AND BOOK_STATUS='B' ";
            string deleteStr = @"DELETE FROM BOOK_DATA WHERE BOOK_ID=@BookId ";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand selectCmd = new SqlCommand(selectStr, conn);
                selectCmd.Parameters.Add(new SqlParameter("@BookId", BookId));
                SqlDataAdapter adapter = new SqlDataAdapter(selectCmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    SqlCommand deleteCmd = new SqlCommand(deleteStr, conn);
                    deleteCmd.Parameters.Add(new SqlParameter("@BookId", BookId));
                    deleteCmd.ExecuteNonQuery();
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Get BookData By Id
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public BookData GetBookDataById(string BookId)
        {
            DataTable dt = new DataTable();            //**最好不要SELECT *，當有敏感或機密性欄位有可能有被外面存取的危險
            string cmdstr = @"SELECT *                   
                              FROM BOOK_DATA bd
                              WHERE bd.BOOK_ID = @BookId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {   //using結束後區域內的變數會被收回，但是期間內做的動作都還存在，that's，與資料庫還在連線中
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", BookId));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);  //sqlDataAdapter是一個連接資料庫的橋接器，給他sql命令和連接的屬性內容後，他會幫忙做和資料庫的底層溝通
                adapter.Fill(dt);
                conn.Close();
            }
            return MapBookDataToList(dt);
        }
        /// <summary>
        /// Map BookData To List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private BookData MapBookDataToList(DataTable dt)
        {
            BookData bd = new BookData();
            foreach (DataRow row in dt.Rows)
            {
                bd.BookId = row["BOOK_ID"].ToString();
                bd.BookName = row["BOOK_NAME"].ToString();
                bd.BookClassId = row["BOOK_CLASS_ID"].ToString();
                bd.BookAuthor = row["BOOK_AUTHOR"].ToString();
                bd.BookBoughtDate = row["BOOK_BOUGHT_DATE"].ToString();
                bd.BookPublisher = row["BOOK_PUBLISHER"].ToString();
                bd.BookNote = row["BOOK_NOTE"].ToString();
                bd.BookStatus = row["BOOK_STATUS"].ToString();
                bd.BookKeeper = row["BOOK_KEEPER"].ToString();
                bd.BookAmount = row["BOOK_AMOUNT"].ToString();
                bd.CreateDate = row["CREATE_DATE"].ToString();
                bd.CreateUser = row["CREATE_USER"].ToString();
                bd.ModifyDate = row["MODIFY_DATE"].ToString();
                bd.ModifyUser = row["MODIFY_USER"].ToString();
            }
            return bd;
        }
    }
}