using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etax_Noble_INET
{
    public class LogFile
    {
        public string FilePath { get; set; } = "";

        public LogFile(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                filePath = path + "\\ServiceLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                this.FilePath = filePath;
            }
            else
            {
                this.FilePath = filePath;
            }
        }

        public void WriteToFile(string Message)
        {
            if (!System.IO.File.Exists(this.FilePath))
            {
                // Create a file to write to.   
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(this.FilePath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(this.FilePath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }

    public class PDFReceipt
    {
        public string Create(string filePath, string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Receipt\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            return path + "\\" + fileName + ".pdf";
        }
    }

    public class CSVInetData
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";

        public CSVInetData(string fileName, bool isSAP = false)
        {
            this.FileName = fileName + ".txt"; ;

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Files\\" + (isSAP ? "SAP_" : "") + "CSV\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            this.FilePath = path + "\\" + this.FileName;
        }

        public void WriteToFile(string message)
        {
            if (!System.IO.File.Exists(this.FilePath))
            {
                // Create a file to write to.   
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(this.FilePath))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(this.FilePath))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}
