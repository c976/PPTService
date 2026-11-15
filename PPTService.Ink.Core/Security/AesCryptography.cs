using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace PPTService.Ink.Core.Security
{
	
	
	public static class AesCryptography
	{
		private static int _iterations = 2;

		private static int _keySize = 256;

		private static string _salt = "aselrias38490a32";

		private static string _vector = "8947az34awl34kjq";

		public static string Encrypt(string value, string password)
		{
			return Encrypt<AesManaged>(value, password);
		}

		public static string Encrypt< T>(string value, string password) where T : SymmetricAlgorithm, new()
		{
			byte[] bytes = Encoding.UTF8.GetBytes(_vector);
			byte[] bytes2 = Encoding.UTF8.GetBytes(_salt);
			byte[] bytes3 = Encoding.UTF8.GetBytes(value);
			byte[] inArray;
			using (T val = new T())
			{
				byte[] bytes4 = new Rfc2898DeriveBytes(password, bytes2, _iterations).GetBytes(_keySize / 8);
				val.Mode = CipherMode.CBC;
				using (ICryptoTransform transform = val.CreateEncryptor(bytes4, bytes))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(bytes3, 0, bytes3.Length);
							cryptoStream.FlushFinalBlock();
							inArray = memoryStream.ToArray();
						}
					}
				}
				val.Clear();
			}
			return Convert.ToBase64String(inArray);
		}

		public static string Decrypt(string value, string password)
		{
			return Decrypt<AesManaged>(value, password);
		}

		public static string Decrypt< T>(string value, string password) where T : SymmetricAlgorithm, new()
		{
			byte[] bytes = Encoding.UTF8.GetBytes(_vector);
			byte[] bytes2 = Encoding.UTF8.GetBytes(_salt);
			byte[] array = Convert.FromBase64String(value);
			byte[] array2;
			int count;
			using (T val = new T())
			{
				byte[] bytes3 = new Rfc2898DeriveBytes(password, bytes2, _iterations).GetBytes(_keySize / 8);
				val.Mode = CipherMode.CBC;
				try
				{
					using (ICryptoTransform transform = val.CreateDecryptor(bytes3, bytes))
					{
						using (MemoryStream stream = new MemoryStream(array))
						{
							using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
							{
								array2 = new byte[array.Length];
								count = cryptoStream.Read(array2, 0, array2.Length);
							}
						}
					}
				}
				catch (Exception)
				{
					return string.Empty;
				}
				val.Clear();
			}
			return Encoding.UTF8.GetString(array2, 0, count);
		}
	}
}
