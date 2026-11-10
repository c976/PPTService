using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cvte.WebService.Utils
{
	internal class DesSecurityUtility
	{
		private static readonly string string_0 = "dfgedfgb";

		public static byte[] smethod_0(byte[] byte_0, string string_1, bool bool_0 = true, bool bool_1 = true)
		{
			using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider
			{
				Key = Encoding.UTF8.GetBytes(string_1),
				IV = Encoding.UTF8.GetBytes(string_0),
				Mode = ((!bool_1) ? CipherMode.CBC : CipherMode.ECB),
				Padding = ((!bool_0) ? PaddingMode.None : PaddingMode.Zeros)
			})
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(byte_0, 0, byte_0.Length);
						cryptoStream.FlushFinalBlock();
					}
					return memoryStream.ToArray();
				}
			}
		}

		public static string smethod_1(string string_1, string string_2)
		{
			return smethod_2(smethod_0(Encoding.UTF8.GetBytes(string_1), string_2));
		}

		public static string smethod_2(byte[] byte_0)
		{
			if (byte_0 == null)
			{
				throw new ArgumentNullException();
			}
			return byte_0.Aggregate(string.Empty, (string string_0, byte b) => string_0 + b.ToString("X2"));
		}

		public static byte[] smethod_3(byte[] byte_0, string string_1, bool bool_0 = true, bool bool_1 = true)
		{
			using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider
			{
				Key = Encoding.UTF8.GetBytes(string_1),
				IV = Encoding.UTF8.GetBytes(string_0),
				Mode = ((!bool_1) ? CipherMode.CBC : CipherMode.ECB),
				Padding = ((!bool_0) ? PaddingMode.None : PaddingMode.Zeros)
			})
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(byte_0, 0, byte_0.Length);
						cryptoStream.FlushFinalBlock();
					}
					return memoryStream.ToArray();
				}
			}
		}

		public static string smethod_4(string string_1, string string_2)
		{
			byte[] bytes = smethod_3(smethod_5(string_1), string_2);
			return Encoding.UTF8.GetString(bytes).Replace("\0", null);
		}

		public static byte[] smethod_5(string string_1)
		{
			if (string_1 == null)
			{
				throw new ArgumentNullException("valueToConvert");
			}
			string_1 = string_1.Replace(" ", string.Empty);
			if (string_1.Length % 2 != 0)
			{
				throw new FormatException("valueToConvert");
			}
			byte[] array = new byte[string_1.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToByte(string_1.Substring(i * 2, 2), 16);
			}
			return array;
		}
	}
}
