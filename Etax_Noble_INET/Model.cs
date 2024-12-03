using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etax_Noble_INET
{
    public class Model
    {
        public class ETAXSignDocument
        {
            public string SellerTaxId { get; set; } = "";
            public string SellerBranchId { get; set; } = "";
            public string UserCode { get; set; } = "";
            public string AccessKey { get; set; } = "";
            public string APIKey { get; set; } = "";
            public string ServiceCode { get; set; } = "";
            //public string PDFContent { get; set; } = "";
            //public string TextContent { get; set; } = "";
            public string SendInbox { get; set; } = "";
            public string SendChat { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string EncryptPassword { get; set; } = "";
            public string SendMail { get; set; } = "";
            public string SendSms { get; set; } = "";
            public string SendInboxHaveBuyer { get; set; } = "";
            public string SellerTemplateCode { get; set; } = "";


            public ETAXSignDocument()
            {
                UserCode = System.Configuration.ConfigurationManager.AppSettings["ETAX:UserCode"].ToString();
                AccessKey = System.Configuration.ConfigurationManager.AppSettings["ETAX:AccessKey"].ToString();
                APIKey = System.Configuration.ConfigurationManager.AppSettings["ETAX:APIKey"].ToString();
                ServiceCode = System.Configuration.ConfigurationManager.AppSettings["ETAX:ServiceCode"].ToString();
                SellerTaxId = System.Configuration.ConfigurationManager.AppSettings["ETAX:SellerTaxId"].ToString();
                SellerBranchId = System.Configuration.ConfigurationManager.AppSettings["ETAX:SellerBranchId"].ToString();
            }
        }

        public class ETAXSignDocument_Reps
        {
            public string status { get; set; } = "";
            public string xmlURL { get; set; } = "";
            public string pdfURL { get; set; } = "";
            public string transactionCode { get; set; } = "";

            public string errorCode { get; set; } = "";
            public string errorMessage { get; set; } = "";

        }

        public class FileAttach
        {
            public string Name { get; set; } = "";
            public string ContentType { get; set; } = "";

            public string FullPath { get; set; } = "";

            public byte[] FileData { get; set; }
        }
    }
}
