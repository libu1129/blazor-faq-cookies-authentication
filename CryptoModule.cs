using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace BlazorCookieAuthentication
{
    public static class CryptoModule
    {
        private static string keyString => AppDomain.CurrentDomain.BaseDirectory.GetType().GUID.ToString("n").Substring(0, 16);

        [Obsolete]
        public static string aes256_encrypt_backup(string text)
        {
            byte[] cipherData;
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(keyString);
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            ICryptoTransform cipher = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, cipher, CryptoStreamMode.Write);
            using StreamWriter sw = new StreamWriter(cs);
            sw.Write(text);



            cipherData = ms.ToArray();


            byte[] combinedData = new byte[aes.IV.Length + cipherData.Length];
            Array.Copy(aes.IV, 0, combinedData, 0, aes.IV.Length);
            Array.Copy(cipherData, 0, combinedData, aes.IV.Length, cipherData.Length);
            return bytes_to_hex(combinedData);
        }

        [Obsolete]
        public static string aes256_decrypt_backup(string text)
        {
            string plainText;
            byte[] combinedData = hex_to_bytes(text);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(keyString);
            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherText = new byte[combinedData.Length - iv.Length];
            Array.Copy(combinedData, iv, iv.Length);
            Array.Copy(combinedData, iv.Length, cipherText, 0, cipherText.Length);
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform decipher = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream ms = new MemoryStream(cipherText);
            using CryptoStream cs = new CryptoStream(ms, decipher, CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            plainText = sr.ReadToEnd();

            return plainText;
        }



        private static string bytes_to_hex(byte[] cipherBytes)
        {
            // 16진수로 변환
            string hex = "";
            foreach (byte x in cipherBytes)
            {
                hex += x.ToString("x2");
            }
            return hex;
        }

        private static byte[] hex_to_bytes(string text)
        {
            // 16진수 문자열을 바이트 배열로 변환
            byte[] cipherBytes = new byte[text.Length / 2];
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                cipherBytes[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            }
            return cipherBytes;
        }


        public static string GetSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


















        public static string aes256_encrypt(string text)
        {
            // 사전 설정
            UTF8Encoding ue = new UTF8Encoding();
            using RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 256;

            // key 및 iv 설정
            byte[] pwdBytes = ue.GetBytes(keyString);
            byte[] keyBytes = new byte[16];
            byte[] IVBytes = new byte[16];
            int lenK = pwdBytes.Length;
            int lenIV = pwdBytes.Length;
            if (lenK > keyBytes.Length) { lenK = keyBytes.Length; }
            if (lenIV > IVBytes.Length) { lenIV = IVBytes.Length; }
            Array.Copy(pwdBytes, keyBytes, lenK);
            Array.Copy(pwdBytes, IVBytes, lenIV);
            rijndael.Key = keyBytes;
            rijndael.IV = IVBytes;

            byte[] message = ue.GetBytes(text);
            using ICryptoTransform transform = rijndael.CreateEncryptor();
            // 암호화 수행 
            byte[] cipherBytes = transform.TransformFinalBlock(message, 0, message.Length);
            rijndael.Clear();

            // 16진수로 변환
            string hex = "";
            foreach (byte x in cipherBytes)
            {
                hex += x.ToString("x2");
            }
            return hex;
        }


        public static string aes256_decrypt(string text)
        {
            // 사전 설정
            UTF8Encoding ue = new UTF8Encoding();
            using RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 256;

            // 16진수 문자열을 바이트 배열로 변환
            byte[] cipherBytes = new byte[text.Length / 2];
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                cipherBytes[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            }

            // key 및 iv 설정
            byte[] pwdBytes = ue.GetBytes(keyString);
            byte[] keyBytes = new byte[16];
            byte[] IVBytes = new byte[16];
            int lenK = pwdBytes.Length;
            int lenIV = pwdBytes.Length;
            if (lenK > keyBytes.Length) { lenK = keyBytes.Length; }
            if (lenIV > IVBytes.Length) { lenIV = IVBytes.Length; }
            Array.Copy(pwdBytes, keyBytes, lenK);
            Array.Copy(pwdBytes, IVBytes, lenIV);
            rijndael.Key = keyBytes;
            rijndael.IV = IVBytes;

            using ICryptoTransform transform = rijndael.CreateDecryptor();
            // 암호화 수행
            byte[] message = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            rijndael.Clear();

            return Encoding.UTF8.GetString(message);
        }
    }
}
