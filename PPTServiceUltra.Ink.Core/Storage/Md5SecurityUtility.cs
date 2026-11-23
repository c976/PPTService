using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class Md5SecurityUtility
	{
		public static string GetMd5HashFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath);
			}
			using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
				{
					byte[] array = mD5CryptoServiceProvider.ComputeHash(inputStream);
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < array.Length; i++)
					{
						stringBuilder.Append(array[i].ToString("x2"));
					}
					return stringBuilder.ToString();
				}
			}
		}

		public static string Encrypt32(string valueToEncrypt)
		{
			if (valueToEncrypt == null)
			{
				throw new ArgumentNullException("valueToEncrypt");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(valueToEncrypt);
			using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
			{
				byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				return stringBuilder.ToString();
			}
		}
	}
}
