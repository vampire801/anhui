using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class Crypto
    {
        public static string MD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(str);
            byte[] out_bytes = md5.ComputeHash(data);
            string out_string = "";
            for (int i = 0; i < out_bytes.Length; i++)
            {
                out_string += out_bytes[i].ToString("x2");
            }
            return out_string.ToLower();
        }
    }
}