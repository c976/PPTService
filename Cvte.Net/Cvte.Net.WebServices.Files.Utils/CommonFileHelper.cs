using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Cvte.Net.WebServices.Files.Utils
{
	
	
	internal static class CommonFileHelper
	{
		public static void DeleteTempFile(string tempPath)
		{
			try
			{
				if (File.Exists(tempPath))
				{
					File.Delete(tempPath);
				}
			}
			catch (Exception)
			{
			}
		}

		public static DirectoryInfo GetTempPathDirectory(string filePath)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(filePath).Parent;
			if (directoryInfo == null)
			{
				directoryInfo = new DirectoryInfo(filePath);
			}
			else if (!directoryInfo.Exists)
			{
				directoryInfo = Directory.CreateDirectory(directoryInfo.FullName);
			}
			return directoryInfo;
		}

		public static string GetFileNamePattern(string filePath)
		{
			return Regex.Escape(Path.GetFileName(filePath)) + ".*.tmp";
		}

		public static void FileCheck(string tempPath, string filePath, string hash)
		{
			if (!string.IsNullOrEmpty(hash) && SecurityUtility.GetMd5HashFromFile(tempPath) != hash)
			{
				DeleteTempFile(tempPath);
				throw new InvalidOperationException("file hash does not match.");
			}
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			File.Move(tempPath, filePath);
		}
	}
}
