using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Etax_Noble_INET_Service
{
    class Program
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"); // 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef9876543210"); // 16 bytes for AES IV

        static void Main(string[] args)
        {
            Start(args);
        }

        public static void Start(string[] args)
        {
            Etax_Noble_INET.LogFile logFile = new Etax_Noble_INET.LogFile();
            //logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Start service.");

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServerSAP"].ToString();

            string encryptPassword = System.Configuration.ConfigurationManager.AppSettings["Env:encryptPassword"].ToString();
            //string original = "ERPP@ssword";
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = Key;
                byte[] iv = IV;

                //// Encrypt the string
                //string encrypted = EncryptString(original, key, iv);
                //Console.WriteLine($"Encrypted: {encrypted}");

                // Decrypt the string
                string decrypted = DecryptString(encryptPassword, key, iv);
                //Console.WriteLine($"Decrypted: {decrypted}");
                connStr = connStr.Replace("{password}", decrypted);
            }

            ICON.Framework.Provider.DBHelper db = new ICON.Framework.Provider.DBHelper(connStr, null);
            try
            {
                Etax_Noble_INET.Business.StartRun(db, args, logFile);

                //Etax_Noble_INET.Business.StartRun_CleanChangeOwner(db, args, logFile);
            }
            catch (Exception ex)
            {
                logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> Error, " + ex.Message);
            }
            finally
            {
                logFile.WriteToFile(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " >>> End service.");
            }
        }

        //// Encrypt a string using AES
        //public static string EncryptString(string plainText, byte[] key, byte[] iv)
        //{
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = key;
        //        aesAlg.IV = iv;

        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //            {
        //                swEncrypt.Write(plainText);
        //            }
        //            return Convert.ToBase64String(msEncrypt.ToArray());
        //        }
        //    }
        //}

        // Decrypt a string using AES
        public static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
