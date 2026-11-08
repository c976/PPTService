using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PPTService
{
	public class Md5SecurityUtility
	{
		public static string GetShaFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
			try
			{
				using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (SHA256Cng sHA256Cng = new SHA256Cng())
					{
						byte[] array = sHA256Cng.ComputeHash(inputStream);
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < array.Length; i++)
						{
							stringBuilder.Append(array[i].ToString("x2"));
						}
						return stringBuilder.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.ToString());
				return null;
			}
		}
	}
}
