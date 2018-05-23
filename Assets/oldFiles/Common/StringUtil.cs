using System;
using System.Collections;
using System.Collections.Generic;
//using Lobby;

namespace Common
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// 字节数组字符串转字符串
        /// </summary>
        /// <returns>字节数组字符串</returns>
        /// <param name="str">字符串</param>
        public static string ByteArray2Str(string str)
        {
            int i = 0;
            for (; i < str.Length; ++i)
            {
                if (str[i] == '\0')
                {
                    break;
                }
            }
            if (i == 0)
            {
                return "";
            }
            return str.Substring(0, i);
        }

        // "313233" -> "123"
        public static void Hex2Str(byte[] hex, ref byte[] str)
        {
            byte[] temp = new byte[200];
            for (int i = 0; i < hex.Length; i++)
            {
                if (hex[i] >= 0x61)
                { // 0x61 = 97 = 'a'
                    temp[i] = (byte)(hex[i] - 0x61 + 10);
                }
                else if (hex[i] >= 0x30)
                { // 0x30 = 48 = '0'
                    temp[i] = (byte)(hex[i] - 0x30);
                }

                if (i % 2 == 1)
                {
                    str[i / 2] = (byte)(temp[i - 1] * 16 + temp[i]);
                }
            }
        }

        // "123" -> "313233"
        public static void Str2Hex(byte[] str, ref byte[] hex)
        {
            byte hi;
            byte lo;
            for (int i = 0; i < str.Length; i++)
            {
                hi = (byte)(str[i] / 16);
                lo = (byte)(str[i] % 16);
                if (hi >= 10)
                {
                    hex[i * 2] = (byte)(hi - 10 + 0x61);
                }
                else
                {
                    hex[i * 2] = (byte)(hi + 0x30);
                }
                if (lo >= 10)
                {
                    hex[i * 2 + 1] = (byte)(lo - 10 + 0x61);
                }
                else
                {
                    hex[i * 2 + 1] = (byte)(lo + 0x30);
                }
            }
        }

        /// <summary>
        /// 解密AES字符串
        /// </summary>
        /// <returns>AES解密后的字符串</returns>
        /// <param name="encryptedString">AES解密前的Hex字符串</param>
        public static string DecryptAesString(string encryptedString)
        {
            string lowerEncryptedString = encryptedString.ToLower();
            char[] charData = lowerEncryptedString.ToCharArray();
            byte[] byteData = new byte[charData.Length];
            for (int i = 0; i < charData.Length; i++)
            {
                byteData[i] = (byte)charData[i];
            }
            byte[] encryptedData = new byte[charData.Length / 2];
            Hex2Str(byteData, ref encryptedData);
            byte[] decryptedData = AESCrypt.Decrypt(encryptedData, AESCrypt.KEY);
            string decryptedString = System.Text.Encoding.Default.GetString(decryptedData);
            return decryptedString;
        }

        /// <summary>
        /// 加密AES字符串
        /// </summary>
        /// <returns>AES加密前的字符串</returns>
        /// <param name="encryptedString">AES加密后的Hex字符串</param>
        public static string EncryptAesString(string normalString)
        {
            char[] charData = normalString.ToCharArray();
            byte[] byteData = new byte[charData.Length];
            for (int i = 0; i < charData.Length; i++)
            {
                byteData[i] = (byte)charData[i];
            }
            byte[] encryptedData = new byte[charData.Length / 2];
            Str2Hex(byteData, ref encryptedData);
            byte[] decryptedData = AESCrypt.Decrypt(encryptedData, AESCrypt.KEY);
            string decryptedString = System.Text.Encoding.Default.GetString(decryptedData);
            return decryptedString;
        }

        public static void test()
        {
            byte[] hexStr = new byte[] { 0x33, 0x31, 0x33, 0x32, 0x33, 0x33 }; //"313233"
            byte[] str = new byte[3];
            Hex2Str(hexStr, ref str);

        }

        public static int GetUintValue(string s)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (!rex.IsMatch(s))
                return -1;

            return int.Parse(s);
        }

        public static bool IsNumber(string s)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            return rex.IsMatch(s);
        }

        //public static bool IsValidPassword(string s)
        //{
        //    if (string.IsNullOrEmpty(s)
        //       || s.Length < GameConstants.PASSWORD_MIN_LENGTH
        //       || s.Length > GameConstants.PASSWORD_MAX_LENGTH)
        //        return false;

        //    return true;
        //}

        /// <summary>
        /// 说明：从网络协议传过来的字符串，被[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]标识的
        /// 后面是以0值填充的，所以需要用此函数转换一下。
        /// </summary>
        public static string NormalizeMarshaledString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            int len = 0;
            char[] str = s.ToCharArray();
            Console.WriteLine(str.Length);
            for (int i = 0; i < str.Length; ++i)
            {
               // DEBUG.Log_Lobby("Convert.ToInt32(str [i]: " + Convert.ToInt32(str [i]));
                if (Convert.ToInt32(str[i]) == 0)
                {
                    len = i;
                    break;
                }
            }
            if (len == 0)
            {
                //len = s.Length;
                return "";
            }
            return s.Substring(0, len);
        }

        /// <summary>
        /// 说明：从网络协议传过来的字符串，被[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]标识的
        /// 后面是以0值填充的，所以需要用此函数转换一下。
        /// </summary>

        public static string NormalizeMarshaledTimeString(string s)
        {

            //DEBUG.Log_Lobby("NormalizeMarshaledTimeString: " + s);
            int len = 0;
            char[] str = s.ToCharArray();
            for (int i = 0; i < str.Length; ++i)
            {
                //DEBUG.Log_Lobby("Convert.ToInt32(str [i]: " + Convert.ToInt32(str [i]));
                if (Convert.ToInt32(str[i]) == 0 || Convert.ToInt32(str[i]) > 10000)
                {
                    len = i;
                    break;
                }
            }
            if (len == 0)
                len = s.Length;

            return s.Substring(0, len);
        }

    }

}


