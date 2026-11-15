using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace PPTService.Ink.Core.Storage.Encryption
{
	
	
	public class AesEncryptHelper
	{
		public static void EncryptStringToFile(string path, string content, string key)
		{
			string contents = AesEncryptorWithBase64(content, key);
			File.WriteAllText(path, contents);
		}

		public static string DecryptStringFromFile(string path, string key)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(path);
			}
			return AesDecryptorWithBase64(File.ReadAllText(path), key);
		}

		public static string AesEncryptorWithBase64(string encryptStr, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			byte[] bytes2 = Encoding.UTF8.GetBytes(encryptStr);
			byte[] array = new RijndaelManaged
			{
				Key = bytes,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateEncryptor().TransformFinalBlock(bytes2, 0, bytes2.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		public static string AesDecryptorWithBase64(string decryptStr, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			byte[] array = Convert.FromBase64String(decryptStr);
			byte[] bytes2 = new RijndaelManaged
			{
				Key = bytes,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes2);
		}
	}
}
