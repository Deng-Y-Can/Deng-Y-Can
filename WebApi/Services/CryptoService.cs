using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.Services
{
    public class CryptoService
    {
        public string Md5(string input)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string Sha1(string input)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string Sha256(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string Sha512(string input)
        {
            using var sha512 = SHA512.Create();
            var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string HmacSha256(string input, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string AesEncrypt(string plainText, string key, string iv = null)
        {
            using var aes = Aes.Create();
            aes.Key = EnsureKeyLength(Encoding.UTF8.GetBytes(key), 32);
            aes.IV = iv != null ? EnsureKeyLength(Encoding.UTF8.GetBytes(iv), 16) : new byte[16];
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        public string AesDecrypt(string cipherText, string key, string iv = null)
        {
            using var aes = Aes.Create();
            aes.Key = EnsureKeyLength(Encoding.UTF8.GetBytes(key), 32);
            aes.IV = iv != null ? EnsureKeyLength(Encoding.UTF8.GetBytes(iv), 16) : new byte[16];
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decrypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }

        public Dictionary<string, string> GenerateRsaKeys(int keySize = 2048)
        {
            using var rsa = RSA.Create(keySize);
            return new Dictionary<string, string>
            {
                ["publicKey"] = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
                ["privateKey"] = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey())
            };
        }

        public string RsaEncrypt(string plainText, string publicKeyBase64)
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64), out _);
            var bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(bytes);
        }

        public string RsaDecrypt(string cipherText, string privateKeyBase64)
        {
            using var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
            var bytes = rsa.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(bytes);
        }

        private byte[] EnsureKeyLength(byte[] key, int length)
        {
            if (key.Length == length) return key;
            var result = new byte[length];
            Array.Copy(key, result, Math.Min(key.Length, length));
            return result;
        }
    }
}
