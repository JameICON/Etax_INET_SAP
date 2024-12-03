using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Generate_Password
{
    internal class Program
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"); // 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef9876543210"); // 16 bytes for AES IV

        static void Main(string[] args)
        {
            int loop = 1;
            while (loop > 0)
            {
                Console.WriteLine("type text password to encrypt ... ");
                string password = Console.ReadLine();
                if (password == null)
                {
                    Console.WriteLine("please type password again");
                }
                else
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        byte[] key = Key;
                        byte[] iv = IV;

                        //// Encrypt the string
                        string encrypted = EncryptString(password, key, iv);
                        Console.WriteLine($"Encrypted Password: {encrypted}");
                        loop = 0;
                    }
                }
            }
            Console.ReadLine();
        }
        public static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }
}
