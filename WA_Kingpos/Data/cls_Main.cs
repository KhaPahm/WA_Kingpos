using System.Data;
using System.Data.SqlClient;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using KingproServer;

namespace WA_Kingpos.Data
{
    public class cls_Main
    {
        /// <summary>
        /// Log loi khi chay
        /// </summary>
        public static string sFilePath = "./App_Data/" + "Log.txt";
        /// <summary>
        /// Report
        /// </summary>
        public static string sFilePathReport = "./Reports/";

        /// <summary>
        /// Gi log loi khi chay
        /// </summary>
        /// <param name="sFilePath"></param>
        /// <param name="Title"></param>
        /// <param name="Body"></param>
        public static void WriterLog(string sFilePath, string Title, string Body, string sConnectionString_live)
        {
            string sFilePath_New = "Log" + DateTime.Now.ToString("yyyyMMdd");
            sFilePath = sFilePath.Replace("Log", sFilePath_New);

            FileStream fs;
            if (!File.Exists(sFilePath))//kiểm tra nếu chưa có file Pass.txt thì tạo ra file Pass.txt
            {
                fs = new FileStream(sFilePath, FileMode.Create);//Tạo file mới tên là Pass.txt
            }
            else
            {
                fs = new FileStream(sFilePath, FileMode.Append);
            }

            StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8);
            sWriter.WriteLine("---------------------------");
            sWriter.WriteLine("DATE: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            sWriter.WriteLine(sConnectionString_live);
            sWriter.WriteLine(Title);
            sWriter.WriteLine(Body);
            //sWriter.WriteLine("---------------------------");
            sWriter.Flush();
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// Ghi log API de doi soat request va response
        /// </summary>
        /// <param name="sFilePath"></param>
        /// <param name="Title"></param>
        /// <param name="Body"></param>
        public static void WriterLog_API(string sFilePath, string Title, string Body, string sKey)
        {
            string sFilePath_New = "API" + DateTime.Now.ToString("yyyyMMdd");
            sFilePath = sFilePath.Replace("Log", sFilePath_New);

            FileStream fs;
            if (!File.Exists(sFilePath))//kiểm tra nếu chưa có file Pass.txt thì tạo ra file Pass.txt
            {
                fs = new FileStream(sFilePath, FileMode.Create);//Tạo file mới tên là Pass.txt
            }
            else
            {
                fs = new FileStream(sFilePath, FileMode.Append);
            }

            StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8);
            sWriter.WriteLine("---------------------------");
            sWriter.WriteLine("DATE: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + sKey);
            //sWriter.WriteLine(sConnectionString_live);
            sWriter.WriteLine(Title);
            sWriter.WriteLine(Body);
            //sWriter.WriteLine("---------------------------");
            sWriter.Flush();
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// Thay các dấu ' thành '' dùng trong các lệnh SQL - mục đích tránh lổi
        /// </summary>
        /// <param name="Text">Chuổi ký tự không dấu</param>
        /// <returns></returns>
        public static string SQLString(string Text)
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = "";
            }
            Text = Text.Trim();
            return "'" + Text.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Thay các dấu ' thành '' dùng trong các lệnh SQL - mục đích tránh lổi
        /// </summary>
        /// <param name="Text">Chuổi ký tự có dấu</param>
        /// <returns></returns>
        public static string SQLStringUnicode(string Text)
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = "";
            }
            return "N" + SQLString(Text);
        }

        /// <summary>
        /// Chuyển dữ liệt sang kiểu bit để lưu xuống server
        /// </summary>
        /// <param name="bNumber"></param>
        /// <returns></returns>
        public static string SQLBit(Boolean bNumber)
        {
            if (bNumber==null)
            {
                bNumber = false;
            }
            if (bNumber)
            {
                return "'" + "1" + "'";
            }
            return "'" + "0" + "'";
        }

        ///<summary>
        ///Thực thi một chuỗi SQL trả về 1 Datatable dung no lock
        ///</summary>
        public static DataTable ReturnDataTable_NoLock(string sSQL, string sConnectionString_live)
        {
            //Table tra về
            DataTable dt = new DataTable();

            //Set câu lệnh sql
            string strSQL = "";
            strSQL += ";SET ANSI_WARNINGS OFF; " + "\n";
            strSQL += ";SET DATEFORMAT DMY; " + "\n";
            strSQL += ";SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" + "\n";
            strSQL += sSQL;

            //Thực hiện câu lệnh sql
            SqlConnection conn = new SqlConnection(KingproServer.KingproServer.sConvert(sConnectionString_live));            
            SqlCommand cmd = new SqlCommand(strSQL, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                conn.Open();
                cmd.CommandTimeout = 0;
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 1205)
                {
                    // Deadlock 	
                    Thread.Sleep(1000);
                    try
                    {
                        cmd.CommandTimeout = 0;
                        da.Fill(dt);
                        conn.Close();
                        return dt;
                    }
                    catch (SqlException ex1)
                    {
                        conn.Close();
                        WriterLog(sFilePath, "ReturnDataTable_NoLock", strSQL + "-" + ex1.ToString(), sConnectionString_live);
                        return dt;
                    }
                }
                else
                {
                    conn.Close();
                    WriterLog(sFilePath, "ReturnDataTable_NoLock", strSQL + "-" + ex.ToString(), sConnectionString_live);
                    return dt;
                }
            }
        }

        ///<summary>
        ///Thực thi một chuỗi SQL trả về 1 Datatable
        ///</summary>
        public static DataTable ReturnDataTable(string sSQL, string sConnectionString_live)
        {
            //Table tra về
            DataTable dt = new DataTable();

            //Set câu lệnh sql
            string strSQL = "";
            strSQL += ";SET ANSI_WARNINGS OFF; " + "\n";
            strSQL += ";SET DATEFORMAT DMY; " + "\n";
            strSQL += ";SET TRANSACTION ISOLATION LEVEL READ COMMITTED;" + "\n";
            strSQL += sSQL;
            //Thực hiện câu lệnh sql
            SqlConnection conn = new SqlConnection(KingproServer.KingproServer.sConvert(sConnectionString_live));
            SqlCommand cmd = new SqlCommand(strSQL, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                conn.Open();
                cmd.CommandTimeout = 0;
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 1205)
                {
                    // Deadlock 	
                    Thread.Sleep(1000);
                    try
                    {
                        cmd.CommandTimeout = 0;
                        da.Fill(dt);
                        conn.Close();
                        return dt;
                    }
                    catch (SqlException ex1)
                    {
                        conn.Close();
                        WriterLog(sFilePath, "ReturnDataTable", strSQL + "-" + ex1.ToString(), sConnectionString_live);
                        return dt;
                    }
                }
                else
                {
                    conn.Close();
                    WriterLog(sFilePath, "ReturnDataTable", strSQL + "-" + ex.ToString(), sConnectionString_live);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Thực thi một chuỗi SQL có dùng Transaction
        /// </summary>
        /// <param name="sSQL">Câu SQL</param>
        /// <returns>Trả về True nếu thực thi chuỗi SQL thành công, ngược lại trả về False</returns>
        public static Boolean ExecuteSQL(string sSQL, string sConnectionString_live)
        {
            //Set câu lệnh sql
            string strSQL = "";
            if (sSQL.Trim() == "")
            {
                return false;
            }
            strSQL += ";SET ANSI_WARNINGS OFF; " + "\n";
            strSQL += ";SET DATEFORMAT DMY ;" + "\n";
            strSQL += ";SET TRANSACTION ISOLATION LEVEL READ COMMITTED;" + "\n";
            strSQL += sSQL;
            //Thực hiện câu lệnh sql
            SqlConnection conn = new SqlConnection(KingproServer.KingproServer.sConvert(sConnectionString_live));
            SqlCommand cmd = new SqlCommand(strSQL, conn);
            SqlTransaction? trans = null;
            try
            {
                conn.Open();
                trans = conn.BeginTransaction();
                cmd.CommandTimeout = 0;
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                trans.Commit();
                conn.Close();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 1205)
                {
                    // Deadlock 
                    trans.Rollback();
                    Thread.Sleep(1000);
                    try
                    {
                        trans = conn.BeginTransaction();
                        cmd.CommandTimeout = 0;
                        cmd.Transaction = trans;
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        conn.Close();
                        return true;
                    }
                    catch (SqlException ex1)
                    {
                        trans.Rollback();
                        conn.Close();
                        WriterLog(sFilePath, "ExecuteSQL", strSQL + "-" + ex1.ToString(), sConnectionString_live);
                        return false;
                    }
                }
                else
                {
                    trans.Rollback();
                    conn.Close();
                    WriterLog(sFilePath, "ExecuteSQL", strSQL + "-" + ex.ToString(), sConnectionString_live);
                    return false;
                }
            }
        }

        /// <summary>
        /// Chuyển đổi Binary sang Base64
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BinaryToBase64(byte[] bytes)
        {
            try
            {
                string result = "data:image/png;base64,";
                result += Convert.ToBase64String(bytes);
                if (result == "data:image/png;base64,")
                    result = "0x";
                return result;
            }
            catch (Exception)
            {
                return "0x";
            }
        }

        /// <summary>
        /// Sinh số ngẩu nhiên
        /// </summary>
        /// <returns></returns>
        public static string RandomNumber()
        {
            var rand = new Random();
            return rand.Next(1, 2000000000).ToString("0000000000");
        }

        /// <summary>
        /// Sinh chuổi ngẩu nhiên
        /// </summary>
        /// <returns></returns>
        public static string RandomString()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        /// <summary>
        /// Ghi log sang database
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="ClientIP"></param>
        /// <param name="Endpoint"></param>
        /// <param name="Method"></param>
        /// <param name="key"></param>
        /// <param name="Values"></param>
        /// <param name="sConnectionString_live"></param>
        public static void InsertLog(string LoginName, string ClientIP, string Endpoint, string Method, string key, string Values)
        {
            string sqlLog = "EXEC [Proc_SYS_Auditting_Log_Insert] " + "\n";
            sqlLog += cls_Main.SQLStringUnicode(LoginName) + ",";
            sqlLog += cls_Main.SQLStringUnicode(ClientIP) + ",";
            sqlLog += cls_Main.SQLStringUnicode("API") + ",";
            sqlLog += cls_Main.SQLStringUnicode(Endpoint) + ",";
            sqlLog += cls_Main.SQLStringUnicode(Method) + ",";
            sqlLog += cls_Main.SQLStringUnicode(key) + ",";
            sqlLog += cls_Main.SQLStringUnicode(Values) + "\n";
            cls_Main.ExecuteSQL(sqlLog, cls_ConnectDB.GetConnect_log());
        }

        public static bool ExecuteSQL(string sSQL, string sConnectionString_live, List<SqlParameter> parameters)
        {
            if (string.IsNullOrWhiteSpace(sSQL))
                return false;

            string strSQL = "";
            strSQL += ";SET ANSI_WARNINGS OFF; \n";
            strSQL += ";SET DATEFORMAT DMY ;\n";
            strSQL += ";SET TRANSACTION ISOLATION LEVEL READ COMMITTED;\n";
            strSQL += sSQL;

            SqlConnection conn = new SqlConnection(KingproServer.KingproServer.sConvert(sConnectionString_live));
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                SqlTransaction trans = null;

                if (parameters != null)
                {
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);
                }

                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    cmd.CommandTimeout = 0;

                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    if (trans != null) trans.Rollback();
                    WriterLog("ExecuteSQL_Error", "ExecuteSQL", strSQL + "-" + ex.ToString(), sConnectionString_live);
                    return false;
                }
            }
        }


    }
}
