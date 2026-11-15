using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService.Ink.Core.Storage.Encryption
{
	
	
	public class Base64FileContentParser : IFileContentParser
	{
		public string Name => "Base64";

		public string FileExtensionName => ".b64";

		public string Deparse(string content)
		{
			return ToBase64String(content);
		}

		public string DeparseToFile(string buildPath, string content)
		{
			string text = Path.Combine(buildPath, Guid.NewGuid().ToString() + FileExtensionName);
			File.WriteAllText(text, Deparse(content));
			return text;
		}

		public string ParseFromFile(string fileAbsolutePath)
		{
			return ParseContent(File.ReadAllText(fileAbsolutePath, Encoding.UTF8));
		}

		public string ParseContent(string content)
		{
			return UnBase64String(content);
		}

		public FileContentParserSaveInfo BuildParserSaveInfo()
		{
			return new Base64FileContentParserSaveInfo(Name);
		}

		public void LoadParserInfo(FileContentParserSaveInfo parserInfo)
		{
		}

		public void Initialize()
		{
		}

		public static string UnBase64String(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return "";
			}
			byte[] bytes = Convert.FromBase64String(value);
			return Encoding.UTF8.GetString(bytes);
		}

		public static string ToBase64String(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return "";
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
		}
	}
}
