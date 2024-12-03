using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Etax_Noble_INET
{
    public static class Business
    {
        private static string _TEST_RECORD = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Env:TestRecord"]) ? ""
            : System.Configuration.ConfigurationManager.AppSettings["Env:TestRecord"].ToString();

        private static LogFile _logFile;
        public static void StartRun(ICON.Framework.Provider.DBHelper db, string[] args, LogFile log)
        {
            _logFile = log;
            string taskType = "";

            if (args.Length > 0)
            {
                taskType = args[0].ToUpper();
            }
            else
            {
                taskType = "ICON";
            }

            //if (taskType == "SAP")
            //{
            //    // ไปดึงข้อมูลจาก view ที่ SAP B1 เตรียมข้อมูลไว้ให้ แล้วไปลงพักข้อมูลไว้ที่ table etax_rawData
            //    SAPData_Prepare();
            //}
            //else
            //{
            //    // ของ ICON จะดึงข้อมูลแล้วส่ง API INET เลย
            //    // ของ SAP B1 ให้ไปดึงข้อมูลที่ table etax_rawData
            //    ICONData_Prepare(db);
            //}

            string serviceType = System.Configuration.ConfigurationManager.AppSettings["serviceType"];
            if (serviceType == null || serviceType == "") serviceType = "ALL";
            serviceType = serviceType.ToUpper();

            if (serviceType == "REM" || serviceType == "ALL")
            {
                ICONData_Prepare(db);
            }

            if (serviceType == "SAP" || serviceType == "ALL")
            {
                SAPData_Prepare(db);
            }
        }

        public static void StartRun_CleanChangeOwner(ICON.Framework.Provider.DBHelper db, string[] args, LogFile log)
        {
            System.Data.DataTable dt = db.ExecuteDataTable(@"
select mig.ID, mig.ChangeOwner_No
	, mig.Customer_ID , mig.Customer_ID1 , mig.Customer_ID2 , mig.Customer_ID3 , mig.Customer_ID4 
	, mig.Customer_ID_Old , mig.Customer_ID1_Old , mig.Customer_ID2_Old , mig.Customer_ID3_Old , mig.Customer_ID4_Old 
	
    , t.New_Customer_ID1, t.New_Customer_ID2, t.New_Customer_ID3, t.New_Customer_ID4
    , t.Remove_Customer_ID1, t.Remove_Customer_ID2, t.Remove_Customer_ID3, t.Remove_Customer_ID4
from Sys_REM_ChangeContract_AddRemove_ViewForNoble t
	inner join [@Contract_ChangeOwner] mig on t.ChangeNumber = mig.ChangeOwner_No
");

            System.Data.IDbTransaction tran = db.BeginTransaction();
            //List<string> newCustomerID = new List<string>();

            try
            {
                #region Add

                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    //string[] oldCustomerIDArr = new string[5] { dr["Customer_ID_Old"].ToString(), dr["Customer_ID1_Old"].ToString(), dr["Customer_ID2_Old"].ToString(), dr["Customer_ID3_Old"].ToString(), dr["Customer_ID4_Old"].ToString() };
                //    //string newId = string.Join(" ", oldCustomerIDArr);

                //    // ลูกค้าที่เพิ่ม
                //    if (dr["Customer_ID"].ToString() != ""
                //        && dr["Customer_ID"].ToString() != dr["Customer_ID_Old"].ToString()
                //        && dr["Customer_ID"].ToString() != dr["Customer_ID1_Old"].ToString()
                //        && dr["Customer_ID"].ToString() != dr["Customer_ID2_Old"].ToString()
                //        && dr["Customer_ID"].ToString() != dr["Customer_ID3_Old"].ToString()
                //        && dr["Customer_ID"].ToString() != dr["Customer_ID4_Old"].ToString()
                //        )
                //    {
                //        db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID1 = '" + dr["Customer_ID"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //    }
                //}

                //// คนที่ 2
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID1"].ToString() != ""
                //        && dr["Customer_ID1"].ToString() != dr["Customer_ID_Old"].ToString()
                //        && dr["Customer_ID1"].ToString() != dr["Customer_ID1_Old"].ToString()
                //        && dr["Customer_ID1"].ToString() != dr["Customer_ID2_Old"].ToString()
                //        && dr["Customer_ID1"].ToString() != dr["Customer_ID3_Old"].ToString()
                //        && dr["Customer_ID1"].ToString() != dr["Customer_ID4_Old"].ToString()
                //        )
                //    {
                //        if (dr["New_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID1 = '" + dr["Customer_ID1"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID2 = '" + dr["Customer_ID1"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                //// คนที่ 3
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID2"].ToString() != ""
                //        && dr["Customer_ID2"].ToString() != dr["Customer_ID_Old"].ToString()
                //        && dr["Customer_ID2"].ToString() != dr["Customer_ID1_Old"].ToString()
                //        && dr["Customer_ID2"].ToString() != dr["Customer_ID2_Old"].ToString()
                //        && dr["Customer_ID2"].ToString() != dr["Customer_ID3_Old"].ToString()
                //        && dr["Customer_ID2"].ToString() != dr["Customer_ID4_Old"].ToString()
                //        )
                //    {
                //        if (dr["New_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID1 = '" + dr["Customer_ID2"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID2 = '" + dr["Customer_ID2"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID3"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID3 = '" + dr["Customer_ID2"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                //// คนที่ 4
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID3"].ToString() != ""
                //        && dr["Customer_ID3"].ToString() != dr["Customer_ID_Old"].ToString()
                //        && dr["Customer_ID3"].ToString() != dr["Customer_ID1_Old"].ToString()
                //        && dr["Customer_ID3"].ToString() != dr["Customer_ID2_Old"].ToString()
                //        && dr["Customer_ID3"].ToString() != dr["Customer_ID3_Old"].ToString()
                //        && dr["Customer_ID3"].ToString() != dr["Customer_ID4_Old"].ToString()
                //        )
                //    {
                //        if (dr["New_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID1 = '" + dr["Customer_ID3"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID2 = '" + dr["Customer_ID3"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID3"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID3 = '" + dr["Customer_ID3"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID4 = '" + dr["Customer_ID4"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                //// คนที่ 5
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID4"].ToString() != ""
                //        && dr["Customer_ID4"].ToString() != dr["Customer_ID_Old"].ToString()
                //        && dr["Customer_ID4"].ToString() != dr["Customer_ID1_Old"].ToString()
                //        && dr["Customer_ID4"].ToString() != dr["Customer_ID2_Old"].ToString()
                //        && dr["Customer_ID4"].ToString() != dr["Customer_ID3_Old"].ToString()
                //        && dr["Customer_ID4"].ToString() != dr["Customer_ID4_Old"].ToString()
                //        )
                //    {
                //        if (dr["New_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID1 = '" + dr["Customer_ID4"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID2 = '" + dr["Customer_ID4"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["New_Customer_ID3"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID3 = '" + dr["Customer_ID4"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set New_Customer_ID4 = '" + dr["Customer_ID4"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                #endregion

                #region Remove

                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID_Old"].ToString() != ""
                //        && dr["Customer_ID_Old"].ToString() != dr["Customer_ID"].ToString()
                //        && dr["Customer_ID_Old"].ToString() != dr["Customer_ID1"].ToString()
                //        && dr["Customer_ID_Old"].ToString() != dr["Customer_ID2"].ToString()
                //        && dr["Customer_ID_Old"].ToString() != dr["Customer_ID3"].ToString()
                //        && dr["Customer_ID_Old"].ToString() != dr["Customer_ID4"].ToString()
                //        )
                //    {
                //        db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID1 = '" + dr["Customer_ID_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //    }
                //}

                //// คนที่ 2
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID1_Old"].ToString() != ""
                //        && dr["Customer_ID1_Old"].ToString() != dr["Customer_ID"].ToString()
                //        && dr["Customer_ID1_Old"].ToString() != dr["Customer_ID1"].ToString()
                //        && dr["Customer_ID1_Old"].ToString() != dr["Customer_ID2"].ToString()
                //        && dr["Customer_ID1_Old"].ToString() != dr["Customer_ID3"].ToString()
                //        && dr["Customer_ID1_Old"].ToString() != dr["Customer_ID4"].ToString()
                //        )
                //    {
                //        if (dr["Remove_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID1 = '" + dr["Customer_ID1_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["Remove_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID2 = '" + dr["Customer_ID1_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                //// คนที่ 3
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID2_Old"].ToString() != ""
                //        && dr["Customer_ID2_Old"].ToString() != dr["Customer_ID"].ToString()
                //        && dr["Customer_ID2_Old"].ToString() != dr["Customer_ID1"].ToString()
                //        && dr["Customer_ID2_Old"].ToString() != dr["Customer_ID2"].ToString()
                //        && dr["Customer_ID2_Old"].ToString() != dr["Customer_ID3"].ToString()
                //        && dr["Customer_ID2_Old"].ToString() != dr["Customer_ID4"].ToString()
                //        )
                //    {
                //        if (dr["Remove_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID1 = '" + dr["Customer_ID2_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["Remove_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID2 = '" + dr["Customer_ID2_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["Remove_Customer_ID3"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID3 = '" + dr["Customer_ID2_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                //// คนที่ 4
                //foreach (System.Data.DataRow dr in dt.Rows)
                //{
                //    if (dr["Customer_ID3_Old"].ToString() != ""
                //        && dr["Customer_ID3_Old"].ToString() != dr["Customer_ID"].ToString()
                //        && dr["Customer_ID3_Old"].ToString() != dr["Customer_ID1"].ToString()
                //        && dr["Customer_ID3_Old"].ToString() != dr["Customer_ID2"].ToString()
                //        && dr["Customer_ID3_Old"].ToString() != dr["Customer_ID3"].ToString()
                //        && dr["Customer_ID3_Old"].ToString() != dr["Customer_ID4"].ToString()
                //        )
                //    {
                //        if (dr["Remove_Customer_ID1"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID1 = '" + dr["Customer_ID3_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["Remove_Customer_ID2"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID2 = '" + dr["Customer_ID3_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else if (dr["Remove_Customer_ID3"].ToString() == "")
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID3 = '" + dr["Customer_ID3_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //        else
                //        {
                //            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID4 = '" + dr["Customer_ID3_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                //        }
                //    }
                //}

                // คนที่ 5
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    if (dr["Customer_ID4_Old"].ToString() != ""
                        && dr["Customer_ID4_Old"].ToString() != dr["Customer_ID"].ToString()
                        && dr["Customer_ID4_Old"].ToString() != dr["Customer_ID1"].ToString()
                        && dr["Customer_ID4_Old"].ToString() != dr["Customer_ID2"].ToString()
                        && dr["Customer_ID4_Old"].ToString() != dr["Customer_ID3"].ToString()
                        && dr["Customer_ID4_Old"].ToString() != dr["Customer_ID4"].ToString()
                        )
                    {
                        if (dr["Remove_Customer_ID1"].ToString() == "")
                        {
                            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID1 = '" + dr["Customer_ID4_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                        }
                        else if (dr["Remove_Customer_ID2"].ToString() == "")
                        {
                            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID2 = '" + dr["Customer_ID4_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                        }
                        else if (dr["Remove_Customer_ID3"].ToString() == "")
                        {
                            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID3 = '" + dr["Customer_ID4_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                        }
                        else
                        {
                            db.ExecuteNonQuery("update Sys_REM_ChangeContract_AddRemove_ViewForNoble set Remove_Customer_ID4 = '" + dr["Customer_ID4_Old"].ToString() + "' where ChangeNumber = '" + dr["ChangeOwner_No"].ToString() + "'");
                        }
                    }
                }

                #endregion

                db.CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error => " + ex.Message);
                db.RollbackTransaction(tran);
            }
            finally
            {
                db.DisposeConnection();
            }
        }

        private static void ICONData_Prepare(ICON.Framework.Provider.DBHelper db)
        {
            // เตรียมข้อมูล
            // ใบเสร็จ
            // CSV file
            string sql = "";

            List<string> types = new List<string>();
            types.Add("RCT"); // ใบเสร็จ
            //types.Add("TIV"); // ใบกำกับ

            string documentName = "";

            foreach (string type in types)
            {
                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Start prepare data type = " + type + ".");
                sql = string.Format(@"
select {2} t.*
    , ucode.Value as API_UserCode
	, acckey.Value as API_AccessKey
	, apikey.Value as API_APIKey
    , token.Value as API_Token
from vw_Noble_DataSend_ETAX t
    left join (
		select CompanyID, Value
		from Sys_Conf_RealEstate
		where GroupName = 'etax' and KEYNAME = 'UserCode'
	) ucode on t.CompanyID = ucode.CompanyID
	left join (
		select CompanyID, Value
		from Sys_Conf_RealEstate
		where GroupName = 'etax' and KEYNAME = 'AccessKey'
	) acckey on t.CompanyID = acckey.CompanyID
	left join (
		select CompanyID, Value
		from Sys_Conf_RealEstate
		where GroupName = 'etax' and KEYNAME = 'APIKey'
	) apikey on t.CompanyID = apikey.CompanyID
    left join (
		select CompanyID, Value
		from Sys_Conf_RealEstate
		where GroupName = 'etax' and KEYNAME = 'APIToken'
	) token on t.CompanyID = token.CompanyID
where 1 = 1
    and DOCUMENT_ISSUE_DTM >= '{1}'      -- format(r.ReceiptDate, 'yyyy-MM-ddTHH:mm:ss') as 
    and DOCUMENT_ISSUE_DTM <= format(getdate(), 'yyyy-MM-ddTHH:mm:ss')
	--and ReceiptType = '{0}'         -- N = ใบรับ (ใบเสร็จรับเงิน), V =ใบเสร็จรับเงิน/ใบกำกับภาษี
	and ReceiptType {0}         -- N = ใบรับ (ใบเสร็จรับเงิน), V =ใบเสร็จรับเงิน/ใบกำกับภาษี
    --and ContractID = 'R980100058'
    --and t.CompanyID = 'NB'
    --and t.BUYER_TAX_ID_TYPE = 'CCPT'
    --and t.DOCUMENT_ID in (
    --    'RV-IIS101-24030026'
	--)
order by DOCUMENT_ISSUE_DTM desc
"
//, type == "TIV" ? "='V'" : " IN ('N','NO')"
, type == "TIV" ? "='V'" : " IN ('NO')"
, System.Configuration.ConfigurationManager.AppSettings["ETAX:StartDate"].ToString()
, _TEST_RECORD == "" ? "" : "top " + _TEST_RECORD
);
                System.Data.DataTable dt = db.ExecuteDataTable(sql);
                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> End prepare data type = " + type + ".");

                string receiptId = "";
                DateTime currentDate = DateTime.Now;

                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Start send data to INET.");
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    ICON.Framework.Provider.DBHelper db2 = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString(), null);

                    System.Data.IDbTransaction tran2 = null;
                    try
                    {
                        tran2 = db2.BeginTransaction();
                        receiptId = dr["DOCUMENT_ID"].ToString();

                        if (string.IsNullOrEmpty(dr["API_UserCode"].ToString()) || string.IsNullOrEmpty(dr["API_AccessKey"].ToString()) || string.IsNullOrEmpty(dr["API_APIKey"].ToString()))
                        {
                            _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Error >>> " + receiptId + "(" + dr["CompanyID"].ToString() + ") is not set Interface config.");
                            throw new Exception("No config");
                        }

                        currentDate = DateTime.Now;
                        documentName = type + "_" + dr["CompanyID"].ToString() + "_" + dr["BranchID"].ToString() + "_" + dr["DOCUMENT_TYPE_CODE"].ToString() + "_" + receiptId + "_" + currentDate.ToString("yyMM") + "_" + currentDate.ToString("yyMMdd") + "_" + currentDate.ToString("HHmmss");
                        Etax_Noble_INET.Model.ETAXSignDocument body = new Model.ETAXSignDocument()
                        {
                            UserCode = dr["API_UserCode"].ToString(),
                            AccessKey = dr["API_AccessKey"].ToString(),
                            APIKey = dr["API_APIKey"].ToString(),
                            SellerTaxId = dr["SELLER_TAX_ID"].ToString(),
                            SellerBranchId = dr["SELLER_BRANCH_ID"].ToString(),
                            SendInbox = "N",
                            SendChat = "N",
                            //PhoneNumber = dr["BUYER_CONTACT_PHONE_NO"].ToString(),
                            //EncryptPassword = dr["BUYER_TAX_ID"].ToString(),
                            //SendMail = !string.IsNullOrEmpty(dr["BUYER_URIID"].ToString()) ? "Y" : "N",
                            //SendSms = dr["BUYER_CONTACT_PHONE_NO"].ToString(),
                            SendSms = "N",
                            SendInboxHaveBuyer = "N",
                            SellerTemplateCode = "",
                        };


                        // Create Receipt PDF file
                        #region PDF Receipt

                        //var response = (HttpWebResponse)request.GetResponse();
                        //var responseStringPdf = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        Etax_Noble_INET.Model.FileAttach reciept = new Model.FileAttach();
                        reciept.Name = documentName + ".pdf";
                        reciept.ContentType = "application/pdf";
                        //reciept.FileData = Encoding.ASCII.GetBytes(responseStringPdf);

                        // Save PDF
                        string pathPdf = AppDomain.CurrentDomain.BaseDirectory + "\\Files\\Receipt\\" + DateTime.Now.ToString("yyyyMMdd");
                        if (!System.IO.Directory.Exists(pathPdf))
                        {
                            System.IO.Directory.CreateDirectory(pathPdf);
                        }

                        string urlParam = "";
                        if (type.ToUpper() == "RCT")
                        {
                            //urlParam = "d=1&pl=1&lang=TH&rcidtype=receipt&g=" + System.Configuration.ConfigurationManager.AppSettings["PFKey:RCT"].ToString() + "&cid=" + dr["ContractID"].ToString() + "&rcid=" + receiptId;
                            if (dr["BUYER_TAX_ID_TYPE"].ToString().ToUpper() == "TXID" || dr["BUYER_TAX_ID_TYPE"].ToString().ToUpper() == "NIDN")
                            {
                                urlParam = "d=1&pl=1&lang=TH&rcidtype=receipt&g=" + System.Configuration.ConfigurationManager.AppSettings["PFKey:RCT"].ToString() + "&cid=" + dr["ContractID"].ToString() + "&rcid=" + receiptId;
                            }
                            else
                            {
                                urlParam = "d=1&pl=1&lang=TH&rcidtype=receipt&g=" + System.Configuration.ConfigurationManager.AppSettings["PFKey:RCT_EN"].ToString() + "&cid=" + dr["ContractID"].ToString() + "&rcid=" + receiptId;
                            }
                        }
                        else if (type.ToUpper() == "TIV")
                        {
                            //urlParam = "pl=1&g=AC8&cid=" + dr["ContractID"].ToString() + "&rcid=" + receiptId + "&rcidtype=receipt&UserID=Receipt&t=1&lg=TH&ReceiptType=V";
                            if (dr["BUYER_TAX_ID_TYPE"].ToString().ToUpper() == "TXID" || dr["BUYER_TAX_ID_TYPE"].ToString().ToUpper() == "NIDN")
                            {
                                urlParam = "pl=1&g=" + System.Configuration.ConfigurationManager.AppSettings["PFKey:INV"].ToString() + "&cid=" + dr["ContractID"].ToString() + "&rcid=" + receiptId + "&rcidtype=receipt&UserID=Receipt&t=1&lg=TH&ReceiptType=V";
                            }
                            else
                            {
                                urlParam = "pl=1&g=" + System.Configuration.ConfigurationManager.AppSettings["PFKey:INV_EN"].ToString() + "&cid=" + dr["ContractID"].ToString() + "&rcid=" + receiptId + "&rcidtype=receipt&UserID=Receipt&t=1&lg=EN&ReceiptType=V";
                            }
                        }

                        var request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["REM:Url"].ToString()
                            + "/REM/PrintForm/PrintFormCaller.aspx?"
                            + urlParam);

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(request.RequestUri, pathPdf + "\\" + reciept.Name);
                        }
                        reciept.FullPath = pathPdf + "\\" + reciept.Name;
                        reciept.FileData = System.IO.File.ReadAllBytes(reciept.FullPath);
                        #endregion

                        // Create CSV file
                        #region CSV file

                        string csvString = GetCsvString(dr);
                        CSVInetData csvFile = new CSVInetData(documentName);
                        csvFile.WriteToFile(csvString);

                        Etax_Noble_INET.Model.FileAttach csv = new Model.FileAttach();
                        csv.Name = documentName + ".txt";
                        csv.ContentType = "text/csv";
                        //reciept.FileData = Encoding.ASCII.GetBytes(csvString);
                        csv.FullPath = csvFile.FilePath;
                        csv.FileData = File.ReadAllBytes(csv.FullPath);

                        #endregion

                        //Model.ETAXSignDocument_Reps sendResult = SendDataToINET(body, receiptId, reciept, csv);
                        Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(dr["API_Token"].ToString(), body, receiptId, reciept, csv);

                        //Create Log for skip success item.
                        if (sendResult != null)
                        {
                            EtaxLog_Create(db2, tran2, dr, csv.Name, reciept.Name, sendResult);

                            if (sendResult.status.ToUpper() != "OK")
                            {
                                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success with error, " + receiptId + " => " + sendResult.errorMessage);
                            }
                        }

                        db2.CommitTransaction(tran2);
                    }
                    catch (Exception ex)
                    {
                        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface error, " + receiptId + " => " + ex.Message);
                        db2.RollbackTransaction(tran2);
                        continue;
                    }
                }
            }

            _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Finish send data to INET.");
        }

        private static void SAPData_Prepare(ICON.Framework.Provider.DBHelper db)
        {
            _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Start prepare data(SAP).");
            //ICON.Framework.Provider.DBHelper db = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServerSAP"].ToString(), null);

            string sql = "";
//            sql = @"
//select t.CompanyID, t.Value as SAPCompanyID
//    , ucode.Value as API_UserCode
//	, acckey.Value as API_AccessKey
//	, apikey.Value as API_APIKey
//    , token.Value as API_Token
//from Sys_Conf_RealEstate t
//    left join (
//		select CompanyID, Value
//		from Sys_Conf_RealEstate
//		where GroupName = 'etax' and KEYNAME = 'UserCode'
//	) ucode on t.CompanyID = ucode.CompanyID
//	left join (
//		select CompanyID, Value
//		from Sys_Conf_RealEstate
//		where GroupName = 'etax' and KEYNAME = 'AccessKey'
//	) acckey on t.CompanyID = acckey.CompanyID
//	left join (
//		select CompanyID, Value
//		from Sys_Conf_RealEstate
//		where GroupName = 'etax' and KEYNAME = 'APIKey'
//	) apikey on t.CompanyID = apikey.CompanyID
//    left join (
//		select CompanyID, Value
//		from Sys_Conf_RealEstate
//		where GroupName = 'etax' and KEYNAME = 'APIToken'
//	) token on t.CompanyID = token.CompanyID
//where t.GroupName = 'etax' 
//    and t.KEYNAME = 'SAPCompanyID'
//    and isnull(t.CompanyID, '') <> ''
//order by t.CompanyID
//";
//            System.Data.DataTable dtCompConfig = dbREM.ExecuteDataTable(sql);


            #region Load Data from DB

            System.Data.DataTable dtDocumentRCT = db.ExecuteDataTable("select C_SELLER_TAX_ID, C_SELLER_BRANCH_ID, H_DOCUMENT_ID, C_COMP_ID from SAP_ETAX_RCT group by C_SELLER_TAX_ID, C_SELLER_BRANCH_ID, H_DOCUMENT_ID, C_COMP_ID");
            System.Data.DataTable dtDocumentINV = db.ExecuteDataTable("select C_SELLER_TAX_ID, C_SELLER_BRANCH_ID, H_DOCUMENT_ID, C_COMP_ID from SAP_ETAX_INV group by C_SELLER_TAX_ID, C_SELLER_BRANCH_ID, H_DOCUMENT_ID, C_COMP_ID");
            System.Data.DataTable dtDocumentCN = db.ExecuteDataTable("select C_SELLER_TAX_ID, C_SELLER_BRANCH_ID, H_DOCUMENT_ID, C_COMP_ID from SAP_ETAX_CN group by C_SELLER_TAX_ID, C_SELLER_BRANCH_ID, H_DOCUMENT_ID, C_COMP_ID");
            int totalDoc = dtDocumentRCT.Rows.Count + dtDocumentINV.Rows.Count + dtDocumentCN.Rows.Count;

            List<Dictionary<string, object>> dataFinal = new List<Dictionary<string, object>>();

            //สำหรับใบเสร็จรับเงิน/ใบกำกับภาษี
            sql = "select " + (_TEST_RECORD == "" ? "" : "top " + _TEST_RECORD) + " * from SAP_ETAX_RCT";
            System.Data.DataTable dtRC = db.ExecuteDataTable(sql);
            List<Dictionary<string, object>> listRCT = ICON.Utilities.Convert.ConvertRowToClass(dtRC);
            foreach (Dictionary<string, object> item in listRCT)
            {
                item.Add("XXXTYPE", "RCT");
                dataFinal.Add(item);
            }

            //สำหรับใบแจ้งหนี้,ใบแจ้งหนี้/ใบกำกับภาษี
            sql = "select " + (_TEST_RECORD == "" ? "" : "top " + _TEST_RECORD) + " * from SAP_ETAX_INV";
            System.Data.DataTable dtINV = db.ExecuteDataTable(sql);
            List<Dictionary<string, object>> listINV = ICON.Utilities.Convert.ConvertRowToClass(dtINV);
            foreach (Dictionary<string, object> item in listINV)
            {
                item.Add("XXXTYPE", "TIV");
                dataFinal.Add(item);
            }

            //สำหรับใบลดหนี้
            sql = "select " + (_TEST_RECORD == "" ? "" : "top " + _TEST_RECORD) + " * from SAP_ETAX_CN";
            System.Data.DataTable dtCN = db.ExecuteDataTable(sql);
            List<Dictionary<string, object>> listCN = ICON.Utilities.Convert.ConvertRowToClass(dtCN);
            foreach (Dictionary<string, object> item in listCN)
            {
                item.Add("XXXTYPE", "CDN");
                dataFinal.Add(item);
            }

            _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> End prepare data(SAP) with " + totalDoc + " file(s).");

            #endregion

            _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Start send data SAP to INET.");
            string receiptId = "", documentName = "", type = "";
            DateTime currentDate = DateTime.Now;

            // no use
            //if (1 == 0)
            //{
            //    foreach (Dictionary<string, object> dr in dataFinal)
            //    {
            //        // Check REM Config
            //        System.Data.DataRow[] drConfig = dtCompConfig.Select("SAPCompanyID = '" + dr["C_COMP_ID"].ToString() + "'");
            //        if (drConfig.Count() > 0)
            //        {
            //            if (string.IsNullOrEmpty(drConfig[0]["API_UserCode"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_AccessKey"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_APIKey"].ToString()))
            //            {
            //                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Error >>> " + receiptId + "(" + dr["CompanyID"].ToString() + ") is not set Interface config.");
            //                continue;
            //            }
            //        }
            //        else
            //        {
            //            continue;
            //        }

            //        ICON.Framework.Provider.DBHelper db2 = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServerSAP"].ToString(), null);
            //        type = dr["XXXTYPE"].ToString();

            //        System.Data.IDbTransaction tran2 = null;
            //        try
            //        {
            //            tran2 = db2.BeginTransaction();

            //            receiptId = dr["H_DOCUMENT_ID"].ToString();
            //            currentDate = DateTime.Now;
            //            documentName = type + "_" + dr["C_COMP_ID"].ToString() + "_" + dr["C_SELLER_BRANCH_ID"].ToString() + "_" + dr["H_DOCUMENT_TYPE_CODE"].ToString() + "_" + receiptId + "_" + currentDate.ToString("yyMM") + "_" + currentDate.ToString("yyMMdd") + "_" + currentDate.ToString("HHmmss");
            //            Etax_Noble_INET.Model.ETAXSignDocument body = new Model.ETAXSignDocument()
            //            {
            //                UserCode = drConfig[0]["API_UserCode"].ToString(),
            //                AccessKey = drConfig[0]["API_AccessKey"].ToString(),
            //                APIKey = drConfig[0]["API_APIKey"].ToString(),
            //                SellerTaxId = dr["C_SELLER_TAX_ID"].ToString(),
            //                SellerBranchId = dr["C_SELLER_BRANCH_ID"].ToString(),
            //                SendInbox = "N",
            //                SendChat = "N",
            //                PhoneNumber = "",
            //                //EncryptPassword = dr["B_BUYER_TAX_ID"].ToString(),
            //                //SendMail = !string.IsNullOrEmpty(dr["B_BUYER_URIID"].ToString()) ? "Y" : "N",
            //                SendSms = "",
            //                SendInboxHaveBuyer = "",
            //                SellerTemplateCode = "",
            //                ServiceCode = System.Configuration.ConfigurationManager.AppSettings["ETAX:ServiceCode_SAP"].ToString()
            //            };

            //            // Create Receipt PDF file
            //            // SAP ไม่สร้างใบเสร็จ
            //            Etax_Noble_INET.Model.FileAttach reciept = new Model.FileAttach();

            //            // Create CSV file
            //            #region CSV file

            //            string csvString = GetCsvString_ForSAP(dr);
            //            CSVInetData csvFile = new CSVInetData(documentName, true);
            //            csvFile.WriteToFile(csvString);

            //            Etax_Noble_INET.Model.FileAttach csv = new Model.FileAttach();
            //            csv.Name = documentName + ".txt";
            //            csv.ContentType = "text/csv";
            //            //reciept.FileData = Encoding.ASCII.GetBytes(csvString);
            //            csv.FullPath = csvFile.FilePath;
            //            csv.FileData = File.ReadAllBytes(csv.FullPath);

            //            #endregion

            //            Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(drConfig[0]["API_Token"].ToString(), body, receiptId, reciept, csv, true);
            //            //Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(body, receiptId, reciept, csv);

            //            //Create Log for skip success item.
            //            if (sendResult != null)
            //            {

            //                // Update Status to SAPB1
            //                ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
            //                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@DocEntry", dr["H_DOCUMENT_ID"].ToString()));
            //                string sap_objectType = "RCT";
            //                switch (dr["H_DOCUMENT_TYPE_CODE"].ToString().ToUpper())
            //                {
            //                    case "380":
            //                        sap_objectType = "INV";
            //                        break;
            //                    case "T02":
            //                        sap_objectType = "TIV";
            //                        break;
            //                    case "81":
            //                        sap_objectType = "CLN";
            //                        break;
            //                    default:
            //                        sap_objectType = "RCT";
            //                        break;
            //                }
            //                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Objtype", sap_objectType));

            //                if (sendResult.status.ToUpper() == "OK")
            //                {
            //                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@SendStatus", "P"));
            //                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ReceiptUrl", sendResult.pdfURL));
            //                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Error", ""));

            //                    _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success, " + dr["H_DOCUMENT_ID"].ToString() + " => " + sendResult.pdfURL);
            //                }
            //                else
            //                {
            //                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@SendStatus", "N"));
            //                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ReceiptUrl", ""));
            //                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Error", sendResult.errorCode + "," + sendResult.errorMessage));

            //                    _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success with error, " + dr["H_DOCUMENT_ID"].ToString() + " => " + sendResult.errorMessage);
            //                }

            //                db2.ExecuteNonQuery("ICON_ETAX_UpdateStatus", sqlParams, tran2, System.Data.CommandType.StoredProcedure);
            //            }
            //            else
            //            {
            //                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + dr["H_DOCUMENT_ID"].ToString() + " => No response.");
            //            }

            //            db2.CommitTransaction(tran2);
            //        }
            //        catch (Exception ex)
            //        {
            //            db2.RollbackTransaction(tran2);
            //            _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + dr["H_DOCUMENT_ID"].ToString() + " => " + ex.Message);
            //            continue;
            //        }
            //    }

            //    _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Finish send data SAP to INET.");

            //}

            // use
            if (1 == 1)
            {
                // RCT
                foreach (System.Data.DataRow drDoc in dtDocumentRCT.Rows)
                {
                    // Check REM Config
                    //System.Data.DataRow[] drConfig = dtCompConfig.Select("SAPCompanyID = '" + drDoc["C_COMP_ID"].ToString() + "'");
                    //if (drConfig.Count() > 0)
                    //{
                    //    if (string.IsNullOrEmpty(drConfig[0]["API_UserCode"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_AccessKey"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_APIKey"].ToString()))
                    //    {
                    //        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Error >>> " + receiptId + "(" + drDoc["CompanyID"].ToString() + ") is not set Interface config.");
                    //        continue;
                    //    }
                    //}
                    //else
                    //{
                    //    continue;
                    //}

                    //ICON.Framework.Provider.DBHelper dbSAP = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServerSAP"].ToString(), null);
                    ICON.Framework.Provider.DBHelper dbSAP = db;
                    System.Data.IDbTransaction tranSAP = null;
                    try
                    {
                        tranSAP = dbSAP.BeginTransaction();
                        System.Data.DataRow[] drItems = dtRC.Select("C_SELLER_TAX_ID = '" + drDoc["C_SELLER_TAX_ID"].ToString() + "' and C_SELLER_BRANCH_ID = '" + drDoc["C_SELLER_BRANCH_ID"].ToString() + "' and H_DOCUMENT_ID = '" + drDoc["H_DOCUMENT_ID"].ToString() + "'");

                        if (drItems.Count() > 0)
                        {
                            //SAP_SendData(drConfig, dbSAP, tranSAP, drDoc, drItems, "RCT");
                            SAP_SendDataWithOutDrConfig(dbSAP, tranSAP, drDoc, drItems, "RCT");
                        }

                        dbSAP.CommitTransaction(tranSAP);
                    }
                    catch (Exception ex)
                    {
                        dbSAP.RollbackTransaction(tranSAP);
                        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + drDoc["H_DOCUMENT_ID"].ToString() + " => " + ex.Message);
                        continue;
                    }
                }

                // TIV
                foreach (System.Data.DataRow drDoc in dtDocumentINV.Rows)
                {
                    // Check REM Config
                    //System.Data.DataRow[] drConfig = dtCompConfig.Select("SAPCompanyID = '" + drDoc["C_COMP_ID"].ToString() + "'");
                    //if (drConfig.Count() > 0)
                    //{
                    //    if (string.IsNullOrEmpty(drConfig[0]["API_UserCode"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_AccessKey"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_APIKey"].ToString()))
                    //    {
                    //        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Error >>> " + receiptId + "(" + drDoc["CompanyID"].ToString() + ") is not set Interface config.");
                    //        continue;
                    //    }
                    //}
                    //else
                    //{
                    //    continue;
                    //}

                    //ICON.Framework.Provider.DBHelper dbSAP = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServerSAP"].ToString(), null);
                    ICON.Framework.Provider.DBHelper dbSAP = db;
                    System.Data.IDbTransaction tranSAP = null;
                    try
                    {
                        tranSAP = dbSAP.BeginTransaction();
                        System.Data.DataRow[] drItems = dtINV.Select("C_SELLER_TAX_ID = '" + drDoc["C_SELLER_TAX_ID"].ToString() + "' and C_SELLER_BRANCH_ID = '" + drDoc["C_SELLER_BRANCH_ID"].ToString() + "' and H_DOCUMENT_ID = '" + drDoc["H_DOCUMENT_ID"].ToString() + "'");

                        if (drItems.Count() > 0)
                        {
                            //SAP_SendData(drConfig, dbSAP, tranSAP, drDoc, drItems, "TIV");
                            SAP_SendDataWithOutDrConfig(dbSAP, tranSAP, drDoc, drItems, "TIV");
                        }

                        dbSAP.CommitTransaction(tranSAP);
                    }
                    catch (Exception ex)
                    {
                        dbSAP.RollbackTransaction(tranSAP);
                        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + drDoc["H_DOCUMENT_ID"].ToString() + " => " + ex.Message);
                        continue;
                    }
                }

                // CDN
                foreach (System.Data.DataRow drDoc in dtDocumentCN.Rows)
                {
                    // Check REM Config
                    //System.Data.DataRow[] drConfig = dtCompConfig.Select("SAPCompanyID = '" + drDoc["C_COMP_ID"].ToString() + "'");
                    //if (drConfig.Count() > 0)
                    //{
                    //    if (string.IsNullOrEmpty(drConfig[0]["API_UserCode"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_AccessKey"].ToString()) || string.IsNullOrEmpty(drConfig[0]["API_APIKey"].ToString()))
                    //    {
                    //        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Error >>> " + receiptId + "(" + drDoc["CompanyID"].ToString() + ") is not set Interface config.");
                    //        continue;
                    //    }
                    //}
                    //else
                    //{
                    //    continue;
                    //}

                    //ICON.Framework.Provider.DBHelper dbSAP = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServerSAP"].ToString(), null);
                    ICON.Framework.Provider.DBHelper dbSAP = db;
                    System.Data.IDbTransaction tranSAP = null;
                    try
                    {
                        tranSAP = dbSAP.BeginTransaction();
                        System.Data.DataRow[] drItems = dtCN.Select("C_SELLER_TAX_ID = '" + drDoc["C_SELLER_TAX_ID"].ToString() + "' and C_SELLER_BRANCH_ID = '" + drDoc["C_SELLER_BRANCH_ID"].ToString() + "' and H_DOCUMENT_ID = '" + drDoc["H_DOCUMENT_ID"].ToString() + "'");

                        if (drItems.Count() > 0)
                        {
                            //SAP_SendData(drConfig, dbSAP, tranSAP, drDoc, drItems, "CDN");
                            SAP_SendDataWithOutDrConfig(dbSAP, tranSAP, drDoc, drItems, "CDN");
                        }

                        dbSAP.CommitTransaction(tranSAP);
                    }
                    catch (Exception ex)
                    {
                        dbSAP.RollbackTransaction(tranSAP);
                        _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + drDoc["H_DOCUMENT_ID"].ToString() + " => " + ex.Message);
                        continue;
                    }
                }

                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Finish send data SAP to INET.");
            }
        }

        private static void SAP_SendData(ICON.Framework.Provider.DBHelper db2, System.Data.IDbTransaction tran2, System.Data.DataRow[] drConfig, System.Data.DataRow dr, System.Data.DataRow[] drItems, string type)
        {
            string receiptId = dr["H_DOCUMENT_ID"].ToString();
            DateTime currentDate = DateTime.Now;
            string documentName = type + "_" + dr["C_COMP_ID"].ToString() + "_" + dr["C_SELLER_BRANCH_ID"].ToString() + "_" + drItems[0]["H_DOCUMENT_TYPE_CODE"].ToString() + "_" + receiptId + "_" + currentDate.ToString("yyMM") + "_" + currentDate.ToString("yyMMdd") + "_" + currentDate.ToString("HHmmss");
            Etax_Noble_INET.Model.ETAXSignDocument body = new Model.ETAXSignDocument()
            {
                UserCode = drConfig[0]["API_UserCode"].ToString(),
                AccessKey = drConfig[0]["API_AccessKey"].ToString(),
                APIKey = drConfig[0]["API_APIKey"].ToString(),
                SellerTaxId = dr["C_SELLER_TAX_ID"].ToString(),
                SellerBranchId = dr["C_SELLER_BRANCH_ID"].ToString(),
                SendInbox = "N",
                SendChat = "N",
                PhoneNumber = "",
                //EncryptPassword = dr["B_BUYER_TAX_ID"].ToString(),
                //SendMail = !string.IsNullOrEmpty(dr["B_BUYER_URIID"].ToString()) ? "Y" : "N",
                SendSms = "",
                SendInboxHaveBuyer = "",
                SellerTemplateCode = "",
                ServiceCode = System.Configuration.ConfigurationManager.AppSettings["ETAX:ServiceCode_SAP"].ToString()
            };

            // Create Receipt PDF file
            // SAP ไม่สร้างใบเสร็จ
            Etax_Noble_INET.Model.FileAttach reciept = new Model.FileAttach();

            // Create CSV file
            #region CSV file

            string csvString = GetCsvString_ForSAP2(drItems);
            CSVInetData csvFile = new CSVInetData(documentName, true);
            csvFile.WriteToFile(csvString);

            Etax_Noble_INET.Model.FileAttach csv = new Model.FileAttach();
            csv.Name = documentName + ".txt";
            csv.ContentType = "text/csv";
            //reciept.FileData = Encoding.ASCII.GetBytes(csvString);
            csv.FullPath = csvFile.FilePath;
            csv.FileData = File.ReadAllBytes(csv.FullPath);

            #endregion


            Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(drConfig[0]["API_Token"].ToString(), body, receiptId, reciept, csv, true);
            //Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(body, receiptId, reciept, csv);

            //Create Log for skip success item.
            if (sendResult != null)
            {

                // Update Status to SAPB1
                ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@DocEntry", dr["H_DOCUMENT_ID"].ToString()));
                string sap_objectType = "RCT";
                switch (drItems[0]["H_DOCUMENT_TYPE_CODE"].ToString().ToUpper())
                {
                    case "380":
                        sap_objectType = "INV";
                        break;
                    case "T02":
                        sap_objectType = "TIV";
                        break;
                    case "81":
                        sap_objectType = "CLN";
                        break;
                    default:
                        sap_objectType = "RCT";
                        break;
                }
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Objtype", sap_objectType));

                if (sendResult.status.ToUpper() == "OK")
                {
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@SendStatus", "P"));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ReceiptUrl", sendResult.pdfURL));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Error", ""));

                    _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success, " + dr["H_DOCUMENT_ID"].ToString() + " => " + sendResult.pdfURL);
                }
                else
                {
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@SendStatus", "N"));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ReceiptUrl", ""));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Error", sendResult.errorCode + "," + sendResult.errorMessage));

                    _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success with error, " + dr["H_DOCUMENT_ID"].ToString() + " => " + sendResult.errorMessage);
                }

                db2.ExecuteNonQuery("ICON_ETAX_UpdateStatus", sqlParams, tran2, System.Data.CommandType.StoredProcedure);
            }
            else
            {
                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + dr["H_DOCUMENT_ID"].ToString() + " => No response.");
            }

        }

        private static void SAP_SendDataWithOutDrConfig(ICON.Framework.Provider.DBHelper db2, System.Data.IDbTransaction tran2, System.Data.DataRow dr, System.Data.DataRow[] drItems, string type)
        {
            string receiptId = dr["H_DOCUMENT_ID"].ToString();
            DateTime currentDate = DateTime.Now;
            string documentName = type + "_" + dr["C_COMP_ID"].ToString() + "_" + dr["C_SELLER_BRANCH_ID"].ToString() + "_" + drItems[0]["H_DOCUMENT_TYPE_CODE"].ToString() + "_" + receiptId + "_" + currentDate.ToString("yyMM") + "_" + currentDate.ToString("yyMMdd") + "_" + currentDate.ToString("HHmmss");
            Etax_Noble_INET.Model.ETAXSignDocument body = new Model.ETAXSignDocument()
            {
                //UserCode = drConfig[0]["API_UserCode"].ToString(),
                //AccessKey = drConfig[0]["API_AccessKey"].ToString(),
                //APIKey = drConfig[0]["API_APIKey"].ToString(),
                //SellerTaxId = dr["C_SELLER_TAX_ID"].ToString(),
                //SellerBranchId = dr["C_SELLER_BRANCH_ID"].ToString(),
                SendInbox = "N",
                SendChat = "N",
                PhoneNumber = "",
                //EncryptPassword = dr["B_BUYER_TAX_ID"].ToString(),
                //SendMail = !string.IsNullOrEmpty(dr["B_BUYER_URIID"].ToString()) ? "Y" : "N",
                SendSms = "",
                SendInboxHaveBuyer = "",
                SellerTemplateCode = "",
                ServiceCode = System.Configuration.ConfigurationManager.AppSettings["ETAX:ServiceCode_SAP"].ToString()
            };

            // Create Receipt PDF file
            // SAP ไม่สร้างใบเสร็จ
            Etax_Noble_INET.Model.FileAttach reciept = new Model.FileAttach();

            // Create CSV file
            #region CSV file

            //string csvString = GetCsvString_ForSAP2(drItems);
            string csvString = GetCsvString_ForSAP3(drItems);
            CSVInetData csvFile = new CSVInetData(documentName, true);
            csvFile.WriteToFile(csvString);

            Etax_Noble_INET.Model.FileAttach csv = new Model.FileAttach();
            csv.Name = documentName + ".txt";
            csv.ContentType = "text/csv";
            //reciept.FileData = Encoding.ASCII.GetBytes(csvString);
            csv.FullPath = csvFile.FilePath;
            csv.FileData = File.ReadAllBytes(csv.FullPath);

            #endregion


            //Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(drConfig[0]["API_Token"].ToString(), body, receiptId, reciept, csv, true);
            Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharpWithOutApiToken(body, receiptId, reciept, csv, true);
            //Model.ETAXSignDocument_Reps sendResult = SendDataToINET_WithRestSharp(body, receiptId, reciept, csv);

            //Create Log for skip success item.
            if (sendResult != null)
            {

                // Update Status to SAPB1
                ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@DocEntry", dr["H_DOCUMENT_ID"].ToString()));
                //string sap_objectType = "RCT";
                //switch (drItems[0]["H_DOCUMENT_TYPE_CODE"].ToString().ToUpper())
                //{
                //    case "380":
                //        sap_objectType = "INV";
                //        break;
                //    case "T02":
                //        sap_objectType = "TIV";
                //        break;
                //    case "81":
                //        sap_objectType = "CLN";
                //        break;
                //    default:
                //        sap_objectType = "RCT";
                //        break;
                //}
                string sap_objectType = drItems[0]["SourceTable"].ToString().ToUpper();
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Objtype", sap_objectType));

                if (sendResult.status.ToUpper() == "OK")
                {
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@SendStatus", "S"));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Message", "Success"));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@TranCode", sendResult.transactionCode));
                    //sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Error", ""));

                    _logFile.WriteToFile(sap_objectType + " : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success, " + dr["H_DOCUMENT_ID"].ToString() + " => " + sendResult.pdfURL + "\ntransactionCode => " + sendResult.transactionCode);
                }
                else
                {
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@SendStatus", "F"));
                    //sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Message", ""));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Message", sendResult.errorCode + "," + sendResult.errorMessage));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@TranCode", sendResult.transactionCode));

                    _logFile.WriteToFile(sap_objectType + " : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Interface success with error, " + dr["H_DOCUMENT_ID"].ToString() + " => " + sendResult.errorMessage);
                }

                db2.ExecuteNonQuery("ICON_ETAX_UpdateStatus", sqlParams, tran2, System.Data.CommandType.StoredProcedure);
            }
            else
            {
                _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + dr["H_DOCUMENT_ID"].ToString() + " => No response.");
            }

        }

        private static Model.ETAXSignDocument_Reps SendDataToINET(Etax_Noble_INET.Model.ETAXSignDocument body, string receiptId
            , Etax_Noble_INET.Model.FileAttach reciept, Etax_Noble_INET.Model.FileAttach csv, bool isSAP = false)
        {
            Model.ETAXSignDocument_Reps respObj = new Model.ETAXSignDocument_Reps();
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            string inbound = "", outbound = "", respResult = "", status = "0", errorMsg = "";
            outbound = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            Dictionary<string, string> data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Newtonsoft.Json.JsonConvert.SerializeObject(body));

            string requestUrl = System.Configuration.ConfigurationManager.AppSettings["ETAX:UrlSignDoc"].ToString();
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", System.Configuration.ConfigurationManager.AppSettings["ETAX:Authorization"].ToString());

            httpWebRequest.BeginGetRequestStream((result) =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, reciept, csv);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    respResult = streamReader.ReadToEnd();
                                    inbound = respResult;
                                    //responseString is depend upon your web service.

                                    Model.ETAXSignDocument_Reps resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ETAXSignDocument_Reps>(respResult);
                                    if (resp.status.ToUpper() == "OK")
                                    {
                                        status = "1";
                                    }
                                    else
                                    {
                                        status = "0";
                                        errorMsg = "Error message from INET => " + resp.errorMessage;

                                        throw new Exception(errorMsg);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            status = "0";
                            throw ex;
                        }
                    }, null);
                }
                catch (Exception ex)
                {
                    _logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error at " + receiptId + ", Detail => " + ex.Message);
                    errorMsg = ex.Message;
                    status = "0";
                }
                finally
                {
                    // Create Interface log
                    ICON.Framework.Provider.DBHelper db2 = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString(), null);
                    string sql = "insert into Sys_REM_Interface_Log (ID, Module, MethodName, ApiType, Inbound, Outbound, CreateDate, CreateBy, Status, ErrorMessage, Ref1, Ref2))";
                    sql += "values(newid(), 'ETAX_Interface', 'PostINET', 'POST', @Inbound, @Outbound, getdate(), 'service', @Status, @ErrorMessage, @Ref1, '');";

                    ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Inbound", inbound));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Outbound", outbound));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Status", status));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ErrorMessage", status == "0" ? errorMsg : ""));
                    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Ref1", receiptId));

                    db2.ExecuteNonQuery(sql, sqlParams);
                }
            }, httpWebRequest);

            return respObj;
        }

        /// <summary>
        /// Writes multi part HTTP POST request. Author : Farhan Ghumra
        /// </summary>
        private static void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, Etax_Noble_INET.Model.FileAttach reciept, Etax_Noble_INET.Model.FileAttach csv)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "PDFContent", reciept.Name, reciept.ContentType));
            /// Write the file data to the stream.
            WriteToStream(s, reciept.FileData);
            WriteToStream(s, trailer);

            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "TextContent", csv.Name, csv.ContentType));
            /// Write the file data to the stream.
            WriteToStream(s, csv.FileData);
            WriteToStream(s, trailer);
        }

        /// <summary>
        /// Writes string to stream. Author : Farhan Ghumra
        /// </summary>
        private static void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes byte array to stream. Author : Farhan Ghumra
        /// </summary>
        private static void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        private static Model.ETAXSignDocument_Reps SendDataToINET_WithRestSharp(string apiToken, Etax_Noble_INET.Model.ETAXSignDocument body, string receiptId
           , Etax_Noble_INET.Model.FileAttach reciept, Etax_Noble_INET.Model.FileAttach csv, bool isSAP = false)
        {
            Model.ETAXSignDocument_Reps respObj = new Model.ETAXSignDocument_Reps();
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            string inbound = "", outbound = "", status = "0", errorMsg = "";
            outbound = "CSVName=" + csv.Name + "|";
            outbound += Newtonsoft.Json.JsonConvert.SerializeObject(body);

            Dictionary<string, string> data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Newtonsoft.Json.JsonConvert.SerializeObject(body));

            try
            {
                var client = new RestSharp.RestClient();
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(System.Configuration.ConfigurationManager.AppSettings["ETAX:UrlSignDoc"].ToString(), RestSharp.Method.POST);
                request.AddHeader("Authorization", string.IsNullOrEmpty(apiToken) ? System.Configuration.ConfigurationManager.AppSettings["ETAX:Authorization"].ToString() : apiToken);
                request.AlwaysMultipartFormData = true;

                request.AddParameter("SellerTaxId", body.SellerTaxId);
                request.AddParameter("SellerBranchId", body.SellerBranchId);
                request.AddParameter("UserCode", body.UserCode);
                request.AddParameter("AccessKey", body.AccessKey);
                request.AddParameter("APIKey", body.APIKey);
                request.AddParameter("ServiceCode", body.ServiceCode);
                if (!isSAP)
                {
                    request.AddFile("PDFContent", reciept.FileData, reciept.Name, reciept.ContentType);
                }
                request.AddFile("TextContent", csv.FileData, csv.Name, csv.ContentType);
                //request.AddParameter("SendInbox", body.SendInbox);
                //request.AddParameter("SendChat", body.SendChat);
                //request.AddParameter("PhoneNumber", body.PhoneNumber);
                //request.AddParameter("EncryptPassword", body.EncryptPassword);
                //request.AddParameter("SendMail", body.SendMail);
                //request.AddParameter("SendSms", body.SendSms);
                //request.AddParameter("SendInboxHaveBuyer", body.SendInboxHaveBuyer);
                //request.AddParameter("SellerTemplateCode", body.SellerTemplateCode);
                RestSharp.IRestResponse response = client.Execute(request);

                inbound = response.Content;
                respObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ETAXSignDocument_Reps>(response.Content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (respObj.status.ToUpper() == "OK")
                    {
                        status = "1";
                    }
                    else
                    {
                        status = "0";
                        errorMsg = "Error message from INET => " + respObj.errorMessage;
                        throw new Exception(errorMsg);
                    }
                }
                else
                {
                    status = "0";
                    throw new Exception(response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            finally
            {
                // Create Interface log
                ICON.Framework.Provider.DBHelper db2 = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString(), null);
                string sql = "insert into Sys_REM_Interface_Log (ID, Module, MethodName, ApiType, Inbound, Outbound, CreateDate, CreateBy, Status, ErrorMessage, Ref1, Ref2)";
                sql += "values(newid(), 'ETAX_Interface', 'PostINET', 'POST', @Inbound, @Outbound, getdate(), 'service', @Status, @ErrorMessage, @Ref1, '');";

                ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Inbound", inbound));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Outbound", outbound));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Status", status));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ErrorMessage", status == "0" ? errorMsg : ""));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Ref1", receiptId));

                db2.ExecuteNonQuery(sql, sqlParams);
            }

            return respObj;
        }

        private static Model.ETAXSignDocument_Reps SendDataToINET_WithRestSharpWithOutApiToken(Etax_Noble_INET.Model.ETAXSignDocument body, string receiptId
           , Etax_Noble_INET.Model.FileAttach reciept, Etax_Noble_INET.Model.FileAttach csv, bool isSAP = false)
        {
            Model.ETAXSignDocument_Reps respObj = new Model.ETAXSignDocument_Reps();
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            string inbound = "", outbound = "", status = "0", errorMsg = "";
            outbound = "CSVName=" + csv.Name + "|";
            outbound += Newtonsoft.Json.JsonConvert.SerializeObject(body);

            Dictionary<string, string> data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Newtonsoft.Json.JsonConvert.SerializeObject(body));

            try
            {
                var client = new RestSharp.RestClient();
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(System.Configuration.ConfigurationManager.AppSettings["ETAX:UrlSignDoc"].ToString(), RestSharp.Method.POST);
                request.AddHeader("Authorization", System.Configuration.ConfigurationManager.AppSettings["ETAX:Authorization"].ToString());
                request.AlwaysMultipartFormData = true;

                request.AddParameter("SellerTaxId", body.SellerTaxId);
                request.AddParameter("SellerBranchId", body.SellerBranchId);
                //request.AddParameter("UserCode", body.UserCode);
                //request.AddParameter("AccessKey", body.AccessKey);
                //request.AddParameter("APIKey", body.APIKey);
                request.AddParameter("ServiceCode", body.ServiceCode);
                if (!isSAP)
                {
                    request.AddFile("PDFContent", reciept.FileData, reciept.Name, reciept.ContentType);
                }
                request.AddFile("TextContent", csv.FileData, csv.Name, csv.ContentType);
                //request.AddParameter("SendInbox", body.SendInbox);
                //request.AddParameter("SendChat", body.SendChat);
                //request.AddParameter("PhoneNumber", body.PhoneNumber);
                //request.AddParameter("EncryptPassword", body.EncryptPassword);
                //request.AddParameter("SendMail", body.SendMail);
                //request.AddParameter("SendSms", body.SendSms);
                //request.AddParameter("SendInboxHaveBuyer", body.SendInboxHaveBuyer);
                //request.AddParameter("SellerTemplateCode", body.SellerTemplateCode);
                RestSharp.IRestResponse response = client.Execute(request);

                inbound = response.Content;
                respObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ETAXSignDocument_Reps>(response.Content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (respObj.status.ToUpper() == "OK")
                    {
                        status = "1";
                    }
                    else
                    {
                        status = "0";
                        errorMsg = "Error message from INET => " + respObj.errorMessage;
                        throw new Exception(errorMsg);
                    }
                }
                else
                {
                    status = "0";
                    throw new Exception(response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            //finally
            //{
            //    // Create Interface log
            //    ICON.Framework.Provider.DBHelper db2 = new ICON.Framework.Provider.DBHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString(), null);
            //    string sql = "insert into Sys_REM_Interface_Log (ID, Module, MethodName, ApiType, Inbound, Outbound, CreateDate, CreateBy, Status, ErrorMessage, Ref1, Ref2)";
            //    sql += "values(newid(), 'ETAX_Interface', 'PostINET', 'POST', @Inbound, @Outbound, getdate(), 'service', @Status, @ErrorMessage, @Ref1, '');";

            //    ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
            //    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Inbound", inbound));
            //    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Outbound", outbound));
            //    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Status", status));
            //    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ErrorMessage", status == "0" ? errorMsg : ""));
            //    sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Ref1", receiptId));

            //    db2.ExecuteNonQuery(sql, sqlParams);
            //}

            return respObj;
        }


        private static string GetCsvString(System.Data.DataRow dr)
        {
            System.Text.StringBuilder csv = new StringBuilder();
            //csv.AppendLine("\"C\",\"SELLER_TAX_ID\",\"SELLER_BRANCH_ID\",\"FILE_NAME\"");
            //csv.AppendLine("\"H\"\",\"DOCUMENT_TYPE_CODE\",\"DOCUMENT_NAME\",\"DOCUMENT_ID\",\"DOCUMENT_ISSUE_DTM\",\"CREATE_PURPOSE_CODE\",\"CREATE_PURPOSE\",\"ADDITIONAL_REF_ASSIGN_ID\",\"ADDITIONAL_REF_ISSUE_DTM\",\"ADDITIONAL_REF_TYPE_CODE\",\"ADDITIONAL_REF_DOCUMENT_NAME\",\"DELIVERY_TYPE_CODE\",\"BUYER_ORDER_ASSIGN_ID\",\"BUYER_ORDER_ISSUE_DTM\",\"BUYER_ORDER_REF_TYPE_CODE\",\"DOCUMENT_REMARK\",\"VOUCHER_NO\",\"SELLER_CONTACT_PERSON_NAME\",\"SELLER_CONTACT_DEPARTMENT_NAME\",\"SELLER_CONTACT_URIID\",\"SELLER_CONTACT_PHONE_NO\",\"FLEX_FIELD\",\"SELLER_BRANCH_ID\",\"SOURCE_SYSTEM\",\"ENCRYPT_PASSWORD\",\"PDF_TEMPLATE_ID\",\"SEND_MAIL_IND\",\"RETURN_FILE_NAME\"");
            //csv.AppendLine("\"B\",\"BUYER_ID\",\"BUYER_NAME\",\"BUYER_TAX_ID_TYPE\",\"BUYER_TAX_ID\",\"BUYER_BRANCH_ID\",\"BUYER_CONTACT_PERSON_NAME\",\"BUYER_CONTACT_DEPARTMENT_NAME\",\"BUYER_URIID\",\"BUYER_CONTACT_PHONE_NO\",\"BUYER_POST_CODE\",\"BUYER_BUILDING_NAME\",\"BUYER_BUILDING_NO\",\"BUYER_ADDRESS_LINE1\",\"BUYER_ADDRESS_LINE2\",\"BUYER_ADDRESS_LINE3\",\"BUYER_ADDRESS_LINE4\",\"BUYER_ADDRESS_LINE5\",\"BUYER_STREET_NAME\",\"BUYER_CITY_SUB_DIV_ID\",\"BUYER_CITY_SUB_DIV_NAME\",\"BUYER_CITY_ID\",\"BUYER_CITY_NAME\",\"BUYER_COUNTRY_SUB_DIV_ID\",\"BUYER_COUNTRY_SUB_DIV_NAME\",\"BUYER_COUNTRY_ID\"");
            //csv.AppendLine("\"L\",\"LINE_ID\",\"PRODUCT_ID\",\"PRODUCT_NAME\",\"PRODUCT_DESC\",\"PRODUCT_BATCH_ID\",\"PRODUCT_EXPIRE_DTM\",\"PRODUCT_CLASS_CODE\",\"PRODUCT_CLASS_NAME\",\"PRODUCT_ORIGIN_COUNTRY_ID\",\"PRODUCT_CHARGE_AMOUNT\",\"PRODUCT_CHARGE_CURRENCY_CODE\",\"PRODUCT_ALLOWANCE_CHARGE_IND\",\"PRODUCT_ALLOWANCE_ACTUAL_AMOUNT\",\"PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"PRODUCT_ALLOWANCE_REASON_CODE\",\"PRODUCT_ALLOWANCE_REASON\",\"PRODUCT_QUANTITY\",\"PRODUCT_UNIT_CODE\",\"PRODUCT_QUANTITY_PER_UNIT\",\"LINE_TAX_TYPE_CODE\",\"LINE_TAX_CAL_RATE\",\"LINE_BASIS_AMOUNT\",\"LINE_BASIS_CURRENCY_CODE\",\"LINE_TAX_CAL_AMOUNT\",\"LINE_TAX_CAL_CURRENCY_CODE\",\"LINE_ALLOWANCE_CHARGE_IND\",\"LINE_ALLOWANCE_ACTUAL_AMOUNT\",\"LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"LINE_ALLOWANCE_REASON_CODE\",\"LINE_ALLOWANCE_REASON\",\"LINE_TAX_TOTAL_AMOUNT\",\"LINE_TAX_TOTAL_CURRENCY_CODE\",\"LINE_NET_TOTAL_AMOUNT\",\"LINE_NET_TOTAL_CURRENCY_CODE\",\"LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT\",\"LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE\",\"PRODUCT_REMARK1\",\"PRODUCT_REMARK2\",\"PRODUCT_REMARK3\",\"PRODUCT_REMARK4\",\"PRODUCT_REMARK5\",\"PRODUCT_REMARK6\",\"PRODUCT_REMARK7\",\"PRODUCT_REMARK8\",\"PRODUCT_REMARK9\",\"PRODUCT_REMARK10\"");
            //csv.AppendLine("\"F\",\"LINE_TOTAL_COUNT\",\"DELIVERY_OCCUR_DTM\",\"INVOICE_CURRENCY_CODE\",\"TAX_TYPE_CODE1\",\"TAX_CAL_RATE1\",\"BASIS_AMOUNT1\",\"BASIS_CURRENCY_CODE1\",\"TAX_CAL_AMOUNT1\",\"TAX_CAL_CURRENCY_CODE1\",\"TAX_TYPE_CODE2\",\"TAX_CAL_RATE2\",\"BASIS_AMOUNT2\",\"BASIS_CURRENCY_CODE2\",\"TAX_CAL_AMOUNT2\",\"TAX_CAL_CURRENCY_CODE2\",\"TAX_TYPE_CODE3\",\"TAX_CAL_RATE3\",\"BASIS_AMOUNT3\",\"BASIS_CURRENCY_CODE3\",\"TAX_CAL_AMOUNT3\",\"TAX_CAL_CURRENCY_CODE3\",\"TAX_TYPE_CODE4\",\"TAX_CAL_RATE4\",\"BASIS_AMOUNT4\",\"BASIS_CURRENCY_CODE4\",\"TAX_CAL_AMOUNT4\",\"TAX_CAL_CURRENCY_CODE4\",\"ALLOWANCE_CHARGE_IND\",\"ALLOWANCE_ACTUAL_AMOUNT\",\"ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"ALLOWANCE_REASON_CODE\",\"ALLOWANCE_REASON\",\"PAYMENT_TYPE_CODE\",\"PAYMENT_DESCRIPTION\",\"PAYMENT_DUE_DTM\",\"ORIGINAL_TOTAL_AMOUNT\",\"ORIGINAL_TOTAL_CURRENCY_CODE\",\"LINE_TOTAL_AMOUNT\",\"LINE_TOTAL_CURRENCY_CODE\",\"ADJUSTED_INFORMATION_AMOUNT\",\"ADJUSTED_INFORMATION_CURRENCY_CODE\",\"ALLOWANCE_TOTAL_AMOUNT\",\"ALLOWANCE_TOTAL_CURRENCY_CODE\",\"CHARGE_TOTAL_AMOUNT\",\"CHARGE_TOTAL_CURRENCY_CODE\",\"TAX_BASIS_TOTAL_AMOUNT\",\"TAX_BASIS_TOTAL_CURRENCY_CODE\",\"TAX_TOTAL_AMOUNT\",\"TAX_TOTAL_CURRENCY_CODE\",\"GRAND_TOTAL_AMOUNT\",\"GRAND_TOTAL_CURRENCY_CODE\",\"TERM_PAYMENT\",\"WITHHOLDINGTAX_TYPE1\",\"WITHHOLDINGTAX_DESCRIPTION1\",\"WITHHOLDINGTAX_RATE1\",\"WITHHOLDINGTAX_BASIS_AMOUNT1\",\"WITHHOLDINGTAX_TAX_AMOUNT1\",\"WITHHOLDINGTAX_TYPE2\",\"WITHHOLDINGTAX_DESCRIPTION2\",\"WITHHOLDINGTAX_RATE2\",\"WITHHOLDINGTAX_BASIS_AMOUNT2\",\"WITHHOLDINGTAX_TAX_AMOUNT2\",\"WITHHOLDINGTAX_TYPE3\",\"WITHHOLDINGTAX_DESCRIPTION3\",\"WITHHOLDINGTAX_RATE3\",\"WITHHOLDINGTAX_BASIS_AMOUNT3\",\"WITHHOLDINGTAX_TAX_AMOUNT3\",\"WITHHOLDINGTAX_TOTAL_AMOUNT\",\"ACTUAL_PAYMENT_TOTAL_AMOUNT\",\"DOCUMENT_REMARK1\",\"DOCUMENT_REMARK2\",\"DOCUMENT_REMARK3\",\"DOCUMENT_REMARK4\",\"DOCUMENT_REMARK5\",\"DOCUMENT_REMARK6\",\"DOCUMENT_REMARK7\",\"DOCUMENT_REMARK8\",\"DOCUMENT_REMARK9\",\"DOCUMENT_REMARK10\",\"DOCUMENT_REMARK11\"");
            //csv.AppendLine("\"T\",\"1\"");

            //string csv = @"
            //'C','SELLER_TAX_ID','SELLER_BRANCH_ID','FILE_NAME'
            //'H'','DOCUMENT_TYPE_CODE','DOCUMENT_NAME','DOCUMENT_ID','DOCUMENT_ISSUE_DTM','CREATE_PURPOSE_CODE','CREATE_PURPOSE','ADDITIONAL_REF_ASSIGN_ID','ADDITIONAL_REF_ISSUE_DTM','ADDITIONAL_REF_TYPE_CODE','ADDITIONAL_REF_DOCUMENT_NAME','DELIVERY_TYPE_CODE','BUYER_ORDER_ASSIGN_ID','BUYER_ORDER_ISSUE_DTM','BUYER_ORDER_REF_TYPE_CODE','DOCUMENT_REMARK','VOUCHER_NO','SELLER_CONTACT_PERSON_NAME','SELLER_CONTACT_DEPARTMENT_NAME','SELLER_CONTACT_URIID','SELLER_CONTACT_PHONE_NO','FLEX_FIELD','SELLER_BRANCH_ID','SOURCE_SYSTEM','ENCRYPT_PASSWORD','PDF_TEMPLATE_ID','SEND_MAIL_IND','RETURN_FILE_NAME'
            //'B','BUYER_ID','BUYER_NAME','BUYER_TAX_ID_TYPE','BUYER_TAX_ID','BUYER_BRANCH_ID','BUYER_CONTACT_PERSON_NAME','BUYER_CONTACT_DEPARTMENT_NAME','BUYER_URIID','BUYER_CONTACT_PHONE_NO','BUYER_POST_CODE','BUYER_BUILDING_NAME','BUYER_BUILDING_NO','BUYER_ADDRESS_LINE1','BUYER_ADDRESS_LINE2','BUYER_ADDRESS_LINE3','BUYER_ADDRESS_LINE4','BUYER_ADDRESS_LINE5','BUYER_STREET_NAME','BUYER_CITY_SUB_DIV_ID','BUYER_CITY_SUB_DIV_NAME','BUYER_CITY_ID','BUYER_CITY_NAME','BUYER_COUNTRY_SUB_DIV_ID','BUYER_COUNTRY_SUB_DIV_NAME','BUYER_COUNTRY_ID'
            //'L','LINE_ID','PRODUCT_ID','PRODUCT_NAME','PRODUCT_DESC','PRODUCT_BATCH_ID','PRODUCT_EXPIRE_DTM','PRODUCT_CLASS_CODE','PRODUCT_CLASS_NAME','PRODUCT_ORIGIN_COUNTRY_ID','PRODUCT_CHARGE_AMOUNT','PRODUCT_CHARGE_CURRENCY_CODE','PRODUCT_ALLOWANCE_CHARGE_IND','PRODUCT_ALLOWANCE_ACTUAL_AMOUNT','PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE','PRODUCT_ALLOWANCE_REASON_CODE','PRODUCT_ALLOWANCE_REASON','PRODUCT_QUANTITY','PRODUCT_UNIT_CODE','PRODUCT_QUANTITY_PER_UNIT','LINE_TAX_TYPE_CODE','LINE_TAX_CAL_RATE','LINE_BASIS_AMOUNT','LINE_BASIS_CURRENCY_CODE','LINE_TAX_CAL_AMOUNT','LINE_TAX_CAL_CURRENCY_CODE','LINE_ALLOWANCE_CHARGE_IND','LINE_ALLOWANCE_ACTUAL_AMOUNT','LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE','LINE_ALLOWANCE_REASON_CODE','LINE_ALLOWANCE_REASON','LINE_TAX_TOTAL_AMOUNT','LINE_TAX_TOTAL_CURRENCY_CODE','LINE_NET_TOTAL_AMOUNT','LINE_NET_TOTAL_CURRENCY_CODE','LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT','LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE','PRODUCT_REMARK1','PRODUCT_REMARK2','PRODUCT_REMARK3','PRODUCT_REMARK4','PRODUCT_REMARK5','PRODUCT_REMARK6','PRODUCT_REMARK7','PRODUCT_REMARK8','PRODUCT_REMARK9','PRODUCT_REMARK10'
            //'F','LINE_TOTAL_COUNT','DELIVERY_OCCUR_DTM','INVOICE_CURRENCY_CODE','TAX_TYPE_CODE1','TAX_CAL_RATE1','BASIS_AMOUNT1','BASIS_CURRENCY_CODE1','TAX_CAL_AMOUNT1','TAX_CAL_CURRENCY_CODE1','TAX_TYPE_CODE2','TAX_CAL_RATE2','BASIS_AMOUNT2','BASIS_CURRENCY_CODE2','TAX_CAL_AMOUNT2','TAX_CAL_CURRENCY_CODE2','TAX_TYPE_CODE3','TAX_CAL_RATE3','BASIS_AMOUNT3','BASIS_CURRENCY_CODE3','TAX_CAL_AMOUNT3','TAX_CAL_CURRENCY_CODE3','TAX_TYPE_CODE4','TAX_CAL_RATE4','BASIS_AMOUNT4','BASIS_CURRENCY_CODE4','TAX_CAL_AMOUNT4','TAX_CAL_CURRENCY_CODE4','ALLOWANCE_CHARGE_IND','ALLOWANCE_ACTUAL_AMOUNT','ALLOWANCE_ACTUAL_CURRENCY_CODE','ALLOWANCE_REASON_CODE','ALLOWANCE_REASON','PAYMENT_TYPE_CODE','PAYMENT_DESCRIPTION','PAYMENT_DUE_DTM','ORIGINAL_TOTAL_AMOUNT','ORIGINAL_TOTAL_CURRENCY_CODE','LINE_TOTAL_AMOUNT','LINE_TOTAL_CURRENCY_CODE','ADJUSTED_INFORMATION_AMOUNT','ADJUSTED_INFORMATION_CURRENCY_CODE','ALLOWANCE_TOTAL_AMOUNT','ALLOWANCE_TOTAL_CURRENCY_CODE','CHARGE_TOTAL_AMOUNT','CHARGE_TOTAL_CURRENCY_CODE','TAX_BASIS_TOTAL_AMOUNT','TAX_BASIS_TOTAL_CURRENCY_CODE','TAX_TOTAL_AMOUNT','TAX_TOTAL_CURRENCY_CODE','GRAND_TOTAL_AMOUNT','GRAND_TOTAL_CURRENCY_CODE','TERM_PAYMENT','WITHHOLDINGTAX_TYPE1','WITHHOLDINGTAX_DESCRIPTION1','WITHHOLDINGTAX_RATE1','WITHHOLDINGTAX_BASIS_AMOUNT1','WITHHOLDINGTAX_TAX_AMOUNT1','WITHHOLDINGTAX_TYPE2','WITHHOLDINGTAX_DESCRIPTION2','WITHHOLDINGTAX_RATE2','WITHHOLDINGTAX_BASIS_AMOUNT2','WITHHOLDINGTAX_TAX_AMOUNT2','WITHHOLDINGTAX_TYPE3','WITHHOLDINGTAX_DESCRIPTION3','WITHHOLDINGTAX_RATE3','WITHHOLDINGTAX_BASIS_AMOUNT3','WITHHOLDINGTAX_TAX_AMOUNT3','WITHHOLDINGTAX_TOTAL_AMOUNT','ACTUAL_PAYMENT_TOTAL_AMOUNT','DOCUMENT_REMARK1','DOCUMENT_REMARK2','DOCUMENT_REMARK3','DOCUMENT_REMARK4','DOCUMENT_REMARK5','DOCUMENT_REMARK6','DOCUMENT_REMARK7','DOCUMENT_REMARK8','DOCUMENT_REMARK9','DOCUMENT_REMARK10','DOCUMENT_REMARK11'
            //'T','1'
            //";


            //### version Mini text file ทางลูกค้าให้ใช้ spec ใหม่นี้  02/01/2024
            csv.AppendLine("\"C\",\"SELLER_TAX_ID\",\"SELLER_BRANCH_ID\",\"FILE_NAME\"");
            csv.AppendLine("\"H\",\"DOCUMENT_TYPE_CODE\",\"DOCUMENT_NAME\",\"DOCUMENT_ID\",\"DOCUMENT_ISSUE_DTM\",\"CREATE_PURPOSE_CODE\",\"CREATE_PURPOSE\",\"ADDITIONAL_REF_ASSIGN_ID\",\"ADDITIONAL_REF_ISSUE_DTM\",\"ADDITIONAL_REF_TYPE_CODE\",\"ADDITIONAL_REF_DOCUMENT_NAME\",\"DELIVERY_TYPE_CODE\",\"BUYER_ORDER_ASSIGN_ID\",\"BUYER_ORDER_ISSUE_DTM\",\"BUYER_ORDER_REF_TYPE_CODE\",\"DOCUMENT_REMARK\",\"VOUCHER_NO\",\"SELLER_CONTACT_PERSON_NAME\",\"SELLER_CONTACT_DEPARTMENT_NAME\",\"SELLER_CONTACT_URIID\",\"SELLER_CONTACT_PHONE_NO\",\"FLEX_FIELD\",\"SELLER_BRANCH_ID\",\"SOURCE_SYSTEM\",\"ENCRYPT_PASSWORD\",\"PDF_TEMPLATE_ID\",\"SEND_MAIL_IND\"");
            csv.AppendLine("\"B\",\"BUYER_ID\",\"BUYER_NAME\",\"BUYER_TAX_ID_TYPE\",\"BUYER_TAX_ID\",\"BUYER_BRANCH_ID\",\"BUYER_CONTACT_PERSON_NAME\",\"BUYER_CONTACT_DEPARTMENT_NAME\",\"BUYER_URIID\",\"BUYER_CONTACT_PHONE_NO\",\"BUYER_POST_CODE\",\"BUYER_BUILDING_NAME\",\"BUYER_BUILDING_NO\",\"BUYER_ADDRESS_LINE1\",\"BUYER_ADDRESS_LINE2\",\"BUYER_ADDRESS_LINE3\",\"BUYER_ADDRESS_LINE4\",\"BUYER_ADDRESS_LINE5\",\"BUYER_STREET_NAME\",\"BUYER_CITY_SUB_DIV_ID\",\"BUYER_CITY_SUB_DIV_NAME\",\"BUYER_CITY_ID\",\"BUYER_CITY_NAME\",\"BUYER_COUNTRY_SUB_DIV_ID\",\"BUYER_COUNTRY_SUB_DIV_NAME\",\"BUYER_COUNTRY_ID\"");
            csv.AppendLine("\"L\",\"LINE_ID\",\"PRODUCT_ID\",\"PRODUCT_NAME\",\"PRODUCT_DESC\",\"PRODUCT_BATCH_ID\",\"PRODUCT_EXPIRE_DTM\",\"PRODUCT_CLASS_CODE\",\"PRODUCT_CLASS_NAME\",\"PRODUCT_ORIGIN_COUNTRY_ID\",\"PRODUCT_CHARGE_AMOUNT\",\"PRODUCT_CHARGE_CURRENCY_CODE\",\"PRODUCT_ALLOWANCE_CHARGE_IND\",\"PRODUCT_ALLOWANCE_ACTUAL_AMOUNT\",\"PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"PRODUCT_ALLOWANCE_REASON_CODE\",\"PRODUCT_ALLOWANCE_REASON\",\"PRODUCT_QUANTITY\",\"PRODUCT_UNIT_CODE\",\"PRODUCT_QUANTITY_PER_UNIT\",\"LINE_TAX_TYPE_CODE\",\"LINE_TAX_CAL_RATE\",\"LINE_BASIS_AMOUNT\",\"LINE_BASIS_CURRENCY_CODE\",\"LINE_TAX_CAL_AMOUNT\",\"LINE_TAX_CAL_CURRENCY_CODE\",\"LINE_ALLOWANCE_CHARGE_IND\",\"LINE_ALLOWANCE_ACTUAL_AMOUNT\",\"LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"LINE_ALLOWANCE_REASON_CODE\",\"LINE_ALLOWANCE_REASON\",\"LINE_TAX_TOTAL_AMOUNT\",\"LINE_TAX_TOTAL_CURRENCY_CODE\",\"LINE_NET_TOTAL_AMOUNT\",\"LINE_NET_TOTAL_CURRENCY_CODE\",\"LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT\",\"LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE\",\"PRODUCT_REMARK\"");
            csv.AppendLine("\"F\",\"LINE_TOTAL_COUNT\",\"DELIVERY_OCCUR_DTM\",\"INVOICE_CURRENCY_CODE\",\"TAX_TYPE_CODE\",\"TAX_CAL_RATE\",\"BASIS_AMOUNT\",\"BASIS_CURRENCY_CODE\",\"TAX_CAL_AMOUNT\",\"TAX_CAL_CURRENCY_CODE\",\"ALLOWANCE_CHARGE_IND\",\"ALLOWANCE_ACTUAL_AMOUNT\",\"ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"ALLOWANCE_REASON_CODE\",\"ALLOWANCE_REASON\",\"PAYMENT_TYPE_CODE\",\"PAYMENT_DESCRIPTION\",\"PAYMENT_DUE_DTM\",\"ORIGINAL_TOTAL_AMOUNT\",\"ORIGINAL_TOTAL_CURRENCY_CODE\",\"LINE_TOTAL_AMOUNT\",\"LINE_TOTAL_CURRENCY_CODE\",\"ADJUSTED_INFORMATION_AMOUNT\",\"ADJUSTED_INFORMATION_CURRENCY_CODE\",\"ALLOWANCE_TOTAL_AMOUNT\",\"ALLOWANCE_TOTAL_CURRENCY_CODE\",\"CHARGE_TOTAL_AMOUNT\",\"CHARGE_TOTAL_CURRENCY_CODE\",\"TAX_BASIS_TOTAL_AMOUNT\",\"TAX_BASIS_TOTAL_CURRENCY_CODE\",\"TAX_TOTAL_AMOUNT\",\"TAX_TOTAL_CURRENCY_CODE\",\"GRAND_TOTAL_AMOUNT\",\"GRAND_TOTAL_CURRENCY_CODE\",\"TERM_PAYMENT\"");
            csv.Append("\"T\",\"1\"");

            string text = csv.ToString();
            foreach (System.Data.DataColumn dc in dr.Table.Columns)
            {
                if (_TEST_RECORD != "" && dc.ColumnName.ToUpper() == "BUYER_URIID")
                {
                    text = text.Replace(dc.ColumnName, System.Configuration.ConfigurationManager.AppSettings["Env:TestEmailAddress"].ToString());
                }
                else
                {
                    text = text.Replace(dc.ColumnName, dr[dc.ColumnName].ToString().Replace("\"", " "));
                }
            }

            return text;
        }

        private static string GetCsvString_ForSAP(Dictionary<string, object> dr)
        {
            System.Text.StringBuilder csv = new StringBuilder();
            csv.AppendLine("\"C\",\"C_SELLER_TAX_ID\",\"C_SELLER_BRANCH_ID\",\"C_FILE_NAME\"");
            csv.AppendLine("\"H\",\"H_DOCUMENT_TYPE_CODE\",\"H_DOCUMENT_NAME\",\"H_DOCUMENT_ID\",\"H_DOCUMENT_ISSUE_DTM\",\"H_CREATE_PURPOSE_CODE\",\"H_CREATE_PURPOSE\",\"H_ADDITIONAL_REF_ASSIGN_ID\",\"H_ADDITIONAL_REF_ISSUE_DTM\",\"H_ADDITIONAL_REF_TYPE_CODE\",\"H_ADDITIONAL_REF_DOCUMENT_NAME\",\"H_DELIVERY_TYPE_CODE\",\"H_BUYER_ORDER_ASSIGN_ID\",\"H_BUYER_ORDER_ISSUE_DTM\",\"H_BUYER_ORDER_REF_TYPE_CODE\",\"H_DOCUMENT_REMARK\",\"H_VOUCHER_NO\",\"H_SELLER_CONTACT_PERSON_NAME\",\"H_SELLER_CONTACT_DEPARTMENT_NAME\",\"H_SELLER_CONTACT_URIID\",\"H_SELLER_CONTACT_PHONE_NO\",\"H_FLEX_FIELD\",\"H_SELLER_BRANCH_ID\",\"H_SOURCE_SYSTEM\",\"H_ENCRYPT_PASSWORD\",\"H_PDF_TEMPLATE_ID\",\"H_SEND_MAIL_IND\"");
            csv.AppendLine("\"B\",\"B_BUYER_ID\",\"B_BUYER_NAME\",\"B_BUYER_TAX_ID_TYPE\",\"B_BUYER_TAX_ID\",\"B_BUYER_BRANCH_ID\",\"B_BUYER_CONTACT_PERSON_NAME\",\"B_BUYER_CONTACT_DEPARTMENT_NAME\",\"B_BUYER_URIID\",\"B_BUYER_CONTACT_PHONE_NO\",\"B_BUYER_POST_CODE\",\"B_BUYER_BUILDING_NAME\",\"B_BUYER_BUILDING_NO\",\"B_BUYER_ADDRESS_LINE1\",\"B_BUYER_ADDRESS_LINE2\",\"B_BUYER_ADDRESS_LINE3\",\"B_BUYER_ADDRESS_LINE4\",\"B_BUYER_ADDRESS_LINE5\",\"B_BUYER_STREET_NAME\",\"B_BUYER_CITY_SUB_DIV_ID\",\"B_BUYER_CITY_SUB_DIV_NAME\",\"B_BUYER_CITY_ID\",\"B_BUYER_CITY_NAME\",\"B_BUYER_COUNTRY_SUB_DIV_ID\",\"B_BUYER_COUNTRY_SUB_DIV_NAME\",\"B_BUYER_COUNTRY_ID\"");
            csv.AppendLine("\"L\",\"L_LINE_ID\",\"L_PRODUCT_ID\",\"L_PRODUCT_NAME\",\"L_PRODUCT_DESC\",\"L_PRODUCT_BATCH_ID\",\"L_PRODUCT_EXPIRE_DTM\",\"L_PRODUCT_CLASS_CODE\",\"L_PRODUCT_CLASS_NAME\",\"L_PRODUCT_ORIGIN_COUNTRY_ID\",\"L_PRODUCT_CHARGE_AMOUNT\",\"L_PRODUCT_CHARGE_CURRENCY_CODE\",\"L_PRODUCT_ALLOWANCE_CHARGE_IND\",\"L_PRODUCT_ALLOWANCE_ACTUAL_AMOUNT\",\"L_PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"L_PRODUCT_ALLOWANCE_REASON_CODE\",\"L_PRODUCT_ALLOWANCE_REASON\",\"L_PRODUCT_QUANTITY\",\"L_PRODUCT_UNIT_CODE\",\"L_PRODUCT_QUANTITY_PER_UNIT\",\"L_LINE_TAX_TYPE_CODE\",\"L_LINE_TAX_CAL_RATE\",\"L_LINE_BASIS_AMOUNT\",\"L_LINE_BASIS_CURRENCY_CODE\",\"L_LINE_TAX_CAL_AMOUNT\",\"L_LINE_TAX_CAL_CURRENCY_CODE\",\"L_LINE_ALLOWANCE_CHARGE_IND\",\"L_LINE_ALLOWANCE_ACTUAL_AMOUNT\",\"L_LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"L_LINE_ALLOWANCE_REASON_CODE\",\"L_LINE_ALLOWANCE_REASON\",\"L_LINE_TAX_TOTAL_AMOUNT\",\"L_LINE_TAX_TOTAL_CURRENCY_CODE\",\"L_LINE_NET_TOTAL_AMOUNT\",\"L_LINE_NET_TOTAL_CURRENCY_CODE\",\"L_LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT\",\"L_LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE\",\"L_PRODUCT_REMARK\"");
            csv.AppendLine("\"F\",\"F_LINE_TOTAL_COUNT\",\"F_DELIVERY_OCCUR_DTM\",\"F_INVOICE_CURRENCY_CODE\",\"F_TAX_TYPE_CODE\",\"F_TAX_CAL_RATE\",\"F_BASIS_AMOUNT\",\"F_BASIS_CURRENCY_CODE\",\"F_TAX_CAL_AMOUNT\",\"F_TAX_CAL_CURRENCY_CODE\",\"F_ALLOWANCE_CHARGE_IND\",\"F_ALLOWANCE_ACTUAL_AMOUNT\",\"F_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"F_ALLOWANCE_REASON_CODE\",\"F_ALLOWANCE_REASON\",\"F_PAYMENT_TYPE_CODE\",\"F_PAYMENT_DESCRIPTION\",\"F_PAYMENT_DUE_DTM\",\"F_ORIGINAL_TOTAL_AMOUNT\",\"F_ORIGINAL_TOTAL_CURRENCY_CODE\",\"F_LINE_TOTAL_AMOUNT\",\"F_LINE_TOTAL_CURRENCY_CODE\",\"F_ADJUSTED_INFORMATION_AMOUNT\",\"F_ADJUSTED_INFORMATION_CURRENCY_CODE\",\"F_ALLOWANCE_TOTAL_AMOUNT\",\"F_ALLOWANCE_TOTAL_CURRENCY_CODE\",\"F_CHARGE_TOTAL_AMOUNT\",\"F_CHARGE_TOTAL_CURRENCY_CODE\",\"F_TAX_BASIS_TOTAL_AMOUNT\",\"F_TAX_BASIS_TOTAL_CURRENCY_CODE\",\"F_TAX_TOTAL_AMOUNT\",\"F_TAX_TOTAL_CURRENCY_CODE\",\"F_GRAND_TOTAL_AMOUNT\",\"F_GRAND_TOTAL_CURRENCY_CODE\",\"F_TERM_PAYMENT\"");
            csv.Append("\"T\",\"1\"");

            string text = csv.ToString();
            string propName = "", propVal = "";
            foreach (KeyValuePair<string, object> entry in dr)
            {
                //propName = entry.Key.Replace("C_", "").Replace("H_", "").Replace("B_", "").Replace("L_", "").Replace("F_", "").Replace("T_", "");
                //text = text.Replace(propName, entry.Value.ToString().Replace("\"", " "));

                if (entry.Value == null)
                {
                    text = text.Replace(entry.Key, "");
                }
                else if (_TEST_RECORD != "" && entry.Key.ToUpper() == "B_BUYER_URIID")
                {
                    text = text.Replace(entry.Key, System.Configuration.ConfigurationManager.AppSettings["Env:TestEmailAddress"].ToString());
                }
                else
                {
                    text = text.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                }
            }

            text = text.Replace("_PER_UNIT", "");

            return text;
        }

        private static string GetCsvString_ForSAP2(System.Data.DataRow[] drItems)
        {
            System.Text.StringBuilder csv = new StringBuilder();
            System.Text.StringBuilder tmp = new StringBuilder();
            tmp.AppendLine("\"C\",\"C_SELLER_TAX_ID\",\"C_SELLER_BRANCH_ID\",\"C_FILE_NAME\"");
            tmp.AppendLine("\"H\",\"H_DOCUMENT_TYPE_CODE\",\"H_DOCUMENT_NAME\",\"H_DOCUMENT_ID\",\"H_DOCUMENT_ISSUE_DTM\",\"H_CREATE_PURPOSE_CODE\",\"H_CREATE_PURPOSE\",\"H_ADDITIONAL_REF_ASSIGN_ID\",\"H_ADDITIONAL_REF_ISSUE_DTM\",\"H_ADDITIONAL_REF_TYPE_CODE\",\"H_ADDITIONAL_REF_DOCUMENT_NAME\",\"H_DELIVERY_TYPE_CODE\",\"H_BUYER_ORDER_ASSIGN_ID\",\"H_BUYER_ORDER_ISSUE_DTM\",\"H_BUYER_ORDER_REF_TYPE_CODE\",\"H_DOCUMENT_REMARK\",\"H_VOUCHER_NO\",\"H_SELLER_CONTACT_PERSON_NAME\",\"H_SELLER_CONTACT_DEPARTMENT_NAME\",\"H_SELLER_CONTACT_URIID\",\"H_SELLER_CONTACT_PHONE_NO\",\"H_FLEX_FIELD\",\"H_SELLER_BRANCH_ID\",\"H_SOURCE_SYSTEM\",\"H_ENCRYPT_PASSWORD\",\"H_PDF_TEMPLATE_ID\",\"H_SEND_MAIL_IND\"");
            tmp.AppendLine("\"B\",\"B_BUYER_ID\",\"B_BUYER_NAME\",\"B_BUYER_TAX_ID_TYPE\",\"B_BUYER_TAX_ID\",\"B_BUYER_BRANCH_ID\",\"B_BUYER_CONTACT_PERSON_NAME\",\"B_BUYER_CONTACT_DEPARTMENT_NAME\",\"B_BUYER_URIID\",\"B_BUYER_CONTACT_PHONE_NO\",\"B_BUYER_POST_CODE\",\"B_BUYER_BUILDING_NAME\",\"B_BUYER_BUILDING_NO\",\"B_BUYER_ADDRESS_LINE1\",\"B_BUYER_ADDRESS_LINE2\",\"B_BUYER_ADDRESS_LINE3\",\"B_BUYER_ADDRESS_LINE4\",\"B_BUYER_ADDRESS_LINE5\",\"B_BUYER_STREET_NAME\",\"B_BUYER_CITY_SUB_DIV_ID\",\"B_BUYER_CITY_SUB_DIV_NAME\",\"B_BUYER_CITY_ID\",\"B_BUYER_CITY_NAME\",\"B_BUYER_COUNTRY_SUB_DIV_ID\",\"B_BUYER_COUNTRY_SUB_DIV_NAME\",\"B_BUYER_COUNTRY_ID\"");
            string text = tmp.ToString();

            Dictionary<string, object> dr = ICON.Utilities.Convert.ConvertRowToClass(drItems[0]);
            foreach (KeyValuePair<string, object> entry in dr)
            {
                //propName = entry.Key.Replace("C_", "").Replace("H_", "").Replace("B_", "").Replace("L_", "").Replace("F_", "").Replace("T_", "");
                //text = text.Replace(propName, entry.Value.ToString().Replace("\"", " "));

                if (entry.Value == null)
                {
                    text = text.Replace(entry.Key, "");
                }
                else
                {
                    text = text.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                }
            }
            csv.Append(text);

            // loop สร้างบรรทัด item
            string text2 = "";
            foreach (System.Data.DataRow line in drItems)
            {
                dr = ICON.Utilities.Convert.ConvertRowToClass(line);
                text2 = "\"L\",\"L_LINE_ID\",\"L_PRODUCT_ID\",\"L_PRODUCT_NAME\",\"L_PRODUCT_DESC\",\"L_PRODUCT_BATCH_ID\",\"L_PRODUCT_EXPIRE_DTM\",\"L_PRODUCT_CLASS_CODE\",\"L_PRODUCT_CLASS_NAME\",\"L_PRODUCT_ORIGIN_COUNTRY_ID\",\"L_PRODUCT_CHARGE_AMOUNT\",\"L_PRODUCT_CHARGE_CURRENCY_CODE\",\"L_PRODUCT_ALLOWANCE_CHARGE_IND\",\"L_PRODUCT_ALLOWANCE_ACTUAL_AMOUNT\",\"L_PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"L_PRODUCT_ALLOWANCE_REASON_CODE\",\"L_PRODUCT_ALLOWANCE_REASON\",\"L_PRODUCT_QUANTITY\",\"L_PRODUCT_UNIT_CODE\",\"L_PRODUCT_QUANTITY_PER_UNIT\",\"L_LINE_TAX_TYPE_CODE\",\"L_LINE_TAX_CAL_RATE\",\"L_LINE_BASIS_AMOUNT\",\"L_LINE_BASIS_CURRENCY_CODE\",\"L_LINE_TAX_CAL_AMOUNT\",\"L_LINE_TAX_CAL_CURRENCY_CODE\",\"L_LINE_ALLOWANCE_CHARGE_IND\",\"L_LINE_ALLOWANCE_ACTUAL_AMOUNT\",\"L_LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"L_LINE_ALLOWANCE_REASON_CODE\",\"L_LINE_ALLOWANCE_REASON\",\"L_LINE_TAX_TOTAL_AMOUNT\",\"L_LINE_TAX_TOTAL_CURRENCY_CODE\",\"L_LINE_NET_TOTAL_AMOUNT\",\"L_LINE_NET_TOTAL_CURRENCY_CODE\",\"L_LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT\",\"L_LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE\",\"L_PRODUCT_REMARK\"";
                foreach (KeyValuePair<string, object> entry in dr)
                {
                    if (entry.Value == null)
                    {
                        text2 = text2.Replace(entry.Key, "");
                    }
                    else if (_TEST_RECORD != "" && entry.Key.ToUpper() == "B_BUYER_URIID")
                    {
                        text2 = text2.Replace(entry.Key, System.Configuration.ConfigurationManager.AppSettings["Env:TestEmailAddress"].ToString());
                    }
                    else
                    {
                        text2 = text2.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                    }
                }

                text2 = text2.Replace("_PER_UNIT", "");
                csv.AppendLine(text2);
            }

            text2 = "\"F\",\"F_LINE_TOTAL_COUNT\",\"F_DELIVERY_OCCUR_DTM\",\"F_INVOICE_CURRENCY_CODE\",\"F_TAX_TYPE_CODE\",\"F_TAX_CAL_RATE\",\"F_BASIS_AMOUNT\",\"F_BASIS_CURRENCY_CODE\",\"F_TAX_CAL_AMOUNT\",\"F_TAX_CAL_CURRENCY_CODE\",\"F_ALLOWANCE_CHARGE_IND\",\"F_ALLOWANCE_ACTUAL_AMOUNT\",\"F_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"F_ALLOWANCE_REASON_CODE\",\"F_ALLOWANCE_REASON\",\"F_PAYMENT_TYPE_CODE\",\"F_PAYMENT_DESCRIPTION\",\"F_PAYMENT_DUE_DTM\",\"F_ORIGINAL_TOTAL_AMOUNT\",\"F_ORIGINAL_TOTAL_CURRENCY_CODE\",\"F_LINE_TOTAL_AMOUNT\",\"F_LINE_TOTAL_CURRENCY_CODE\",\"F_ADJUSTED_INFORMATION_AMOUNT\",\"F_ADJUSTED_INFORMATION_CURRENCY_CODE\",\"F_ALLOWANCE_TOTAL_AMOUNT\",\"F_ALLOWANCE_TOTAL_CURRENCY_CODE\",\"F_CHARGE_TOTAL_AMOUNT\",\"F_CHARGE_TOTAL_CURRENCY_CODE\",\"F_TAX_BASIS_TOTAL_AMOUNT\",\"F_TAX_BASIS_TOTAL_CURRENCY_CODE\",\"F_TAX_TOTAL_AMOUNT\",\"F_TAX_TOTAL_CURRENCY_CODE\",\"F_GRAND_TOTAL_AMOUNT\",\"F_GRAND_TOTAL_CURRENCY_CODE\",\"F_TERM_PAYMENT\"";
            foreach (KeyValuePair<string, object> entry in dr)
            {
                if (entry.Value == null)
                {
                    text2 = text2.Replace(entry.Key, "");
                }
                else if (entry.Key.ToUpper() == "F_LINE_TOTAL_COUNT")
                {
                    text2 = text2.Replace(entry.Key, drItems.Count().ToString());
                }
                else
                {
                    text2 = text2.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                }
            }
            csv.AppendLine(text2);
            csv.Append("\"T\",\"1\"");


            return csv.ToString();
        }

        private static string GetCsvString_ForSAP3(System.Data.DataRow[] drItems)
        {
            System.Text.StringBuilder csv = new StringBuilder();
            System.Text.StringBuilder tmp = new StringBuilder();

            //tmp.AppendLine("\"C\",\"SELLER_TAX_ID\",\"SELLER_BRANCH_ID\",\"FILE_NAME\"");
            //tmp.AppendLine("\"H\"\",\"DOCUMENT_TYPE_CODE\",\"DOCUMENT_NAME\",\"DOCUMENT_ID\",\"DOCUMENT_ISSUE_DTM\",\"CREATE_PURPOSE_CODE\",\"CREATE_PURPOSE\",\"ADDITIONAL_REF_ASSIGN_ID\",\"ADDITIONAL_REF_ISSUE_DTM\",\"ADDITIONAL_REF_TYPE_CODE\",\"ADDITIONAL_REF_DOCUMENT_NAME\",\"DELIVERY_TYPE_CODE\",\"BUYER_ORDER_ASSIGN_ID\",\"BUYER_ORDER_ISSUE_DTM\",\"BUYER_ORDER_REF_TYPE_CODE\",\"DOCUMENT_REMARK\",\"VOUCHER_NO\",\"SELLER_CONTACT_PERSON_NAME\",\"SELLER_CONTACT_DEPARTMENT_NAME\",\"SELLER_CONTACT_URIID\",\"SELLER_CONTACT_PHONE_NO\",\"FLEX_FIELD\",\"SELLER_BRANCH_ID\",\"SOURCE_SYSTEM\",\"ENCRYPT_PASSWORD\",\"PDF_TEMPLATE_ID\",\"SEND_MAIL_IND\",\"RETURN_FILE_NAME\"");
            //tmp.AppendLine("\"B\",\"BUYER_ID\",\"BUYER_NAME\",\"BUYER_TAX_ID_TYPE\",\"BUYER_TAX_ID\",\"BUYER_BRANCH_ID\",\"BUYER_CONTACT_PERSON_NAME\",\"BUYER_CONTACT_DEPARTMENT_NAME\",\"BUYER_URIID\",\"BUYER_CONTACT_PHONE_NO\",\"BUYER_POST_CODE\",\"BUYER_BUILDING_NAME\",\"BUYER_BUILDING_NO\",\"BUYER_ADDRESS_LINE1\",\"BUYER_ADDRESS_LINE2\",\"BUYER_ADDRESS_LINE3\",\"BUYER_ADDRESS_LINE4\",\"BUYER_ADDRESS_LINE5\",\"BUYER_STREET_NAME\",\"BUYER_CITY_SUB_DIV_ID\",\"BUYER_CITY_SUB_DIV_NAME\",\"BUYER_CITY_ID\",\"BUYER_CITY_NAME\",\"BUYER_COUNTRY_SUB_DIV_ID\",\"BUYER_COUNTRY_SUB_DIV_NAME\",\"BUYER_COUNTRY_ID\"");
            //tmp.AppendLine("\"L\",\"LINE_ID\",\"PRODUCT_ID\",\"PRODUCT_NAME\",\"PRODUCT_DESC\",\"PRODUCT_BATCH_ID\",\"PRODUCT_EXPIRE_DTM\",\"PRODUCT_CLASS_CODE\",\"PRODUCT_CLASS_NAME\",\"PRODUCT_ORIGIN_COUNTRY_ID\",\"PRODUCT_CHARGE_AMOUNT\",\"PRODUCT_CHARGE_CURRENCY_CODE\",\"PRODUCT_ALLOWANCE_CHARGE_IND\",\"PRODUCT_ALLOWANCE_ACTUAL_AMOUNT\",\"PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"PRODUCT_ALLOWANCE_REASON_CODE\",\"PRODUCT_ALLOWANCE_REASON\",\"PRODUCT_QUANTITY\",\"PRODUCT_UNIT_CODE\",\"PRODUCT_QUANTITY_PER_UNIT\",\"LINE_TAX_TYPE_CODE\",\"LINE_TAX_CAL_RATE\",\"LINE_BASIS_AMOUNT\",\"LINE_BASIS_CURRENCY_CODE\",\"LINE_TAX_CAL_AMOUNT\",\"LINE_TAX_CAL_CURRENCY_CODE\",\"LINE_ALLOWANCE_CHARGE_IND\",\"LINE_ALLOWANCE_ACTUAL_AMOUNT\",\"LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"LINE_ALLOWANCE_REASON_CODE\",\"LINE_ALLOWANCE_REASON\",\"LINE_TAX_TOTAL_AMOUNT\",\"LINE_TAX_TOTAL_CURRENCY_CODE\",\"LINE_NET_TOTAL_AMOUNT\",\"LINE_NET_TOTAL_CURRENCY_CODE\",\"LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT\",\"LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE\",\"PRODUCT_REMARK1\",\"PRODUCT_REMARK2\",\"PRODUCT_REMARK3\",\"PRODUCT_REMARK4\",\"PRODUCT_REMARK5\",\"PRODUCT_REMARK6\",\"PRODUCT_REMARK7\",\"PRODUCT_REMARK8\",\"PRODUCT_REMARK9\",\"PRODUCT_REMARK10\"");
            //tmp.AppendLine("\"F\",\"LINE_TOTAL_COUNT\",\"DELIVERY_OCCUR_DTM\",\"INVOICE_CURRENCY_CODE\",\"TAX_TYPE_CODE1\",\"TAX_CAL_RATE1\",\"BASIS_AMOUNT1\",\"BASIS_CURRENCY_CODE1\",\"TAX_CAL_AMOUNT1\",\"TAX_CAL_CURRENCY_CODE1\",\"TAX_TYPE_CODE2\",\"TAX_CAL_RATE2\",\"BASIS_AMOUNT2\",\"BASIS_CURRENCY_CODE2\",\"TAX_CAL_AMOUNT2\",\"TAX_CAL_CURRENCY_CODE2\",\"TAX_TYPE_CODE3\",\"TAX_CAL_RATE3\",\"BASIS_AMOUNT3\",\"BASIS_CURRENCY_CODE3\",\"TAX_CAL_AMOUNT3\",\"TAX_CAL_CURRENCY_CODE3\",\"TAX_TYPE_CODE4\",\"TAX_CAL_RATE4\",\"BASIS_AMOUNT4\",\"BASIS_CURRENCY_CODE4\",\"TAX_CAL_AMOUNT4\",\"TAX_CAL_CURRENCY_CODE4\",\"ALLOWANCE_CHARGE_IND\",\"ALLOWANCE_ACTUAL_AMOUNT\",\"ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"ALLOWANCE_REASON_CODE\",\"ALLOWANCE_REASON\",\"PAYMENT_TYPE_CODE\",\"PAYMENT_DESCRIPTION\",\"PAYMENT_DUE_DTM\",\"ORIGINAL_TOTAL_AMOUNT\",\"ORIGINAL_TOTAL_CURRENCY_CODE\",\"LINE_TOTAL_AMOUNT\",\"LINE_TOTAL_CURRENCY_CODE\",\"ADJUSTED_INFORMATION_AMOUNT\",\"ADJUSTED_INFORMATION_CURRENCY_CODE\",\"ALLOWANCE_TOTAL_AMOUNT\",\"ALLOWANCE_TOTAL_CURRENCY_CODE\",\"CHARGE_TOTAL_AMOUNT\",\"CHARGE_TOTAL_CURRENCY_CODE\",\"TAX_BASIS_TOTAL_AMOUNT\",\"TAX_BASIS_TOTAL_CURRENCY_CODE\",\"TAX_TOTAL_AMOUNT\",\"TAX_TOTAL_CURRENCY_CODE\",\"GRAND_TOTAL_AMOUNT\",\"GRAND_TOTAL_CURRENCY_CODE\",\"TERM_PAYMENT\",\"WITHHOLDINGTAX_TYPE1\",\"WITHHOLDINGTAX_DESCRIPTION1\",\"WITHHOLDINGTAX_RATE1\",\"WITHHOLDINGTAX_BASIS_AMOUNT1\",\"WITHHOLDINGTAX_TAX_AMOUNT1\",\"WITHHOLDINGTAX_TYPE2\",\"WITHHOLDINGTAX_DESCRIPTION2\",\"WITHHOLDINGTAX_RATE2\",\"WITHHOLDINGTAX_BASIS_AMOUNT2\",\"WITHHOLDINGTAX_TAX_AMOUNT2\",\"WITHHOLDINGTAX_TYPE3\",\"WITHHOLDINGTAX_DESCRIPTION3\",\"WITHHOLDINGTAX_RATE3\",\"WITHHOLDINGTAX_BASIS_AMOUNT3\",\"WITHHOLDINGTAX_TAX_AMOUNT3\",\"WITHHOLDINGTAX_TOTAL_AMOUNT\",\"ACTUAL_PAYMENT_TOTAL_AMOUNT\",\"DOCUMENT_REMARK1\",\"DOCUMENT_REMARK2\",\"DOCUMENT_REMARK3\",\"DOCUMENT_REMARK4\",\"DOCUMENT_REMARK5\",\"DOCUMENT_REMARK6\",\"DOCUMENT_REMARK7\",\"DOCUMENT_REMARK8\",\"DOCUMENT_REMARK9\",\"DOCUMENT_REMARK10\",\"DOCUMENT_REMARK11\"");

            tmp.AppendLine("\"C\",\"C_SELLER_TAX_ID\",\"C_SELLER_BRANCH_ID\",\"C_FILE_NAME\"");
            tmp.AppendLine("\"H\",\"H_DOCUMENT_TYPE_CODE\",\"H_DOCUMENT_NAME\",\"H_DOCUMENT_ID\",\"H_DOCUMENT_ISSUE_DTM\",\"H_CREATE_PURPOSE_CODE\",\"H_CREATE_PURPOSE\",\"H_ADDITIONAL_REF_ASSIGN_ID\",\"H_ADDITIONAL_REF_ISSUE_DTM\",\"H_ADDITIONAL_REF_TYPE_CODE\",\"H_ADDITIONAL_REF_DOCUMENT_NAME\",\"H_DELIVERY_TYPE_CODE\",\"H_BUYER_ORDER_ASSIGN_ID\",\"H_BUYER_ORDER_ISSUE_DTM\",\"H_BUYER_ORDER_REF_TYPE_CODE\",\"H_DOCUMENT_REMARK\",\"H_VOUCHER_NO\",\"H_SELLER_CONTACT_PERSON_NAME\",\"H_SELLER_CONTACT_DEPARTMENT_NAME\",\"H_SELLER_CONTACT_URIID\",\"H_SELLER_CONTACT_PHONE_NO\",\"H_FLEX_FIELD\",\"H_SELLER_BRANCH_ID\",\"H_SOURCE_SYSTEM\",\"H_ENCRYPT_PASSWORD\",\"H_PDF_TEMPLATE_ID\",\"H_SEND_MAIL_IND\",\"H_PDF_NAME\"");
            tmp.AppendLine("\"B\",\"B_BUYER_ID\",\"B_BUYER_NAME\",\"B_BUYER_TAX_ID_TYPE\",\"B_BUYER_TAX_ID\",\"B_BUYER_BRANCH_ID\",\"B_BUYER_CONTACT_PERSON_NAME\",\"B_BUYER_CONTACT_DEPARTMENT_NAME\",\"B_BUYER_URIID\",\"B_BUYER_CONTACT_PHONE_NO\",\"B_BUYER_POST_CODE\",\"B_BUYER_BUILDING_NAME\",\"B_BUYER_BUILDING_NO\",\"B_BUYER_ADDRESS_LINE1\",\"B_BUYER_ADDRESS_LINE2\",\"B_BUYER_ADDRESS_LINE3\",\"B_BUYER_ADDRESS_LINE4\",\"B_BUYER_ADDRESS_LINE5\",\"B_BUYER_STREET_NAME\",\"B_BUYER_CITY_SUB_DIV_ID\",\"B_BUYER_CITY_SUB_DIV_NAME\",\"B_BUYER_CITY_ID\",\"B_BUYER_CITY_NAME\",\"B_BUYER_COUNTRY_SUB_DIV_ID\",\"B_BUYER_COUNTRY_SUB_DIV_NAME\",\"B_BUYER_COUNTRY_ID\"");
            string text = tmp.ToString();

            Dictionary<string, object> dr = ICON.Utilities.Convert.ConvertRowToClass(drItems[0]);
            foreach (KeyValuePair<string, object> entry in dr)
            {
                //propName = entry.Key.Replace("C_", "").Replace("H_", "").Replace("B_", "").Replace("L_", "").Replace("F_", "").Replace("T_", "");
                //text = text.Replace(propName, entry.Value.ToString().Replace("\"", " "));

                if (entry.Value == null)
                {
                    text = text.Replace(entry.Key, "");
                }
                else
                {
                    text = text.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                }
            }
            csv.Append(text);

            // loop สร้างบรรทัด item
            string text2 = "";
            foreach (System.Data.DataRow line in drItems)
            {
                dr = ICON.Utilities.Convert.ConvertRowToClass(line);
                text2 = "\"L\",\"L_LINE_ID\",\"L_PRODUCT_ID\",\"L_PRODUCT_NAME\",\"L_PRODUCT_DESC\",\"L_PRODUCT_BATCH_ID\",\"L_PRODUCT_EXPIRE_DTM\",\"L_PRODUCT_CLASS_CODE\",\"L_PRODUCT_CLASS_NAME\",\"L_PRODUCT_ORIGIN_COUNTRY_ID\",\"L_PRODUCT_CHARGE_AMOUNT\",\"L_PRODUCT_CHARGE_CURRENCY_CODE\",\"L_PRODUCT_ALLOWANCE_CHARGE_IND\",\"L_PRODUCT_ALLOWANCE_ACTUAL_AMOUNT\",\"L_PRODUCT_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"L_PRODUCT_ALLOWANCE_REASON_CODE\",\"L_PRODUCT_ALLOWANCE_REASON\",\"L_PRODUCT_QUANTITY\",\"L_PRODUCT_UNIT_CODE\",\"L_PRODUCT_QUANTITY_PER_UNIT\",\"L_LINE_TAX_TYPE_CODE\",\"L_LINE_TAX_CAL_RATE\",\"L_LINE_BASIS_AMOUNT\",\"L_LINE_BASIS_CURRENCY_CODE\",\"L_LINE_TAX_CAL_AMOUNT\",\"L_LINE_TAX_CAL_CURRENCY_CODE\",\"L_LINE_ALLOWANCE_CHARGE_IND\",\"L_LINE_ALLOWANCE_ACTUAL_AMOUNT\",\"L_LINE_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"L_LINE_ALLOWANCE_REASON_CODE\",\"L_LINE_ALLOWANCE_REASON\",\"L_LINE_TAX_TOTAL_AMOUNT\",\"L_LINE_TAX_TOTAL_CURRENCY_CODE\",\"L_LINE_NET_TOTAL_AMOUNT\",\"L_LINE_NET_TOTAL_CURRENCY_CODE\",\"L_LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT\",\"L_LINE_NET_INCLUDE_TAX_TOTAL_CURRENCY_CODE\",\"L_PRODUCT_REMARK1\",\"L_PRODUCT_REMARK2\",\"L_PRODUCT_REMARK3\",\"L_PRODUCT_REMARK4\",\"L_PRODUCT_REMARK5\",\"L_PRODUCT_REMARK6\",\"L_PRODUCT_REMARK7\",\"L_PRODUCT_REMARK8\",\"L_PRODUCT_REMARK9\",\"L_PRODUCT_REMARK10\"";
                foreach (KeyValuePair<string, object> entry in dr)
                {
                    if (entry.Value == null)
                    {
                        text2 = text2.Replace(entry.Key, "");
                    }
                    else if (_TEST_RECORD != "" && entry.Key.ToUpper() == "B_BUYER_URIID")
                    {
                        text2 = text2.Replace(entry.Key, System.Configuration.ConfigurationManager.AppSettings["Env:TestEmailAddress"].ToString());
                    }
                    else
                    {
                        text2 = text2.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                    }
                }

                text2 = text2.Replace("_PER_UNIT", "");
                csv.AppendLine(text2);
            }

            text2 = "\"F\",\"F_LINE_TOTAL_COUNT\",\"F_DELIVERY_OCCUR_DTM\",\"F_INVOICE_CURRENCY_CODE\",\"F_TAX_TYPE_CODE1\",\"F_TAX_CAL_RATE1\",\"F_BASIS_AMOUNT1\",\"F_BASIS_CURRENCY_CODE1\",\"F_TAX_CAL_AMOUNT1\",\"F_TAX_CAL_CURRENCY_CODE1\",\"F_TAX_TYPE_CODE2\",\"F_TAX_CAL_RATE2\",\"F_BASIS_AMOUNT2\",\"F_BASIS_CURRENCY_CODE2\",\"F_TAX_CAL_AMOUNT2\",\"F_TAX_CAL_CURRENCY_CODE2\",\"F_TAX_TYPE_CODE3\",\"F_TAX_CAL_RATE3\",\"F_BASIS_AMOUNT3\",\"F_BASIS_CURRENCY_CODE3\",\"F_TAX_CAL_AMOUNT3\",\"F_TAX_CAL_CURRENCY_CODE3\",\"F_TAX_TYPE_CODE4\",\"F_TAX_CAL_RATE4\",\"F_BASIS_AMOUNT4\",\"F_BASIS_CURRENCY_CODE4\",\"F_TAX_CAL_AMOUNT4\",\"F_TAX_CAL_CURRENCY_CODE4\",\"F_ALLOWANCE_CHARGE_IND\",\"F_ALLOWANCE_ACTUAL_AMOUNT\",\"F_ALLOWANCE_ACTUAL_CURRENCY_CODE\",\"F_ALLOWANCE_REASON_CODE\",\"F_ALLOWANCE_REASON\",\"F_PAYMENT_TYPE_CODE\",\"F_PAYMENT_DESCRIPTION\",\"F_PAYMENT_DUE_DTM\",\"F_ORIGINAL_TOTAL_AMOUNT\",\"F_ORIGINAL_TOTAL_CURRENCY_CODE\",\"F_LINE_TOTAL_AMOUNT\",\"F_LINE_TOTAL_CURRENCY_CODE\",\"F_ADJUSTED_INFORMATION_AMOUNT\",\"F_ADJUSTED_INFORMATION_CURRENCY_CODE\",\"F_ALLOWANCE_TOTAL_AMOUNT\",\"F_ALLOWANCE_TOTAL_CURRENCY_CODE\",\"F_CHARGE_TOTAL_AMOUNT\",\"F_CHARGE_TOTAL_CURRENCY_CODE\",\"F_TAX_BASIS_TOTAL_AMOUNT\",\"F_TAX_BASIS_TOTAL_CURRENCY_CODE\",\"F_TAX_TOTAL_AMOUNT\",\"F_TAX_TOTAL_CURRENCY_CODE\",\"F_GRAND_TOTAL_AMOUNT\",\"F_GRAND_TOTAL_CURRENCY_CODE\",\"F_TERM_PAYMENT\",\"F_WITHHOLDINGTAX_TYPE1\",\"F_WITHHOLDINGTAX_DESCRIPTION1\",\"F_WITHHOLDINGTAX_RATE1\",\"F_WITHHOLDINGTAX_BASIS_AMOUNT1\",\"F_WITHHOLDINGTAX_TAX_AMOUNT1\",\"F_WITHHOLDINGTAX_TYPE2\",\"F_WITHHOLDINGTAX_DESCRIPTION2\",\"F_WITHHOLDINGTAX_RATE2\",\"F_WITHHOLDINGTAX_BASIS_AMOUNT2\",\"F_WITHHOLDINGTAX_TAX_AMOUNT2\",\"F_WITHHOLDINGTAX_TYPE3\",\"F_WITHHOLDINGTAX_DESCRIPTION3\",\"F_WITHHOLDINGTAX_RATE3\",\"F_WITHHOLDINGTAX_BASIS_AMOUNT3\",\"F_WITHHOLDINGTAX_TAX_AMOUNT3\",\"F_WITHHOLDINGTAX_TOTAL_AMOUNT\",\"F_ACTUAL_PAYMENT_TOTAL_AMOUNT\",\"F_DOCUMENT_REMARK1\",\"F_DOCUMENT_REMARK2\",\"F_DOCUMENT_REMARK3\",\"F_DOCUMENT_REMARK4\",\"F_DOCUMENT_REMARK5\",\"F_DOCUMENT_REMARK6\",\"F_DOCUMENT_REMARK7\",\"F_DOCUMENT_REMARK8\",\"F_DOCUMENT_REMARK9\",\"F_DOCUMENT_REMARK10\",\"F_DOCUMENT_REMARK11\",\"F_DOCUMENT_REMARK12\",\"F_DOCUMENT_REMARK13\",\"F_DOCUMENT_REMARK14\",\"F_DOCUMENT_REMARK15\",\"F_DOCUMENT_REMARK16\",\"F_DOCUMENT_REMARK17\",\"F_DOCUMENT_REMARK18\",\"F_DOCUMENT_REMARK19\",\"F_DOCUMENT_REMARK20\"";
            foreach (KeyValuePair<string, object> entry in dr)
            {
                if (entry.Value == null)
                {
                    text2 = text2.Replace(entry.Key, "");
                }
                else if (entry.Key.ToUpper() == "F_LINE_TOTAL_COUNT")
                {
                    text2 = text2.Replace(entry.Key, drItems.Count().ToString());
                }
                else
                {
                    text2 = text2.Replace(entry.Key, entry.Value.ToString().Replace("\"", " "));
                }
            }
            csv.AppendLine(text2);
            csv.Append("\"T\",\"1\"");


            return csv.ToString();
        }

        private static void EtaxLog_Create(ICON.Framework.Provider.DBHelper db, System.Data.IDbTransaction tran, System.Data.DataRow dr, string csvName, string pdfName, Model.ETAXSignDocument_Reps sendResult)
        {
            System.Data.DataTable dtLog = db.ExecuteDataTable("select ID from Etax_Log where ContractID = '" + dr["ContractID"].ToString() + "' and ReceiptID = '" + dr["DOCUMENT_ID"].ToString() + "'", tran);
            if (dtLog.Rows.Count > 0)
            {
                string sql = @"
update Etax_Log set ETaxStatus = @ETaxStatus
    , ETaxErrorMessege = @ETaxErrorMessege
    , CSVFileName = @CSVFileName
    , PDFFileName = @PDFFileName
    , PDFSuccessFileName = @PDFSuccessFileName
    , ETaxDate = getdate()
where ID = @ID
";
                ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ID", dtLog.Rows[0]["ID"].ToString()));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ETaxStatus", sendResult.status.ToUpper() == "OK" ? "success" : "error"));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ETaxErrorMessege", sendResult.status.ToUpper() == "ER" ? sendResult.errorMessage : ""));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@CSVFileName", csvName));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@PDFFileName", pdfName));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@PDFSuccessFileName", sendResult.status.ToUpper() == "OK" ? sendResult.pdfURL : ""));

                db.ExecuteNonQuery(sql, sqlParams, tran);
            }
            else
            {
                string sql = @"
insert into Etax_Log (ID, ReceiptID, ContractID, Amount
    , Email, EmailStatus, ETaxStatus, ETaxDate, ETaxErrorMessege
    , CreateDate, CreateBy, CSVFileName, PDFFileName
    , PDFSuccessFileName, PDFSuccessFTPPath, PDFSuccessDate
)
values (newid(), @ReceiptID, @ContractID, @Amount
    , @Email, @EmailStatus, @ETaxStatus, getdate(), @ETaxErrorMessege
    , getdate(), 'service', @CSVFileName, @PDFFileName
    , @PDFSuccessFileName, '', getdate()
)
";

                ICON.Framework.Provider.DBParameterCollection sqlParams = new ICON.Framework.Provider.DBParameterCollection();
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ReceiptID", dr["DOCUMENT_ID"].ToString()));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ContractID", dr["ContractID"].ToString()));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Amount", Convert.ToDecimal(dr["LINE_NET_INCLUDE_TAX_TOTAL_AMOUNT"].ToString())));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@Email", dr["BUYER_URIID"].ToString()));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@EmailStatus", dr["BUYER_URIID"].ToString() == "" ? "" : "sending"));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ETaxStatus", sendResult.status.ToUpper() == "OK" ? "success" : "error"));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@ETaxErrorMessege", sendResult.status.ToUpper() != "OK" ? sendResult.errorMessage : ""));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@CSVFileName", csvName));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@PDFFileName", pdfName));
                sqlParams.Add(new ICON.Framework.Provider.DBParameter("@PDFSuccessFileName", sendResult.status.ToUpper() == "OK" ? sendResult.pdfURL : ""));
                //sqlParams.Add(new ICON.Framework.Provider.DBParameter("@PDFSuccessDate", sendResult.status.ToUpper() == "OK" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : null));

                db.ExecuteNonQuery(sql, sqlParams, tran);
            }

            if (sendResult.status.ToUpper() == "OK")
            {
                db.ExecuteNonQuery(string.Format(@"
insert into Sys_File_Receipt (ReceiptID, ContractID, IsSentMail, SentTo
    , CreateDate, CreateBy, ReceiptFileData, ModifyDate, ModifyBy, ReceiptStatus)
select ReceiptID, '{1}', {2}, '{3}'
    , getdate(), 'service', '', getdate(), 'service', ''
from Sys_FI_Receipt
where ReceiptID = '{0}'
"
        , dr["DOCUMENT_ID"].ToString()
        , dr["ContractID"].ToString()
        , string.IsNullOrEmpty(dr["BUYER_URIID"].ToString()) ? "0" : "1"
        , dr["BUYER_URIID"].ToString()
        ), tran);
            }
        }
    }
}
