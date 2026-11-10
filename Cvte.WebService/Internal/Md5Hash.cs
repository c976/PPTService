using System;
using System.Security.Cryptography;
using System.Text;

namespace Cvte.WebService.Internal
{
	internal class Md5Hash
	{
		public static string smethod_0(string string_0)
		{
			if (string_0 == null)
			{
				throw new ArgumentNullException("valueToEncrypt");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(string_0);
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
