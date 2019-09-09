using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookManagement.Models
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

            string sql = @"SELECT bd.BOOK_ID AS BookId,bc.BOOK_CLASS_NAME AS 圖書類別,bd.BOOK_NAME AS 書名,CONVERT(VARCHAR(10),bd.BOOK_BOUGHT_DATE,111) AS 購書日期,
	                       bc1.CODE_NAME AS 借閱狀態,ISNULL(mm.USER_ENAME,'')AS 借閱人
                           FROM BOOK_DATA bd
	                       INNER JOIN BOOK_CLASS bc ON bd.BOOK_CLASS_ID = bc.BOOK_CLASS_ID
	                       LEFT JOIN MEMBER_M mm ON mm.USER_ID = bd.BOOK_KEEPER
	                       INNER JOIN BOOK_CODE bc1 ON bc1.CODE_ID=bd.BOOK_STATUS AND bc1.CODE_TYPE='BOOK_STATUS'
                           WHERE (bd.BOOK_NAME LIKE '%' + @BOOK_NAME + '%' OR @BOOK_NAME='') AND
                                 (bc.BOOK_CLASS_ID = @BOOK_CLASS_ID OR @BOOK_CLASS_ID ='') AND
                                 (mm.USER_ID = @USER_ID OR @USER_ID = '') AND
                                 (bc1.CODE_ID = @CODE_ID OR @CODE_ID = '')
                           ORDER BY 購書日期";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BOOK_NAME", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BOOK_CLASS_ID", arg.BookClass == null ? string.Empty : arg.BookClass));
                cmd.Parameters.Add(new SqlParameter("@USER_ID", arg.BookKeeperID == null ? string.Empty : arg.BookKeeperID));
                cmd.Parameters.Add(new SqlParameter("@CODE_ID", arg.BookStatus == null ? string.Empty : arg.BookStatus));
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
                    BoughtDate = row["購書日期"].ToString() //**或許可以在sql查詢語法做資料轉型在邏輯的部份的時候會比較單純，單純做邏輯
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
            string sql = @"SELECT BOOK_CLASS_NAME,BOOK_CLASS_ID
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
                    Value = row["BOOK_CLASS_ID"].ToString()
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
            string cmdstr = @"SELECT CODE_NAME,CODE_ID
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
                    Value = row["CODE_ID"].ToString()
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
            string cmdstr = @"SELECT USER_ID,USER_ENAME, USER_CNAME
                              FROM MEMBER_M";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open(); //不要Open()似乎也可以
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
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
                    Value = row["USER_ID"].ToString()    //Value是給電腦讀的
                });
            }
            return result;
        }
        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <param name="arg">asss</param>
        /// <returns></returns>
        public int InsertBookData(Models.AddBookArg arg)
        {
            int BookID = -1;
            try                                                     //**如果日期是空值的話雖然可以存入資料庫
            {                                                       //**但是在查詢的時候會沒辦法從DB(NULL)轉換成字串，
                DateTime date = DateTime.Parse(arg.BookBoughtDate); //**所以還是要 1.判斷日期不可以是空值 或是 2.當日期是空值的時候以空白字串存入資料庫
                string cmdstr = @"INSERT INTO BOOK_DATA           
                              ([BOOK_NAME],[BOOK_CLASS_ID],[BOOK_AUTHOR],[BOOK_BOUGHT_DATE]
                               ,[BOOK_PUBLISHER],[BOOK_NOTE],[BOOK_STATUS],[BOOK_KEEPER]
                               ,[BOOK_AMOUNT],[CREATE_DATE],[CREATE_USER],[MODIFY_DATE]
                               ,[MODIFY_USER])
                              VALUES(
                               @BookName,@BookClassId,@BookAuthor,@BookBoughtDate
                               ,@BookPublisher,@BookNote,@BookStatus,@BookKeeper
                               ,@BookAmount,@CreateDate,@CreateUser,@ModifyDate
                               ,@ModifyUser)
                              SELECT SCOPE_IDENTITY()";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    DateTime SysTime = DateTime.Now;
                    SqlCommand cmd = new SqlCommand(cmdstr, conn);
                    cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName));
                    cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClass));
                    cmd.Parameters.Add(new SqlParameter("@BookAuthor", arg.BookAuthor == null ? (Object)DBNull.Value : arg.BookAuthor));
                    cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", arg.BookBoughtDate == null ? (Object)DBNull.Value : arg.BookBoughtDate));
                    cmd.Parameters.Add(new SqlParameter("@BookPublisher", arg.BookPublisher == null ? (Object)DBNull.Value : arg.BookPublisher));
                    cmd.Parameters.Add(new SqlParameter("@BookNote", arg.BookNote == null ? (Object)DBNull.Value : arg.BookNote));
                    cmd.Parameters.Add(new SqlParameter("@BookStatus", 'A')); //書籍狀態是'可借出'
                    cmd.Parameters.Add(new SqlParameter("@BookKeeper", (Object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@BookAmount", (Object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@CreateDate", SysTime));
                    cmd.Parameters.Add(new SqlParameter("@CreateUser", "admin"));
                    cmd.Parameters.Add(new SqlParameter("@ModifyDate", SysTime));
                    cmd.Parameters.Add(new SqlParameter("@ModifyUser", "admin"));
                    SqlTransaction Tran = conn.BeginTransaction();
                    cmd.Transaction = Tran;
                    try
                    {
                        BookID = Convert.ToInt32(cmd.ExecuteScalar());      //**BookID可以用來跳到編輯頁面
                        Tran.Commit();
                    }
                    catch (Exception)
                    {
                        Tran.Rollback();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                    //**如果書本簡介超過資料庫中設定的長度 會跳錯誤所以要注意長度限制
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return BookID;
        }
        /// <summary>
        /// 由書籍類別代號找類別名稱
        /// </summary>
        /// <param name="BookClassId"></param>
        /// <returns></returns>
        private string GetClassNameById(string BookClassId)
        {
            DataTable dt = new DataTable();
            string cmdstr = @"SELECT BOOK_CLASS_ID,BOOK_CLASS_NAME
                             FROM BOOK_CLASS 
                             WHERE BOOK_CLASS_ID=@BookClassId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                cmd.Parameters.Add(new SqlParameter("@BookClassId", BookClassId));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
            }
            try
            {
                return dt.Rows[0]["BOOK_CLASS_NAME"].ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string GetBookStatusNameById(string BookStatusId)
        {
            DataTable dt = new DataTable();
            string cmdstr = @"SELECT CODE_NAME
                             FROM BOOK_CODE
                             WHERE CODE_ID = @CodeId AND CODE_TYPE='BOOK_STATUS'";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, conn);
                cmd.Parameters.Add(new SqlParameter("@CodeId", BookStatusId));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
            }   //**這裡的try寫法要改一下
            try
            {
                return dt.Rows[0]["CODE_NAME"].ToString();
            }
            catch (Exception ex) { throw; }
        }
        /// <summary>
        /// 刪除書籍
        /// </summary>
        /// <param name="BookId"></param>
        public bool DeleteBookDataById(string BookId)

        {
            DataTable dt = new DataTable();
            string selectStr = @"SELECT * FROM BOOK_DATA WHERE BOOK_ID=@BookId AND BOOK_STATUS='B' OR BOOK_STATUS='C' ";
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
                    SqlTransaction Tran = conn.BeginTransaction();
                    deleteCmd.Transaction = Tran;
                    try
                    {
                        deleteCmd.ExecuteNonQuery();
                        Tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        Tran.Rollback();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Get BookData By Id抓取圖書明細
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public List<BookData> GetBookDetails(string BookId)
        {
            DataTable dt = new DataTable();            //**最好不要SELECT *，當有敏感或機密性欄位有可能有被外面存取的危險
            string cmdstr = @"SELECT [BOOK_ID], [BOOK_NAME], [BOOK_CLASS_ID],[BOOK_AUTHOR],CONVERT(VARCHAR(10),bd.BOOK_BOUGHT_DATE,111) AS BOOK_BOUGHT_DATE
                                    ,[BOOK_PUBLISHER],[BOOK_NOTE],[BOOK_STATUS],[BOOK_KEEPER]
                                    ,[BOOK_AMOUNT],[CREATE_DATE] ,[CREATE_USER] ,[MODIFY_DATE]
                                    ,[MODIFY_USER]         
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
        /// 修改圖書(抓取預設值)
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public List<BookData> GetBookData(string BookId)
        {
            DataTable dt = new DataTable();            //**最好不要SELECT *，當有敏感或機密性欄位有可能有被外面存取的危險
            string cmdstr = @"SELECT [BOOK_ID], [BOOK_NAME], [BOOK_CLASS_ID],[BOOK_AUTHOR],CONVERT(VARCHAR(10),bd.BOOK_BOUGHT_DATE,126) AS BOOK_BOUGHT_DATE
                                    ,[BOOK_PUBLISHER],[BOOK_NOTE],[BOOK_STATUS],[BOOK_KEEPER]
                                    ,[BOOK_AMOUNT],[CREATE_DATE] ,[CREATE_USER] ,[MODIFY_DATE]
                                    ,[MODIFY_USER]         
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
        private List<BookData> MapBookDataToList(DataTable dt)
        {
            List<BookData> bd = new List<BookData>();
            foreach (DataRow row in dt.Rows)
            {
                bd.Add(new BookData()
                {
                    BookId = row["BOOK_ID"].ToString(),
                    BookName = row["BOOK_NAME"].ToString(),
                    BookClassId = row["BOOK_CLASS_ID"].ToString(),
                    BookAuthor = row["BOOK_AUTHOR"].ToString(),
                    BookBoughtDate = row["BOOK_BOUGHT_DATE"].ToString(),
                    BookPublisher = row["BOOK_PUBLISHER"].ToString(),
                    BookNote = row["BOOK_NOTE"].ToString(),
                    BookStatusId = row["BOOK_STATUS"].ToString(),
                    BookKeeper = row["BOOK_KEEPER"].ToString(),
                    BookAmount = row["BOOK_AMOUNT"].ToString(),
                    CreateDate = row["CREATE_DATE"].ToString(),
                    CreateUser = row["CREATE_USER"].ToString(),
                    ModifyDate = row["MODIFY_DATE"].ToString(),
                    ModifyUser = row["MODIFY_USER"].ToString(),
                    BookClassName = GetClassNameById(row["BOOK_CLASS_ID"].ToString()),
                    BookStatusName = GetBookStatusNameById(row["BOOK_STATUS"].ToString())
                });
            }
            return bd;
        }
        /// <summary>
        /// 修改圖書(儲存)
        /// </summary>
        /// <param name="bookData"></param>
        public void UpdateBookData(BookData bookData)   //***這個功能有問題
        {
            string sqlstr = @"UPDATE BOOK_DATA
                           SET BOOK_NAME=@BookName,
                               BOOK_BOUGHT_DATE=@BookBoughtDate,
                               BOOK_CLASS_ID=@BookClassId,
                               BOOK_STATUS=@CodeName,
                               BOOK_KEEPER=@KeeperId,
                               BOOK_AUTHOR=@BookAuthor,
                               BOOK_PUBLISHER=@BookPublisher,
                               BOOK_NOTE=@BookNote
                           WHERE BOOK_ID=@BookID";
            //借閱紀錄新增
            if (!string.IsNullOrEmpty(bookData.BookKeeper))
            {
                sqlstr += @"INSERT INTO BOOK_LEND_RECORD(BOOK_ID,KEEPER_ID,LEND_DATE)
                         VALUES(@BookID,@KeeperId,@LendDate)";
            }
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlstr, conn);
                cmd.Parameters.Add(new SqlParameter("@BookID", bookData.BookId));
                cmd.Parameters.Add(new SqlParameter("@BookName", bookData.BookName));
                cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", bookData.BookBoughtDate));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", bookData.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@CodeName", bookData.BookStatusId));
                cmd.Parameters.Add(new SqlParameter("@KeeperId", bookData.BookKeeper==null?string.Empty:bookData.BookKeeper));
                cmd.Parameters.Add(new SqlParameter("@BookAuthor", bookData.BookAuthor));
                cmd.Parameters.Add(new SqlParameter("@BookPublisher", bookData.BookPublisher));
                cmd.Parameters.Add(new SqlParameter("@BookNote", bookData.BookNote));
                cmd.Parameters.Add(new SqlParameter("@LendDate", DateTime.Now));
                SqlTransaction Tran = conn.BeginTransaction();
                cmd.Transaction = Tran;
                try
                {
                    cmd.ExecuteNonQuery();
                    Tran.Commit();
                }
                catch (Exception ex)
                {
                    Tran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}