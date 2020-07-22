using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AesHelper
    {
        String key = "adgaw334^*^&#$#$W2343qwreqwr12";

        String iv = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";


        public string AesEncrypt(string rawInput, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(rawInput))
            {
                return string.Empty;
            }

            if (key == null || iv == null || key.Length < 1 || iv.Length < 1)
            {
                throw new ArgumentException("Key/Iv is null.");
            }

            using (var rijndaelManaged = new RijndaelManaged()
            {
                Key = key, // 密钥，长度可为128， 196，256比特位
                IV = iv,  //初始化向量(Initialization vector), 用于CBC模式初始化
                KeySize = 256,//接受的密钥长度
                BlockSize = 128,//加密时的块大小，应该与iv长度相同
                Mode = CipherMode.CBC,//加密模式
                Padding = PaddingMode.PKCS7
            }) //填白模式，对于AES, C# 框架中的 PKCS　＃７等同与Java框架中 PKCS #5
            {
                using (var transform = rijndaelManaged.CreateEncryptor(key, iv))
                {
                    var inputBytes = Encoding.UTF8.GetBytes(rawInput);//字节编码， 将有特等含义的字符串转化为字节流
                    var encryptedBytes = transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);//加密
                    return Convert.ToBase64String(encryptedBytes);//将加密后的字节流转化为字符串，以便网络传输与储存。
                }
            }
        }


        public string AesDecrypt(string encryptedInput, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(encryptedInput))
            {
                return string.Empty;
            }

            if (key == null || iv == null || key.Length < 1 || iv.Length < 1)
            {
                throw new ArgumentException("Key/Iv is null.");
            }

            using (var rijndaelManaged = new RijndaelManaged()
            {
                Key = key,
                IV = iv,
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                using (var transform = rijndaelManaged.CreateDecryptor(key, iv))
                {
                    var inputBytes = Convert.FromBase64String(encryptedInput);
                    var encryptedBytes = transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Encoding.UTF8.GetString(encryptedBytes);
                }
            }
        }
    }
}
